using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceHolder : MonoBehaviour
{
    /*
     *  TODO:   Maybe have pieces connected along the edge 
     *          then have them lerp out to spaces when selected
     * 
     */ 

    private int _touchID;
    private SpriteRenderer _spriteRenderer;

    private bool _placed;
    public bool Placed
    {
        get { return _placed; }
    }

    private Piece _piece;
    public Piece Piece
    {
        get { return _piece; }
    }

    public Coord centerCoord;
    protected Queue<Coord> lastPositions;
    private const int framesBeforeLockIn = 10;
    private const int leniencyFrames = 5;
    protected readonly Vector3 baseDragOffset = 0.1f * Vector3.up;

    public void Init(Piece piece)
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _piece = piece;
        _placed = false;
        ListenforInput();
        lastPositions = new Queue<Coord>();

    }

    public void ListenforInput()
    {
        Services.EventManager.Register<TouchDown>(OnTouchDown);
        Services.EventManager.Register<MouseDown>(OnMouseDownEvent);
        _touchID = -1;
    }

    protected virtual bool IsPointContaintedWithinSprite(Vector2 point)
    {
        Vector2 extents = _spriteRenderer.bounds.extents;
        Vector2 centerPoint = transform.position;

        return  point.x >= centerPoint.x - (extents.x * 0.5f)&&
                point.x <= centerPoint.x + (extents.x * 0.5f)&&
                point.y >= centerPoint.y - (extents.y * 1.5f)&&
                point.y <= centerPoint.y + (extents.y * 1.5f);
    }

    public Vector3 GetCenterpoint(bool centerTile = false)
    {
        Vector3 centerPos = Vector3.zero;
        foreach (Tile tile in Piece.Tiles)
        {
            centerPos += tile.transform.localPosition;
        }
        centerPos /= Piece.Tiles.Count;
        if (centerTile)
        {
            Tile closestTile = null;
            float closestDistance = Mathf.Infinity;
            foreach (Tile tile in Piece.Tiles)
            {
                float dist = Vector3.Distance(tile.transform.localPosition, centerPos);
                if (dist < closestDistance)
                {
                    closestTile = tile;
                    closestDistance = dist;
                }
            }
            return closestTile.transform.localPosition;
        }
        return centerPos;
    }

    public void Reposition(Vector3 pos, bool centered = false)
    {
       if (centered) pos -= GetCenterpoint();
        transform.position = pos;
    }

    public void SetTileCoords(Coord centerPos)
    {
        centerCoord = centerPos;

        foreach (Tile tile in Piece.Tiles)
        {
            tile.SetCoord(tile.relativeCoord.Add(centerPos));
        }
    }

    protected void QueuePosition(Coord pos)
    {
        if (lastPositions.Count >= (framesBeforeLockIn + leniencyFrames))
        {
            lastPositions.Dequeue();
        }
        lastPositions.Enqueue(pos);
    }

    protected void OnTouchDown(TouchDown e)
    {
        Vector2 touchWorldPos = Services.GameManager.MainCamera.ScreenToWorldPoint(e.touch.position);
        if (IsPointContaintedWithinSprite(touchWorldPos) && _touchID == -1)
        {
            _touchID = e.touch.fingerId;
            OnInputDown();
        }
    }

    protected void OnMouseDownEvent(MouseDown e)
    {
        Vector2 mouseWorldPos = Services.GameManager.MainCamera.ScreenToWorldPoint(e.mousePos);
        if (IsPointContaintedWithinSprite(mouseWorldPos))
        {
            OnInputDown();
        }
    }

    protected void OnInputDown()
    {

        Services.EventManager.Unregister<TouchDown>(OnTouchDown);
        Services.EventManager.Unregister<MouseDown>(OnMouseDownEvent);

        Services.EventManager.Register<TouchMove>(OnTouchMove);
        Services.EventManager.Register<MouseMove>(OnMouseMoveEvent);

        Services.EventManager.Register<TouchUp>(OnTouchUp);
        Services.EventManager.Register<MouseUp>(OnMouseUpEvent);

    }

    protected void OnTouchMove(TouchMove e)
    {
        if (e.touch.fingerId == _touchID)
        {
            OnInputDrag(Services.GameManager.MainCamera.ScreenToWorldPoint(e.touch.position));
        }
    }

    protected void OnMouseMoveEvent(MouseMove e)
    {
        OnInputDrag(Services.GameManager.MainCamera.ScreenToWorldPoint(e.mousePos));
    }

    protected void OnInputDrag(Vector2 inputPos)
    {
            
            Reposition(new Vector3(
                inputPos.x,
                inputPos.y,
                transform.position.z));
            //QueuePosition(snappedCoord);
            
        

        //if (!Services.GameManager.disableUI) SetLegalityGlowStatus();

    }

    protected void OnTouchUp(TouchUp e)
    {
        if (e.touch.fingerId == _touchID)
        {
            OnInputUp();
            _touchID = -1;
        }
    }

    protected void OnMouseUpEvent(MouseUp e)
    {
        OnInputUp();
    }

    protected void OnInputUp()
    {

        Services.EventManager.Unregister<TouchMove>(OnTouchMove);
        Services.EventManager.Unregister<MouseMove>(OnMouseMoveEvent);

        Services.EventManager.Register<TouchDown>(OnTouchDown);
        Services.EventManager.Register<MouseDown>(OnMouseDownEvent);

    }
}

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

        return  point.x >= centerPoint.x - (extents.x * 1f)&&
                point.x <= centerPoint.x + (extents.x * 1f)&&
                point.y >= centerPoint.y - (extents.y * 1f)&&
                point.y <= centerPoint.y + (extents.y * 1f);
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
        Debug.Log("T: " + transform.position);

    }

    public void SetTileCoords(Vector3 centerPos)
    {

        centerCoord = new Coord(Mathf.RoundToInt(centerPos.x), Mathf.RoundToInt(centerPos.y));
        foreach (Tile tile in Piece.Tiles)
        {
            tile.SetCoord(tile.relativeCoord.Add(centerCoord));
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

    public bool IsPlacementLegal()
    {
        return false;
    }

    public bool IsPlacementLegal(Coord centerCoord)
    {
        List<Coord> hypotheticalTileCoords = new List<Coord>();
        foreach (Tile tile in Piece.Tiles)
        {
            hypotheticalTileCoords.Add(tile.coord);
        }
        foreach (Coord coord in hypotheticalTileCoords)
        {
            if (!Services.MapManager.IsCoordContainedInMap(coord)) return false;

        }
        return true;
    }

    public void PlaceAtLocation(Coord ceneterCoordLocation)
    {
        //SetTileCoords(transform.localPosition);
        /*Reposition(new Vector3(
                                ceneterCoordLocation.x,
                                ceneterCoordLocation.y,
                                transform.position.z));
                                */
        transform.localPosition = new Vector3(ceneterCoordLocation.x, ceneterCoordLocation.y, -8);
    }


    public void PlaceAtCurrentLocation()
    {
        OnPlace();
        PlaceAtLocation(centerCoord);
        _placed = true;
        
    }

    public void OnPlace()
    {
        foreach(Tile tile in Piece.Tiles)
        {
            MapTile mapTile = Services.MapManager.Map[tile.coord.x, tile.coord.y];

        }
        if(!Placed)
        Services.EventManager.Fire(new PlayMade(Piece));
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
        if (!Placed)
        {
            Vector3 screenInputPos = Services.GameManager.MainCamera.WorldToScreenPoint(inputPos);
            float mapEdgeScreenHeight = Services.CameraController.GetMapEdgeScreenHeight();
            Coord roundedInputCoord = new Coord(Mathf.RoundToInt(screenInputPos.x),
                                                Mathf.RoundToInt(screenInputPos.y));
            float pieceOffset = mapEdgeScreenHeight * 0.001f;

            Vector3 piecePos = new Vector3(inputPos.x, inputPos.y + pieceOffset, transform.position.z);
            SetTileCoords(transform.localPosition);
            Reposition(piecePos);

            //QueuePosition(snappedCoord);

        }

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
        if (!Placed)
        {
            // piece snapping
        }
        if(IsPlacementLegal(centerCoord))
        {
            PlaceAtCurrentLocation();
            Debug.Log("Placed");
        }

        
        Services.EventManager.Unregister<TouchMove>(OnTouchMove);
        Services.EventManager.Unregister<MouseMove>(OnMouseMoveEvent);

        Services.EventManager.Register<TouchDown>(OnTouchDown);
        Services.EventManager.Register<MouseDown>(OnMouseDownEvent);

    }
}

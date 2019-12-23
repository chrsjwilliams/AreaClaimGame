using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile : MonoBehaviour
{
    private Coord _coord;
    public Coord Coord
    {
        get { return _coord; }
    }

    private Player _occupant;
    public Player Occupant
    {
        get { return _occupant; }
    }

    public bool isOccupied
    {
        get { return Occupant != null; }
    }

    private SpriteRenderer _spriteRenderer;
    public SpriteRenderer SpriteRenderer
    {
        get { return _spriteRenderer; }
    }

    private int _touchID;

    public void Init(Coord coord)
    {
        _coord = coord;
        transform.localPosition = new Vector2(coord.x, coord.y);
        _spriteRenderer = GetComponent<SpriteRenderer>();
        ListenforInput();
    }

    public void ListenforInput()
    {
        Services.EventManager.Register<TouchDown>(OnTouchDown);
        Services.EventManager.Register<MouseDown>(OnMouseDownEvent);
        _touchID = -1;
    }

    protected virtual bool IsPointContaintedWithinSprite(Vector2 point)
    {
        Vector2 extents = SpriteRenderer.bounds.extents;
        Vector2 centerPoint = transform.position;

        return  point.x >= centerPoint.x - extents.x &&
                point.x <= centerPoint.x + extents.x &&
                point.y >= centerPoint.y - extents.y &&
                point.y <= centerPoint.y + extents.y;
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
        SpriteRenderer.color = Services.GameScene.CurrentPlayer.ColorScheme[0];
        Services.EventManager.Fire(new PlayMade(this));

        Services.EventManager.Unregister<TouchDown>(OnTouchDown);
        Services.EventManager.Unregister<MouseDown>(OnMouseDownEvent);

        Services.EventManager.Register<TouchUp>(OnTouchUp);
        Services.EventManager.Register<MouseUp>(OnMouseUpEvent);

    }

    protected void OnTouchMove(TouchMove e)
    {
        if(e.touch.fingerId == _touchID)
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

    }

    protected void OnTouchUp(TouchUp e)
    {
        if(e.touch.fingerId == _touchID)
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

        Services.EventManager.Register<TouchDown>(OnTouchDown);
        Services.EventManager.Register<MouseDown>(OnMouseDownEvent);

    }
}

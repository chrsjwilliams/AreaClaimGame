using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{

    private Player _owner;
    public Player Owner
    {
        get { return _owner; }
    }

    public Coord relativeCoord;

    public Coord coord;

    private bool _isCentralTile;
    public bool IsCentralTile
    {
        get { return _isCentralTile; }
    }

    private int _strength;
    public int Strength
    {
        get { return _strength; }
    }
    private SpriteRenderer _spriteRenderer;

    public void Init(Coord pos,  Player player,int str, bool central)
    {
        relativeCoord = pos;
        _owner = player;
        transform.localPosition = new Vector2(pos.x, pos.y);
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _strength = str;
        _isCentralTile = central;
        _spriteRenderer.color = Services.GameScene.players[Owner.PlayerNumber].ColorScheme[Strength - 1];
        
    }

    public void SetCoord(Coord _coord) { coord = _coord; }
    public void SetCoord(IntVector2 _coord) { coord = new Coord(_coord.x, _coord.y); }
    public void SetCoord(Vector2 _coord) { coord = new Coord((int)_coord.x, (int)_coord.y); }
    public void SetCoord(int x, int y) { coord = new Coord(x, y); }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

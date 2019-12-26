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

    private Coord _piecePos;
    public Coord PiecePos
    {
        get { return _piecePos; }
    }

    private Coord _coord;
    public Coord Coord
    {
        get { return _coord; }
    }

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
    public Coord relativeCoord;
    private SpriteRenderer _spriteRenderer;

    public void Init(Coord pos,  Player player,int str, bool central)
    {
        _piecePos = pos;
        _owner = player;
        transform.localPosition = new Vector2(pos.x, pos.y);
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _strength = str;
        _isCentralTile = central;
        _spriteRenderer.color = Services.GameScene.players[Owner.PlayerNumber].ColorScheme[Strength - 1];
        
    }

    public void SetCoord(Coord coord) { _coord = coord; }
    public void SetCoord(IntVector2 coord) { _coord = new Coord(coord.x, coord.y); }
    public void SetCoord(Vector2 coord) { _coord = new Coord((int)coord.x, (int)coord.y); }
    public void SetCoord(int x, int y) { _coord = new Coord(x, y); }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

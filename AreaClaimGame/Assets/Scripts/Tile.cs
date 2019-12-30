using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{

    public Player owner{ get; private set; }

    public Coord relativeCoord;

    public Coord coord;

    public bool isCentralTile{ get; private set; }

    public int strength { get; private set; }
    private SpriteRenderer _spriteRenderer;

    public void Init(Coord pos,  Player player,int str, bool central)
    {
        relativeCoord = pos;
        owner = player;
        transform.localPosition = new Vector2(pos.x, pos.y);
        _spriteRenderer = GetComponent<SpriteRenderer>();
        strength = str;
        isCentralTile = central;
        _spriteRenderer.color = Services.GameScene.players[owner.playerNum].colorScheme[strength];
        
    }

    public void SetCoord(Coord _coord) { coord = _coord; }
    public void SetCoord(IntVector2 _coord) { coord = new Coord(_coord.x, _coord.y); }
    public void SetCoord(Vector2 _coord) { coord = new Coord((int)_coord.x, (int)_coord.y); }
    public void SetCoord(int x, int y) { coord = new Coord(x, y); }


    public void OnRemove()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

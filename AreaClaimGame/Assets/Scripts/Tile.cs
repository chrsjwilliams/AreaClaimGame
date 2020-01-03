using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{

    public Player owner{ get; private set; }

    public Coord relativeCoord;

    public Coord coord;

    public bool isCentralTile{ get; private set; }

    public Piece piece { get; private set; }

    public int strength { get; private set; }
    private SpriteRenderer _spriteRenderer;

    public void Init(Coord pos,  Player player,Piece piece_, int str, bool central)
    {
        relativeCoord = pos;
        owner = player;
        piece = piece_;
        transform.localPosition = new Vector2(pos.x, pos.y);
        _spriteRenderer = GetComponent<SpriteRenderer>();
        strength = str;
        isCentralTile = central;

        _spriteRenderer.color = Services.GameScene.players[owner.playerNum - 1].colorScheme[strength];
        
    }

    public void SetCoord(Coord _coord) { coord = _coord; }
    public void SetCoord(IntVector2 _coord) { coord = new Coord(_coord.x, _coord.y); }
    public void SetCoord(Vector2 _coord) { coord = new Coord((int)_coord.x, (int)_coord.y); }
    public void SetCoord(int x, int y) { coord = new Coord(x, y); }


    public bool HasAdjacentAuxTiles()
    {
        if (!isCentralTile) return false;
        List<Tile> adjacentTiles = GetAdjacentFriendlyTiles();
        bool hasSupportingAuxTiles = false;
        foreach (Tile tile in adjacentTiles)
        {
            if (!tile.isCentralTile) hasSupportingAuxTiles = true;
        }
        return !hasSupportingAuxTiles;
    }

    public bool CanBeRemoved()
    {
        if (isCentralTile) return HasAdjacentAuxTiles() || strength > 1;
        else return strength <= 0;
    }

    public void RemoveTile()
    {
        piece.RemoveTile(this);
        OnRemove();

    }

    public void ReduceStrength(int str)
    {
        strength -= str;
        if (strength <= 0)
        {
            //Remove();
        }
        else
        {
            _spriteRenderer.color = owner.colorScheme[strength];
        }
    }

    public List<Tile> GetAdjacentFriendlyTiles()
    {
        List<Tile> adjacentTiles = new List<Tile>();
        foreach(Coord dir in Coord.Directions())
        {
            Coord frontierCoord = coord.Add(dir);
            Tile frontierTile = Services.MapManager.Map[frontierCoord.x, frontierCoord.y].OccupyingTile;
            if(frontierTile != null && frontierTile.owner == owner)
            {

                adjacentTiles.Add(frontierTile);
            }
        }

        return adjacentTiles;
    }

    public void OnRemove()
    {
        Destroy(gameObject);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece
{
    private Player _owner;
    public Player Owner
    {
        get { return _owner; }
    }

    private PieceHolder _holder;

    private int _index;

    private int _strength;
    public int Strength
    {
        get { return _strength; }
    }

    private List<Tile> _tiles = new List<Tile>();
    public List<Tile> Tiles
    {
        get { return _tiles; }
    }

    protected static int[,,] piece = new int[6, 3, 3]
    { 
        //  ###
        {
            { 0,0,0 },
            { 1,1,1 },
            { 0,0,0 }
        },
        //  #
        //  #
        //  #
       {
            { 0,1,0 },
            { 0,1,0 },
            { 0,1,0 }
        },
        //     #
        //    ##
        {
            { 0,1,0 },
            { 1,1,0 },
            { 0,0,0 }
        },
        //    ##
        //     #
        {
            { 0,0,0 },
            { 1,1,0 },
            { 0,1,0 }
        },
        //     ##
        //     #
        {
            { 0,0,0 },
            { 0,1,1 },
            { 0,1,0 }
        }
        ,
        //     #
        //     ##
        {
            { 0,1,0 },
            { 0,1,1 },
            { 0,0,0 }
        }
    };

    public Piece(int index, Player player, int strength)
    {
        _index = index;
        _owner = player;
        _strength = strength;

    }

    public void MakePhysicalPiece()
    {
        _holder = GameObject.Instantiate(Services.Prefabs.PieceHolder, Services.GameScene.transform).GetComponent<PieceHolder>();
        _holder.transform.position = Vector3.zero;
        _holder.gameObject.name = "Player " + (Owner.PlayerNumber + 1) + " Piece Holder";
        //_holder.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        _holder.Init(this);

        bool centralTile;
        int tileStrength;
        string pieceName;
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                if(piece[_index, x, y] == 1)
                {
                    centralTile = x == 1 && y == 1 ? true : false;
                    tileStrength = centralTile ? Strength + 1 : Strength;  
                    Tile newTile = MonoBehaviour.Instantiate(Services.Prefabs.PlayerTile, _holder.transform);
                    Coord newCoord = new Coord(x-1, y-1);
                    newTile.Init(newCoord, Owner,tileStrength, centralTile);
                    pieceName = newTile.name.Replace("(Clone)", "");
                    newTile.name = pieceName;

                    _tiles.Add(newTile);
                }
            }
        }
    }
}

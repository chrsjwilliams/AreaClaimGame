using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece
{
    public Player owner { get; private set;}
    public PieceHolder holder { get; private set; }

    private int _index;
    public int strength { get; private set; }

    public List<Tile> tiles { get ; private set; }
    public Tile centerTile { get; private set; }

    public bool selected;
    public bool burningFromHand { get; private set; }

    public static int[,,] piece = new int[6, 3, 3]
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

    public Piece(int index, Player player, int _strength)
    {
        _index = index;
        owner = player;
        strength = strength;
        tiles = new List<Tile>();
    }

    public void MakePhysicalPiece()
    {
        holder = GameObject.Instantiate(Services.Prefabs.PieceHolder, Services.MapManager.TileMapHolder.transform).GetComponent<PieceHolder>();
        holder.gameObject.name = "Player " + owner.playerNum + " Piece Holder";
        holder.Init(this);

        int tileStrength;
        string pieceName;
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                bool isCentralTile = false;

                if (piece[_index, x, y] == 1)
                {                    
                    Tile newTile = MonoBehaviour.Instantiate(Services.Prefabs.PlayerTile, holder.transform);
    
                    Coord newCoord = new Coord(x-1, y-1);
                    if (newCoord.Equals(Coord.Zero))
                    {
                        centerTile = newTile;
                        isCentralTile = true;
                    }
                    tileStrength = isCentralTile ? strength + 1 : strength;
                    newTile.Init(newCoord, owner,tileStrength, isCentralTile);
                    pieceName = newTile.name.Replace("(Clone)", "");
                    newTile.name = pieceName;

                    tiles.Add(newTile);
                }
            }
        }
    }

    public virtual void DestroyPiece()
    {
        holder.RemoveAllInputEvents();
        foreach (Tile tile in tiles) tile.OnRemove();
        GameObject.Destroy(holder);

    }

    public void BurnFromHand()
    {
        holder.HideFromInput();
        // Burn Task to move piece off screen and fade it
        burningFromHand = true;
        foreach (Tile tile in tiles) tile.OnRemove();
    }

    public void OnDrawn()
    {
        holder.ListenforInput();
    }
}

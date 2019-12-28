using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{

    private int _mapWidth;
    public int MapWidth
    {
        get { return _mapWidth; }
    }

    private int _mapHeight;
    public int MapHeight
    {
        get { return _mapHeight; }
    }

    private const string TILE_MAP_HOLDER = "TileMapHolder";
    private MapTile[,] _map;
    public MapTile[,] Map
    {
        get { return _map; }
    }

    private GameObject _tileMapHolder;
    public GameObject TileMapHolder
    {
        get { return _tileMapHolder; }
    }

    [SerializeField]
    private IntVector2 _center;
    public IntVector2 Center
    {
        get { return _center; }
    }

    public void Init()
    {
        
    }

    public void GenerateMap()
    {
        GameObject tilePrefab = Services.Prefabs.MapTile;
        _tileMapHolder = GameObject.Find(TILE_MAP_HOLDER);

        _mapHeight = 10;
        _mapWidth = 6;

        _map = new MapTile[MapWidth, MapHeight];
        for(int x = 0; x < MapWidth; x++)
        {
            for(int y = 0; y < MapHeight; y++)
            {
                MapTile newTile = Instantiate(tilePrefab, _tileMapHolder.transform).GetComponent<MapTile>();
                Coord tileCoord = new Coord(x, y);
                newTile.Init(tileCoord);
                _map[x, y] = newTile;
                newTile.name = "MapTile[X: " + x + ", Y: " + y + "]";
            }
        }

        // TODO: Board Animation. Sine wave maybe?
    }

    public bool IsCoordContainedInMap(Coord coord)
    {
        return  0 <= coord.x && coord.x < MapWidth &&
                0 <= coord.y && coord.y < MapHeight;
    }
}

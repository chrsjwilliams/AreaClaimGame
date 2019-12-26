using UnityEngine;


[CreateAssetMenu (menuName = "Prefab DB")]
public class PrefabDB : ScriptableObject
{

    [SerializeField] private GameObject _player;
    public GameObject Player
    {
        get { return _player; }
    }

    [SerializeField] private GameObject[] _scenes;
    public GameObject[] Scenes
    {
        get { return _scenes; }
    }

    [SerializeField] private GameObject _mapTile;
    public GameObject MapTile
    {
        get { return _mapTile; }
    }

    [SerializeField] private Tile _playerTile;
    public Tile PlayerTile
    {
        get { return _playerTile; }
    }

    [SerializeField] private GameObject _pieceHolder;
    public GameObject PieceHolder
    {
        get { return _pieceHolder; }
    }


}

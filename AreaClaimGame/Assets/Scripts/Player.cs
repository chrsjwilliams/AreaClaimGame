using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private  Color[] _colorScheme;
    public Color[] ColorScheme
    {
        get { return _colorScheme; }
    }
    private int _playerNum;
    public int PlayerNumber
    {
        get { return _playerNum; }
    }

    /*
     *  TODO: Construct player hand and deck 
     * 
     */
    public List<Piece> hand;

    public void Init(int playerNumber, bool isAI)
    {
        _playerNum = playerNumber;
        if (PlayerNumber == 0)
        {
            _colorScheme = Services.GameManager.Player1ColorScheme;
        }
        else
        {
            _colorScheme = Services.GameManager.Player2ColorScheme;
        }
    }   

    // Update is called once per frame
    void Update()
    {
        
    }
}

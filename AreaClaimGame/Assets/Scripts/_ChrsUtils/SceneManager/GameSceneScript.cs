using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GameSceneScript : Scene<TransitionData>
{
    public bool gameOver;

    public static bool hasWon { get; private set; }

    public const int LEFT_CLICK = 0;
    public const int RIGHT_CLICK = 1;

    TaskManager _tm = new TaskManager();

    private int _turnNumber;
    public int TurnNumber
    {
        get { return _turnNumber; }
    }

    public Player[] players;
    private Player _currentPlayer;
    public Player CurrentPlayer
    {
        get { return _currentPlayer; }
    }

    public readonly Vector3[] cameraPositions = { new Vector3(2, 2, -10), new Vector3(2, 4, -10), new Vector3(2, 3, -10) };
    private float panSpeed = 3f;

    private int _touchID;
    public int TouchID
    {
        get { return _touchID; }
    }

    float t = 0;

    private void Start()
    {
        
    }

    internal override void OnEnter(TransitionData data)
    {
        Services.GameScene = this;
        gameOver = false;
        for(int i = 0; i < players.Length; i++)
        {
            players[i].Init(i, false);
        }
        _turnNumber = 0;
        Services.CameraController.SetScreenEdges();
        Services.MapManager.GenerateMap();
        _currentPlayer = players[0];
        Services.EventManager.Register<PlayMade>(OnPlayMade);
        _currentPlayer = players[0];
        _tm.Do(new LerpCameraToCurrentPlayer());
    }



    internal override void OnExit()
    {
        Services.GameScene = null;
    }


    public void SwapScene()
    {
        Services.AudioManager.SetVolume(1.0f);
        Services.Scenes.Swap<TitleSceneScript>();
    }

    public void SceneTransition()
    {
        _tm.Do
        (
            new ActionTask(SwapScene)
        );
    }

    private void EndGame()
    {
        Services.AudioManager.FadeAudio();

    }

    public void EndTransition()
    {

    }
    
	// Update is called once per frame
	void Update ()
    {
        _tm.Update();

        if (Input.GetKeyDown(KeyCode.M))
        {
            Piece p = new Piece(0, players[CurrentPlayer.PlayerNumber], 1);
            p.MakePhysicalPiece();
        }
	}

    public void OnPlayMade(PlayMade play)
    {
        _turnNumber++;
        _currentPlayer = players[TurnNumber % players.Length];

        _tm.Do(new LerpCameraToCurrentPlayer());
    }
}

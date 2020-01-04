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

    public TaskManager taskManager = new TaskManager();

    public int turnNumber{ get; private set; }

    public Player[] players;
    public Player currentPlayer { get; private set; }

    public readonly Vector3[] cameraPositions = { new Vector3(2, 1.25f, -10), new Vector3(2, 5f, -10), new Vector3(2, 3, -10) };
    private float panSpeed = 3f;

    public int touchID { get; private set; }

    private void Start()
    {
        
    }

    internal override void OnEnter(TransitionData data)
    {
        Services.GameScene = this;
        gameOver = false;
        Services.MapManager.GenerateMap();
        Services.CameraController.SetScreenEdges();

        for (int i = 0; i < players.Length; i++)
        {
            players[i].Init(i + 1, false);
        }
        turnNumber = 0;
        currentPlayer = players[0];
        Services.EventManager.Register<PlayMade>(OnPlayMade);
        taskManager.Do(new LerpCameraToCurrentPlayer());
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
        taskManager.Do
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
        taskManager.Update();
	}

    public void FloodFill(Coord coord, Player player, List<Coord> visitedCoords)
    {

        if (coord.x < 0 || coord.y < 0 || coord.x >= Services.MapManager.MapWidth || coord.y >= Services.MapManager.MapHeight) return;
        if (Services.MapManager.Map[coord.x, coord.y].isOccupied) return;
        if (visitedCoords.Contains(coord)) return;
        visitedCoords.Add(coord);
        Debug.Log(coord);
        Services.MapManager.Map[coord.x, coord.y].SpriteRenderer.color = player.colorScheme[2];

        FloodFill(new Coord(coord.x + 1, coord.y), player, visitedCoords);
        FloodFill(new Coord(coord.x - 1, coord.y), player, visitedCoords);
        FloodFill(new Coord(coord.x, coord.y + 1), player, visitedCoords);
        FloodFill(new Coord(coord.x, coord.y - 1), player,visitedCoords);

        return;
    }

    public void OnPlayMade(PlayMade play)
    {
        TaskQueue playMadeTasks = new TaskQueue();
        playMadeTasks.Add(new ParameterizedActionTask<Boolean>(currentPlayer.LockHand, true));
        playMadeTasks.Add(new ParameterizedActionTask<Vector3>(
                                currentPlayer.DrawPieceTask,
                                currentPlayer.pieceSpawnPosition.position));
        //Debug.Log(play.piece.centerTile.coord);
        FloodFill(play.piece.centerTile.coord, currentPlayer, new List<Coord>());
        turnNumber++;
        currentPlayer = players[turnNumber % players.Length];

        
        playMadeTasks.Add(new LerpCameraToCurrentPlayer());
        playMadeTasks.Add(new ParameterizedActionTask<Boolean>(currentPlayer.LockHand, false));

        taskManager.Do(playMadeTasks);
    }
}

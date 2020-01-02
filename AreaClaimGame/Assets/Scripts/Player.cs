using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Color[] colorScheme { get; private set; }
    public int playerNum { get; private set; }

    /*
     *  TODO:   Construct player hand and deck 
     *          Dynamically adjust pice strength
     * 
     */
    public int maxHandSize = 3;
    public int startingHandSize = 3;
    public Vector3 handOffset;
    public Vector2 handSpacing;
    public int piecesPerHandRow = 1;
    public List<Piece> hand;
    public Vector3[] handTargetPositions;
    public Piece selectedPiece;
    public int selectedPieceHandPos;

    private ShuffleBag<Piece> _pieceDeck;

    public List<Piece> boardPieces { get; protected set; }
    public Transform pieceSpawnPosition;

    public void Init(int playerNumber, bool isAI)
    {
        playerNum = playerNumber;
        if (this.playerNum == 0)
        {
            colorScheme = Services.GameManager.Player1ColorScheme;
        }
        else
        {
            colorScheme = Services.GameManager.Player2ColorScheme;
        }

        hand = new List<Piece>();
        boardPieces = new List<Piece>();
        InitDeck();
        DrawPieces(startingHandSize, pieceSpawnPosition.position, true);
        OrganizeHand(hand, true);
        
    }  

    private void InitDeck()
    {
        _pieceDeck = new ShuffleBag<Piece>(Piece.piece.GetLength(0));
        Piece[] pieceExamples = new Piece[Piece.piece.GetLength(0)];
        for(int i = 0; i < Piece.piece.GetLength(0); i++)
        {
            pieceExamples[i] = new Piece(i, this, 1);
            _pieceDeck.Add(pieceExamples[i], 1);
        }    
    }

    public void DrawPieces(int numPiecesToDraw, Vector3 startPos, bool instantly = false)
    {
        int handSpace = maxHandSize - hand.Count;
        if (selectedPiece != null) handSpace -= 1;
        int numPiecesToBurn = Mathf.Max(numPiecesToDraw - handSpace, 0);
        for(int i = numPiecesToBurn - 1; i >= 0; i--)
        {
            BurnFromHand(hand[i]);
        }
        for(int i = 0; i < numPiecesToDraw; i++)
        {
            DrawPiece(startPos, instantly);
        }
    }



    public virtual void DrawPiece(Piece piece)
    {
        piece.MakePhysicalPiece();
        AddPieceToHand(piece);
    }

    public virtual void DrawPieceTask(Vector3 startPos)
    {
        Piece piece = _pieceDeck.Next();
        Task drawTask = new DrawNewPiece(piece, startPos, this);
        Services.GameScene.taskManager.Do(drawTask);
        QueueUpNextPiece();
    }

    public virtual void DrawPiece(Vector3 startPos, bool instantly = false)
    {
        Piece piece = _pieceDeck.Next();

        if (instantly)
        {
            DrawPiece(piece);
        }
        else
        {
            Task drawTask = new DrawNewPiece(piece, startPos, this);
            Services.GameScene.taskManager.Do(drawTask);
            QueueUpNextPiece();
        }
    }

    public void AddPieceToHand(Piece piece)
    {
        hand.Add(piece);
        OrganizeHand(hand);
    }

    public void InsertPieceIntoHand(Piece piece, int index)
    {
        hand.Insert(index, piece);
        OrganizeHand(hand);
    }

    protected virtual void BurnFromHand(Piece piece)
    {
        piece.BurnFromHand();
        hand.Remove(piece);
    }

    private void QueueUpNextPiece()
    {
        /*
        Piece nextPiece = _pieceDeck.Next();

        nextPiece.MakePhysicalPiece();
        Vector3 position;
        position = 
        */
    }

    public void OrganizeHand(List<Piece> heldPieces, bool instant = false)
    {
        bool emptySpace = false;
        if(selectedPiece != null)
        {
            emptySpace = true;
        }
        handTargetPositions = new Vector3[maxHandSize];
        for(int i = 0; i < maxHandSize; i++)
        {
            int handPos = i;
            if(emptySpace && 1 > selectedPieceHandPos)
            {
                handPos -= 1;
            }
           
                Vector3 newPos = GetHandPosition(i);
                if (instant) heldPieces[handPos].holder.Reposition(newPos, true);
                handTargetPositions[i] = newPos;

            
        }
    }

    public Vector3 GetHandPosition(int handIndex)
    {
        Vector3 offset = handOffset;
        float spacingMultiplier = 1;
        if(playerNum == 1)
        {
            spacingMultiplier = -1;
            offset = new Vector3(-handOffset.x, -handOffset.y, handOffset.z);
        }

        offset += Services.CameraController.screenEdges[playerNum];
        offset = new Vector3(offset.x, offset.y, 0);

        Vector3 newPos = new Vector3(handSpacing.x * (handIndex % piecesPerHandRow) * spacingMultiplier,
                                     handSpacing.y * (handIndex / piecesPerHandRow) * spacingMultiplier,
                                     0)
                                     + offset;
        return newPos;
    }

    public virtual void OnPieceSelected(Piece piece)
    {
        if (selectedPiece != null) CancelSelectedPiece();

        selectedPiece = piece;
        for(int i = 0; i < hand.Count; i++)
        {
            if(hand[i] == selectedPiece)
            {
                selectedPieceHandPos = i;
                Debug.Log(i);
                break;
            }
        }
        hand.Remove(piece);

        OrganizeHand(hand);
    }

    public virtual void CancelSelectedPiece()
    {
        if (selectedPiece == null) return;

        int handPosToPlace = selectedPieceHandPos;
        hand.Insert(handPosToPlace, selectedPiece);
        selectedPiece = null;
        OrganizeHand(hand);
    }

    public virtual void OnPiecePlaced(Piece piece)
    {
        selectedPiece = null;
        OrganizeHand(hand);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && playerNum == 0)
        {
            hand[0].holder.Reposition(Vector3.zero, true);
        }
    }
}

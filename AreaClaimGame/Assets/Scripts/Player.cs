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
    protected List<Vector3> handTargetPositions;
    public Piece selectedPiece;
    private int selectedPieceHandPos;

    private ShuffleBag<Piece> _pieceDeck;

    public List<Piece> boardPieces { get; protected set; }

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
        DrawPieces(startingHandSize);
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

    public void DrawPieces(int numPiecesToDraw)
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
            DrawPiece();
        }
    }

    public virtual void DrawPiece(Piece piece)
    {
        piece.MakePhysicalPiece();
        AddPieceToHand(piece);
    }

    public virtual void DrawPiece()
    {
        Piece piece = _pieceDeck.Next();
        DrawPiece(piece);
    }

    public void AddPieceToHand(Piece piece)
    {
        hand.Add(piece);
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
        int provisionalHandCount = heldPieces.Count;
        bool emptySpace = false;
        if(selectedPiece != null)
        {
            provisionalHandCount += 1;
            emptySpace = true;
        }
        handTargetPositions = new List<Vector3>();
        for(int i =0; i < provisionalHandCount; i++)
        {
            int handPos = i;
            if(emptySpace && 1> selectedPieceHandPos)
            {
                handPos -= 1;
            }
            if(!emptySpace || (emptySpace && i != selectedPieceHandPos))
            {
                Vector3 newPos = GetHandPosition(i);
                if (instant) heldPieces[handPos].holder.Reposition(newPos, true);
                handTargetPositions.Add(newPos);
            }
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
                break;
            }
        }
        hand.Remove(piece);

        OrganizeHand(hand);
    }

    public virtual void CancelSelectedPiece()
    {
        if (selectedPiece == null) return;

        int handPosToPlace = Mathf.Min(selectedPieceHandPos, hand.Count);
        hand.Insert(handPosToPlace, selectedPiece);
        selectedPiece = null;
        OrganizeHand(hand);
        hand.Insert(handPosToPlace, selectedPiece);
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

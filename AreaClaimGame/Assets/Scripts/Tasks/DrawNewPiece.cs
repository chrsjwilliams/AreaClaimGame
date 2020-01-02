using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasingEquations;

public class DrawNewPiece : Task
{
    private Piece piece;
    private Vector3 startPos;
    private Vector3 targetPos;
    private Vector3 startScale;
    private Vector3 targetScale;
    private float timeElapsed;
    private float duration;
    private int selectedPieceIndex;

    public DrawNewPiece(Piece piece_, Vector3 startPos_, Player player = null)
    {
        piece = piece_;
        startPos = startPos_;
        selectedPieceIndex = piece.owner.selectedPieceHandPos;
    }

    protected override void Init()
    {
        timeElapsed = 0;
        piece.MakePhysicalPiece();
        piece.holder.Reposition(startPos);
        startPos = piece.holder.transform.position;
        duration = 0.5f;
        targetPos = piece.owner.GetHandPosition(piece.owner.selectedPieceHandPos) -
                        (piece.holder.GetCenterpoint() * PieceHolder.unselecetdScale.x);
        
        startScale = piece.holder.transform.localScale;
        targetScale = PieceHolder.unselecetdScale;
    }

    internal override void Update()
    {
        timeElapsed += Time.deltaTime;

        piece.holder.Reposition(Vector3.Lerp(startPos, targetPos,
                                 Easing.QuartEaseOut(timeElapsed / duration)));
        piece.holder.transform.localScale = Vector3.Lerp(startScale, targetScale,
                                  Easing.ExpoEaseOut(timeElapsed / duration));

        if (timeElapsed >= duration) SetStatus(TaskStatus.Success);
    }

    protected override void OnSuccess()
    {
        piece.owner.InsertPieceIntoHand(piece, selectedPieceIndex);
        piece.OnDrawn();
    }
}

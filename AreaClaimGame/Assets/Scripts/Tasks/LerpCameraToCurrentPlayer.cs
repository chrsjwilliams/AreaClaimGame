using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasingEquations;

public class LerpCameraToCurrentPlayer : Task
{

    private Vector3 startPos;
    private Vector3 targetPos;
    private float timeElapsed;
    private float duration = 1f;


    public LerpCameraToCurrentPlayer()
    {
        startPos = Services.GameManager.MainCamera.transform.position;
        targetPos = Services.GameScene.cameraPositions[Services.GameScene.CurrentPlayer.playerNum];

    }

    protected override void Init()
    {
        timeElapsed = 0;
    }

    internal override void Update()
    {
        timeElapsed += Time.deltaTime;
        Vector3 lerpPos = Vector3.Lerp(startPos, targetPos,
                                       Easing.QuartEaseOut(timeElapsed / duration));
        Services.GameManager.MainCamera.transform.position = lerpPos;
        if (timeElapsed >= duration) SetStatus(TaskStatus.Success);
    }

    protected override void OnSuccess()
    {
        Services.GameManager.MainCamera.transform.position = targetPos;
    }

    protected override void OnAbort()
    {
        base.OnAbort();

    }

    protected override void CleanUp()
    {
        base.CleanUp();
    }
}

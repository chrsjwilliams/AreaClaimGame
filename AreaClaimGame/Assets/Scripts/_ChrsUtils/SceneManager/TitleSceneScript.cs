using UnityEngine;
using UnityEngine.UI;

public class TitleSceneScript : Scene<TransitionData>
{
    public KeyCode startGame = KeyCode.Space;

    [SerializeField]private float SECONDS_TO_WAIT = 0.1f;

    private TaskManager _tm = new TaskManager();

    private Text title;
    private Text click;


    internal override void OnEnter(TransitionData data)
    {
        title = GameObject.Find("TITLE").GetComponent<Text>();
        //click = GameObject.Find("Click").GetComponent<Text>();

        /*
        _tm.Do
        (

                        new WaitTask(SECONDS_TO_WAIT))
               .Then(new LERPColor(title, white, fontColor, 0.5f))
               .Then(new LERPColor(click, white, fontColor, 0.5f)
        );
        */

    }

    internal override void OnExit()
    {

    }

    private void StartGame()
    {
        /*
        _tm.Do
        (
               
                        new WaitTask(SECONDS_TO_WAIT))
               .Then(   new LERPColor(click,fontColor, white, 0.5f))
               .Then(   new LERPColor(title,fontColor, white, 0.5f))
               .Then(   new WaitTask(SECONDS_TO_WAIT))
              .Then(new ActionTask(ChangeScene)
        );
        */
    }

    private void TitleTransition()
    {

    }

    private void ChangeScene()
    {
        Services.Scenes.Swap<GameSceneScript>();
    }

    private void Update()
    {
        _tm.Update();
        if (Input.GetKeyDown(startGame) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            Services.AudioManager.PlayClip(Clips.CLICK);
            StartGame();
        }
    }
}

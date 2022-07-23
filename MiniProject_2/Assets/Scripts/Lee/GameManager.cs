//using System.Reflection.PortableExecutable;
//using System.ComponentModel.DataAnnotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private int stage;
    private int totalStage = 2;

    public Timer timer;
    public SoccerBall soccerBall;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        NextStage();
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void NextStage()
    {
        if(timer.LimitTime > 0)
        {
            if(soccerBall.isGoal == true){
                if(SceneManager.GetActiveScene().buildIndex < 2){
                    Debug.Log("stage:" + SceneManager.GetActiveScene().buildIndex);
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                }else{
                    SceneManager.LoadScene(4);  // SuccessEnding Scene
                    Invoke("ExitGame", 5);
                }
            }
        }
        else{
            Debug.Log("soccerBall.isGoal: " + soccerBall.isGoal);
            Debug.Log("timer.LimitTime: " + timer.LimitTime);
            SceneManager.LoadScene(3);  // FailureEnding Scene
            Invoke("ExitGame", 5);  // 5초뒤 exitgame 호출
        }
    }

    public void OnClickStartButton()
    {
        stage = 1;
        SceneManager.LoadScene(1);
    }
}

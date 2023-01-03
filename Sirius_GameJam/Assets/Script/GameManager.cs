using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Player player;

    void Awake()
    {
        instance = this;
    }

    public void LoadGameScene(){
        SceneManager.LoadScene("SampleScene");
    }

    public void ExitGame(){
        Application.Quit();
    }
}

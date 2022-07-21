using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int totalItem;   // 총 아이템 수
    public int currentItem; // 현재까지 먹은 아이템 수
    public int totalTarget;
    public int currentTarget;
    public int stage;
    public int lastStage = 1;
    public Text totalItemText;
    public Text currentItemText;
    // Start is called before the first frame update
    void Awake()
    {
        totalItemText.text = "/ " + totalItem.ToString();
    }

    // Update is called once per frame
    public void GetItem()
    {
        currentItemText.text = currentItem.ToString();
    }

    public void ExitGame(){
        Debug.Log("ExitGame");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }


    public void NextStage(){
        if(totalItem == currentItem){
            SceneManager.LoadScene(stage + 1);
        }
    }
}

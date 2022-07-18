using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int totalItem;   // 총 아이템 수
    public int currentItem; // 현재까지 먹은 아이템 수
    public int stage;
    public Text totalItemText;
    public Text currentItemText;
    // Start is called before the first frame update
    void Awake()
    {
        totalItemText.text = "/" + totalItem.ToString();
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
}

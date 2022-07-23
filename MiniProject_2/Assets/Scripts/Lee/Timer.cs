using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float LimitTime;
    public Text textTimer;
    public Slider TimeSlider;
    // Start is called before the first frame update
    void Start()
    {
        LimitTime = 30.0f;
    }

    // Update is called once per frame
    void Update()
    {
        LimitTime -= Time.deltaTime;
        textTimer.text = "Remaining Time : " + Mathf.Round(LimitTime);
        ShowScrollBar();
    }

    public void ShowScrollBar() {
        TimeSlider.value = LimitTime / 30.0f;
    }
}

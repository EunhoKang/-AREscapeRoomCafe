using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    [HideInInspector]public float time;

    private void Start()
    {
        time = 1500f;
    }

    private void Update()
    {
        if (time > 0)
            time -= Time.deltaTime;

        timeText.text = $"남은 시간: {Mathf.Ceil(time).ToString()}초";
        if (time <=0)
        {
            //Timeout
        } 
    }
}

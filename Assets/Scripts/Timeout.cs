using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Timeout : MonoBehaviour
{
    public Text Timer;
    public GameObject self;
    void Update()
    {
        if (Timer.text == "0")
        {
            Debug.Log("Time OUT!");
            self.SetActive(false);
        }       
    }   
}

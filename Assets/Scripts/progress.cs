using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class progress : MonoBehaviour
{
    public Scrollbar Progressbar;
    float STARTVALUE = 0;
    public float Value;

    void Start()
    {
        Progressbar.value = STARTVALUE;
    }

    private void Update()
    {
        Progressbar.value = Value;
    }
}

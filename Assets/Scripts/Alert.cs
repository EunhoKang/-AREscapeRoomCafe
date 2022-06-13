using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Alert : MonoBehaviour
{
    public TextMeshProUGUI tmp;
    public void SetText(string text){
        tmp.text=text;
    }
}

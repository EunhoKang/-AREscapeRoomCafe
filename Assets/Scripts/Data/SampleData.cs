using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SampleData
{
    public string playerName;
    public List<string> dataSet=new List<string>();
    public SampleData(string _name, List<string> _set){
        playerName=_name;
        dataSet=new List<string>(_set);
    } 
}

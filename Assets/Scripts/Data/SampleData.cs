using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SampleData
{
    public string playerName;
    public List<double> dataSet=new List<double>();
    public SampleData(string _name, List<double> _set){
        playerName=_name;
        dataSet=new List<double>(_set);
    } 
}

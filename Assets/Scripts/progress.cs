using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Progress : MonoBehaviour
{
    public List<Sprite> sprites=new List<Sprite>();
    public Image img;
    [HideInInspector]public int lastidx=0;
    public void SetProgress(int index){
        if(lastidx>=index)return;
        img.sprite=sprites[index];
        lastidx=index;
    }
}

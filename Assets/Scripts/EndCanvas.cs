using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class EndCanvas : MonoBehaviour
{
    public TextMeshProUGUI txt;

    public void GameEnd(bool isWin){
        if(isWin)txt.text="탈출에 성공했습니다!";
        else txt.text="탈출에 실패했습니다.";
    }
}

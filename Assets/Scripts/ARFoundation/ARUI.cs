using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class ARUI : MonoBehaviour
{
    [HideInInspector]public List<bool> inventory=new List<bool>(){false,false,false,false,false,false};
    public Alert alert;
    public ItemExplan exp;
    public EndCanvas endCanvas;
    public Progress pro;
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
            GameEnd(false);
        } 
    }

    public void InventoryChange(int index){
        inventory[index]=true;
        alert.SetText(exp.strs[index]);
        exp.SetButton(index);
        pro.SetProgress(index);
    }
    public void RankWrite(float score){
        RealTimeDataManager.manager.ReadUserData();
        var sample=new SampleData(AuthManager.manager.auth.CurrentUser.Email, score);
        if(RealTimeDataManager.manager.dataList.FindIndex(x=>x.playerName==sample.playerName)>=0){
            RealTimeDataManager.manager.dataList.RemoveAll(x=>x.playerName==sample.playerName);
        }
        RealTimeDataManager.manager.dataList.Add(sample);
        RealTimeDataManager.manager.PostObject<List<SampleData>>($"users", 
            RealTimeDataManager.manager.dataList,() => {}, Debug.Log);
    }
    public void GameEnd(bool isWin){
        if(isWin){
            RankWrite(time);
        }
        endCanvas.gameObject.SetActive(true);
        endCanvas.GameEnd(isWin);
    }
}

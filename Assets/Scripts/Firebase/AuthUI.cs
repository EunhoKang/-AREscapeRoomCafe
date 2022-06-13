using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using TMPro;
public class AuthUI : MonoBehaviour
{
    [SerializeField] TMP_InputField emailField;
    [SerializeField] TMP_InputField passField;
    [SerializeField] TextMeshProUGUI rank;
    [SerializeField] TextMeshProUGUI profile;
    public void SignUp(){
        AuthManager.manager.SignUp(emailField.text,passField.text,()=>{
            Debug.Log(emailField.text + "로 회원가입\n");
        },Debug.Log);
    }
    public void SignIn(){
        AuthManager.manager.SignIn(emailField.text,passField.text,()=>{
            Debug.Log(emailField.text + " 로 로그인 하셨습니다.");
        },Debug.Log);
        RealTimeDataManager.manager.ReadUserData();
        SetProfile();
    }
    public void Sample_RankUpdate(){
        RealTimeDataManager.manager.ReadUserData();
        var sample=new SampleData(AuthManager.manager.auth.CurrentUser.Email, 123.45f);
        if(RealTimeDataManager.manager.dataList.FindIndex(x=>x.playerName==sample.playerName)>=0){
            RealTimeDataManager.manager.dataList.RemoveAll(x=>x.playerName==sample.playerName);
        }
        RealTimeDataManager.manager.dataList.Add(sample);
        RealTimeDataManager.manager.PostObject<List<SampleData>>($"users", 
            RealTimeDataManager.manager.dataList,() => {}, Debug.Log);
        SceneManager.LoadSceneAsync("AR");
    }
    public void Sample_ReadRank(){
        RealTimeDataManager.manager.ReadUserData();
        string txt="";
        RealTimeDataManager.manager.dataList.OrderByDescending(x=>x.data);
        for(int i=0;i<RealTimeDataManager.manager.dataList.Count;++i){
            txt+=$"{i+1}위: {string.Format("{0:0.###}",RealTimeDataManager.manager.dataList[i].data)} - {RealTimeDataManager.manager.dataList[i].playerName}\n";
        }
        rank.text=txt;
    }
    public void SetProfile(){
        SampleData s=RealTimeDataManager.manager.dataList.Find(x=>x.playerName==AuthManager.manager.auth.CurrentUser.Email);
        string txt="";
        txt=$"Email:\t{s.playerName}\n최고기록:\t{s.data}";
        profile.text=txt;
    }
}

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
    public void SignUp(){
        AuthManager.manager.SignUp(emailField.text,passField.text,()=>{
            Debug.Log(emailField.text + "로 회원가입\n");
        },Debug.Log);
    }
    public void SignIn(){
        AuthManager.manager.SignIn(emailField.text,passField.text,()=>{
            Debug.Log(emailField.text + " 로 로그인 하셨습니다.");
        },Debug.Log);
    }
    public void Sample_RankUpdate(){
        RealTimeDataManager.manager.ReadUserData();
        var sample=new SampleData(AuthManager.manager.auth.CurrentUser.Email, new List<double>(){0.1,0.2});
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
    }
}

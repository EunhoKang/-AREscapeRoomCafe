using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using TMPro;
public class AuthUI : MonoBehaviour
{
    [SerializeField] GameObject logo;
    [SerializeField] Image logoImage;
    [SerializeField] GameObject login;
    [SerializeField] TMP_InputField emailField;
    [SerializeField] TMP_InputField passField;
    [SerializeField] TextMeshProUGUI rank;
    [SerializeField] TextMeshProUGUI profile;
    private void Start() {
        StartCoroutine(StartCor());
    }
    IEnumerator StartCor(){
        for(float i=0;i<=1;i+=0.1f){
            logoImage.color=new Color(1,1,1,i);
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(0.8f);
        logo.SetActive(false);
        login.SetActive(true);
    }
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
        txt=$"Email:\t{s.playerName}\n최고기록:\t{string.Format("{0:0.###}",s.data)}";
        profile.text=txt;
    }
}

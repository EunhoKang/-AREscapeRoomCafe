using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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
            RealTimeDataManager.manager.PostObject<SampleData>($"users/{AuthManager.manager.auth.CurrentUser.Email}", 
            new SampleData("eunho", new List<double>(){1,2}),() => {}, Debug.Log);
        },Debug.Log);
        //SceneManager.LoadSceneAsync("AR");
    }
    public void Sample_RankUpdate(){
        var sample=new SampleData(AuthManager.manager.auth.CurrentUser.DisplayName, new List<double>(){0.1f,0.2f});
        RealTimeDataManager.manager.PostObject<SampleData>($"rank/{AuthManager.manager.auth.CurrentUser.Email}/", sample,
            () => {}, Debug.Log);
    }
    public void Sample_ReadRank(){
        //
    }
}

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
    public void SignUp(){
        AuthManager.manager.SignUp(emailField.text,passField.text,()=>{
            Debug.Log(emailField.text + "로 회원가입\n");
        },Debug.Log);
    }
    public void SignIn(){
        AuthManager.manager.SignIn(emailField.text,passField.text,()=>{
            Debug.Log(emailField.text + " 로 로그인 하셨습니다.");
        },Debug.Log);
        SceneManager.LoadSceneAsync("AR");
    }
}

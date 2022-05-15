using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AuthUI : MonoBehaviour
{
    public TMP_InputField id;
    public TMP_InputField pw;
    public void SignIn(){
        AuthManager.manager.SignInUser(id.text,pw.text,()=>{Debug.Log("Sign In Success");},Debug.Log);
    }
    public void SignUp(){
        AuthManager.manager.SignUpUser(id.text,pw.text,()=>{Debug.Log("Sign Up Success");},Debug.Log);
    }
}

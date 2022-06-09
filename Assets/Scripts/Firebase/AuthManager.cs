using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Auth;
using Firebase;
using System;

public class AuthManager : MonoBehaviour
{
    public static AuthManager manager=null;
    Firebase.FirebaseApp app;
    public Firebase.Auth.FirebaseAuth auth;
    public Firebase.Auth.FirebaseUser user;

    void Awake()
    { 
        if(manager==null)manager=this;
        else if(manager!=this)Destroy(gameObject);
        DontDestroyOnLoad(this);
    }
    private void Start() {
        InitializeFirebase();
    }
    void InitializeFirebase() {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available) {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                app = Firebase.FirebaseApp.DefaultInstance;
                auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
                auth.StateChanged += AuthStateChanged;
                AuthStateChanged(this, null);

                // Set a flag here to indicate whether Firebase is ready to use by your app.
            } else {
                UnityEngine.Debug.LogError(System.String.Format(
                "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs) {
        if (auth.CurrentUser != user) {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null) {
            Debug.Log("Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn) {
                Debug.Log("Signed in " + user.UserId);
            }
        }
    }
    public void SignUp(string id, string pw, Action callback, Action<AggregateException> fallback){
        auth.CreateUserWithEmailAndPasswordAsync(id, pw).ContinueWith(
            task => {
                if (!task.IsCanceled && !task.IsFaulted)callback();
                else fallback(task.Exception);
            }
        );
    }
    public void SignIn(string id, string pw, Action callback, Action<AggregateException> fallback){
        auth.SignInWithEmailAndPasswordAsync(id, pw).ContinueWith(
            task => {
                if (!task.IsCanceled && !task.IsFaulted)callback();
                else fallback(task.Exception);
            }
        );
    }
    public FirebaseUser GetUser(){
        return user;
    }
}

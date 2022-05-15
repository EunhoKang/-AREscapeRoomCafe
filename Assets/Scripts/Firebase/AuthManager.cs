using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase.Auth;

public class AuthManager : MonoBehaviour
{
    // 싱글톤 객체 생성
    public static AuthManager manager = null;

    [HideInInspector]
    private FirebaseAuth auth;
    private User user;

    // 싱글톤 정의
    public void Awake(){
        if (manager == null){ // 객체가 지정 안되어있으면
            manager = this; // 지정
        }
        else if (manager != this){ // 지정은 되어있는데 또 객체 생성되면
            Destroy(this.gameObject); // 해당 객체 공중분해
            Destroy(this);
        }
        DontDestroyOnLoad(this);

        auth = FirebaseAuth.DefaultInstance;
    }

    //(추가) UI 핸들러에서 로그인이 완료되었는지 확인하여 다음 UI으로 넘어갈 수 있도록 해줌
    [HideInInspector]public bool signInComplete = false;
    //(추가) Auth 씬이 언로드 되고 메뉴 씬이 로드되었는지를 체크해줌
    [HideInInspector]public bool isMainSceneLoaded = false;
    //(추가) 로그인한 유저의 닉네임을 저장함. 이후 게임에서 로컬 조작에 사용됨
    [HideInInspector]public string currentLocalPlayerId; 
    [HideInInspector]public bool isSignIn=false;

    //(추가) 유저를 만들고, 성공하면 매개변수 callback으로 받은 함수를 수행함
    public void SignUpUser(string email, string password, Action callback, Action<AggregateException> fallback){
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if(task.IsCanceled || task.IsFaulted) fallback(task.Exception);
            else callback();
        });
    }

    //(추가) 로그인을 시도하고, 성공하면 매개변수 callback으로 받은 함수를 수행함
    public void SignInUser(string email, string password, Action callback, Action<AggregateException> fallback){
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            isSignIn=true;
            if(task.IsCanceled || task.IsFaulted) fallback(task.Exception);
            else callback();
        });
    }

    //(추가) 서버 상으로 로그아웃을 진행함
    public void SignOut() => auth.SignOut();

    //(추가) 다른 코드에서 유저의 이메일 ID를 가져올 수 있도록 해줌
    public string GetUserId(){
        return auth.CurrentUser.UserId;
    }

    //(추가) 다른 코드에서 유저 변수 자체를 가져올 수 있도록 해줌
    public User GetUser(){
        return user;
    }

    //(추가) 현재 로컬의 유저 변수에 매개변수로 받은 user 대입
    public void SetUser(User user){
        this.user = user;
    }

    //(추가) 현 로컬에 로그인한 유저의 닉네임을 매니저에 따로 저장해줌
    public void SetLocalPlayerId(){
        currentLocalPlayerId = user.nickname;
    }

    //(추가) 현 로컬의 유저 닉네임을 가져옴
    public string GetUserNickname(){
        return user.nickname;
    }
}
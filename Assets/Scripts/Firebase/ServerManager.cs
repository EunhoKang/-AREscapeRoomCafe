/*
using System; // EventSystem 사용 목적
using System.Linq; // Aggregate 사용 목적
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Extensions;
using Firebase.Database;

public class ServerManager : MonoBehaviour
{
    // 싱글톤 객체 생성
    public static ServerManager manager = null;

    [HideInInspector]
    public FirebaseApp app;

    // 라이브러리를 통해 불러온 FirebaseDatabase 관련 객체를 선언해서 사용
    public DatabaseReference reference;
    
    //(추가) 파이어베이스의 초기화가 완료되었는지 확인
    public bool isFirebaseInitialized = false;

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

        // 파이어베이스 메소드 호출 전 Google Play 서비스 확인
        InitailizeFirebase();
    }

    //(추가) 메세지가 서버에 새로 등록되었는지 확인하는 이벤트 핸들러
    private EventHandler<ChildChangedEventArgs> messageListener;

    //(추가) 기초 파이어베이스 초기화
    void InitailizeFirebase(){
        //(추가) 파이어베이스 SDK에서 다른 메소드를 호출하기 전에 Google Play 서비스를 확인
        // 필요한 경우 SDK에 필요한 버전으로 업데이트를 해야함
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available){
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.

                app = FirebaseApp.DefaultInstance;
                
                // (추가)하단 주석은 이전 버전의 파이어베이스 SDK에서 사용하던 코드로,
                // 현재 파이어베이스 리얼타임 데이터베이스의 URL은 Assets/google-services.json에서 관리함
                //FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://illisoft-climateandpeople-default-rtdb.firebaseio.com/");
                reference = FirebaseDatabase.DefaultInstance.RootReference;

                isFirebaseInitialized = true;
                Debug.Log("Firebase Initialize Complete");

                // Set a flag here to indicate whether Firebase is ready to use by your app.
            } else {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });

        // 파이어베이스 로딩이 다 되면 다음 씬으로 전환
        SceneManager.LoadSceneAsync("Auth");
    }

    //(추가) 입력받은 레퍼런스의 경로를 분석하고 잘라서 지정한 데이터베이스 상의 경로에 접근할 수 있도록 함
    public DatabaseReference GetReferenceFromPath(string path){
        var splitPath = path.Split('/');
        return splitPath.Aggregate(reference, (current, child) => current.Child(child));
    }

    // 주어진 원시 JSON값을 이용해 주어진 경로에 데이터를 씀
    //(추가) 매개변수 json에 "null"을 넣어줄 경우 그 경로의 값을 삭제하는 기능이 있음
    public void PostJSON(string path, string json, Action callback, Action<AggregateException> fallback){
        var customReference = GetReferenceFromPath(path);

        customReference.SetRawJsonValueAsync(json).ContinueWith(task =>
        {
            if(task.IsCanceled){
                Debug.LogError("PostJSON was canceled.");
                fallback(task.Exception);
                return;
            }
            else if(task.IsFaulted){
                Debug.LogError("PostJSON encountered an error : " + task.Exception);
                fallback(task.Exception);
                return;
            }
            callback();
        });
    }

    public void DeleteData(string path,Action callback, Action<AggregateException> fallback){
        var customReference = GetReferenceFromPath(path);
        customReference.RemoveValueAsync().ContinueWith(task =>{
            if(task.IsCanceled){
                Debug.LogError("PostJSON was canceled.");
                fallback(task.Exception);
                return;
            }
            else if(task.IsFaulted){
                Debug.LogError("PostJSON encountered an error : " + task.Exception);
                fallback(task.Exception);
                return;
            }

            callback();
        });
    }

    // 주어진 데이터를 JSON 형태로 직렬화하여 데이터베이스에 데이터를 씀
    public void PostObject<T>(string path, T obj, Action callback, Action<AggregateException> fallback){
        PostJSON(path, StringSerializationAPI.Serialize(typeof(T),obj), callback, Debug.Log);
    }

    // 주어진 JSON값을 "고유 Key값 형태로" 데이터 목록에 추가함
    public void PushJSON(string path, string json, Action callback, Action<AggregateException> fallback){
        var customReference = GetReferenceFromPath(path);

        customReference.Push().SetRawJsonValueAsync(json).ContinueWith(task => {
            if(task.IsCanceled){
                Debug.LogError("PushJSON was canceled.");
                fallback(task.Exception);
                return;
            }
            else if(task.IsFaulted){
                Debug.LogError("PushJSON encountered an error : " + task.Exception);
                fallback(task.Exception);
                return;
            }
            callback();
        });
    }

    // 주어진 데이터를 JSON 값으로 직렬화하여 데이터 목록에 추가함
    public void PushObject<T>(string path, T obj, Action callback, Action<AggregateException> fallback){
        PushJSON(path, StringSerializationAPI.Serialize(typeof(T),obj), callback, Debug.Log);
    }

    // 데이터베이스에서 가져온 JSON 값을 역직렬화하여 원하는 데이터의 형태로 가져온 뒤 callback에 넣어줌 
    // 여기서 json => {}는 람다 함수
    // DataSnapshot json을 매개변수로 한 람다 함수 내부에서 GetRawJsonValue()를 통해 원하는 값을 가져오고,
    // 이를 형식 매개 변수로 받은 변수형으로 전환한 뒤 callback에 매개변수로 넣어주어 실행하는 매커니즘
    public void GetObject<T>(string path, Action<T> callback, Action<AggregateException> fallback){
        GetJSON(path, json => {
            callback((T)StringSerializationAPI.Deserialize(typeof(T),json.GetRawJsonValue()));
        }, Debug.Log);
    }

    // JSON 값을 주어진 데이터베이스 경로에서 가져온 뒤 작업 결과값을 callback에 넣어줌
    public void GetJSON(string path, Action<DataSnapshot> callback, Action<AggregateException> fallback){
        var customReference = GetReferenceFromPath(path);

        customReference.GetValueAsync().ContinueWith(task => {
            if(task.IsCanceled){
                Debug.LogError("GetJSON was canceled.");
                fallback(task.Exception);
                return;
            }
            else if(task.IsFaulted){
                Debug.LogError("GetJSON encountered an error : " + task.Exception);
                fallback(task.Exception);
                return;
            }

            callback(task.Result);
        });
    }

    // Child의 Add, Change, Remove에 대한 이벤트 리스너 정의
    // 각각 리스너 추가, 리스너 중단 메소드를 갖고 있음

    //(추가) 지정한 경로의 Child 브랜치에 값이 생성되는 것을 감지하여
    // 그 생성된 값을 매개변수로 callback 함수인 onChildAdded 실행
    public KeyValuePair<DatabaseReference, EventHandler<ChildChangedEventArgs>> ListenForChildAdded(
        string path, Action<ChildChangedEventArgs> onChildAdded, Action<AggregateException> fallback) {
            var customReference = GetReferenceFromPath(path);

            void CurrentListener(object o, ChildChangedEventArgs args){
                if (args.DatabaseError != null){
                    fallback(new AggregateException(new Exception(args.DatabaseError.Message)));
                    Debug.LogError(args.DatabaseError.Message);
                    return;
                }
                onChildAdded(args);
            }

            var listenerPair = new KeyValuePair<DatabaseReference, EventHandler<ChildChangedEventArgs>>(
                customReference, CurrentListener);
            customReference.ChildAdded += CurrentListener;

            return listenerPair;
    }

    //(추가) 지정한 경로의 Child 브랜치에 값이 생성되는 것을 감지하는 것을 멈춤
    public void StopListeningForChildAdded(KeyValuePair<DatabaseReference, EventHandler<ChildChangedEventArgs>> listener){
        listener.Key.ChildAdded -= listener.Value;
    }

    //(추가) 지정한 경로의 Child 브랜치에 값이 삭제되는 것을 감지하여
    // 그 삭제될 값을 매개변수로 callback 함수인 onChildRemoved 실행
    public KeyValuePair<DatabaseReference, EventHandler<ChildChangedEventArgs>> ListenForChildRemoved(
        string path, Action<ChildChangedEventArgs> onChildRemoved, Action<AggregateException> fallback) {
            var customReference = GetReferenceFromPath(path);

            void CurrentListener(object o, ChildChangedEventArgs args){
                if (args.DatabaseError != null){
                    fallback(new AggregateException(new Exception(args.DatabaseError.Message)));
                    Debug.LogError(args.DatabaseError.Message);
                    return;
                }
                onChildRemoved(args);
            }

            var listenerPair = new KeyValuePair<DatabaseReference, EventHandler<ChildChangedEventArgs>>(
                customReference, CurrentListener);
            customReference.ChildRemoved += CurrentListener;

            return listenerPair;
    }

    //(추가) 지정한 경로의 Child 브랜치에 값이 삭제되는 것을 감지하는 것을 멈춤
    public void StopListeningForChildRemoved(KeyValuePair<DatabaseReference, EventHandler<ChildChangedEventArgs>> listener){
        listener.Key.ChildRemoved -= listener.Value;
    }

    //(추가) 지정한 경로의 Child 브랜치에 값이 수정되는 것을 감지하여
    // 그 수정된 값을 매개변수로 callback 함수인 onChildChanged 실행
    public KeyValuePair<DatabaseReference, EventHandler<ChildChangedEventArgs>> ListenForChildChanged(
        string path, Action<ChildChangedEventArgs> onChildChanged, Action<AggregateException> fallback) {
            var customReference = GetReferenceFromPath(path);

            void CurrentListener(object o, ChildChangedEventArgs args){
                if (args.DatabaseError != null){
                    fallback(new AggregateException(new Exception(args.DatabaseError.Message)));
                    Debug.LogError(args.DatabaseError.Message);
                    return;
                }
                onChildChanged(args);
            }

            var listenerPair = new KeyValuePair<DatabaseReference, EventHandler<ChildChangedEventArgs>>(
                customReference, CurrentListener);
            customReference.ChildChanged += CurrentListener;

            return listenerPair;
    }

    //(추가) 지정한 경로의 Child 브랜치에 값이 수정되는 것을 감지하는 것을 멈춤
    public void StopListeningForChildChanged(KeyValuePair<DatabaseReference, EventHandler<ChildChangedEventArgs>> listener){
        listener.Key.ChildChanged -= listener.Value;
    }

    // Cloud Function의 실시간 데이터베이스 트리거
    // 특정한 데이터베이스의 위치가 변경되기를 기다린 후
    // 이벤트가 발생할 때 트리거되어 데이터베이스의 이벤트를 수행함

    //(추가) 정확히는 직접 지정한 경로의 값이 변화하는지를 감지한 후
    // 그 변화된 값을 매개변수로 callback 함수인 onValueChanged를 실행함
    // 지정한 경로의 값 변화를 직접 확인한다는 점에서
    // 자식 브랜치의 변화를 모두 확인하는 Child 관련 함수들과 구분됨
    public KeyValuePair<DatabaseReference, EventHandler<ValueChangedEventArgs>> ListenForValueChanged(
        string path, Action<ValueChangedEventArgs> onValueChanged, Action<AggregateException> fallback) {
            var customReference = GetReferenceFromPath(path);

            void CurrentListener(object o, ValueChangedEventArgs args){
                if (args.DatabaseError != null){
                    fallback(new AggregateException(new Exception(args.DatabaseError.Message)));
                    Debug.LogError(args.DatabaseError.Message);
                    return;
                }
                onValueChanged(args);
            }

            var listenerPair = new KeyValuePair<DatabaseReference, EventHandler<ValueChangedEventArgs>>(
                customReference, CurrentListener);
            customReference.ValueChanged += CurrentListener;

            return listenerPair;
    }

    //(추가) 직접 지정한 경로의 값이 변화하는 것을 감지하는 것을 멈춤
    public void StopListeningForValueChanged(KeyValuePair<DatabaseReference, EventHandler<ValueChangedEventArgs>> listener){
        listener.Key.ValueChanged -= listener.Value;
    }
   
    // 데이터베이스 경로에 데이터가 존재하는지 확인함
    public void CheckIfNodeExists(string path, Action<bool> callback, Action<AggregateException> fallback){
        GetJSON(path, snapshot => callback(snapshot.Exists), Debug.Log);
    }

    // Authenticaton 관련 메소드
    //(추가) 매개변수로 받은 user를 데이터베이스 상의 직접 지정한 경로에 작성함
    public void PostUser(User user, Action callback, Action<AggregateException> fallback){
        var messageJSON = StringSerializationAPI.Serialize(typeof(User),user);
        reference.Child($"users/{AuthManager.manager.GetUserId()}").SetRawJsonValueAsync(messageJSON).ContinueWith(task =>
        {
            if (task.IsCanceled || task.IsFaulted) fallback(task.Exception);
            else callback();
        });
    }

    //(추가) 데이터베이스 상에서 유저의 정보를 긁어옴
    // 유저의 닉네임 정보 등이 데이터베이스에 저장되기 때문에 해당 메소드들이 존재
    public void GetUser(Action<User> callback, Action<AggregateException> fallback){
        reference.Child($"users/{AuthManager.manager.GetUserId()}").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCanceled || task.IsFaulted) fallback(task.Exception);
            else callback(StringSerializationAPI.Deserialize(typeof(User),task.Result.GetRawJsonValue()) as User);
        });
    }
}
*/
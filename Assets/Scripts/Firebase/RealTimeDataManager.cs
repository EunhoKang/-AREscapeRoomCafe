using System; 
using System.Linq; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Extensions;
using Firebase.Database;
using Newtonsoft.Json;

public class RealTimeDataManager : MonoBehaviour
{
    public static RealTimeDataManager manager=null;
    DatabaseReference reference;

    void Awake()
    { 
        if(manager==null)manager=this;
        else if(manager!=this)Destroy(gameObject);
        DontDestroyOnLoad(this);
    }
    
    void Start()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public DatabaseReference GetReferenceFromPath(string path){
        var splitPath = path.Split('/');
        return splitPath.Aggregate(reference, (current, child) => current.Child(child));
    }
 
    void ReadUserData()
    {
        FirebaseDatabase.DefaultInstance.GetReference("users")
            .GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                // Do something with snapshot...
            for ( int i = 0; i < snapshot.ChildrenCount; i++)
                Debug.Log(snapshot.Child(i.ToString()).Child("username").Value);
              
            }
        });
    }
 
    void WriteUserData(string userId, string username)
    {
        reference.Child("users").Child(userId).Child("username").SetValueAsync(username);
    }

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

    public void PostObject<T>(string path, T obj, Action callback, Action<AggregateException> fallback){
        PostJSON(path, JsonConvert.SerializeObject(obj), callback, Debug.Log);
    }

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

    public void GetObject<T>(string path, Action<T> callback, Action<AggregateException> fallback){
        GetJSON(path, json => {
            callback(JsonConvert.DeserializeObject<T>(json.GetRawJsonValue()));
        }, Debug.Log);
    }
}

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
    public List<SampleData> dataList=new List<SampleData>();
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
        ReadUserData();
    }

    public DatabaseReference GetReferenceFromPath(string path){
        var splitPath = path.Split('/');
        return splitPath.Aggregate(reference, (current, child) => current.Child(child));
    }
 
    public void ReadUserData()
    {
        FirebaseDatabase.DefaultInstance.GetReference("users")
            .GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("?????????????????????");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                // Do something with snapshot...
                List<SampleData> lists=new List<SampleData>();
                for ( int i = 0; i < snapshot.ChildrenCount; i++){
                    var sample=new SampleData(snapshot.Child(i.ToString()).Child("playerName").Value.ToString(),
                    Double.Parse(snapshot.Child(i.ToString()).Child("data").Value.ToString()));
                    lists.Add(sample);
                }
                dataList=lists;
            }

        });
    }
 
    public void WriteUserData(string path,string userId, string username)
    {
        reference.Child(path).Child(userId).Child("username").SetValueAsync(username);
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

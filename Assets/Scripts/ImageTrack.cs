using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ImageTrack : MonoBehaviour
{
    [HideInInspector]public int temp=0;

    ARRaycastManager raymanager;
    static List<ARRaycastHit> hits=new List<ARRaycastHit>();
    static List<ARRaycastHit> centerhits=new List<ARRaycastHit>();
    private Vector2 touchPos;
    bool isSpawned;
    private GameObject spawnedObj;
    public Camera ARCam;
    public GameObject ARIndicator;

    
    public GameObject[] objs;
    private Dictionary<string,GameObject> editObjs = new Dictionary<string, GameObject>();
    private Dictionary<string,GameObject> prefabObjs = new Dictionary<string, GameObject>();
    public ARTrackedImageManager trackedImageManager;
    [HideInInspector]public GameObject lastTarget;

    private void Awake() {
        trackedImageManager=GetComponent<ARTrackedImageManager>();//매니저 할당
        foreach(var prefab in objs){//모든 오브젝트 생성 후 비활성화
            GameObject clone=Instantiate(prefab);
            editObjs[prefab.name]=clone;//각각의 오브젝트를 딕셔너리에 이름을 key로 저장
            prefabObjs[prefab.name]=prefab;
            clone.SetActive(false);
        }
    }
    void Start()
    {
        raymanager = GetComponent<ARRaycastManager>();
    }
    private void OnEnable() {
        trackedImageManager.trackedImagesChanged+=OnTrackedImagesChanged;
    }
    private void OnDisable() {
        trackedImageManager.trackedImagesChanged-=OnTrackedImagesChanged;
    }
    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs){//해당 함수를 이벤트로 호출
        foreach(var trackedImage in  eventArgs.added){//이미지가 추가되거나 업데이트되면 이미지 업데이트
            UpdateImage(trackedImage);
        }
        foreach(var trackedImage in  eventArgs.updated){
            UpdateImage(trackedImage);
        }
        foreach(var trackedImage in  eventArgs.removed){//이미지가 상에서 사라지면 비활성화
            editObjs[trackedImage.name].SetActive(false);
        }
    }

    void UpdateImage(ARTrackedImage trackedImage){
        GameObject trackedObject = editObjs[trackedImage.referenceImage.name];
        if(trackedImage.trackingState == TrackingState.Tracking){//현재 이미지가 tracking되고 있는지 확인
            trackedObject.transform.position=trackedImage.transform.position;//확인되면 오브젝트를 배치
            trackedObject.transform.rotation=trackedImage.transform.rotation;
            lastTarget=prefabObjs[trackedImage.referenceImage.name];
            trackedObject.SetActive(true);
        }else{
            trackedObject.SetActive(false);//확인되지 않으면 전부 끔
        }
    }
    void Update()
    {
        var screenCenter = ARCam.ViewportToScreenPoint(new Vector3(0.5f,0.5f));//스크린 중앙을 찍는다. Viewport에서 0.5,0.5가 중심
        if(raymanager.Raycast(screenCenter, centerhits, TrackableType.All)){//광선 쏘기. 1개 이상의 hits가 있으면 true
            Pose hitpos=centerhits[0].pose;//광선에 맞은 가장 첫번째 오브젝트

            var cameraFoward=ARCam.transform.forward;//카메라 앞방향 벡터
            var cameraBearing=new Vector3(cameraFoward.x,0,cameraFoward.z).normalized;//바닥과 평행하게 놓기 위해 y성분 없앰

            hitpos.rotation=Quaternion.LookRotation(cameraBearing);//hitpos의 방향을 맞춤
            ARIndicator.SetActive(true);
            ARIndicator.transform.SetPositionAndRotation(hitpos.position,hitpos.rotation);//인디케이터 소한 후 방향 맞추기
        }
        if(Input.touchCount>0){
            touchPos=Input.GetTouch(0).position;
            if(raymanager.Raycast(touchPos,hits,TrackableType.PlaneWithinPolygon)){//터치 시 PlanePolygon 찾음
                var hitPos=hits[0].pose;
                if(!isSpawned){
                    spawnedObj=Instantiate(lastTarget,hitPos.position,hitPos.rotation);//찾은 위치에 오브젝트 생성
                    Debug.Log($"{lastTarget.name} : {++temp}");
                    isSpawned=true;
                }else{
                    spawnedObj.transform.position=hitPos.position;//이미 생성되었다면 위치 변경
                }
            }
        }
    }
}

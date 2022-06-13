using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class MultipleImageTracking : MonoBehaviour
{
    public List<string> imageNames;
    public GameObject[] Objs;
    private Dictionary<string, GameObject> spawnedObjs = new Dictionary<string, GameObject>();
    private Dictionary<string, int> spawnedInt = new Dictionary<string, int>();
    private ARTrackedImageManager ARTrackedImageManager;
    public ARUI arui;

    private void Awake()
    {
        ARTrackedImageManager = GetComponent<ARTrackedImageManager>();
        var i=0;
        foreach(GameObject obj in Objs)
        {
            spawnedObjs.Add(imageNames[i], obj);
            spawnedInt.Add(imageNames[i],i++);
            obj.SetActive(false);
        }
    }

    void OnEnable()
    {
        ARTrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        ARTrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            UpdateImage(trackedImage);
        }

        foreach (var trackedImage in eventArgs.updated)
        {
            UpdateImage(trackedImage);
        }

        foreach (var trackedImage in eventArgs.removed)
        {
            spawnedObjs[trackedImage.name].SetActive(false);
        }
    }

    private void UpdateImage(ARTrackedImage trackedImage)
    {
        GameObject trackedObject = spawnedObjs[trackedImage.referenceImage.name];
        int idx=spawnedInt[trackedImage.referenceImage.name];

        if(trackedImage.trackingState == TrackingState.Tracking)
        {
            if(!arui.inventory[idx]){
                trackedObject.SetActive(true);
                arui.InventoryChange(idx);
                if(trackedImage.referenceImage.name=="F")arui.GameEnd(true);
            }
        }
        else
        {
            trackedObject.SetActive(false);
        }
    }
}
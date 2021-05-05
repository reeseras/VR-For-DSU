using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;
using System;

/// <summary>
/// Allows the current object and all children (recursive) to be reset back to their initial position.
/// Accounts for Normcore synchronization by requesting ownership on Update().
/// Code by Reese Rasmussen
/// </summary>

public class ResetPosition : MonoBehaviour {
    private List<TransformData> initialData = new List<TransformData>();
    private int resetNumber = 0; // a generally pointless number for Normcore to track and sync when the objects' poisitions are reset.
    private ResetPositionSync rpSync;
    private bool requestOwnership = false; 

    // An arbitrary number for Normcore to track. Triggers ResetObjectPosition for all users.
    public int ResetNumber {
        get => resetNumber;
        set {
            resetNumber = value;
            if (resetNumber > 0) { // Prevents function from being called before objects have been initialized.
                ResetObjectPosition(); // when normcore calls to set this varaiable, ResetobjectPosition() is also called.
            }
        }
    }

    struct TransformData {
        public Vector3 position;
        public Quaternion rotation;
    }

    private int mIndex = 0; // tracks the current position on initialData when resetting position.


    void Start() {
        TransformData currentData;

        rpSync = GetComponent<ResetPositionSync>();

        // store current gameObject (index 0)
        currentData.position = gameObject.transform.position;
        currentData.rotation = gameObject.transform.rotation;
        initialData.Add(currentData);

        // store all child objects (index starts at 1)
        foreach (Transform child in gameObject.transform) {
            currentData.position = child.position;
            currentData.rotation = child.rotation;
            initialData.Add(currentData);
            SetInitialDataR(child);
        }

    }

    void Update() {
        if (requestOwnership) {
            // child components share ownership with their parent
            // Since Normcore needs a frame to set the data, requesting ownership needs to happen on Update()
            try {
                RealtimeTransform[] rtArray = gameObject.GetComponentsInChildren<RealtimeTransform>();
                foreach (RealtimeTransform rt in rtArray) {
                    // Debug.Log("RequestOwnership() on: " + rt.gameObject.name);
                    rt.RequestOwnership();
                }
            } catch (NullReferenceException e) { // should prevent errors if RealtimeTransform is not attached to any objects.
                Debug.LogWarning("ResetObjectPosition() has no RealtimeTransform for " + gameObject.name + "?\n" + e);
            }
            ResetObjectPositionP2(); 
        }
    }

    // Recursive version of SetInitialData() (for all child objects)
    private void SetInitialDataR(Transform parent) {
        TransformData currentData;

        foreach (Transform child in parent) {
            currentData.position = child.position;
            currentData.rotation = child.rotation;
            initialData.Add(currentData);
            SetInitialDataR(child);
        }
    }

    // triggers RequestOwnership to happen on Update().
    public void ResetObjectPosition() {
        // move on the realtime model (Normcore)
        requestOwnership = true;

    }

    // Resets the position of the current object and all child objects.
    public void ResetObjectPositionP2() {
        requestOwnership = false;
        Debug.Log("ResetObjectPositionP2() of: " + gameObject.name);

        // reset parent
        mIndex = 0;
        gameObject.transform.position = initialData[mIndex].position;
        gameObject.transform.rotation = initialData[mIndex].rotation;
        mIndex++;

        // reset children to parent
        for (int i = 0; i < gameObject.transform.childCount; i++) {
            gameObject.transform.GetChild(i).position = initialData[mIndex].position;
            gameObject.transform.GetChild(i).rotation = initialData[mIndex].rotation;
            mIndex++;
            ResetObjectPositionR(gameObject.transform.GetChild(i));
        }

        
    }

    // Recursive version of ResetObjectPosition2
    private void ResetObjectPositionR(Transform parent) {
        for (int j = 0; j < parent.transform.childCount; j++) {
            parent.transform.GetChild(j).position = initialData[mIndex].position;
            parent.transform.GetChild(j).rotation = initialData[mIndex].rotation;
            mIndex++;
            ResetObjectPositionR(parent.transform.GetChild(j));
        }
    }

    // this function should be placed wherever ResetObjectPosition() is placed in order for Normcore to sync. (For example, under onClick() events)
    public void IncrementResetNumber() {
        resetNumber++;
        rpSync.SetResetNumber(resetNumber);
    }

}

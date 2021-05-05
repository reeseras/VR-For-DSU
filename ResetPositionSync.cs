using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;

public class ResetPositionSync : RealtimeComponent<ResetPositionSyncModel>
{
    private int _resetNumber;

    private ResetPosition rpCode;

    void Awake() {
        Debug.Log("ResetPositionSync Awake()");
        rpCode = GetComponent<ResetPosition>();
    }

    protected override void OnRealtimeModelReplaced(ResetPositionSyncModel previousModel, ResetPositionSyncModel currentModel) {
        Debug.Log("ResetPositionSync OnRealtimeModelReplaced()");
        if (previousModel != null) {
            Debug.Log("ResetPositionSync previousModel != null");
            previousModel.resetNumberDidChange -= ResetNumberDidChange;
        }

        if (currentModel != null) {
            Debug.Log("ResetPositionSync currentModel != null");
            currentModel.resetNumber = rpCode.ResetNumber;

            Debug.Log("ResetPositionSync rpCode.ResetNumber: " + rpCode.ResetNumber);
            Debug.Log("ResetPositionSync model.resetNumber: " + model.resetNumber);
            rpCode.ResetNumber = model.resetNumber;

            Debug.Log("ResetPositionSync currentModel2");
            currentModel.resetNumberDidChange += ResetNumberDidChange;
        }
        Debug.Log("ResetPositionSync OnRealtimeModelReplaced Complete");
    }

    public void ResetNumberDidChange(ResetPositionSyncModel model, int value) {
        Debug.Log("ResetPositionSync ResetNumberDidChange() to: " + value);
        _resetNumber = value;
        rpCode.ResetNumber = value;
    }

    public void SetResetNumber(int value) {
        Debug.Log("ResetPositionSync SetResetNumber() to: " + value);
        model.resetNumber = value;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Normal.Realtime;

/// <summary>
/// Detects the distance between two objects. Code is placed on the moveable object containing the triggering collider.
/// Code by Reese Rasmussen
/// </summary>

public class PNSnap2 : MonoBehaviour
{
    public GameObject childObject; // The object whose position should be changed.
    public GameObject snapPosition; // the childObject will snap to the position of this object.
    public Animator animator;
    public Vector3 offsetPosition; // if offset from snapPosition is needed.
    // public Interactable powerSwitch;

    private const double SNAP_DISTANCE = 0.22; // default 0.22, minimum distance between the current gameObject and snapPosition before snapping.

    private bool holding = false; // checks whether the object is being held (allows manipulation off the snap position)
    private bool power = false; // check the power bool (for the animator/normcore)

    // Normcore public variables:
    public bool Holding {
        get => holding;
        set {
            holding = value;
        }
    }
    public bool Power {
        get => power;
        set {
            power = value;
        }
    }
    public bool AnimatorPower {
        get => animator.GetBool("Power");
        set {
            animator.SetBool("Power", value);
        }
    }

    // Every frame, check if the current gameObject and snapPosition are within SNAP_DISTANCE, then snap them together.
    // Doesn't seem to handle 3 dimensions very well (large minimum distance along one axis, tiny among the others)
    void Update() {
        // Debug.Log("Distance: " + Vector3.Distance(gameObject.transform.position, snapPosition.transform.position));
        if (!holding && Vector3.Distance(gameObject.transform.position, snapPosition.transform.position) <= SNAP_DISTANCE) {
            childObject.GetComponent<RealtimeTransform>().RequestOwnership();
            SnapTogether();
            childObject.GetComponent<RealtimeTransform>().ClearOwnership();
        } else if (holding) {
            animator.SetBool("Combined", false);
        } else {
            animator.SetBool("Combined", false);
        }
    }

    // sets the childObject's position & rotation to the snapPosition + offsetPosition.
    private void SnapTogether() {
        StopObject();
        childObject.transform.position = snapPosition.transform.position + offsetPosition;
        childObject.transform.rotation = snapPosition.gameObject.transform.rotation;
        animator.SetBool("Combined", true);
    }

    // set childObject's velocity to 0
    // due to issues between normcore and MRTK, the rigidbody was removed from the object.
    // issues might be fixed in the future, so this function exists but does nothing.
    private void StopObject() {
        //childObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        //childObject.GetComponent<Rigidbody>().isKinematic = true;
    }

    // Add this to the button for switching the power
    public void PowerToggle() {
        if (power) {
            power = false;
            animator.SetBool("Power", false);
        } else {
            power = true;
            animator.SetBool("Power", true);
        }
    }

    // Add this to the child object's OnManipulationStart() component.
    // Allows manipulation to the snapped object.
    public void HoldStart() {
        holding = true;
        childObject.GetComponent<RealtimeTransform>().RequestOwnership();
        animator.SetBool("Combined", false);
    }

    // Add this to the child object's OnManipulationEnd() component.
    public void HoldEnd() {
        holding = false;
        childObject.GetComponent<RealtimeTransform>().ClearOwnership();
        StopObject(); // because Normcore & MRTK turn of isKinetic upon manipulating an object.
    }
}

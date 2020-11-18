using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollision : MonoBehaviour {

    public float minDistance = 1.0f;
    public float actualDistance = 2.0f;
    public float maxDistance = 4.0f;
    public float smooth = 10.0f;
    Vector3 dollyDir;
    public Vector3 dollyDirAdjusted;
    public float distance;
    private float scroll = 0.0f;
    public float scrollSensitivity = 1.0f;

    void Awake () {
        dollyDir = transform.localPosition.normalized;
        distance = transform.localPosition.magnitude;
	}
	
	void Update () {
        scroll = Input.GetAxis("Mouse ScrollWheel");
        actualDistance -= scroll * scrollSensitivity;
        actualDistance = Mathf.Clamp(actualDistance, minDistance, maxDistance);
        Vector3 desiredCameraPos = transform.parent.TransformPoint(dollyDir * actualDistance);
        RaycastHit hit;
        if(Physics.Linecast(transform.parent.position, desiredCameraPos, out hit, ~(1 << 9))){
            distance = Mathf.Clamp((hit.distance * 0.9f), minDistance, actualDistance);
        } else{
            distance = actualDistance;
        }
        transform.localPosition = Vector3.Lerp(transform.localPosition, dollyDir * distance, Time.deltaTime * smooth);
	}
}

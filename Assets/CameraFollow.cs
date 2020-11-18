using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public float cameraMoveSpeed = 120.0f;
    public GameObject cameraFollowObj;
    public float clampAngleUp = 90.0f;
    public float clampAngleDown = -30.0f;
    public float inputSensitivity = 150.0f;
    public float mouseX;
    public float mouseY;
    private float rotY = 0.0f;
    private float rotX = 0.0f;
    
	void Start () {
        Vector3 rot = transform.localRotation.eulerAngles;
        rotY = rot.y;
        rotX = rot.x;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
	}
	
	void Update () {
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
        rotY -= mouseX * inputSensitivity * Time.deltaTime;
        rotX += mouseY * inputSensitivity * Time.deltaTime;
        rotX = Mathf.Clamp(rotX, clampAngleDown, clampAngleUp);
        Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
        transform.rotation = localRotation;
    }

    private void LateUpdate(){
        CameraUpdater();
    }

    void CameraUpdater()
    {
        Transform target = cameraFollowObj.transform;
        float step = cameraMoveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);
    }
}

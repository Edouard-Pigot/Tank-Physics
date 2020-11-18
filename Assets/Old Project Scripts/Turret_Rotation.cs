using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret_Rotation : MonoBehaviour {

    public GameObject Turret;
    public GameObject Canon;
    public GameObject Camera;
    public float sensitivity = 1.0f;
    public float smoothing = 2.0f;
    public float minY = -60f;
    public float maxY = 60f;
    public float rotY = 0f;
    public float rotX = 0f;
    public float smoothedX = 0f;
    public float smoothedY = 0f;
    public Vector2 mouseLook;
    public Vector2 smoothV;
    public Vector2 md;
    public AudioSource turretAS;
    public float pastValue = 0.0f;
    public float actualValue = 0.0f;
    public float audioSensitivity = 0.1f;
    public float volume = 0.0f;
    public float maxVolume = 0.25f;
    public float cameraMultiplicator = 1;
    public float cameraTurnCompensation = 0.0f;

    void Update()
    {
        pastValue = actualValue;
        md = new Vector2(Input.GetAxis("Mouse X") * sensitivity, Input.GetAxis("Mouse Y") * sensitivity);
        md = Vector2.Scale(md, new Vector2(sensitivity * smoothing, sensitivity * smoothing));
        smoothedX = Mathf.Lerp(smoothedX, md.x, 1f / smoothing);
        smoothedY = Mathf.Lerp(smoothedY, md.y, 1f / smoothing);
        rotX = Turret.transform.localEulerAngles.y + smoothedX;
        rotY += smoothedY;
        rotY = Mathf.Clamp(rotY, minY, maxY);
        actualValue = rotX;
        volume = Mathf.Abs(actualValue - pastValue) * audioSensitivity;
        if(volume > maxVolume)
        {
            volume = maxVolume;
        }
        turretAS.volume = volume;
        /*rotX = Turret.transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivity;
        rotY += Input.GetAxis("Mouse Y") * sensitivity;
        rotY = Mathf.Clamp(rotY, minY, maxY);*/

        Turret.transform.localEulerAngles = new Vector3(0, rotX, 0);
        Canon.transform.localEulerAngles = new Vector3(rotY * -cameraMultiplicator, 0, 0);
        Camera.transform.localEulerAngles = new Vector3(cameraTurnCompensation + (rotY * cameraMultiplicator), 0, -180);
    }

    public void ChangeCamera()
    {
        if (cameraMultiplicator == 1)
        {
            cameraMultiplicator = -1;
        }
        else
        {
            cameraMultiplicator = 1;
        }
    }
}

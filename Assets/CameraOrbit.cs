using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    protected Transform xFormCamera;
    protected Transform xFormParent;

    protected Vector3 localRotation;
    protected float cameraDistance = 10f;

    public float mouseSensitivity = 4f;
    public float scrollSensitivity = 2f;
    public float orbitDampening = 10f;
    public float scrollDampening = 6f;

    public bool cameraDisabled = false;

    void Start()
    {
        this.xFormCamera = this.transform;
        this.xFormParent = this.transform.parent;
    }
    
    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
            cameraDisabled = !cameraDisabled;

        if (!cameraDisabled)
        {
            if(Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
            {
                localRotation.x += Input.GetAxis("Mouse X") * mouseSensitivity;
                localRotation.y += Input.GetAxis("Mouse Y") * mouseSensitivity;

                localRotation.y = Mathf.Clamp(localRotation.y, 0f, 90f);
            }

            if(Input.GetAxis("Mouse ScrollWheel") != 0f)
            {
                float scrollAmount = Input.GetAxis("Mouse ScrollWheel") * scrollSensitivity;

                scrollAmount *= (this.cameraDistance * 0.3f);

                this.cameraDistance += scrollAmount * -1f;

                this.cameraDistance = Mathf.Clamp(this.cameraDistance, 1.5f, 100f);
            }
        }

        Quaternion qt = Quaternion.Euler(localRotation.y, localRotation.x, 0);
        this.xFormParent.rotation = Quaternion.Lerp(this.xFormParent.rotation, qt, Time.deltaTime * orbitDampening);

        if(this.xFormCamera.localPosition.z != this.cameraDistance * -1f)
        {
            this.xFormCamera.localPosition = new Vector3(0f, 0f, Mathf.Lerp(this.xFormCamera.localPosition.z, this.cameraDistance * -1f, Time.deltaTime * scrollDampening));
        }
    }
}

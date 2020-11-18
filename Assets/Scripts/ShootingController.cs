using UnityEngine;
using System.Collections;
//using UnityStandardAssets.Characters.FirstPerson;

public class ShootingController : MonoBehaviour
{
    //Drags
    public Camera mainCamera;
    public GameObject sightImage;
    public GameObject bulletObj;
    public Transform targetObj;
    public Transform mirrorTargetObj;
    public GameObject hitMarker;

    //The bullet's initial speed
    //Sniper rifle
    public float bulletSpeed = 850f;

    //Need the initial camera FOV so we can zoom
    float initialFOV;
    //To change the zoom
    int currentZoom = 1;

    //To change sensitivity when zoomed
    //MouseLook mouseLook;
    float standardSensitivity;
    float zoomSensitivity = 0.1f;

    bool canFire = true;

    public static Vector3 windSpeed = new Vector3(2f, 0f, 3f);

    void Start()
    {

        //mouseLook = GetComponent<RigidbodyFirstPersonController>().mouseLook;

        //standardSensitivity = mouseLook.XSensitivity;
    }

    void Update()
    {
        FireBullet();
    }

    void FireBullet()
    {
        //Sometimes when we move the mouse is really high in the webplayer, so the mouse cursor ends up outside
        //of the webplayer so we cant fire, despite locking the cursor, so add alternative fire button
        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.F)) && canFire)
        {
            //Create a new bullet
            GameObject newBullet = Instantiate(bulletObj, mainCamera.transform.position, mainCamera.transform.rotation) as GameObject;

            //Give it speed
            newBullet.GetComponent<Bullet>().currentVelocity = bulletSpeed * mainCamera.transform.forward;

            canFire = false;
        }

        //Has to release the trigger to fire again
        if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.F))
        {
            canFire = true;
        }
    }

    //Add marker where we hit target
    //Called from the bullet script
    public void AddMarker(Vector3 hitCoordinates)
    {
        //Add a marker where we hit the target
        Instantiate(hitMarker, hitCoordinates, Quaternion.identity);

        //The coordinates of the hit in localPosition of the target
        Vector3 localHitCoordinates = targetObj.InverseTransformPoint(hitCoordinates);

        //The global coordinates of the hit but in relation to the mirror target
        //The marker has the same local position in relation to both the target and the mirror
        Vector3 globalMirrorHit = mirrorTargetObj.transform.TransformPoint(localHitCoordinates);

        //Add another marker
        Instantiate(hitMarker, globalMirrorHit, Quaternion.identity);
    }
}
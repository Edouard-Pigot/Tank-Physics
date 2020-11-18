using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    public Vector3 currentPosition;
    public Vector3 currentVelocity;

    Vector3 newPosition = Vector3.zero;
    Vector3 newVelocity = Vector3.zero;

    private float startTime;
    private float journeyLength;


    void Awake()
    {
        currentPosition = transform.position;
    }

    void Update()
    {
        DestroyBullet();
        transform.position = Vector3.Lerp(currentPosition, newPosition, Time.deltaTime * 1f);
        transform.rotation = Quaternion.LookRotation(newVelocity);
    }

    void FixedUpdate()
    {
        MoveBullet();
    }

    void CheckHit()
    {
        Vector3 fireDirection = (newPosition - currentPosition).normalized;
        float fireDistance = Vector3.Distance(newPosition, currentPosition);

        RaycastHit hit;

        Debug.DrawRay(currentPosition, fireDirection * fireDistance, Color.blue, 5f);
        if (Physics.Raycast(currentPosition, fireDirection, out hit, fireDistance))
        {
            Debug.DrawRay(hit.point, hit.normal, Color.red, 5f);
            Debug.Log(Vector3.Angle(newVelocity, -hit.normal) * 2);
            if ((Vector3.Angle(newVelocity, -hit.normal) * 2) < 15f)
            {
                Destroy(gameObject);
            }
            Vector3 reflection = Vector3.Reflect(this.transform.forward, hit.normal);
            Debug.DrawRay(hit.point, reflection * 50f, Color.green, 5f);
            /*Quaternion newRotation = new Quaternion();
            newRotation.SetFromToRotation(hit.normal, this.transform.position);
            transform.rotation = newRotation * transform.rotation;*/
            transform.rotation = Quaternion.LookRotation(reflection);
            newPosition = hit.point;
            float speed = newVelocity.z;
            newVelocity = Vector3.zero;
            newVelocity = transform.forward * speed;
            //hit.collider.enabled = false;
            //Debug.DrawRay(hit.point, newRotation.eulerAngles, Color.yellow, 5f);
        }
    }

    void MoveBullet()
    {
        startTime = Time.time;
        //Use an integration method to calculate the new position of the bullet
        float h = Time.fixedDeltaTime;
        BulletBallistics.CurrentIntegrationMethod(h, currentPosition, currentVelocity, out newPosition, out newVelocity);

        CheckHit();
        
        currentPosition = newPosition;
        currentVelocity = newVelocity;
    }

    void DestroyBullet()
    {
        if (transform.position.y < -100f)
        {
            Destroy(gameObject);
        }
    }
}
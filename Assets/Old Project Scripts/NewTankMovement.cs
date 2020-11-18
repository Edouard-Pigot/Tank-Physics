using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewTankMovement : MonoBehaviour
{
    public Rigidbody tankRigidbody;
    public Transform tankCenterOfMass;
    public GameObject leftTrack;
    public GameObject rightTrack;
    public Renderer leftTrackRenderer;
    public Renderer rightTrackRenderer;
    public float leftTrackTextureOffset;
    public float rightTrackTextureOffset;
    public float trackTextureOffsetMulitplier = 0.1f;
    public string engineStartingButton;
    public float throttle = 0.0f;
    public float turnMultiplicator = 0.0f;
    public float throttleInput;
    public float turnInput;
    public bool engineStarted;
    public AudioSource engineStatus;
    public AudioSource engineRunning;
    public AudioClip engineIdling;
    public AudioClip engineStarting;
    public AudioClip engineStoping;
    public AudioClip engineDriving;
    public float maximumPitch;
    public float minimumPitch;
    public WheelCollider[] wheelColliders = new WheelCollider[16];
    public Transform[] wheelMeshesPosition = new Transform[16];
    public Transform[] wheelMeshesRotation = new Transform[16];
    public float wheelsHeightOffset = 0.01f;
    public float maximumThrottle = 1600f;
    public float maximumTurnMultiplicator = 1100f;
    public float brakeForce = 1000f;
    public bool isBraking;
    public float wheelsRpm;
    public float forwardOrBackward;
    public Transform wheelCollidersTransform;
    public Transform wheelMeshesRight;
    public Transform wheelMeshesLeft;

    public float throttleAcceleration = 10f;
    public float throttleDeceleration = 10f;
    public float turnMultiplicatorAcceleration = 10f;
    public float turnMultiplicatorDeceleration = 10f;

    void Start()
    {
        tankRigidbody = GetComponent<Rigidbody>();
        tankRigidbody.centerOfMass = tankCenterOfMass.localPosition;
        leftTrackRenderer = leftTrack.GetComponent<Renderer>();
        rightTrackRenderer = rightTrack.GetComponent<Renderer>();
        trackTextureOffsetMulitplier = 0.05f / 10000f;
        engineStartingButton = "EngineStart";
        int children = wheelCollidersTransform.childCount;
        for (int i = 0; i < children; ++i)
        {
            wheelColliders[i] = wheelCollidersTransform.GetChild(i).GetComponent<WheelCollider>();
        }
        children = wheelMeshesRight.childCount;
        for (int i = 1; i < children; ++i)
        {
            wheelMeshesPosition[i-1] = wheelMeshesRight.GetChild(i);
            wheelMeshesRotation[i - 1] = wheelMeshesRight.GetChild(i).GetChild(0);
        }
        for (int i = 1; i < children; ++i)
        {
            wheelMeshesPosition[i+7] = wheelMeshesLeft.GetChild(i);
            wheelMeshesRotation[i + 7] = wheelMeshesLeft.GetChild(i).GetChild(0);
        }
    }

    private void FixedUpdate()
    {
        wheelsRpm = wheelColliders[1].rpm;
        if (engineStarted)
        {
            if ((Input.GetAxis("Vertical") == -1) && (throttle > -maximumThrottle))
            {
                throttle = throttle - throttleAcceleration * Time.deltaTime;
            }
            else if ((Input.GetAxis("Vertical") == 1) && (throttle < maximumThrottle))
            {
                throttle = throttle + throttleAcceleration * Time.deltaTime;
            }
            else
            {
                if (throttle > throttleDeceleration * Time.deltaTime)
                {
                    throttle = throttle - throttleDeceleration * Time.deltaTime;
                }
                else if (throttle < -throttleDeceleration * Time.deltaTime)
                {
                    throttle = throttle + throttleDeceleration * Time.deltaTime;
                }
                else
                {
                    throttle = 0;
                }
            }
            if ((Input.GetAxis("Horizontal") == -1) && (turnMultiplicator > -maximumTurnMultiplicator))
            {
                turnMultiplicator = turnMultiplicator - turnMultiplicatorAcceleration * Time.deltaTime;
            }
            else if ((Input.GetAxis("Horizontal") == 1) && (turnMultiplicator < maximumTurnMultiplicator))
            {
                turnMultiplicator = turnMultiplicator + turnMultiplicatorAcceleration * Time.deltaTime;
            }
            else
            {
                if (turnMultiplicator > turnMultiplicatorDeceleration * Time.deltaTime)
                {
                    turnMultiplicator = turnMultiplicator - turnMultiplicatorDeceleration * Time.deltaTime;
                }
                else if (turnMultiplicator < -turnMultiplicatorDeceleration * Time.deltaTime)
                {
                    turnMultiplicator = turnMultiplicator + turnMultiplicatorDeceleration * Time.deltaTime;
                }
                else
                {
                    turnMultiplicator = 0;
                }
            }
        }
        else
        {
            throttle = 0;
            turnMultiplicator = 0;
        }
        Move(throttle, turnMultiplicator);
        Brake();
    }

    void Update()
    {
        if (Input.GetButtonDown(engineStartingButton))
        {
            if (engineStarted == false)
            {
                engineStarted = true;
                StartCoroutine(playEngineSound());
            }
            else
            {
                engineStatus.loop = false;
                engineStarted = false;
                StopCoroutine(playEngineSound());
                engineStatus.clip = engineStoping;
                engineStatus.Play();
            }
        }
        if (Input.GetButtonDown("Restart") && this.transform.position.y <= 5)
        {
            this.transform.SetPositionAndRotation(new Vector3(this.transform.position.x, this.transform.position.y + 10, this.transform.position.z), new Quaternion(0, 0, 0, 0));
        }
        AudioEngine();
        UpdateMeshesPositions();
        leftTrackTextureOffset += Time.time * wheelColliders[3].rpm * trackTextureOffsetMulitplier * Mathf.Abs(forwardOrBackward);
        rightTrackTextureOffset += Time.time * wheelColliders[11].rpm * trackTextureOffsetMulitplier * Mathf.Abs(forwardOrBackward);
        leftTrackRenderer.material.mainTextureOffset = new Vector2(0, leftTrackTextureOffset);
        rightTrackRenderer.material.mainTextureOffset = new Vector2(0, rightTrackTextureOffset);
    }

    IEnumerator playEngineSound()
    {
        engineStatus.clip = engineStarting;
        engineStatus.Play();
        yield return new WaitForSeconds(engineStatus.clip.length);
        if (engineStarted == true)
        {
            engineStatus.loop = true;
            engineStatus.clip = engineIdling;
            engineStatus.Play();
        }
    }

    void Move(float motorTorque, float turnTorque)
    {
        if (motorTorque >= 0)
        {
            forwardOrBackward = 1.0f;
        }
        else
        {
            forwardOrBackward = -1.0f;
        }
        for (int i = 0; i < 8; i++)
        {
            if (i != 0 && i != 7 && i != 8 && i != 15)
            {
                wheelColliders[i].motorTorque = motorTorque - turnTorque;
            }
        }
        for (int i = 8; i < 16; i++)
        {
            if (i != 0 && i != 7 && i != 8 && i != 15)
            {
                wheelColliders[i].motorTorque = motorTorque + turnTorque;
            }
        }
    }

    void UpdateMeshesPositions()
    {
        for (int i = 0; i < 16; i++)
        {
            Quaternion quat;
            Vector3 pos;
            wheelColliders[i].GetWorldPose(out pos, out quat);
            if (i != 0 && i != 7 && i != 8 && i != 15)
            {
                wheelMeshesPosition[i].position = pos;
                wheelMeshesPosition[i].position = wheelMeshesPosition[i].position - new Vector3(0, wheelsHeightOffset, 0);
                wheelMeshesRotation[i].rotation = quat;
            }
            else
            {
                wheelMeshesPosition[i].position = pos;
                wheelMeshesRotation[0].rotation = wheelMeshesRotation[1].rotation;
                wheelMeshesRotation[7].rotation = wheelMeshesRotation[1].rotation;
                wheelMeshesRotation[8].rotation = wheelMeshesRotation[9].rotation;
                wheelMeshesRotation[15].rotation = wheelMeshesRotation[9].rotation;
            }
            wheelMeshesRotation[i].rotation = wheelMeshesRotation[i].rotation * new Quaternion(0, 0, -180, 1);
        }
    }

    void AudioEngine()
    {
        float x = Mathf.Clamp(Mathf.Abs(throttle) / 1600f + Mathf.Abs(turnMultiplicator) / 1100f, 0f, 0.5f);
        float y = Mathf.Clamp(tankRigidbody.velocity.magnitude / 6.0f, 0f, 0.5f);
        if (engineStarted)
        {
            if (!engineRunning.isPlaying)
            {
                engineRunning.Play();
            }
            //engineRunning.pitch = 0.286f * x + 0.7143f;
            engineRunning.pitch = x + y;
            //engineRunning.volume = 19.075f * Mathf.Pow(x, 5.0f) - 57.543f * Mathf.Pow(x, 4.0f) + 65.445f * Mathf.Pow(x, 3.0f) - 34.588f * Mathf.Pow(x, 2.0f) + 8.6212f * x;
            engineRunning.volume = x + y;
            engineStatus.volume = 1 - engineRunning.volume;
        }
        else
        {
            engineStatus.volume = 1;
            engineRunning.Stop();
        }
    }

    void Brake()
    {
        isBraking = Input.GetKey(KeyCode.B);
        if (isBraking || (throttle == 0 && turnMultiplicator == 0))
        {
            for (int i = 0; i < 16; i++)
            {
                wheelColliders[i].brakeTorque = brakeForce;
            }
        }
        else
        {
            for (int i = 0; i < 16; i++)
            {
                wheelColliders[i].brakeTorque = 0;
            }
        }
    }
}

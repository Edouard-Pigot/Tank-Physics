using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelDisplacement : MonoBehaviour
{
    public int nbOfWheels = 0;
    public int nbOfStaticWheels = 0;
    public WheelCollider[] wheelColliders;
    public Transform[] wheelTransforms;
    public Transform[] wheelRotations;
    public Transform[] staticBoneWheelTransforms;
    public Transform[] staticBoneWheelRotations;
    public Transform[] staticWheel;
    public int wheelsHeightOffset = 0;

    void Start()
    {
       /* wheelColliders = new WheelCollider[nbOfWheels - nbOfStaticWheels];
        wheelTransforms = new Transform[nbOfWheels - nbOfStaticWheels];
        wheelBones = new Transform[nbOfWheels - nbOfStaticWheels];
        staticWheelTransforms = new Transform[nbOfStaticWheels];*/
    }
    
    void Update()
    {
        for (int i = 0; i < wheelTransforms.Length; i++)
        {
            Vector3 wheelColliderPosition;
            Quaternion wheelColliderRotation;
            wheelColliders[i].GetWorldPose(out wheelColliderPosition, out wheelColliderRotation);

            wheelTransforms[i].position = wheelColliderPosition - new Vector3(0, wheelsHeightOffset, 0);
            wheelRotations[i].rotation = wheelColliderRotation;

            //wheelMeshesRotation[i].rotation = wheelMeshesRotation[i].rotation * new Quaternion(0, 0, -180, 1);
        }
        /*for (int i = 0; i < staticBoneWheelTransforms.Length; i++)
        {
            Vector3 wheelColliderPosition;
            Quaternion wheelColliderRotation;
            wheelColliders[i].GetWorldPose(out wheelColliderPosition, out wheelColliderRotation);
            staticBoneWheelTransforms[i].position = wheelColliderPosition;

            //wheelMeshesRotation[i].rotation = wheelMeshesRotation[i].rotation * new Quaternion(0, 0, -180, 1);
        }*/
        for (int i = 0; i < staticBoneWheelRotations.Length; i++)
        {
            Vector3 wheelColliderPosition;
            Quaternion wheelColliderRotation;
            wheelColliders[i].GetWorldPose(out wheelColliderPosition, out wheelColliderRotation);
            staticBoneWheelRotations[i].rotation = wheelColliderRotation;

            //wheelMeshesRotation[i].rotation = wheelMeshesRotation[i].rotation * new Quaternion(0, 0, -180, 1);
        }
        for (int i = 0; i < staticWheel.Length; i++)
        {
            Vector3 wheelColliderPosition;
            Quaternion wheelColliderRotation;
            wheelColliders[i].GetWorldPose(out wheelColliderPosition, out wheelColliderRotation);
            staticWheel[i].rotation = wheelColliderRotation;

            //wheelMeshesRotation[i].rotation = wheelMeshesRotation[i].rotation * new Quaternion(0, 0, -180, 1);
        }
    }
}

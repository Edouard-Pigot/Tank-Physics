using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank_Movement : MonoBehaviour {

    public WheelCollider[] wheelColliders = new WheelCollider[16];
    public Transform[] wheelMeshesPos = new Transform[16];
    public Transform[] wheelMeshesQuat = new Transform[16];
    public float offset = 0.01f;
    public bool isOffset = false;

    private void Update()
    {
        UpdateMeshesPositions();
    }

    void UpdateMeshesPositions()
    {
        for(int i = 0; i < 16; i++)
        {
            Quaternion quat;
            Vector3 pos;
            wheelColliders[i].GetWorldPose(out pos, out quat);
            if (i != 0 && i != 7 && i != 8 && i != 16)
            {
                wheelMeshesPos[i].position = pos;
                wheelMeshesPos[i].position = wheelMeshesPos[i].position - new Vector3(0, offset, 0);
                wheelMeshesQuat[i].rotation = quat;
            }
            else
            {
                wheelMeshesPos[i].position = pos;
                wheelMeshesQuat[i].rotation = quat;
                if (isOffset)
                {
                    wheelMeshesPos[i].position = wheelMeshesPos[i].position - new Vector3(0, offset, 0);
                }
            }
            wheelMeshesQuat[i].rotation = wheelMeshesQuat[i].rotation * new Quaternion(0, 0, -180, 1);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TracksConnector : MonoBehaviour
{
    public GameObject[] tracks;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < tracks.Length; i++)
        {
            tracks[i].transform.position = transform.position;
            tracks[i].transform.rotation = transform.rotation;
        }
    }
}

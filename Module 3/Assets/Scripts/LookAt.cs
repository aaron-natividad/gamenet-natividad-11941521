using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class LookAt : MonoBehaviourPunCallbacks
{
    private Camera camera;

    void Update()
    {
        camera = Camera.allCameras[0];
        transform.LookAt(camera.transform);
    }
}

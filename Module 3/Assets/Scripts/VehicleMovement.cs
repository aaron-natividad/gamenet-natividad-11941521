using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class VehicleMovement : MonoBehaviourPunCallbacks
{
    public Camera camera;
    public float speed = 20;
    public float rotationSpeed = 200;
    public float currentSpeed = 0;
    public float baseFOV = 60;
    public bool isControlEnabled;

    void Start()
    {
        isControlEnabled = false;
    }

    void LateUpdate()
    {
        if (isControlEnabled)
        {
            // Movement
            float translation = Input.GetAxis("Vertical") * speed * Time.deltaTime;
            float rotation = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;

            transform.Translate(0, 0, translation);
            currentSpeed = translation;
            transform.Rotate(0, rotation, 0);

            photonView.RPC("ResetVelocity", RpcTarget.All);

            // Camera zoom out when going zoom zoom
            camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, baseFOV + (10 * Input.GetAxis("Vertical")), 0.1f);
        }
    }

    [PunRPC]
    public void ResetVelocity()
    {
        // Velocity reset per frame
        GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
    }
}

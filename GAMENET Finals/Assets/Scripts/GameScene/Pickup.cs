using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Pickup : MonoBehaviourPunCallbacks
{
    public bool isDeathPickup; // Check this property to get the right projectile
    // PHYSICS
    Rigidbody2D rb;

    #region Unity Methods
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Don't simulate gravity and physics if not master client
        if (!PhotonNetwork.IsMasterClient)
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }
    #endregion

    #region RPC Methods
    // Function to be called by other scripts
    public void DestroyPickup()
    {
        photonView.RPC("DestroyPickupRPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void DestroyPickupRPC()
    {
        Destroy(gameObject);
    }
    #endregion
}

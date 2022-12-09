using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public PlayerMaster pm;

    void Start()
    {
        if(!photonView.IsMine)
        {
            pm.Physics.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
            pm.Movement.isMine = false;
        }
        else
        {
            pm.Movement.isMine = true;
        }
    }
}

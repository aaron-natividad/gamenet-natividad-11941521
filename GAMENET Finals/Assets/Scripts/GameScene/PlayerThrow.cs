using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerThrow : MonoBehaviourPunCallbacks
{
    public PlayerMaster pm;

    [Header("Projectile Stats")]
    public int OwnerOrder;
    public string OwnerName;
    public float throwSpeed;                

    private bool isThrowEnabled;    // Used to activate throw logic
    private bool hasRockStored;     // Check if player has rock
    private bool isDeathProjectile;
    private bool isQueued;          // Used for negative edge
    private int inputQueue;         // Used for Feint/Throw

    // Update is called once per frame
    void Update()
    {
        if(!hasRockStored)
        {
            isThrowEnabled = false;
            pm.Movement.SetIsDashEnabled(true);
        }

        if (isThrowEnabled)
        {
            // queue up throw
            if (Input.GetMouseButtonDown(0) && !pm.Movement.GetInDash() && !isQueued)
            {
                QueueInput(0);
            }
            // queue up feint
            else if (Input.GetMouseButtonDown(1) && !pm.Movement.GetInDash() && !isQueued)
            {
                QueueInput(1);
            }
            // throw/feint
            else if (Input.GetMouseButtonUp(inputQueue) && isQueued)
            {
                if(inputQueue == 0)
                {
                    pm.RPC.SpawnProjectile(throwSpeed, isDeathProjectile);
                    hasRockStored = false;
                }
                isQueued = false;
                isDeathProjectile = false;
                pm.Movement.SetStandardPlayerMovementEnabled(true);
                pm.Movement.changeStates(3);
            }
        }
    }

    public void QueueInput(int input)
    {
        inputQueue = input;
        isQueued = true;
        pm.Movement.changeStates(2);
        pm.Movement.SetStandardPlayerMovementEnabled(false);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.tag == "Pickup" && hasRockStored == false)
        {
            if (pm.Movement.GetInDash())
            {
                isDeathProjectile = col.gameObject.GetComponent<Pickup>().isDeathPickup;
                col.gameObject.GetComponent<Pickup>().DestroyPickup();
                pm.Movement.SetIsDashEnabled(false);
                isThrowEnabled = true;
                hasRockStored = true;
            }
        }
    }
}

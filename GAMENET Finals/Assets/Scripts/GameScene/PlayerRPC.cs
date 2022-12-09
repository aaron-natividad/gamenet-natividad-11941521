using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerRPC : MonoBehaviourPunCallbacks
{
    public PlayerMaster pm;

    [Header("RPC Stats")]
    public float RespawnTime;

    public void SpawnProjectile(float throwSpeed, bool isDeathProjectile)
    {
        GameObject p = PhotonNetwork.Instantiate(isDeathProjectile? pm.DeathProjectilePrefab.name : pm.ProjectilePrefab.name, transform.position + new Vector3(0, 1, 0), Quaternion.identity);

        p.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer.ActorNumber);
        p.GetComponent<Projectile>().mine = true;  // handled client side so hitbox is only calculated once
        p.GetComponent<Projectile>().Initialize(Input.GetAxisRaw("Horizontal") * throwSpeed, Input.GetAxisRaw("Vertical") * throwSpeed, photonView.Owner.NickName, pm.Throw.OwnerOrder);
    }

    public void Die()
    {
        photonView.RPC("DieRPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void DieRPC()
    {
        Instantiate(pm.DeathExplosionPrefab, transform.position, Quaternion.identity);
        SetEnabled(false);
        StartCoroutine(RespawnCountdown());
    }

    public void SetEnabled(bool isEnabled)
    {
        pm.Collision.enabled = isEnabled;
        pm.Physics.simulated = isEnabled;
        pm.Movement.enabled = isEnabled;
        pm.Throw.enabled = isEnabled;
        pm.Sprite.enabled = isEnabled;
    }

    IEnumerator RespawnCountdown()
    {
        float respawnTime = RespawnTime;

        while (respawnTime > 0)
        {
            yield return new WaitForSeconds(1.0f);
            respawnTime--;
        }

        if (photonView.IsMine)
        {
            GameObject[] respawnPoints = GameObject.FindGameObjectsWithTag("Pickup");
            int index = Random.Range(0, respawnPoints.Length);
            transform.position = respawnPoints[index].transform.position;
            respawnPoints[index].GetComponent<Pickup>().DestroyPickup();
        }
        
        Instantiate(pm.DeathExplosionPrefab, transform.position, Quaternion.identity);
        SetEnabled(true);
    }
}

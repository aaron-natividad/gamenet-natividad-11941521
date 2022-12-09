using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Projectile : MonoBehaviourPunCallbacks
{
    [Header("Prefabs")]
    public GameObject PickupPrefab;
    public GameObject DeathPickupPrefab;

    [Header("Components")]
    public Rigidbody2D physics;

    [Header("Projectile Stats")]
    public int OwnerOrder;
    public string OwnerName;
    public bool isDeathProjectile;
    public bool spawned = false;
    public bool mine = false;

    void Update()
    {
        if (mine) // Calculate Rotation from Owner Local Client
        {
            // rotate
            Vector2 v = physics.velocity;
            float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        else
        {
            physics.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    public void Initialize(float initX, float initY, string ownerName, int ownerOrder)
    {
        OwnerOrder = ownerOrder;
        OwnerName = ownerName;
        physics.velocity = new Vector2(initX, initY);
    }

    [PunRPC]
    public void DestroyProjectile()
    {
        Destroy(gameObject);
    }

    [PunRPC]
    public void SpawnPickup()
    {
        PhotonNetwork.InstantiateRoomObject(isDeathProjectile ? DeathPickupPrefab.name : PickupPrefab.name, transform.position, Quaternion.identity);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Hit wall
        if ((collision.gameObject.name == "GroundHitbox" || collision.gameObject.name == "WallBox") && !spawned && mine)
        {
            spawned = true;
            photonView.RPC("SpawnPickup", RpcTarget.AllBuffered);
            photonView.RPC("DestroyProjectile", RpcTarget.AllBuffered);
        }

        // Kill enemy
        if(collision.gameObject.tag == "Player" && mine)
        {
            if(collision.gameObject.GetComponent<PlayerThrow>().OwnerName != OwnerName)
            {
                collision.gameObject.GetComponent<PlayerRPC>().Die();
                GameManager.instance.AddScore(OwnerName, isDeathProjectile);
            }
        }
    }
}

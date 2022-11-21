using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class RocketGun : Weapon
{
    public RaycastHit target;
    public GameObject missilePrefab;

    private GameObject missile;
    // Start is called before the first frame update
    void Start()
    {
        InitWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit = UpdateLockOn(2f, 200f);
        if (targetExists)
        {
            if (hit.collider.gameObject.CompareTag("Player") && !hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
            {
                if (Input.GetMouseButtonDown(0) && fireCD <= 0)
                {
                    fireCD = fireRate;
                    target = hit;
                    Fire();
                }
            } 
        }
        fireCD -= Time.deltaTime;
    }

    [PunRPC]
    public void SpawnRocket(Vector3 position)
    {
        missile = PhotonNetwork.Instantiate(missilePrefab.name, position, Quaternion.identity);
        missile.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer.ActorNumber);
        
    }

    protected override void Fire()
    {
        //photonView.RPC("SpawnRocket", RpcTarget.AllBuffered, weaponOut.position);
        SpawnRocket(weaponOut.position);
        missile.GetComponent<Missile>().SetTarget(target.collider.gameObject);
        missile.GetComponent<Missile>().mine = true;  // handled client side so hitbox is only calculated once
        missile.GetComponent<Missile>().damage = damage;
        missile.GetComponent<Missile>().ownerName = photonView.Owner.NickName;
    }
}

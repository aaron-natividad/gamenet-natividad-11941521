using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class Weapon : MonoBehaviourPunCallbacks
{
    public Camera camera;
    public Transform weaponOut;
    public Image crosshair;
    public LayerMask layerMask;

    [Header("Hit Effect")]
    public GameObject hitEffect;
    public float effectDuration;

    [Header("Weapon Stats")]
    public int damage;
    public float fireRate;
    protected float fireCD = 0;
    public float bulletSpreadAmount;
    public bool isAuto;
    public bool targetExists;

    protected virtual void Fire()
    {

    }
    
    [PunRPC]
    protected void CreateHitEffect(Vector3 position)
    {
        GameObject hitObject = Instantiate(hitEffect, position, Quaternion.identity);
        Destroy(hitObject, effectDuration);
    }
    
    protected void InitWeapon()
    {
        crosshair = GameObject.Find("LockOnCursor").GetComponent<Image>();
    }

    protected RaycastHit UpdateLockOn(float detectionRadius, float distance)
    {
        RaycastHit hit;

        if (Physics.SphereCast(weaponOut.position, detectionRadius, weaponOut.forward, out hit, distance, layerMask))
        {
            if (hit.collider.gameObject.CompareTag("Player") && !hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
            {
                crosshair.transform.position = camera.WorldToScreenPoint(hit.collider.gameObject.transform.position + new Vector3(0,0.8f,0));
                targetExists = true;
            }
            else
            {
                crosshair.transform.localPosition = new Vector3(0, 100, 0);
                targetExists = false;
            }
        }
        else
        {
            crosshair.transform.localPosition = new Vector3(0, 100, 0);
            targetExists = false;
        }
        return hit;
    }

    public void ApplyDamage(RaycastHit hit, int damage)
    {
        PlayerStats enemy = hit.collider.gameObject.GetComponent<PlayerStats>();
        hit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, photonView.Owner.NickName, damage);
        if (enemy.health <= 0)
        {
            enemy.ActivatePlayer(false);
        }
    }
}

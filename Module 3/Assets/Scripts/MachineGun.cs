using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class MachineGun : Weapon
{
    LineRenderer line;

    void Start()
    {
        InitWeapon();
        line = GetComponent<LineRenderer>();
        line.enabled = false;
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if(fireCD <= 0)
            {
                fireCD = fireRate;
                Fire();
            }
        }
        fireCD -= Time.deltaTime;
    }

    protected override void Fire()
    {
        line.SetPosition(0, weaponOut.position);
        RaycastHit hit;
        Vector3 rayDirection = weaponOut.forward + new Vector3(Random.Range(-bulletSpreadAmount, bulletSpreadAmount), Random.Range(-bulletSpreadAmount, bulletSpreadAmount));

        if (Physics.Raycast(weaponOut.position, rayDirection, out hit, 900f, layerMask))
        {
            if (hit.collider.gameObject.CompareTag("Player") && !hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
            {
                ApplyDamage(hit, damage);
            }
            photonView.RPC("CreateHitEffect",RpcTarget.AllBuffered,hit.point);
            line.SetPosition(1, hit.point);
            StartCoroutine(ShootLaser());
        }
    }

    IEnumerator ShootLaser()
    {
        line.enabled = true;
        yield return new WaitForSeconds(effectDuration);
        line.enabled = false;
    }
}

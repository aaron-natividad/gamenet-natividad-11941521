using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class Laser : Weapon
{
    public RaycastHit target;
    LineRenderer line;

    // Start is called before the first frame update
    void Start()
    {
        InitWeapon();
        line = GetComponent<LineRenderer>();
        line.enabled = false;
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

    protected override void Fire()
    {
        line.SetPosition(0, weaponOut.position);
        photonView.RPC("CreateHitEffect", RpcTarget.AllBuffered, target.transform.position);
        line.SetPosition(1, target.transform.position);
        ApplyDamage(target, damage);
        StartCoroutine(ShootLaser());
    }

    IEnumerator ShootLaser()
    {
        line.enabled = true;
        yield return new WaitForSeconds(effectDuration);
        line.enabled = false;
    }
}

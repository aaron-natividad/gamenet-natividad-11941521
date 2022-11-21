using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Missile : MonoBehaviourPunCallbacks
{
    public GameObject explosionEffect;
    public GameObject target;
    public Vector3 position;

    public string ownerName;
    public int damage;

    public float speed = 60;
    public float rotationSpeed = 0.001f;
    public float currentSpeed = 0;
    public bool mine = false;

    public Vector3 targetDirection;

    void Start()
    {
        if (target != null)
            transform.LookAt(target.transform.position);
        else
            transform.LookAt(position);

        Destroy(gameObject, 5);
    }

    // Update is called once per frame
    void Update()
    {
        if(mine)
            DoMovement();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Default") && mine)
        {
            if (collision.gameObject.CompareTag("Player") && !collision.gameObject.GetComponent<PhotonView>().IsMine)
            {
                PlayerStats enemy = collision.gameObject.GetComponent<PlayerStats>();
                collision.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, ownerName, damage);
                if (enemy.health <= 0)
                {
                    enemy.ActivatePlayer(false);
                }
            }
            Debug.Log(collision.gameObject.name);
            PhotonNetwork.Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        Vector3 pos = transform.position;
        GameObject effect = Instantiate(explosionEffect, pos, Quaternion.identity);
        Destroy(effect, 0.5f);
    }

    void DoMovement()
    {
        if (target != null) 
        {
            targetDirection = target.transform.position - transform.position;
        }
        else 
        {
            targetDirection = position - transform.position;
        }

        float singleStep = rotationSpeed * Time.deltaTime;
        Vector3 newRotation = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0f);

        float translation = speed * Time.deltaTime;
        currentSpeed = translation;
        transform.Translate(0, 0, translation);
        transform.rotation = Quaternion.LookRotation(newRotation);
    }

    public void SetTarget(GameObject target)
    {
        this.target = target;
    }
}

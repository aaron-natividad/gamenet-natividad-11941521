using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class Shooting : MonoBehaviourPunCallbacks
{
    public Camera camera;
    public GameObject hitEffectPrefab;
    private Animator animator;

    [Header("Healthbar")]
    public float startHealth = 100;
    private float health;
    public Image healthBar;

    [Header("PlayerUI")]
    public PlayerUI playerUI;

    // Stats
    private int score;
    private bool canFire;
    private bool isAlive;

    // Start is called before the first frame update
    void Start()
    {
        // Get component stuff
        animator = this.GetComponent<Animator>();
        playerUI = GameObject.Find("Player UI(Clone)").GetComponent<PlayerUI>();

        // Initialize Stats
        photonView.RPC("RegainHealth", RpcTarget.AllBuffered);
        score = 0;
        canFire = true;
        isAlive = true;
    }

    public void Fire()
    {
        if (canFire)
        {
            RaycastHit hit; // Initialize raycast hit
            Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f)); // Shoot raycast

            if (Physics.Raycast(ray, out hit, 200))
            {
                photonView.RPC("CreateHitEffects", RpcTarget.All, hit.point); // Spawn hit effect

                // Check if enemy is hit
                if (hit.collider.gameObject.CompareTag("Player") && !hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
                {
                    // Let enemy take damage
                    ApplyDamage(hit);
                }
            }
        }
    }

    public void Die()
    {
        if (photonView.IsMine)
        {
            animator.SetBool("isDead", true);
            StartCoroutine(RespawnCountdown());
        }
    }

    public void ApplyDamage(RaycastHit hit)
    {
        Shooting enemy = hit.collider.gameObject.GetComponent<Shooting>();

        hit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, 25);
        if (enemy.health <= 0 && enemy.isAlive)
        {
            score++;
            enemy.isAlive = false; // client side switch
            playerUI.UpdateScoreText(score);
            if (score >= GameManager.instance.maxScore)
            { 
                photonView.RPC("Win", RpcTarget.AllBuffered);
            }
        }
    }

    // Wait 5s and Respawn
    IEnumerator RespawnCountdown()
    {
        float respawnTime = 5.0f;

        while(respawnTime > 0)
        {
            photonView.RPC("EnablePlayer", RpcTarget.AllBuffered, false);
            transform.GetComponent<PlayerMovementController>().enabled = false;
            playerUI.UpdateRespawnText("You are killed. Respawning in " + respawnTime.ToString(".00"));
            yield return new WaitForSeconds(1.0f);
            respawnTime--;
        }

        animator.SetBool("isDead", false);
        playerUI.UpdateRespawnText("");

        int randomIndex = Random.Range(0, GameManager.instance.RespawnPoints.Length);
        this.transform.position = GameManager.instance.RespawnPoints[randomIndex].position;

        transform.GetComponent<PlayerMovementController>().enabled = true;
        photonView.RPC("EnablePlayer", RpcTarget.AllBuffered, true);
        photonView.RPC("RegainHealth", RpcTarget.AllBuffered);
    }

    IEnumerator WinCountdown(string winnerName)
    {
        float winCountdownTime = 5.0f;

        while(winCountdownTime > 0)
        {
            playerUI.UpdateWinText(winnerName + " wins!", true);
            transform.GetComponent<PlayerMovementController>().enabled = false;
            EnablePlayer(false);
            yield return new WaitForSeconds(1.0f);
            winCountdownTime--;
        }
        GameManager.instance.LeaveRoom();
    }

    [PunRPC]
    public void RegainHealth()
    {
        health = startHealth;
        healthBar.fillAmount = health / startHealth;
    }

    [PunRPC]
    public void TakeDamage(int damage, PhotonMessageInfo info)
    {
        this.health -= damage;
        this.healthBar.fillAmount = health / startHealth;

        if (health <= 0 && isAlive == true)
        {
            Die();
            GameManager.instance.UpdateKillHistory(info.Sender.NickName + " killed " + info.photonView.Owner.NickName);
            playerUI.UpdateKillUI();
        }
    }

    [PunRPC]
    public void CreateHitEffects(Vector3 position)
    {
        GameObject hitEffectGameObject = Instantiate(hitEffectPrefab, position, Quaternion.identity);
        Destroy(hitEffectGameObject, 0.2f);
    }

    [PunRPC]
    public void Win(PhotonMessageInfo info)
    {
        GameManager.instance.DeclareWinner(info.Sender.NickName);
    }

    public void GameOver(string winnerName)
    {
        StartCoroutine(WinCountdown(winnerName));
    }

    [PunRPC]
    public void EnablePlayer(bool isEnabled)
    {
        transform.gameObject.tag = isEnabled ? "Player" : "Respawn";
        transform.GetComponent<CapsuleCollider>().enabled = isEnabled;
        transform.GetComponent<Rigidbody>().useGravity = isEnabled;
        isAlive = isEnabled;
        canFire = isEnabled;
    }
}

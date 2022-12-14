using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityStandardAssets.Characters.FirstPerson;
using TMPro;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public GameObject fpsModel;
    public GameObject nonFpsModel;

    public GameObject playerUIPrefab;
    public PlayerMovementController playerMovementController;
    public Camera fpsCamera;

    private Animator animator;
    public Avatar fpsAvatar, nonFpsAvatar;

    private Shooting shooting;

    [SerializeField]
    TextMeshProUGUI playerNameText;

    // Start is called before the first frame update
    void Start()
    {
        // Get component stuff
        playerMovementController = this.GetComponent<PlayerMovementController>();
        animator = this.GetComponent<Animator>();
        shooting = this.GetComponent<Shooting>();

        // Change model based on photon view
        fpsModel.SetActive(photonView.IsMine);
        nonFpsModel.SetActive(!photonView.IsMine);
        // Change anim state based on photon view
        animator.SetBool("isLocalPlayer", photonView.IsMine);
        animator.avatar = photonView.IsMine ? fpsAvatar : nonFpsAvatar;

        // Set username of TextMeshProUGUI regardless of player
        playerNameText.text = photonView.Owner.NickName;

        if (photonView.IsMine)
        {
            // Instantiate and enable player UI
            GameObject playerUI = Instantiate(playerUIPrefab);
            playerMovementController.fixedTouchField = playerUI.transform.Find("RotationTouchField").GetComponent<FixedTouchField>();
            playerMovementController.joystick = playerUI.transform.Find("Fixed Joystick").GetComponent<Joystick>();
            fpsCamera.enabled = true;
            // Add fire button as listener
            playerUI.transform.Find("FireButton").GetComponent<Button>().onClick.AddListener(() => shooting.Fire());
        }
        else
        {
            // Make Non-FPS
            playerMovementController.enabled = false;
            GetComponent<RigidbodyFirstPersonController>().enabled = false;
            fpsCamera.enabled = false;
        }
    }
}

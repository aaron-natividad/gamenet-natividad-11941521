using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMaster : MonoBehaviour
{
    [Header("Player Script Components")]
    public PlayerSetup Setup;
    public PlayerMovement Movement;
    public PlayerThrow Throw;
    public PlayerRPC RPC;

    [Header("Player Unity Components")]
    public Rigidbody2D Physics;
    public CapsuleCollider2D Collision;

    [Header("Player Sprite")]
    public SpriteRenderer Sprite;
    public Animator Anim;

    [Header("Player Dash")]
    public DashTrail Trail;
    public Material StandardMaterial;
    public Material DashMaterial;

    [Header("Player Prefabs")]
    public GameObject ProjectilePrefab;
    public GameObject DeathProjectilePrefab;
    public GameObject DeathExplosionPrefab;

    [Header("Player Children")]
    public GroundCheck Ground;
}

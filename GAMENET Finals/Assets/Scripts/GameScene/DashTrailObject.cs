using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashTrailObject : MonoBehaviour
{
	[Header("Sprite")]
	public SpriteRenderer sprite;

	// PRIVATE STATS
	private bool Initiated;
	private float Life;
	private float MaxLife;
	private DashTrail Spawner;

	// Use this for initialization
	void Start()
	{
		sprite.enabled = false;
	}

	// Update is called once per frame
	void Update()
	{
		if (Initiated)
		{
			sprite.enabled = true;
			Life += Time.deltaTime;
			sprite.color = Color.Lerp(Spawner.StartColor, Spawner.EndColor, Life / MaxLife);

			if (Life >= MaxLife)
			{
				Spawner.Remove(gameObject);
				Destroy (gameObject);
			}
		}
	}

	public void Initiate(float life, Vector2 position, DashTrail trail)
	{
		MaxLife = life;
		transform.position = position;
		Life = 0;
		Spawner = trail;
		Initiated = true;
	}
}

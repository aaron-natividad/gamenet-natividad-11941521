using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashTrail : MonoBehaviour
{
	// Base code taken from CloudyWater, but modified to fit this project better

	[Header("Player Parts")]
	public PlayerMaster pm;
	public GameObject DashObject;

	[Header("Dash Image Stats")]
	public int DashImageCount;
	public float ImageLife;
	public Color StartColor;
	public Color EndColor;

	// PRIVATE HANDLERS
	private float SpawnInterval;
	private float SpawnTimer;
	private List<GameObject> DashImages;

	void Start()
	{
		SpawnInterval = ImageLife / DashImageCount;
		DashImages = new List<GameObject>();
	}

	void Update()
	{
		if (pm.Anim.GetBool("isDashing"))
		{
			pm.Sprite.material = pm.DashMaterial;
			SpawnTimer += Time.deltaTime;

			if (SpawnTimer >= SpawnInterval)
			{
				GameObject trail = GameObject.Instantiate(DashObject);

				trail.GetComponent<DashTrailObject>().Initiate(ImageLife, transform.position, this);
				trail.transform.localScale = pm.Sprite.gameObject.transform.localScale;
				DashImages.Add(trail);

				SpawnTimer = 0;
			}
		}
        else
        {
			pm.Sprite.material = pm.StandardMaterial;
        }
	}

	public void Remove(GameObject obj)
	{
		DashImages.Remove(obj);
	}
}
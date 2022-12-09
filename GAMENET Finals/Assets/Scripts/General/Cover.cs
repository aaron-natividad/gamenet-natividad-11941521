using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cover : MonoBehaviour
{
    public Vector3 UncoverLocation;
    public float Acceleration;
    public bool done = false;

    private float speed = 0;
    private bool covered = true;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (covered)
        {
            if (Mathf.Abs(transform.position.y) < 5f)
            {
                transform.position = new Vector3(0, 0, 0);
                speed = 0;
                done = true;
            }
            else
            {
                transform.position += new Vector3(0, speed, 0);
                speed += Acceleration;
                done = false;
            }
        }
        else
        {
            if (Mathf.Abs(UncoverLocation.y - transform.position.y) < 5f)
            {
                transform.position = UncoverLocation;
                done = true;
            }
            else
            {
                transform.position -= new Vector3(0, speed, 0);
                speed += Acceleration;
                done = false;
            }
        }
    }

    public void SetCover(bool isCovered)
    {
        covered = isCovered;
        done = false;
    }
}

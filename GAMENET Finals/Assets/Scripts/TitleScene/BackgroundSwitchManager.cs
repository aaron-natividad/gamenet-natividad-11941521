using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSwitchManager : MonoBehaviour
{
    public int BackgroundIndex;
    public List<Vector3> BackgroundLocations;

    public float InitialMovementSpeed;
    public float MovementAcceleration;
    public float VibrateTime;
    public float VibrateMagnitude;

    private int Sequence;
    private float MovementSpeed;
    private float VibrateCD;

    public bool MoveToLocation()
    {
        float distance = transform.position.y - BackgroundLocations[BackgroundIndex].y;
        MovementSpeed += MovementAcceleration;

        if (Mathf.Abs(distance) < VibrateMagnitude)
        {
            MovementSpeed = 0;
            if (VibrateCD >= 0)
            {
                transform.position = BackgroundLocations[BackgroundIndex] + new Vector3 (0,Random.Range(-VibrateMagnitude,VibrateMagnitude),0);
                VibrateCD -= Time.deltaTime;
                return false;
            }
            else
            {
                transform.position = BackgroundLocations[BackgroundIndex];
                return true;
            }
        }
        else if(distance < 0)
        {
            VibrateCD = VibrateTime;
            transform.position += new Vector3(0, MovementSpeed, 0);
            return false;
        }
        else if (distance > 0)
        {
            VibrateCD = VibrateTime;
            transform.position -= new Vector3(0, MovementSpeed, 0);
            return false;
        }
        return false;
    }
}

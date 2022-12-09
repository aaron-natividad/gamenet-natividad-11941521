using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float MaxShakeTime;
    public float ShakeMagnitude;
    private float ShakeTime;

    // Update is called once per frame
    void Update()
    {
        if(ShakeTime > 0)
        {
            transform.position = new Vector3(Random.Range(-ShakeMagnitude, ShakeMagnitude), Random.Range(-ShakeMagnitude, ShakeMagnitude), -10);
            ShakeTime -= Time.deltaTime;
        }
        else
        {
            transform.position = new Vector3(0, 0, -10);
        }
    }

    public void Shake()
    {
        ShakeTime = MaxShakeTime;
    }
}

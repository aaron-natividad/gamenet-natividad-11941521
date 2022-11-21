using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LockOnTest : MonoBehaviour
{
    public Camera camera;
    public Image crosshair;
    public GameObject item;
    void Start()
    {
        crosshair = GameObject.Find("LockOnCursor").GetComponent<Image>();
    }
    // Update is called once per frame
    void Update()
    {
        camera = Camera.current;
        crosshair.transform.position = camera.WorldToScreenPoint(item.transform.position);
    }
}

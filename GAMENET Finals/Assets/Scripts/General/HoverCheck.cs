using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverCheck : MonoBehaviour
{
    public bool isHighlighted;
    public int HoverID;

    private void OnMouseOver()
    {
        isHighlighted = true;
    }

    private void OnMouseExit()
    {
        isHighlighted = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartManager : MonoBehaviour
{
    [Header("Cart Locations")]
    public List<Vector3> CartLocations;

    [Header("Cart Parameters")]
    public int CartIndex;
    public float LerpValue;
    public float Allowance;

    private void Update()
    {
        float distance = transform.position.x - CartLocations[CartIndex].x; // Get distance for position locking

        if (Mathf.Abs(distance) < Allowance)
            transform.position = CartLocations[CartIndex]; // Set location to cart
        else
            transform.position = Vector3.Lerp(transform.position, CartLocations[CartIndex], LerpValue); // Lerp to cart location
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public PlayerMovement playerMovement;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Steppable" || collision.gameObject.tag == "Pickup")
        {
            playerMovement.SetGrounded(true);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Steppable" || collision.gameObject.tag == "Pickup")
        {
            playerMovement.SetGrounded(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Steppable" || other.gameObject.tag == "Pickup")
        {
            playerMovement.SetGrounded(false);
        }
    }
}

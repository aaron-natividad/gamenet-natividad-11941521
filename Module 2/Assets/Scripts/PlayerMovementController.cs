using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerMovementController : MonoBehaviour
{
    public Joystick joystick;
    public FixedTouchField fixedTouchField;

    private RigidbodyFirstPersonController rbController;

    private Animator animator;

    void Start()
    {
        rbController = this.GetComponent<RigidbodyFirstPersonController>();
        animator = this.GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if(rbController != null)
        {
            rbController.joystickInputAxis.x = joystick.Horizontal;
            rbController.joystickInputAxis.y = joystick.Vertical;
            rbController.mouseLook.lookInputAxis = fixedTouchField.TouchDist;
        }

        animator.SetFloat("horizontal", joystick.Horizontal);
        animator.SetFloat("vertical", joystick.Vertical);

        if(Mathf.Abs(joystick.Horizontal) > 0.9 || Mathf.Abs(joystick.Vertical) > 0.9)
        {
            animator.SetBool("isRunning", true);
            rbController.movementSettings.ForwardSpeed = 10;
        }
        else
        {
            animator.SetBool("isRunning", false);
            rbController.movementSettings.ForwardSpeed = 5;
        }
    }
}

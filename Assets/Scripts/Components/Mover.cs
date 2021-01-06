using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class Mover : MonoBehaviour
{
    //[SerializeField]
    private InputSystem inputSystem => Core.InputSystem;

    [SerializeField]
    private float moveSpeed = 4f;
    [SerializeField]
    private float groundControl = 0.2f;
    [SerializeField]
    private float brakeRatio = 0.2f;
    [SerializeField]
    private float airControl = 0.05f;
    [SerializeField]
    private float verticalAirSpeed = 0.1f;
    [Space]
    [SerializeField]
    private float jumpSpeed = 4f;
    [SerializeField]
    private float jumpInAirTime = 0.2f;
    [Space]
    [SerializeField]
    private float groundNormalThreshold = 0.5f;
    [SerializeField]
    private float groundStick = 1f;

    private float gravityScale => Core.GravityScale;

    Rigidbody2D rbody;

    float airTime = 0f;
    bool jumping = false;
    bool grounded;
    Collider2D ground;
    Vector2 down;
    Vector2 forward;

    private void OnEnable()
    {
        inputSystem.OnJumpEvent += TryJump;
        inputSystem.OnPauseEvent += Pause;
        rbody = GetComponent<Rigidbody2D>();
    }

    private void Pause()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnDisable()
    {
        inputSystem.OnJumpEvent -= TryJump;
        inputSystem.OnPauseEvent -= Pause;
    }

    private void TryJump(bool obj)
    {
        jumping = obj;
    }

    static float ClampVelAxis(float value, float control)
    {
        float vAbs = Mathf.Abs(value);
        return Mathf.Clamp(value - control, -vAbs, vAbs); // TODO: Finish
    }

    private void FixedUpdate()
    {
        // If intending to have moving bodies, will have to implement different system

        airTime += Time.fixedDeltaTime;

        Vector2 newVelocity = rbody.velocity;

        if (jumping && airTime < jumpInAirTime)
        {
            newVelocity += new Vector2(0, jumpSpeed);
            airTime = jumpInAirTime;
            grounded = false;
            jumping = false;
        }
        else if (ground == null) grounded = false;

        if (grounded)
        {
            rbody.gravityScale = 0;

            if (ground.attachedRigidbody == null)
                newVelocity += down * groundStick;
            //ground.attachedRigidbody.velocity += -down * groundStick;

            newVelocity += forward * (((inputSystem.Move.x * moveSpeed) * groundControl) - rbody.velocity.x * brakeRatio);
        }
        else
        {
            rbody.gravityScale = gravityScale;

            newVelocity += new Vector2(
                ClampVelAxis(inputSystem.Move.x * moveSpeed, rbody.velocity.x) * airControl,
                inputSystem.Move.y * verticalAirSpeed);
        }
        rbody.velocity = newVelocity;
        grounded = false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        int size = collision.contactCount;
        for (int i = 0; i < size; i++)
        {
            ContactPoint2D contact = collision.GetContact(i);

            if (contact.normal.y > groundNormalThreshold)
            {
                if (!grounded)
                {
                    airTime = 0f;
                    grounded = true;
                    ground = contact.collider;
                    down = -contact.normal;
                    forward = MathUtil.RotateRight90(contact.normal);
                }
                else
                {
                    if (contact.rigidbody == null)
                        ground = contact.collider;
                    down = Vector2.down;
                    forward = Vector2.right;
                }
            }

            // Perform other checks here, such as if holding on to a roof?
            // Velocity on impact for deformables or detonatables?
        }
    }
    
}

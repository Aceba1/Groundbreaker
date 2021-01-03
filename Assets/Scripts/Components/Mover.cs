using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    private float airControl = 0.05f;
    [SerializeField]
    private float verticalAirSpeed = 0.1f;
    [SerializeField]
    private float jumpSpeed = 4f;

    Rigidbody2D rbody;

    bool grounded;
    Collider2D ground;

    private void OnEnable()
    {
        rbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    static float ClampVelAxis(float value, float control)
    {
        float vAbs = Mathf.Abs(value);
        return Mathf.Clamp(value - control, -vAbs, vAbs); // TODO: Finish
    }

    private void FixedUpdate()
    {
        // If intending to have moving bodies, will have to implement different system

        Vector2 newVelocity;
        if (grounded)
        {
            newVelocity = new Vector2(
                (inputSystem.Move.x * moveSpeed - rbody.velocity.x) * groundControl,
                inputSystem.Jump ? jumpSpeed : 0f);
        }
        else
        {
            newVelocity = new Vector2(
                ClampVelAxis(inputSystem.Move.x * moveSpeed, rbody.velocity.x) * airControl,
                inputSystem.Move.y * verticalAirSpeed);
        }
        rbody.velocity += newVelocity;
        grounded = false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        int size = collision.contactCount;
        for (int i = 0; i < size; i++)
        {
            ContactPoint2D contact = collision.GetContact(i);

            if (contact.normal.y > 0.7f)
            {
                grounded = true;
                ground = contact.collider;
            }

            // Perform other checks here, such as if holding on to a roof?
            // Velocity on impact for deformables or detonatables?
        }
    }
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class Mover : MonoBehaviour
{
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
    [SerializeField]
    private Joystick joystick;
    [SerializeField]
    private SimpleButton jumpButton;

    Rigidbody2D rbody;
    bool grounded;
    Collider2D ground;
    bool jump;

    private void OnEnable()
    {
        jumpButton.OnPressEvent += Jump; // TODO: Must replace with something that activates on press!
        rbody = GetComponent<Rigidbody2D>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        jump |= Input.GetButtonDown("Jump");
    }

    void Jump() =>
        jump = true;

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
                (joystick.Horizontal * moveSpeed - rbody.velocity.x) * groundControl, 
                jump ? jumpSpeed : 0f);
        }
        else
        {
            newVelocity = new Vector2(
                ClampVelAxis(joystick.Horizontal * moveSpeed, rbody.velocity.x) * airControl,
                joystick.Vertical * verticalAirSpeed);
        }
        rbody.velocity += newVelocity;
        jump = false;
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

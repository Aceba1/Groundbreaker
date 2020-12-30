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
    private float groundControl = 0.75f;
    [SerializeField]
    private float airControl = 0.25f;
    [SerializeField]
    private float jumpSpeed = 4f;
    [SerializeField]
    private Joystick joystick;
    [SerializeField]
    private Button jumpButton;

    Rigidbody2D rbody;
    bool grounded;
    Collider2D ground;
    bool jump;

    private void OnEnable()
    {
        jumpButton.onClick.AddListener(Jump); // TODO: Must replace with something that activates on press!
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

    static float ClampSigned(float value, float control)
    {
        Mathf.Sign(value)
    }

    private void FixedUpdate()
    {
        // If intending to have moving bodies, will have to implement different system

        Vector2 newVelocity = new Vector2(ClampSigned(joystick.Horizontal * moveSpeed, rbody.velocity.x), 0f);
        if (grounded)
        {
            newVelocity *= groundControl;
            if (jump)
                newVelocity += Vector2.up * jumpSpeed;
        }
        else
        {
            newVelocity *= airControl;
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

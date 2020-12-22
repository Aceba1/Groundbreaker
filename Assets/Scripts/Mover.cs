using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Mover : MonoBehaviour
{
    Rigidbody2D rbody;
    bool jump;
    bool jumping;

    private void Awake()
    {
        rbody = GetComponent<Rigidbody2D>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        jump = Input.GetButton("Jump");
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        int size = collision.contactCount;
        for (int i = 0; i < size; i++)
        {
            ContactPoint2D contact = collision.GetContact(i);
            
            if (jump && contact.normal.y > 0.8f)
            {
                rbody.velocity += Vector2.up * 4;
            }
        }
    }
}

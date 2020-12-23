using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(Mover))]
public class Player : MonoBehaviour
{
    Mover mover;
    InputController input;
    // Start is called before the first frame update
    void OnEnable ()
    {
        mover = GetComponent<Mover>();
        input = GetComponent<InputController>();
    }

    // Update is called once per frame
    void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //if (Input.GetKey(KeyCode.LeftShift))
        // Cycle brush types
        //else
        radius = Mathf.Round(radius * 10f + Input.mouseScrollDelta.y) * 0.1f;
        
        bool leftClick = Input.GetMouseButton(0);
        bool rightClick = Input.GetMouseButton(1);
        if (leftClick || rightClick)
        {
            Collider2D other = Physics2D.OverlapPoint(mousePos);
            if (other != null)
            {
                Deformable deformable = other.GetComponent<Deformable>();
                if (deformable != null)
                    deformable.Deform(other.transform.InverseTransformPoint(mousePos), radius, rightClick);
            }
        }
    }

    private static readonly Vector3 INVALID_V3 = new Vector3(-1337, -1337, -1337);

    //DEBUG 
    Vector3 mousePos = INVALID_V3;
    float radius = 1;

    private void OnDrawGizmos()
    {
        if (mousePos == INVALID_V3)
            return;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(mousePos, radius);
    }
}

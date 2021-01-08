using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(Mover))]
public class Player : MonoBehaviour
{
    //Mover mover;

    SquareBrush brush = new SquareBrush(1f);

    void OnEnable()
    {
        //mover = GetComponent<Mover>();
    }

    // Update is called once per frame
    void Update()
    {
        
        //if (Input.GetKey(KeyCode.LeftShift))
        // Cycle brush types
        //else
        radius = Mathf.Max(Mathf.Round(radius * 10f + Input.mouseScrollDelta.y) * 0.1f, 0.1f);



        bool touched = Input.touchCount > 0;
        bool leftClick = Input.GetMouseButton(0);
        bool rightClick = Input.GetMouseButton(1);
        if (touched || leftClick || rightClick)
        {
            if (touched)
                mousePos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            else
                mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            brush.worldRadius = radius;
            brush.massStrength = leftClick ? -1f : 1f;
            Core.DeformWorld.Deform(brush, mousePos);
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
        Gizmos.DrawWireCube(mousePos, Vector3.one * (radius * 2));
        //Gizmos.DrawWireSphere(mousePos, radius);
    }
}

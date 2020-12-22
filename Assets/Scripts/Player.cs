using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Mover))]
public class Player : MonoBehaviour
{
    Mover mover;
    InputController input;
    // Start is called before the first frame update
    void Awake()
    {
        mover = GetComponent<Mover>();
        input = GetComponent<InputController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

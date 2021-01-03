using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour
{
    [SerializeField]
    private InputSystem inputSystem;
    public static InputSystem InputSystem { get; private set; }

    [SerializeField]
    private Deformable deformWorld;
    public static Deformable DeformWorld { get; private set; }

    private void OnEnable()
    {
        InputSystem = inputSystem;
        DeformWorld = deformWorld;
        Application.targetFrameRate = 60;
    }

    // Start is called before the first frame update
    void Start()
    {
    }
}

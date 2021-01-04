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

    [SerializeField]
    private float gravityScale;
    public static float GravityScale { get; private set; }

    private void OnEnable()
    {
        InputSystem = inputSystem;
        DeformWorld = deformWorld;
        GravityScale = gravityScale;
        Application.targetFrameRate = 60;
    }

#if UNITY_EDITOR
    private void Update()
    {
        InputSystem = inputSystem;
        DeformWorld = deformWorld;
        GravityScale = gravityScale;
    }
#endif
}

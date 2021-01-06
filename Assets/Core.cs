using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Move to Player gameobject
[RequireComponent(typeof(InputMapper))]
public class Core : MonoBehaviour
{
    [SerializeField]
    private List<InputSystem> inputSystems;
    public static InputSystem InputSystem { get; private set; }

    [SerializeField]
    private Deformable deformWorld;
    public static Deformable DeformWorld { get; private set; }

    [SerializeField]
    private float gravityScale;
    
    public static float GravityScale { get; private set; }

    private InputMapper inputMapper;

    private void Awake()
    {
        inputMapper = GetComponent<InputMapper>();

        foreach (InputSystem system in inputSystems)
            inputMapper.AddToWatch(system);

        InputSystem = inputMapper;
    }

    private void OnEnable()
    {
        DeformWorld = deformWorld;
        GravityScale = gravityScale;
        Application.targetFrameRate = 60;
    }

    private void OnDisable()
    {
        foreach (InputSystem system in inputSystems)
            inputMapper.RemoveFromWatch(system);
    }

    void SwitchInput()
    {
    }

#if UNITY_EDITOR
    private void Update()
    {
        DeformWorld = deformWorld;
        GravityScale = gravityScale;
    }
#endif
}

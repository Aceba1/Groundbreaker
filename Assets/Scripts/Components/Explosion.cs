using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Explosion : MonoBehaviour
{
    public DeformBrush brush;
    [SerializeField]
    private DeformBrush.Shape shape;

    [SerializeField]
    private float mass;
    [SerializeField]
    private DeformBrush.Effect massEffect;

    [SerializeField]
    private DeformBrush sBrush;

    [SerializeField]
    private float simpleRadius;

    private ParticleSystem localParticleSystem;

    // Start is called before the first frame update
    void Start()
    {
        if (localParticleSystem == null)
            localParticleSystem = GetComponent<ParticleSystem>();
        if (localParticleSystem == null)
            localParticleSystem = GetComponentInChildren<ParticleSystem>();

        switch (shape)
        {
            case DeformBrush.Shape.Circle:
                brush = new CircleBrush(simpleRadius, mass, massEffect);
                break;

            case DeformBrush.Shape.Square:
                brush = new SquareBrush(simpleRadius, mass, massEffect);
                break;

            default:
                throw new Exception($"Shape type {shape} has no case constructor!");
        }
    }

    public void Explode(Deformable deformable)
    {
        var tr = localParticleSystem.transform;
        tr.localPosition = Vector3.back;
        tr.localScale = Vector3.one * simpleRadius;
        localParticleSystem.Play(true);
        deformable.Deform(brush, transform.position);
    }
}

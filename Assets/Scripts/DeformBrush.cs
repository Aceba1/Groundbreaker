using System.Collections.Generic;
using UnityEngine;

abstract class DeformBrush : MonoBehaviour
{
    public readonly Shape shape;
    public readonly Effect effect;

    public Vector2 worldPos;

    protected DeformBrush(Shape shape, Effect effect)
    {
        this.shape = shape;
        this.effect = effect;
    }

    public abstract BoundsInt GetBounds();

    public enum Effect : byte
    {
        SetMass,
        ModMass,
        SetHard,
        ModHard,
    }
    public enum Shape : byte
    {
        Circle,
        Rect,
        Tri,
        Quad,
        Poly
    }
}

class CircleBrush : DeformBrush
{
    public CircleBrush(Effect effect = Effect.SetMass) : base(Shape.Circle, effect)
    {

    }

    public override BoundsInt GetBounds()
    {
        throw new System.NotImplementedException();
    }
}
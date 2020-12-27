using System.Collections.Generic;
using UnityEngine;

abstract class DeformBrush
{
    public readonly Shape shape;
    public readonly Effect massEffect;

    public Vector2 worldPos;

    protected DeformBrush(Shape shape, Effect massEffect, Effect hardEffect)
    {
        this.shape = shape;
        this.massEffect = massEffect;
        this.hardEffect = hardEffect;
    }

    public abstract BoundsInt GetBounds();

    public enum Effect : byte
    {
        None,
        Set,
        Mod,
        SetIfEmpty,
        ModIfEmpty
    }
    public enum Shape : byte
    {
        Circle,
        Square,
        Rect,
        Quad,
        Tri,
        Convex
    }
}

class CircleBrush : DeformBrush, IPointCloudBrush
{
    public CircleBrush(Effect massEffect = Effect.Set, Effect hardEffect = Effect.None) : base(Shape.Circle, massEffect, hardEffect)
    {

    }

    public override BoundsInt GetBounds()
    {
        throw new System.NotImplementedException();
    }

    public void Modify(byte[,] cloud, int squareCount, Vector2 offset, float pointSize)
    {
        throw new System.NotImplementedException();
    }
}
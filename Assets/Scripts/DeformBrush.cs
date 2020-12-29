using System.Collections.Generic;
using UnityEngine;

abstract class DeformBrush : IPointCloudBrush
{
    public readonly Shape shape;
    public readonly Effect massEffect;
    public float massStrength;
    //public readonly Effect hardEffect;

    protected DeformBrush(Shape shape, float mass, Effect massEffect)//, Effect hardEffect)
    {
        this.shape = shape;
        this.massEffect = massEffect;
        this.massStrength = mass;
        //this.hardEffect = hardEffect;
    }

    public abstract Vector4 GetBounds();
    public abstract int Modify(byte[][] cloud, int squareCount, Vector2 relativePos, float pointSize);

    public enum Effect : byte
    {
        None,
        Set,
        Mod,
        SetIfNotEmpty,
        ModIfNotEmpty
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

class CircleBrush : DeformBrush
{
    public float worldRadius;

    public CircleBrush(float radius, float mass = 1f, Effect massEffect = Effect.Set /*Effect hardEffect = Effect.None*/) : base(Shape.Circle, mass, massEffect)//, hardEffect)
    {
        worldRadius = radius;
    }

    public override Vector4 GetBounds() =>
        new Vector4(-worldRadius, -worldRadius, worldRadius, worldRadius);

    public override int Modify(byte[][] cloud, int size, Vector2 relativePos, float pointSize)
    {
        int deltaMass = 0;

        float relX = relativePos.x / pointSize, 
            relY = relativePos.y / pointSize, 
            radius = worldRadius / pointSize;

        int minX = Mathf.Max((int)(relX - radius + 0.9f), 0),
            maxX = Mathf.Min((int)(relX + radius), size),
            minY = Mathf.Max((int)(relY - radius + 0.9f), 0),
            maxY = Mathf.Min((int)(relY + radius), size);

        float radSq = radius * radius;

        for (int y = minY; y <= maxY; y++)
        {
            var strip = cloud[y];
            for (int x = minX; x <= maxX; x++)
            {
                // Within radius check
                float distSq = (x - relX) * (x - relX) + (y - relY) * (y - relY);
                if (distSq >= radSq) continue;

                byte value = strip[x];
                int mass = PointCloud.GetMass(value);
                int hard = PointCloud.GetHard(value);

                //if (modifyHard != 0)
                //    hard = Mathf.Clamp(hard + modifyHard, 0, 15);

                deltaMass -= mass; // Calculate new mass
                switch (massEffect)
                {
                    default:
                    case Effect.None:
                        break;

                    case Effect.Set:
                        {
                            int feather = RadProximity(radius, Mathf.Sqrt(distSq));
                            mass = massStrength > 0 ?
                                Mathf.Max(mass, feather) :
                                Mathf.Min(mass, 15 - feather);
                        }
                        break;

                    case Effect.Mod:
                        break;

                    case Effect.SetIfNotEmpty:
                        if (mass > MarchingSquares.THRESHOLD)
                        {
                            int feather = RadProximity(radius, Mathf.Sqrt(distSq));
                            mass = massStrength > 0 ?
                                Mathf.Max(mass, feather) :
                                Mathf.Min(mass, 15 - feather);
                        }
                        break;

                    case Effect.ModIfNotEmpty:
                        break;
                }

                deltaMass += mass;

                strip[x] = PointCloud.JoinValues(mass, hard);
            }
        }

        return deltaMass;
    }


    static int RadProximity(float radius, float dist) =>
        (int)(Mathf.Clamp01(radius - dist) * 15);
}

class SquareBrush : DeformBrush
{
    public float worldRadius;

    public SquareBrush(float radius, float mass = 1f, Effect massEffect = Effect.Set) : base(Shape.Circle, mass, massEffect)//, hardEffect)
    {
        worldRadius = radius;
    }

    public override Vector4 GetBounds() =>
        new Vector4(-worldRadius, -worldRadius, worldRadius, worldRadius);

    public override int Modify(byte[][] cloud, int size, Vector2 relativePos, float pointSize)
    {
        int deltaMass = 0;

        float relX = relativePos.x / pointSize,
            relY = relativePos.y / pointSize,
            radius = worldRadius / pointSize;

        int minX = Mathf.Max((int)(relX - radius + 0.9f), 0),
            maxX = Mathf.Min((int)(relX + radius), size),
            minY = Mathf.Max((int)(relY - radius + 0.9f), 0),
            maxY = Mathf.Min((int)(relY + radius), size);

        for (int y = minY; y <= maxY; y++)
        {
            var strip = cloud[y];
            for (int x = minX; x <= maxX; x++)
            {
                byte value = strip[x];

                float dist = Mathf.Max(Mathf.Abs(relX - x), Mathf.Abs(relY - y));

                int mass = PointCloud.GetMass(value);
                int hard = PointCloud.GetHard(value);

                //if (modifyHard != 0)
                //    hard = Mathf.Clamp(hard + modifyHard, 0, 15);
                
                deltaMass -= mass; // Calculate new mass
                switch (massEffect)
                {
                    default:
                    case Effect.None:
                        break;

                    case Effect.Set:
                        {
                            int feather = SquProximity(radius, dist);
                            mass = massStrength > 0 ?
                                Mathf.Max(mass, feather) :
                                Mathf.Min(mass, 15 - feather);
                        }
                        break;

                    case Effect.Mod:
                        break;

                    case Effect.SetIfNotEmpty:
                        if (mass > MarchingSquares.THRESHOLD)
                        {
                            int feather = SquProximity(radius, dist);
                            mass = massStrength > 0 ?
                                Mathf.Max(mass, feather) :
                                Mathf.Min(mass, 15 - feather);
                        }
                        break;

                    case Effect.ModIfNotEmpty:
                        break;
                }
                deltaMass += mass;

                strip[x] = PointCloud.JoinValues(mass, hard);
            }
        }

        return deltaMass;
    }


    static int SquProximity(float radius, float dist) => 
        (int)(Mathf.Clamp01(radius - dist) * 15);
}
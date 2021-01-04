using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

static class MathUtil
{
    public static float SideOfLine(float aX, float aY, float bX, float bY, float pX, float pY) =>
        (pX - aX) * (bY - aY) -
        (pY - aY) * (bX - aX);

    public static float SideOfLine(Vector2 A, Vector2 B, Vector2 P) =>
        (B.y - A.y) * (P.x - A.x) -
        (B.x - A.x) * (P.y - A.y);

    public static bool PointInQuad(Vector2 A, Vector2 B, Vector2 C, Vector2 D, Vector2 point) => 
        SideOfLine(A, B, point) > 0 && // To the right of
        SideOfLine(B, C, point) > 0 && 
        SideOfLine(C, D, point) > 0 && 
        SideOfLine(D, A, point) > 0;

    public static float DepthInRectangle(Vector2 A, Vector2 B, Vector2 C, Vector2 D, Vector2 point) =>
        Mathf.Min(
            SquaredSigned(SideOfLine(A, B, point)) / DistanceSq(A, B),
            SquaredSigned(SideOfLine(B, C, point)) / DistanceSq(B, C),
            SquaredSigned(SideOfLine(C, D, point)) / DistanceSq(C, D),
            SquaredSigned(SideOfLine(D, A, point)) / DistanceSq(D, A));

    public static float DepthInRectangle(Edge A, Edge B, Edge C, Edge D, Vector2 point) =>
        Mathf.Min(
            SquaredSigned(A.SideOf(point)) / A.lengthSq,
            SquaredSigned(B.SideOf(point)) / B.lengthSq,
            SquaredSigned(C.SideOf(point)) / C.lengthSq,
            SquaredSigned(D.SideOf(point)) / D.lengthSq);

    public static float Squared(float value) => 
        value * value;

    public static float SquaredSigned(float value) => 
        value * Mathf.Abs(value); // * Mathf.Sign(value)

    public static float DistanceSq(Vector2 A, Vector2 B) =>
        (B.x - A.x) * (B.x - A.x) + (B.y - A.y) * (B.y - A.y);

    public static bool PointInConvex(Vector2 point, params Vector2[] poly)
    {
        for (int i = 1; i < poly.Length; i++)
            if (SideOfLine(poly[i - 1], poly[i], point) <= 0) // To the left of?
                return false;
        return true;
    }

    public static float Slope(Vector2 A, Vector2 B) =>
        (B.y - A.y) / (B.x - A.x);

    public static bool Collinear(Vector2 A, Vector2 B, Vector2 C) =>
        (C.y - B.y) * (B.x - A.x) ==
        (B.y - A.y) * (C.x - B.x);

    public static Vector2 RotateRight90(Vector2 value) =>
        new Vector2(value.y, -value.x);
    public static Vector2 RotateLeft90(Vector2 value) =>
        new Vector2(-value.y, value.x);

    public struct Edge
    {
        public Edge(Vector2 A, Vector2 B)
        {
            this.A = A;
            this.B = B;
            lengthSq = DistanceSq(A, B);
            length = Mathf.Sqrt(lengthSq);
        }

        public float SideOf(Vector2 point) =>
            SideOfLine(A, B, point);

        public readonly Vector2 A, B;

        public readonly float lengthSq;
        public readonly float length;
    }
}
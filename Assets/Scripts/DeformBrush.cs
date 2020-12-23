using System.Collections.Generic;
using UnityEngine;

abstract class DeformBrush
{
    public Effect type;

    enum Effect : byte
    {
        SetMass,
        ModMass,
        SetHard,
        ModHard,
    }
    enum Shape : byte
    {
        Circle,
        Rect,
        Tri,
        Quad,
        Poly
    }
}

class 
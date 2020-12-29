using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface IPointCloudBrush
{
    //TODO: Rotation?

    Vector4 GetBounds();

    int Modify(byte[][] cloud, int squareCount, Vector2 relativePos, float pointSize);
}
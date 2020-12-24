using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

interface IPointCloudBrush
{
    //void Modify(byte[,] cloud, int size, Vector2 offset, float pointSize, float rotation);

    void Modify(byte[,] cloud, int squareCount, Vector2 offset, float pointSize);
}
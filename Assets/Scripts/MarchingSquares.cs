using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class MarchingSquares
{
    PointCloud pointCloud;
    float scale;
    readonly List<Vector3> points;
    readonly List<int> triangles;

    public MarchingSquares()
    {
        points = new List<Vector3>();
        triangles = new List<int>();
    }

    public MarchingSquares(PointCloud page, float scale) : this() =>
        SetCloudPage(page, scale);

    public void SetCloudPage(PointCloud page, float scale) {
        pointCloud = page;
        this.scale = scale;
    }

    void AddTriangle(int A, int B, int C)
    {
        triangles.Add(A);
        triangles.Add(B);
        triangles.Add(C);
    }

    void MakeTriangle(Vector2 A, Vector2 B, Vector2 C)
    {
        int c = points.Count;
        points.Add(A * scale);
        points.Add(B * scale);
        points.Add(C * scale);
        AddTriangle(c, c + 1, c + 2);
    }

    void MakeQuad(Vector2 A, Vector2 B, Vector2 C, Vector2 D)
    {
        int c = points.Count;
        points.Add(A * scale);
        points.Add(B * scale);
        points.Add(C * scale);
        points.Add(D * scale);
        AddTriangle(c, c + 1, c + 2);
        AddTriangle(c, c + 2, c + 3);
    }

    void MakePentagon(Vector2 A, Vector2 B, Vector2 C, Vector2 D, Vector2 E)
    {
        int c = points.Count;
        points.Add(A * scale);
        points.Add(B * scale);
        points.Add(C * scale);
        points.Add(D * scale);
        points.Add(E * scale);
        AddTriangle(c, c + 1, c + 2);
        AddTriangle(c, c + 2, c + 3);
        AddTriangle(c, c + 3, c + 4);
    }

    void MakeHexagon(Vector2 A, Vector2 B, Vector2 C, Vector2 D, Vector2 E, Vector2 F)
    {
        int c = points.Count;
        points.Add(A * scale);
        points.Add(B * scale);
        points.Add(C * scale);
        points.Add(D * scale);
        points.Add(E * scale);
        points.Add(F * scale);
        AddTriangle(c, c + 1, c + 2);
        AddTriangle(c, c + 2, c + 3);
        AddTriangle(c, c + 3, c + 4);
        AddTriangle(c, c + 4, c + 5);
    }

    public Mesh March()
    {
        if (pointCloud == null)
            throw new NullReferenceException("MarchingSquares.March() : pointCloud is undefined");

        points.Clear();
        triangles.Clear();

        byte[][] cloud = pointCloud.cloud;
        int size = pointCloud.size;

        //TODO: Cache right, top interpolated points for left, bottom of next

        var lastStrip = cloud[0];
        for (int y = 1; y < size; y++)
        {
            var nextStrip = cloud[y];

            //TODO: Cache left points
            for (int x = 0; x < size; x++)
            {
                //TODO: Swap dual-index array for multi-array? Will permit row point cache
                int BL = PointCloud.GetMass(lastStrip[x]);
                int BR = PointCloud.GetMass(lastStrip[x + 1]);
                int TL = PointCloud.GetMass(nextStrip[x]);
                int TR = PointCloud.GetMass(nextStrip[x + 1]);
                GenerateMesh(BL, BR, TL, TR, x, y);
            }
            lastStrip = nextStrip;
        }
        Mesh mesh = new Mesh();
        mesh.SetVertices(points);
        mesh.SetTriangles(triangles, 0);
        return mesh;
    }

    static int MapCase(int BL, int BR, int TL, int TR)
    {
        return (BL > 7 ? 1 : 0) + (BR > 7 ? 2 : 0) + (TL > 7 ? 8 : 0) + (TR > 7 ? 4 : 0);
    }

    // 8 4
    // 1 2

    void GenerateMesh(int BL, int BR, int TL, int TR, int x, int y)
    {
        switch (MapCase(BL, BR, TL, TR))
        {
            case 0: 
                return;

            #region Corners
            case 0b_0001:
                MakeTriangle(
                    new Vector2(x, y),
                    new Vector2(x, y + 0.5f/*Mathf.Lerp(y, y + 1, (BL - TL) / 15f)*/),
                    new Vector2(x + 0.5f/*Mathf.Lerp(x, x + 1, (BR - BL) / 15f)*/, y)
                    ); return;
            case 0b_0010:
                MakeTriangle(
                    new Vector2(x + 1, y),
                    new Vector2(x + 0.5f, y),
                    new Vector2(x + 1, y + 0.5f)
                    ); return;
            case 0b_0100:
                MakeTriangle(
                    new Vector2(x + 1, y + 1),
                    new Vector2(x + 1, y + 0.5f),
                    new Vector2(x + 0.5f, y + 1)
                    ); return;
            case 0b_1000:
                MakeTriangle(
                    new Vector2(x, y + 1),
                    new Vector2(x + 0.5f, y + 1),
                    new Vector2(x, y + 0.5f)
                    ); return;
            #endregion

            #region Walls
            case 0b_0011:
                MakeQuad(
                    new Vector2(x, y),
                    new Vector2(x, y + 0.5f),
                    new Vector2(x + 1, y + 0.5f),
                    new Vector2(x + 1, y)
                    ); return;
            case 0b_0110:
                MakeQuad(
                    new Vector2(x + 1, y),
                    new Vector2(x + 0.5f, y),
                    new Vector2(x + 0.5f, y + 1),
                    new Vector2(x + 1, y + 1)
                    ); return;
            case 0b_1100:
                MakeQuad(
                    new Vector2(x + 1, y + 1),
                    new Vector2(x + 1, y + 0.5f),
                    new Vector2(x, y + 0.5f),
                    new Vector2(x, y + 1)
                    ); return;
            case 0b_1001:
                MakeQuad(
                    new Vector2(x, y + 1),
                    new Vector2(x + 0.5f, y + 1),
                    new Vector2(x + 0.5f, y),
                    new Vector2(x, y)
                    ); return;
            #endregion

            #region Valleys
            case 0b_0111:
                MakePentagon(
                    new Vector2(x + 1, y),
                    new Vector2(x, y),
                    new Vector2(x, y + 0.5f),
                    new Vector2(x + 0.5f, y + 1),
                    new Vector2(x + 1, y + 1)
                    ); return;
            case 0b_1110:
                MakePentagon(
                    new Vector2(x + 1, y + 1),
                    new Vector2(x + 1, y),
                    new Vector2(x + 0.5f, y),
                    new Vector2(x, y + 0.5f),
                    new Vector2(x, y + 1)
                    ); return;
            case 0b_1101:
                MakePentagon(
                    new Vector2(x, y + 1),
                    new Vector2(x + 1, y + 1),
                    new Vector2(x + 1, y + 0.5f),
                    new Vector2(x + 0.5f, y + 0),
                    new Vector2(x, y)
                    ); return;
            case 0b_1011:
                MakePentagon(
                    new Vector2(x, y),
                    new Vector2(x, y + 1),
                    new Vector2(x + 0.5f, y + 1),
                    new Vector2(x + 1, y + 0.5f),
                    new Vector2(x + 1, y)
                    ); return;
            #endregion

            #region Saddles

            case 0b_0101:
                if (BL + BR + TL + TR > 30)
                {
                    MakeHexagon(
                        new Vector2(x, y),
                        new Vector2(x, y + 0.5f),
                        new Vector2(x + 0.5f, y + 1),
                        new Vector2(x + 1, y + 1),
                        new Vector2(x + 1, y + 0.5f),
                        new Vector2(x + 0.5f, y)); 
                    return;
                }
                MakeTriangle( // case 0b_0001
                    new Vector2(x, y),
                    new Vector2(x, y + 0.5f/*Mathf.Lerp(y, y + 1, (BL - TL) / 15f)*/),
                    new Vector2(x + 0.5f/*Mathf.Lerp(x, x + 1, (BR - BL) / 15f)*/, y)
                    );
                MakeTriangle( // case 0b_0100
                    new Vector2(x + 1, y + 1),
                    new Vector2(x + 1, y + 0.5f),
                    new Vector2(x + 0.5f, y + 1)
                    ); 
                return;

            case 0b_1010:
                if (BL + BR + TL + TR > 30)
                {
                    MakeHexagon(
                        new Vector2(x + 1, y),
                        new Vector2(x + 0.5f, y),
                        new Vector2(x, y + 0.5f),
                        new Vector2(x, y + 1),
                        new Vector2(x + 0.5f, y + 1),
                        new Vector2(x + 1, y + 0.5f));
                    return;
                }
                MakeTriangle( // case 0b_0010
                    new Vector2(x + 1, y),
                    new Vector2(x + 0.5f, y),
                    new Vector2(x + 1, y + 0.5f)
                    );
                MakeTriangle( // case 0b_1000
                    new Vector2(x, y + 1),
                    new Vector2(x + 0.5f, y + 1),
                    new Vector2(x, y + 0.5f)
                    );
                return;

            #endregion

            case 15:
                MakeQuad(
                    new Vector2(x, y),
                    new Vector2(x, y + 1),
                    new Vector2(x + 1, y + 1),
                    new Vector2(x + 1, y)
                    ); return;
            default: return;
        }
    }

    //struct Square
    //{

    //}
}
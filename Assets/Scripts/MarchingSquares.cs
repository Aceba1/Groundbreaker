using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

class MarchingSquares
{
    public const int THRESHOLD = 7;

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

        //Dictionary<int, Vector2>

        //TODO: Cache right, top interpolated points for left, bottom of next

        var lastStrip = cloud[0];
        for (int y = 1; y < size; y++)
        {
            var nextStrip = cloud[y];

            //TODO: Cache left points
            
            for (int x = 0; x < size; x++)
            {
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

    static int MapCase(int BL, int BR, int TL, int TR) =>
        (BL > THRESHOLD ? 1 : 0) +
        (BR > THRESHOLD ? 2 : 0) +
        (TL > THRESHOLD ? 8 : 0) +
        (TR > THRESHOLD ? 4 : 0);

    static float Intp(int weightA, int weightB) => 
        Mathf.Clamp((weightA + weightB - THRESHOLD) / (30f - THRESHOLD * 2), 0, 1);
    
    static float Intn(int weightA, int weightB) =>
        1f - Intp(weightA, weightB);

    //void GenerateMeshOptimized

    void GenerateMesh(int BL, int BR, int TL, int TR, int x, int y)
    {
        switch (MapCase(BL, BR, TL, TR))
        {
            default:
            case 0: 
                return;

            #region Corners
            case 0b_0001: // TopRight Face
                MakeTriangle(
                    new Vector2(x, y),
                    new Vector2(x, y + Intp(BL, TL)),
                    new Vector2(x + Intp(BL, BR), y)
                    ); return;
            case 0b_0010: // TopLeft Face
                MakeTriangle(
                    new Vector2(x + 1, y),
                    new Vector2(x + Intn(BL, BR), y),
                    new Vector2(x + 1, y + Intp(BR, TR))
                    ); return;
            case 0b_0100: // BottomLeft Face
                MakeTriangle(
                    new Vector2(x + 1, y + 1),
                    new Vector2(x + 1, y + Intn(BR, TR)),
                    new Vector2(x + Intn(TL, TR), y + 1)
                    ); return;
            case 0b_1000: //BottomRight Face
                MakeTriangle(
                    new Vector2(x, y + 1),
                    new Vector2(x + Intp(TL, TR), y + 1),
                    new Vector2(x, y + Intn(BL, TL))
                    ); return;
            #endregion

            #region Walls
            case 0b_0011: // Top Face
                MakeQuad(
                    new Vector2(x, y),
                    new Vector2(x, y + Intp(BL, TL)),
                    new Vector2(x + 1, y + Intp(BR, TR)),
                    new Vector2(x + 1, y)
                    ); return;
            case 0b_0110: // Left Face
                MakeQuad(
                    new Vector2(x + 1, y),
                    new Vector2(x + Intn(BL, BR), y),
                    new Vector2(x + Intn(TL, TR), y + 1),
                    new Vector2(x + 1, y + 1)
                    ); return;
            case 0b_1100: // Bottom Face
                MakeQuad(
                    new Vector2(x + 1, y + 1),
                    new Vector2(x + 1, y + Intn(BR, TR)),
                    new Vector2(x, y + Intn(BL, TL)),
                    new Vector2(x, y + 1)
                    ); return;
            case 0b_1001: // Right Face
                MakeQuad(
                    new Vector2(x, y + 1),
                    new Vector2(x + Intp(TL, TR), y + 1),
                    new Vector2(x + Intp(BL, BR), y),
                    new Vector2(x, y)
                    ); return;
            #endregion

            #region Valleys
            case 0b_0111: // TopLeft Face
                MakePentagon(
                    new Vector2(x + 1, y),
                    new Vector2(x, y),
                    new Vector2(x, y + Intp(BL, TL)),
                    new Vector2(x + Intn(TL, TR), y + 1),
                    new Vector2(x + 1, y + 1)
                    ); return;
            case 0b_1110: // BottomLeft Face
                MakePentagon(
                    new Vector2(x + 1, y + 1),
                    new Vector2(x + 1, y),
                    new Vector2(x + Intn(BL, BR), y),
                    new Vector2(x, y + Intn(BL, TL)),
                    new Vector2(x, y + 1)
                    ); return;
            case 0b_1101: // BottomRight Face
                MakePentagon(
                    new Vector2(x, y + 1),
                    new Vector2(x + 1, y + 1),
                    new Vector2(x + 1, y + Intn(BR, TR)),
                    new Vector2(x + Intp(BL, BR), y),
                    new Vector2(x, y)
                    ); return;
            case 0b_1011: // TopRight Face
                MakePentagon(
                    new Vector2(x, y),
                    new Vector2(x, y + 1),
                    new Vector2(x + Intp(TL, TR), y + 1),
                    new Vector2(x + 1, y + Intp(BR, TR)),
                    new Vector2(x + 1, y)
                    ); return;
            #endregion

            #region Saddles

            case 0b_0101: // BottomLeft to TopRight
                if (BL + BR + TL + TR > 30)
                {
                    MakeHexagon(
                        new Vector2(x, y),
                        new Vector2(x, y + Intp(BL,TL)),
                        new Vector2(x + Intn(TL,TR), y + 1),
                        new Vector2(x + 1, y + 1),
                        new Vector2(x + 1, y + Intn(BR,TR)),
                        new Vector2(x + Intp(BL,BR), y)); 
                    return;
                }
                MakeTriangle( // TopRight Face
                    new Vector2(x, y),
                    new Vector2(x, y + Intp(BL,TL)),
                    new Vector2(x + Intp(BL,BR), y)
                    );
                MakeTriangle( // BottomLeft Face
                    new Vector2(x + 1, y + 1),
                    new Vector2(x + 1, y + Intn(BL,TL)),
                    new Vector2(x + Intn(TL,TR), y + 1)
                    ); 
                return;

            case 0b_1010:
                if (BL + BR + TL + TR > 30)
                {
                    MakeHexagon(
                        new Vector2(x + 1, y),
                        new Vector2(x + Intn(BL,BR), y),
                        new Vector2(x, y + Intp(BL,TL)),
                        new Vector2(x, y + 1),
                        new Vector2(x + Intp(TL,TR), y + 1),
                        new Vector2(x + 1, y + Intn(BR,TR)));
                    return;
                }
                MakeTriangle( // TopLeft Face
                    new Vector2(x + 1, y),
                    new Vector2(x + Intn(BL, BR), y),
                    new Vector2(x + 1, y + Intp(BR, TR))
                    );
                MakeTriangle( // BottomRight Face
                    new Vector2(x, y + 1),
                    new Vector2(x + Intp(TL, TR), y + 1),
                    new Vector2(x, y + Intn(BL, TL))
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
        }
    }

    #region Tracer

    public Vector2[][] MarchTrace()
    {
        if (pointCloud == null)
            throw new NullReferenceException("MarchingSquares.MarchTrace() : pointCloud is undefined");

        HashSet<Outline> total = new HashSet<Outline>();
        Dictionary<int, Outline> processing = new Dictionary<int, Outline>();
        Outline current = null;
        
        points.Clear();

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
                int BL = PointCloud.GetMass(lastStrip[x]);
                int BR = PointCloud.GetMass(lastStrip[x + 1]);
                int TL = PointCloud.GetMass(nextStrip[x]);
                int TR = PointCloud.GetMass(nextStrip[x + 1]);

                switch (MapCase(BL, BR, TL, TR))
                {
                    // For easier visualization, imagine bending the bit line to a U shape
                    // 0b_8-4-2-1
                    // 8-4-\
                    // 1-2-/

                    // Empty
                    case 0: break;

                    #region Corners

                    // Up Right Face: Use current, Merge with below
                    case 0b_0001:

                        if (y == 0) // On the wall?
                        {
                            current = new Outline();
                            total.Add(current);
                            current.AddFirst(new Vector2(x + 0.5f, y)); // CALCULATE
                        }
                        if (x == 0) // On the floor?
                            current.AppendLast(new Vector2(x + 0.5f, y)); // CALCULATE
                        else
                        {
                            var merge = processing[x];
                            current.AddLast(merge);
                            processing.Remove(x);
                            total.Remove(merge);
                        }
                        break;

                    // Up Left Face: Use below, Add to Last, Set Current
                    case 0b_0010:

                        if (x == 0) // On the floor?
                        {
                            current = new Outline();
                            total.Add(current);
                            current.AddFirst(new Vector2(x + 0.5f, y)); // CALCULATE
                        }
                        else
                            current = processing[x];
                        current.AppendLast(new Vector2(x + 1, y + 0.5f)); // CALCULATE
                        processing.Remove(x);
                        break;

                    // Down Left Face: Create new Outline, Open for above, Set Current
                    case 0b_0100:

                        current = new Outline();
                        total.Add(current);
                        current.AddFirst(new Vector2(x + 0.5f, y + 1)); // CALCULATE
                        current.AddLast(new Vector2(x + 1, y + 0.5f)); // CALCULATE
                        processing[x] = current;
                        break;

                    // Down Right Face: Use current, Add to First, Open for above
                    case 0b_1000:

                        current.AppendFirst(new Vector2(x + 0.5f, y + 1));
                        processing[x] = current;
                        break;

                    #endregion

                    #region Walls

                    // Up Face: Use current, Add to Last
                    case 0b_1100:

                        if (x == 0) // On the wall? 
                        {
                            current = new Outline();
                            total.Add(current);
                            current.AddFirst(new Vector2(x, y + 0.5f)); // CALCULATE 
                        }
                        current.AppendLast(new Vector2(x + 1, y + 0.5f)); // CALCULATE
                        break;

                    // Left Face: Use below, Join to Last, Open for above
                    case 0b_0110:

                        if (y == 0) // On the floor?
                        {
                            current = new Outline();
                            total.Add(current);
                            current.AddFirst(new Vector2(x + 0.5f, y)); // CALCULATE
                            processing[x] = current;
                        }
                        else
                            current = processing[x];
                        current.AppendLast(new Vector2(x + 0.5f, y + 1)); // CALCULATE
                        break;

                    // Down Face: Use current, Add to First
                    case 0b_0011:

                        if (x == 0) // On the wall?
                        {
                            current = new Outline();
                            total.Add(current);
                            current.AddFirst(new Vector2(x, y + 0.5f)); // CALCULATE 
                        }
                        current.AppendFirst(new Vector2(x + 1, y + 0.5f)); // CALCULATE
                        break;

                    // Right Face: Use below, Add to First, Open for above
                    case 0b_1001:

                        if (y == 0) // On the floor?
                        {
                            current = new Outline();
                            total.Add(current);
                            current.AddFirst(new Vector2(x + 0.5f, y)); // CALCULATE
                            processing[x] = current;
                        }
                        else
                            current = processing[x];
                        current.AppendFirst(new Vector2(x + 0.5f, y + 1));
                        break;

                    #endregion

                    #region Valleys 

                    // Up Left Face: Use current, Add to Last, Open for above
                    case 0b_0111:

                        current.AppendLast(new Vector2(x + 0.5f, y + 1)); // CALCULATE
                        processing[x] = current;
                        break;

                    // Up Right Face: Create new Outline, Open for above, Set Current
                    case 0b_1011:

                        current = new Outline();
                        total.Add(current);
                        current.AddFirst(new Vector2(x + 0.5f, y + 1)); // CALCULATE
                        current.AddFirst(new Vector2(x + 1, y + 0.5f)); // CALCULATE
                        processing[x] = current;
                        break;

                    // Down Right Face: Use below, Add to First, Set Current
                    case 0b_1101:

                        current = processing[x];
                        current.AppendLast(new Vector2(x + 1, y + 0.5f)); // CALCULATE
                        processing.Remove(x);
                        break;

                    // Down Left Face: Use current, Merge with below
                    case 0b_1110:

                        {
                            var merge = processing[x];
                            current.AddFirst(merge);
                            total.Remove(merge);
                            processing.Remove(x);
                            break;
                        }

                    #endregion

                    #region Saddles

                    // Down Left, Top Right
                    case 0b_0101:
                        if (BL + BR + TL + TR > 30)
                        {

                        }
                        else
                        {

                        }

                        break;

                    // Down Right, Top Left
                    case 0b_1010:
                        if (BL + BR + TL + TR > 30)
                        {

                        }
                        else
                        {

                        }

                        break;

                    #endregion

                    // Full
                    case 0b_1111:
                        // Should check if at corners of grid
                        break;
                }

                // -- FILLED SPACE --

                //if (x == 0)
                //{
                //    if (y == 0)
                //    {
                //        current = new Outline();
                //        current.AddLast(Vector2.zero); // It is what it is
                //    }
                //}

                //GenerateMesh(BL, BR, TL, TR, x, y);
            }
            lastStrip = nextStrip;
        }

        Vector2[][] output = new Vector2[total.Count][];

        int i = 0;
        foreach (var shape in total)
            output[i++] = shape.ToArray();

        

        //mesh.SetVertices(vertices);
        //mesh.SetSubMesh(0, new SubMeshDescriptor(0, vertices.Count, MeshTopology.LineStrip));

        //mesh.subMeshCount = total.Count;
        //mesh.SetSubMeshes(descriptors);

        return output;
    }
    
    class Outline : LinkedList<Vector2>
    {
        public void AppendLast(Vector2 point)
        {
            if (Count >= 2 && MathUtil.Collinear(Last.Previous.Value, Last.Value, point))
                RemoveLast();
            AddLast(point);
        }

        public void AppendFirst(Vector2 point)
        {
            if (Count >= 2 && MathUtil.Collinear(First.Next.Value, First.Value, point))
                RemoveFirst();
            AddFirst(point);
        }

        public void AddLast(Outline points)
        {
            if (points == this) return;

            //TODO: Use Append at beginning?
            var last = points.First;
            for (int i = 0; i < points.Count; i++)
            {
                AddLast(last.Value);
                last = last.Next;
            }
        }

        public void AddFirst(Outline points)
        {
            if (points == this) return;

            var last = points.Last;
            for (int i = 0; i < points.Count; i++) {
                AddFirst(last.Value);
                last = last.Previous;
            }
        }
    }

    #endregion

    //struct Square
    //{

    //}
}
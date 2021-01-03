using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

internal class MarchingSquares
{
    public const int THRESHOLD = 7;
    private const int TPROC_LEFT = -1;
    private const int TPROC_RIGHT = -2;
    private PointCloud pointCloud;
    private float scale;
    private readonly List<Vector3> points;
    private readonly List<int> triangles;

    public MarchingSquares()
    {
        points = new List<Vector3>();
        triangles = new List<int>();
    }

    public MarchingSquares(PointCloud page, float scale) : this() =>
        SetCloudPage(page, scale);

    public void SetCloudPage(PointCloud page, float scale)
    {
        pointCloud = page;
        this.scale = scale;
    }

    private void AddTriangle(int A, int B, int C)
    {
        triangles.Add(A);
        triangles.Add(B);
        triangles.Add(C);
    }

    private void MakeTriangle(Vector2 A, Vector2 B, Vector2 C)
    {
        int c = points.Count;
        points.Add(A * scale);
        points.Add(B * scale);
        points.Add(C * scale);
        AddTriangle(c, c + 1, c + 2);
    }

    private void MakeQuad(Vector2 A, Vector2 B, Vector2 C, Vector2 D)
    {
        int c = points.Count;
        points.Add(A * scale);
        points.Add(B * scale);
        points.Add(C * scale);
        points.Add(D * scale);
        AddTriangle(c, c + 1, c + 2);
        AddTriangle(c, c + 2, c + 3);
    }

    private void MakePentagon(Vector2 A, Vector2 B, Vector2 C, Vector2 D, Vector2 E)
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

    private void MakeHexagon(Vector2 A, Vector2 B, Vector2 C, Vector2 D, Vector2 E, Vector2 F)
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

    public void March()
    {
        if (pointCloud == null)
            throw new NullReferenceException("MarchingSquares.March() : pointCloud is undefined");

        points.Clear();
        triangles.Clear();

        byte[][] cloud = pointCloud.cloud;
        int size = pointCloud.Size;

        // <X-Coord, Index>
        Dictionary<int, int> vertexCellIndex = new Dictionary<int, int>();

        //TODO: Cache right, top interpolated points for left, bottom of next

        var lastStrip = cloud[0];
        for (int y = 0; y < size; y++)
        {
            var nextStrip = cloud[y + 1];

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
    }

    public void UpdateMesh(Mesh mesh)
    {
        mesh.Clear();
        mesh.SetVertices(points);
        mesh.SetTriangles(triangles, 0);
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.SetVertices(points);
        mesh.SetTriangles(triangles, 0);
        return mesh;
    }

    public Mesh MarchMesh()
    {
        March();
        return CreateMesh();
    }

    public void MarchMesh(Mesh meshToUpdate)
    {
        March();
        UpdateMesh(meshToUpdate);
    }

    private static int MapCase(int BL, int BR, int TL, int TR) =>
        (BL > THRESHOLD ? 1 : 0) +
        (BR > THRESHOLD ? 2 : 0) +
        (TL > THRESHOLD ? 8 : 0) +
        (TR > THRESHOLD ? 4 : 0);

    private static float Intp(int weightA, int weightB) =>
        Mathf.Clamp((weightA + weightB - THRESHOLD) / 15f, 0, 1);

    private static float Intn(int weightA, int weightB) =>
        1f - Intp(weightA, weightB);

    //void GenerateMeshOptimized

    private void GenerateMesh(int BL, int BR, int TL, int TR, int x, int y)
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

            #endregion Corners

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

            #endregion Walls

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

            #endregion Valleys

            #region Saddles

            case 0b_0101: // BottomLeft to TopRight
                if (BL + BR + TL + TR > 30)
                {
                    MakeHexagon(
                        new Vector2(x, y),
                        new Vector2(x, y + Intp(BL, TL)),
                        new Vector2(x + Intn(TL, TR), y + 1),
                        new Vector2(x + 1, y + 1),
                        new Vector2(x + 1, y + Intn(BR, TR)),
                        new Vector2(x + Intp(BL, BR), y));
                    return;
                }
                MakeTriangle( // TopRight Face
                    new Vector2(x, y),
                    new Vector2(x, y + Intp(BL, TL)),
                    new Vector2(x + Intp(BL, BR), y)
                    );
                MakeTriangle( // BottomLeft Face
                    new Vector2(x + 1, y + 1),
                    new Vector2(x + 1, y + Intn(BL, TL)),
                    new Vector2(x + Intn(TL, TR), y + 1)
                    );
                return;

            case 0b_1010:
                if (BL + BR + TL + TR > 30)
                {
                    MakeHexagon(
                        new Vector2(x + 1, y),
                        new Vector2(x + Intn(BL, BR), y),
                        new Vector2(x, y + Intn(BL, TL)),
                        new Vector2(x, y + 1),
                        new Vector2(x + Intp(TL, TR), y + 1),
                        new Vector2(x + 1, y + Intp(BR, TR)));
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

            #endregion Saddles

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

    private HashSet<Outline> tracerTotal;
    private Dictionary<int, Outline> tracerProc;
    private Outline tracerCurr;

    public IEnumerator<Vector2[][]> MarchTrace()
    {
        if (pointCloud == null)
            throw new NullReferenceException("MarchingSquares.MarchTrace() : pointCloud is undefined");

        tracerTotal = new HashSet<Outline>();
        tracerProc = new Dictionary<int, Outline>();
        tracerCurr = null;
        points.Clear();

        byte[][] cloud = pointCloud.cloud;
        int size = pointCloud.Size;

        //TODO: Cache right, top interpolated points for left, bottom of next

        var lastStrip = cloud[0];
        for (int y = 0; y < size; y++)
        {
            var nextStrip = cloud[y + 1];

            //TODO: Cache left points
            for (int x = 0; x < size; x++)
            {
                Square square = new Square(
                    lastStrip[x],
                    lastStrip[x + 1],
                    nextStrip[x],
                    nextStrip[x + 1]);

                switch (square.MapCase)
                {
                    // For easier visualization, imagine bending the bit line to a U shape
                    // 0b_8-4-2-1
                    // 8-4-\
                    // 1-2-/

                    // Empty
                    default:
                    case 0: break;

                    #region Corners

                    // Up Right Face: Use current, Merge with below
                    case 0b_0001:

                        TraceCornerUR(x, y, size, square);
                        yield return GenTrace();
                        break;

                    // Up Left Face: Use below, Add to Last, Set Current
                    case 0b_0010:

                        TraceCornerUL(x, y, size, square);
                        yield return GenTrace(); 
                        break;

                    // Down Left Face: Create new Outline, Open for above, Set Current
                    case 0b_0100:

                        TraceCornerDL(x, y, size, square);
                        yield return GenTrace(); 
                        break;

                    // Down Right Face: Use current, Add to First, Open for above
                    case 0b_1000:

                        TraceCornerDR(x, y, size, square);
                        yield return GenTrace(); 
                        break;

                    #endregion Corners

                    #region Walls

                    // Up Face: Use current, Add to Last
                    case 0b_0011:

                        TraceWallU(x, y, size, square);
                        yield return GenTrace(); 
                        break;

                    // Left Face: Use below, Join to Last, Open for above
                    case 0b_0110:

                        TraceWallL(x, y, size, square);
                        yield return GenTrace(); 
                        break;

                    // Down Face: Use current, Add to First
                    case 0b_1100:

                        TraceWallD(x, y, size, square);
                        yield return GenTrace(); 
                        break;

                    // Right Face: Use below, Add to First, Open for above
                    case 0b_1001:

                        TraceWallR(x, y, size, square);
                        yield return GenTrace(); 
                        break;

                    #endregion Walls

                    #region Valleys

                    // Up Left Face: Use current, Add to Last, Open for above
                    case 0b_0111:

                        TraceValleyUL(x, y, size, square);
                        yield return GenTrace(); 
                        break;

                    // Up Right Face: Create new Outline, Open for above, Set Current
                    case 0b_1011:

                        TraceValleyUR(x, y, size, square);
                        yield return GenTrace(); 
                        break;

                    // Down Right Face: Use below, Add to First, Set Current
                    case 0b_1101:
                        TraceValleyDR(x, y, size, square);
                        yield return GenTrace();
                        break;

                    // Down Left Face: Use current, Merge with below
                    case 0b_1110:
                        TraceValleyDL(x, y, size, square);
                        yield return GenTrace();
                        break;

                    #endregion Valleys

                    #region Saddles

                    // Down Left, Top Right
                    case 0b_0101:
                        if (square.TotalMass > 30)
                        {
                        }
                        else
                        {
                        }
                        yield return GenTrace();
                        break;

                    // Down Right, Top Left
                    case 0b_1010:
                        ;
                        if (square.TotalMass > 30)
                        {
                        }
                        else
                        {
                        }
                        yield return GenTrace();
                        break;

                    #endregion Saddles

                    // Full
                    case 0b_1111:
                        TraceFilled(x, y, size);
                        yield return GenTrace();
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

        //mesh.SetVertices(vertices);
        //mesh.SetSubMesh(0, new SubMeshDescriptor(0, vertices.Count, MeshTopology.LineStrip));

        //mesh.subMeshCount = total.Count;
        //mesh.SetSubMeshes(descriptors);

        var result = GenTrace();
        tracerTotal.Clear();
        tracerProc.Clear();
        tracerCurr = null;

        yield return result;
    }

    private Outline TraceNewOutline()
    {
        tracerCurr = new Outline();
        tracerTotal.Add(tracerCurr);
        return tracerCurr;
    }

    private void TraceMergeToEnd(Outline target, Outline sacrifice)
    {
        if (target == sacrifice) return;

        target.AddLast(sacrifice);
        tracerTotal.Remove(sacrifice);

        foreach (int key in tracerProc.Keys.ToArray()) // It fears manipulation
            if (tracerProc[key] == sacrifice)
                tracerProc[key] = target;
    }

    private void TraceFilled(int x, int y, int size)
    {
        if (x == 0)
        {
            if (y == 0)
            {
                tracerProc[TPROC_LEFT] = TraceNewOutline();
                tracerCurr.AddLast(Vector2.zero);
            }
            else if (y == size - 1)
            {
                tracerCurr = tracerProc[TPROC_LEFT];
                tracerCurr.AppendLast(new Vector2(0, size));
            }
        }
        else if (x == size - 1)
        {
            if  (y == 0)
            {
                tracerCurr.AppendFirst(new Vector2(size, 0));
                tracerProc[TPROC_RIGHT] = tracerCurr;
            }
            else if (y == size - 1)
            {
                tracerCurr.AppendLast(new Vector2(size, size));
                TraceMergeToEnd(tracerCurr, tracerProc[TPROC_RIGHT]);
                tracerProc.Remove(TPROC_RIGHT);
            }
        }
    }

    private void TraceCornerDR(int x, int y, int size, Square square)
    {
        if (x == 0) // On the wall?
        {
            tracerProc[TPROC_LEFT] = TraceNewOutline();
            tracerCurr.AddFirst(new Vector2(x, y + square.Intn_R));
        }
        if (y == size - 1) // On the roof?
        {

        }
        tracerCurr.AppendFirst(new Vector2(x + square.Intp_U, y + 1));
        tracerProc[x] = tracerCurr;
    }

    private void TraceCornerDL(int x, int y, int size, Square square)
    {
        if (y == size - 1) // On the roof?
        {

        }
        if (x == size - 1) // On the wall?
        {
             
        }
        tracerCurr = new Outline();
        tracerTotal.Add(tracerCurr);
        tracerCurr.AddFirst(new Vector2(x + square.Intn_U, y + 1));
        tracerCurr.AddFirst(new Vector2(x + 1, y + square.Intn_R));
        tracerProc[x] = tracerCurr;
    }

    private void TraceCornerUL(int x, int y, int size, Square square)
    {
        if (y == 0) // On the floor?
        {
            if (x == 0)
            tracerCurr = new Outline();
            tracerTotal.Add(tracerCurr);
            tracerCurr.AddFirst(new Vector2(x + square.Intn_D, y));
        }
        else
            tracerCurr = tracerProc[x];
        tracerCurr.AppendLast(new Vector2(x + 1, y + square.Intp_R));
        tracerProc.Remove(x);
    }

    private void TraceCornerUR(int x, int y, int size, Square square)
    {
        if (x == 0) // On the wall?
        {
            TraceNewOutline()
                .AddFirst(new Vector2(x + square.Intp_D, y));
        }
        if (y == 0) // On the floor?
            tracerCurr.AppendLast(new Vector2(x + square.Intp_D, y));
        else
        {
            TraceMergeToEnd(tracerCurr, tracerProc[x]);
            tracerProc.Remove(x);
        }
    }

    private void TraceWallR(int x, int y, int size, Square square)
    {
        if (y == 0) // On the floor?
        {
            if (x == 0) // At the corner?
            {
                TraceNewOutline();
                tracerCurr.AddFirst(Vector2.zero);
                tracerProc[TPROC_LEFT] = tracerCurr;
            }
            tracerCurr.AddFirst(new Vector2(x + square.Intp_D, y));
            tracerProc[x] = tracerCurr;
        }
        else if (y == size - 1) // On the roof?
        {
            if (x == 0) // The other corner?
            {
                tracerCurr = tracerProc[TPROC_LEFT];
                tracerCurr.AppendLast(new Vector2(0, size));
            }
            tracerCurr.AppendLast(new Vector2(x + square.Intp_U, y + 1));
            TraceMergeToEnd(tracerCurr, tracerProc[x]);
            tracerProc.Remove(x);
        }
        else
        {
            tracerProc[x].AppendFirst(new Vector2(x + square.Intp_U, y + 1));
        }
    }

    private void TraceWallD(int x, int y, int size, Square square)
    {
        if (x == 0) // On the wall?
        {
            tracerCurr = new Outline();
            tracerTotal.Add(tracerCurr);
            tracerCurr.AddFirst(new Vector2(x, y + square.Intn_L));
        }
        else if (x == size - 1) // On the other wall?
        {
            tracerProc[TPROC_RIGHT] = tracerCurr;
        }
        tracerCurr.AppendFirst(new Vector2(x + 1, y + square.Intn_R));
    }

    private void TraceWallL(int x, int y, int size, Square square)
    {
        if (y == 0) // On the floor?
        {
            TraceNewOutline();
            tracerCurr.AddFirst(new Vector2(x + square.Intn_D, y));
            tracerCurr.AppendLast(new Vector2(x + square.Intn_U, y + 1));
            tracerProc[x] = tracerCurr;
        }
        else if (y == size - 1) // On the roof?
        {
            tracerCurr.AppendLast(new Vector2(x + square.Intn_U, y + 1));
            TraceMergeToEnd(tracerCurr, tracerProc[x]);
            tracerProc.Remove(x);
        }
        else
            tracerProc[x].AppendLast(new Vector2(x + square.Intn_U, y + 1));
    }

    private void TraceWallU(int x, int y, int size, Square square)
    {
        if (x == 0) // On the wall?
        {
            tracerCurr = new Outline();
            tracerTotal.Add(tracerCurr);
            tracerCurr.AddFirst(new Vector2(x, y + square.Intp_L));
        }
        tracerCurr.AppendLast(new Vector2(x + 1, y + square.Intp_R));
    }

    private void TraceValleyDL(int x, int y, int size, Square square)
    {
        if (x == 0) // On the wall?
        {
            tracerProc[-1] = TraceNewOutline();
            tracerCurr.AddLast(new Vector2(0, y + square.Intn_L));
        }
        TraceMergeToEnd(tracerProc[x], tracerCurr);
        tracerProc.Remove(x);
    }

    private void TraceValleyDR(int x, int y, int size, Square square)
    {
        if (y == 0) // On the floor?
        {
            // There should be a current present
        }
        else
            tracerCurr = tracerProc[x];
        tracerCurr.AppendFirst(new Vector2(x + 1, y + square.Intn_R));
        tracerProc.Remove(x);
    }

    private void TraceValleyUR(int x, int y, int size, Square square)
    {
        tracerCurr = new Outline();
        tracerTotal.Add(tracerCurr);
        tracerCurr.AddFirst(new Vector2(x + square.Intp_U, y + 1));
        tracerCurr.AddLast(new Vector2(x + 1, y + square.Intp_R));
        tracerProc[x] = tracerCurr;
    }

    private void TraceValleyUL(int x, int y, int size, Square square)
    {

        tracerCurr.AppendLast(new Vector2(x + square.Intn_U, y + 1));
        tracerProc[x] = tracerCurr;
    }

    private Vector2[][] GenTrace()
    {
        Vector2[][] output = new Vector2[tracerTotal.Count][];

        int i = 0;
        foreach (var shape in tracerTotal)
            output[i++] = shape.ToArray();
        return output;
    }

    private class Outline : LinkedList<Vector2>
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
            for (int i = 0; i < points.Count; i++)
            {
                AddFirst(last.Value);
                last = last.Previous;
            }
        }
    }

    struct Square
    {
        public Square(byte BL, byte BR, byte TL, byte TR)
        {
            //this.BL = BL;
            //this.BR = BR;
            //this.TL = TL;
            //this.TR = TR;
            m_BL = (byte)PointCloud.GetMass(BL);
            m_BR = (byte)PointCloud.GetMass(BR);
            m_TL = (byte)PointCloud.GetMass(TL);
            m_TR = (byte)PointCloud.GetMass(TR);
            // Get hard?
        }

        //byte BL, BR, TL, TR;
        byte m_BL, m_BR, m_TL, m_TR;

        public float Intp_U => Intp(m_TL, m_TR);
        public float Intn_U => Intn(m_TL, m_TR);
        public float Intp_R => Intp(m_BR, m_TR);
        public float Intn_R => Intn(m_BR, m_TR);
        public float Intp_D => Intp(m_BL, m_BR);
        public float Intn_D => Intn(m_BL, m_BR);
        public float Intp_L => Intp(m_BL, m_TL);
        public float Intn_L => Intn(m_BL, m_TL);

        public int TotalMass => m_BL + m_BR + m_TL + m_TR;

        public int MapCase => MapCase(m_BL, m_BR, m_TL, m_TR);
    }

    #endregion Tracer
}
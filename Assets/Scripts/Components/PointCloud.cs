using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(PolygonCollider2D))]
class PointCloud : MonoBehaviour
{
    [SerializeField]
    public byte[][] cloud;

    public byte Size { get; private set; }
    public int TotalMass { get; private set; }
    //bool filled;

    float pointSize;
    MeshFilter meshFilter;
    PolygonCollider2D polyCollider;
    MarchingSquares marching;
    
    //DEBUG:
    Vector2[][] trace;

    public void OnEnable()
    {
        polyCollider = GetComponent<PolygonCollider2D>();
        meshFilter = GetComponent<MeshFilter>();
    }

    public void Initialize(byte squareCount, float pointScale, Material material)
    {
        if (marching != null)
            Debug.LogError("PointCloud.Initialize() : Method has already been called!");

        pointSize = pointScale;
        Size = squareCount;
        marching = new MarchingSquares(this, pointScale);

        cloud = new byte[Size + 1][];
        for (int i = 0; i <= Size; i++)
            cloud[i] = new byte[Size + 1];

        GetComponent<MeshRenderer>().material = material;
    }

    public void MarchMesh()
    {

        if (meshFilter.mesh == null)
            meshFilter.mesh = marching.MarchMesh();
        else
            marching.MarchMesh(meshFilter.mesh);

        marchTrace = marching.MarchTrace();
    }

    IEnumerator<Vector2[][]> marchTrace;

    private void Update()
    {
        if (marchTrace != null && !Input.GetKey(KeyCode.LeftControl))
        {
            trace = marchTrace.Current;
            if (!marchTrace.MoveNext()) marchTrace = null;
        }
    }

    //? These should be inlined by compiler

    // Mass, mostly for interpolation
    internal static int GetMass(byte value) => value & 0x0F;
    // Hardness, how indestructible it is
    internal static int GetHard(byte value) => (value & 0xF0) >> 4;

    internal static byte JoinValues(int mass, int hard) => (byte)((mass & 0x0F) | ((hard << 4) & 0xF0));

    public IEnumerable<PairItem> GetIterator()
    {
        for (int y = 0; y <= Size; y++)
        {
            var strip = cloud[y];
            for (int x = 0; x <= Size; x++)
            {
                byte value = strip[x];
                int mass = GetMass(value);
                int hard = GetHard(value);
                yield return new PairItem(x, y, mass, hard);
            }
        }

        yield break;
    }

    private void OnDrawGizmos()
    {
        if (trace != null && trace.Length != 0)
        {
            for (int i = 0; i < trace.Length; i++)
            {
                var shape = trace[i];
                Vector2 last = transform.TransformPoint(shape[0] * pointSize);
                Gizmos.color = Color.white;
                for (int j = 0; j < shape.Length - 1; j++)
                {
                    Vector2 next = transform.TransformPoint(shape[j + 1] * pointSize);
                    Gizmos.DrawSphere(last, 0.05f + (Mathf.Repeat(j, 20) * 0.005f));
                    Gizmos.DrawLine(last, next);
                    Gizmos.color = Color.HSVToRGB(Mathf.Repeat(shape.Length / 10f + j * 0.05f, 1f), 1, 1);
                    last = next;
                }
            }
        }
    }

    public struct PairItem
    {
        public int mass, hard;
        public int X, Y;

        public PairItem(int X, int Y, int mass, int hard)
        {
            this.X = X;
            this.Y = Y;
            this.mass = mass;
            this.hard = hard;
        }
    }
}
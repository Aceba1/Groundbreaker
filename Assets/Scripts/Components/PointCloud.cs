using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
class PointCloud : MonoBehaviour
{
    [SerializeField]
    public byte[][] cloud;

    public byte size { get; private set; }
    public int totalMass { get; private set; }
    //bool filled;

    MeshFilter meshFilter;

    MarchingSquares marching;

    public void OnEnable()
    {
        meshFilter = GetComponent<MeshFilter>();
    }

    public void Initialize(byte squareCount, float pointScale, Material material)
    {
        if (marching != null)
            Debug.LogError("PointCloud.Initialize() : Method has already been called!");

        size = squareCount;
        marching = new MarchingSquares(this, pointScale);

        cloud = new byte[size + 1][];
        for (int i = 0; i <= size; i++)
            cloud[i] = new byte[size + 1];

        GetComponent<MeshRenderer>().material = material;
    }

    public void MarchMesh()
    {
        if (meshFilter.mesh == null)
            meshFilter.mesh = marching.MarchMesh();
        else
            marching.MarchMesh(meshFilter.mesh);
    }

    //? These should be inlined by compiler

    // Mass, mostly for interpolation
    internal static int GetMass(byte value) => value & 0x0F;
    // Hardness, how indestructible it is
    internal static int GetHard(byte value) => (value & 0xF0) >> 4;

    internal static byte JoinValues(int mass, int hard) => (byte)((mass & 0x0F) | ((hard << 4) & 0xF0));

    public IEnumerable<PairItem> GetIterator()
    {
        for (int y = 0; y <= size; y++)
        {
            var strip = cloud[y];
            for (int x = 0; x <= size; x++)
            {
                byte value = strip[x];
                int mass = GetMass(value);
                int hard = GetHard(value);
                yield return new PairItem(x, y, mass, hard);
            }
        }

        yield break;
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
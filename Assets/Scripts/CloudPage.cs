using System.Collections.Generic;
using UnityEngine;

class CloudPage
{
    byte[,] cloud;

    // DEBUG
    public int
        lastMinX = 0,
        lastMaxX = 0,
        lastMinY = 0,
        lastMaxY = 0;
    public Vector2 lastCursorPos;
    public float lastRadius;

    public readonly byte size;
    public int totalMass { get; private set; }
    //bool filled;

    public CloudPage(byte cloudSize)
    {
        size = cloudSize;
    }

    public void AllocateCloud() =>
        cloud = new byte[size + 1, size + 1];

    public void Modify(float relX, float relY, float radius, int weight = 1, int modifyHard = 0)
    {
        if (cloud == null)
            AllocateCloud();

        int minX = Mathf.Max((int)(relX - radius + 0.9f), 0),
            maxX = Mathf.Min((int)(relX + radius), size),
            minY = Mathf.Max((int)(relY - radius + 0.9f), 0),
            maxY = Mathf.Min((int)(relY + radius), size);

        float radSq = radius * radius;

        for (int y = minY; y <= maxY; y++)
            for (int x = minX; x <= maxX; x++)
            {
                // Within radius check
                float distSq = (x - relX) * (x - relX) + (y - relY) * (y - relY);
                if (distSq >= radSq) continue;

                byte value = cloud[y, x];
                int mass = GetMass(value);
                int hard = GetHard(value);

                if (modifyHard != 0)
                    hard = Mathf.Clamp(hard + modifyHard, 0, 15);

                if (weight != 0)
                {
                    // Calculate new mass
                    totalMass -= mass;
                    float deep = radius - Mathf.Sqrt(distSq) / (1f + hard / 2f);
                    int feather = (int)(Mathf.Clamp01(deep) * 15);

                    if (weight > 0)
                        mass = Mathf.Max(mass, feather);
                    else
                        mass = Mathf.Min(mass, 7 - feather);

                    totalMass += mass;
                }

                cloud[y, x] = JoinValues(mass, hard);
            }

        // DEBUG
        lastMinX = minX;
        lastMaxX = maxX;
        lastMinY = minY;
        lastMaxY = maxY;
        lastRadius = radius;
        lastCursorPos = new Vector2(relX, relY);
    }

    //? These should be inlined by compiler

    // Mass, mostly for interpolation
    private static int GetMass(byte value) => value & 0x0F;
    // Hardness, how indestructible it is
    private static int GetHard(byte value) => (value & 0xF0) >> 2;

    private static byte JoinValues(int mass, int hard) => (byte)((mass & 0x0F) + ((hard & 0x0F) << 2));

    public IEnumerable<CloudPairEnumerator> GetIterator()
    {
        if (cloud == null)
            yield break;

        for (int y = 0; y <= size; y++)
            for (int x = 0; x <= size; x++)
            {
                byte value = cloud[y, x];
                int mass = GetMass(value);
                int hard = GetHard(value);
                yield return new CloudPairEnumerator(x, y, mass, hard);
            }

        yield break;
    }

    public struct CloudPairEnumerator
    {
        public int mass, hard;
        public int X, Y;

        public CloudPairEnumerator(int X, int Y, int mass, int hard)
        {
            this.X = X;
            this.Y = Y;
            this.mass = mass;
            this.hard = hard;
        }
    }
}
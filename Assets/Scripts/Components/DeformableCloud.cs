using System.Collections.Generic;
using UnityEngine;

class DeformableCloud : Deformable
{
    [SerializeField]
    private Material material;
    [SerializeField]
    [Range(4, 128)]
    private byte pageDetail = 16;
    [SerializeField]
    [Range(1, 32)]
    private int pageSize = 4;

    private float PointSize => pageSize / (float)pageDetail;

    Dictionary<Vector2Int, PointCloud> clouds;

    private void OnEnable()
    {
        clouds = new Dictionary<Vector2Int, PointCloud>();
        CreatePage(Vector2Int.zero);
    }

    private PointCloud CreatePage(Vector2Int coord)
    {
        var obj = new GameObject("Page " + coord);
        obj.transform.SetParent(transform);
        obj.transform.localPosition = new Vector3(coord.x * pageSize, coord.y * pageSize);
        
        var cloud = obj.AddComponent<PointCloud>();
        cloud.Initialize(pageDetail, PointSize, material);
        
        clouds.Add(coord, cloud);
        return cloud;
    }

    private void OnDisable()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);
        clouds.Clear();
        clouds = null;
    }

    public override void Deform(IPointCloudBrush brush, Vector2 worldPos)
    {
        Vector2 localPos = transform.InverseTransformPoint(worldPos);
        //TODO: Choose all clouds touching radius

        // X1, Y1, X2, Y2;
        Vector4 bounds = brush.GetBounds();

        int minX = Mathf.FloorToInt((localPos.x + bounds.x) / pageSize),
            maxX = Mathf.FloorToInt((localPos.x + bounds.z) / pageSize),
            minY = Mathf.FloorToInt((localPos.y + bounds.y) / pageSize),
            maxY = Mathf.FloorToInt((localPos.y + bounds.w) / pageSize);

        float pointSize = this.PointSize;

        for (int y = minY; y <= maxY; y++)
            for (int x = minX; x <= maxX; x++)
            {
                Vector2Int coord = new Vector2Int(x, y);
                if (!clouds.TryGetValue(coord, out PointCloud cloud))
                    cloud = CreatePage(coord);

                if (brush.Modify(cloud.cloud, pageDetail, localPos - (new Vector2(x, y) * pageSize), pointSize) != 0)
                    cloud.MarchMesh();
            }
    }

    private void OnDrawGizmosSelected()
    {
        // Ignoring rotations for now
        // Debug only

        float pointSize = this.PointSize;

        Gizmos.color = Color.white;
        for (int i = 0; i <= pageDetail; i++)
        {
            var right = new Vector3(i * pointSize, 0f);
            var up = new Vector3(0f, i * pointSize);
            Gizmos.DrawLine(transform.position + up, transform.position + Vector3.right * pageSize + up);
            Gizmos.DrawLine(transform.position + right, transform.position + Vector3.up * pageSize + right);
            Gizmos.color = Color.gray;
        }

        if (clouds != null)
            foreach (var pair in clouds)
            {
                Vector3 offset = new Vector3(pair.Key.x, pair.Key.y) * pageSize;
                PointCloud cloud = pair.Value;

                foreach (var item in cloud.GetIterator())
                {
                    if (item.mass == 0) continue;
                    Gizmos.color = Color.Lerp(Color.white, Color.red, item.hard / 15f);
                    Gizmos.DrawCube(transform.position + transform.rotation * (offset + new Vector3(
                        item.X * pointSize, item.Y * pointSize, 1f)), // position
                        Vector3.one * (pointSize * (item.mass / 15f))); // size
                }
            }
    }
}
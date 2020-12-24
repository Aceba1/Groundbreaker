using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
class DeformableCloud : Deformable
{
    [SerializeField]
    [Range(4, 128)]
    private byte pageDetail = 16;
    [SerializeField]
    [Range(1, 32)]
    private int pageSize = 4;

    private float pointSize => pageSize / (float)pageDetail;

    MeshFilter meshFilter;
    Dictionary<Vector2Int, PointCloud> clouds;

    private void OnEnable()
    {
        meshFilter = GetComponent<MeshFilter>();
        clouds = new Dictionary<Vector2Int, PointCloud>();
        clouds.Add(Vector2Int.zero, new PointCloud(pageDetail, pointSize));
    }

    private void OnDisable()
    {
        clouds.Clear();
        clouds = null;
    }

    public void Deform(DeformBrush brush)
    {
        //brush.SetRelativity()
    }

    public override void Deform(Vector2 localPos, float radius, float strength)
    {
        //TODO: Choose all clouds touching radius
        float pointSize = this.pointSize;
        var cloud = clouds[Vector2Int.zero];
        cloud.Modify(localPos.x / pointSize, localPos.y / pointSize, radius / pointSize, (int)strength, 1);
        meshFilter.mesh = cloud.MarchMesh();
    }

    private void OnDrawGizmosSelected()
    {
        // Ignoring rotations for now
        // Debug only

        float pointSize = this.pointSize;

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

                Vector3 minX = new Vector3(cloud.lastMinX * pointSize, 0f),
                    maxX = new Vector3(cloud.lastMaxX * pointSize, 0f),
                    minY = new Vector3(0f, cloud.lastMinY * pointSize),
                    maxY = new Vector3(0f, cloud.lastMaxY * pointSize);
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position + minX + minY, transform.position + maxX + minY);
                Gizmos.DrawLine(transform.position + minX + maxY, transform.position + maxX + maxY);
                Gizmos.DrawLine(transform.position + minX + minY, transform.position + minX + maxY);
                Gizmos.DrawLine(transform.position + maxX + minY, transform.position + maxX + maxY);

                Gizmos.DrawWireSphere(transform.position + new Vector3(cloud.lastCursorPos.x, cloud.lastCursorPos.y) * pointSize, cloud.lastRadius * pointSize);

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
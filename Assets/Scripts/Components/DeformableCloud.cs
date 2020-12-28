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

    //DEBUG:
    Vector2[][] trace;

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

    public override void Deform(IPointCloudBrush brush, Vector2 worldPos)
    {
        //TODO: Choose all clouds touching radius
        float pointSize = this.pointSize;
        var cloud = clouds[Vector2Int.zero];
        brush.Modify(cloud.cloud, pageDetail, transform.InverseTransformPoint(worldPos), pointSize);
        trace = cloud.TraceMesh();
    }

    private void OnDrawGizmos()
    {
        float pointSize = this.pointSize;

        if (trace != null && trace.Length != 0)
        {
            for (int i = 0; i < trace.Length; i++)
            {
                Gizmos.color = Random.ColorHSV(0, 1, 1, 1, 1, 1, 1, 1);
                var shape = trace[i];
                Vector2 last = transform.TransformPoint(shape[shape.Length - 1] * pointSize);
                for (int j = 0; j < shape.Length; j++)
                {
                    Vector2 next = transform.TransformPoint(shape[j] * pointSize);
                    Gizmos.DrawSphere(next, 0.1f);
                    Gizmos.DrawLine(last, next);
                    last = next;
                }
            }
        }
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
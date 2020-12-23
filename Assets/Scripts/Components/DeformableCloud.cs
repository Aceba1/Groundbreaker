using System.Collections.Generic;
using UnityEngine;

class DeformableCloud : Deformable
    {
        [SerializeField]
        private byte pageDetail;
        [SerializeField]
        private int pageSize;

        private float pointSize => pageSize / (float)pageDetail;

        Dictionary<Vector2Int, CloudPage> clouds;

        private void Awake()
        {
            clouds = new Dictionary<Vector2Int, CloudPage>();
            clouds.Add(Vector2Int.zero, new CloudPage(pageDetail));
        }

        public override void Deform(Vector2 localPos, float radius, float strength)
        {
            //TODO: Choose all clouds touching radius
            float pointSize = this.pointSize;
            clouds[Vector2Int.zero].Modify(localPos.x / pointSize, localPos.y / pointSize, radius / pointSize, (int)strength);
        }

        private void OnDrawGizmos()
        {
            // Ignoring rotations for now
            // Debug only
            if (clouds == null)
                Awake();

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

            foreach (var pair in clouds)
            {
                Vector3 offset = new Vector3(pair.Key.x, pair.Key.y) * pageSize;
                CloudPage cloud = pair.Value;

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
                    Gizmos.color = Color.Lerp(Color.white, Color.red, item.hard / 7f);
                    Gizmos.DrawCube(transform.position + transform.rotation * (offset + new Vector3(
                        item.X * pointSize, item.Y * pointSize, 1f)), // position
                        Vector3.one * (pointSize * (item.mass / 7f))); // size
                }
            }
        }
    }

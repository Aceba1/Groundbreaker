//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.U2D;

//[RequireComponent(typeof(SpriteShapeController))]
//[RequireComponent(typeof(PolygonCollider2D))]
//public class DeformableSpline : Deformable
//{
//    List<Vector2> points;
//    PolygonCollider2D collider;
//    SpriteShapeController controller;
//    Spline spline;

//    private void OnEnable()
//    {
//        collider = GetComponent<PolygonCollider2D>();
//        controller = GetComponent<SpriteShapeController>();
//        spline = controller.spline;

//        int size = spline.GetPointCount();
//        points = new List<Vector2>(size);

//        for (int i = 0; i < size; i++)
//            points.Add(spline.GetPosition(i));
//    }

//    // Start is called before the first frame update
//    void Start()
//    {
        
//    }

//    // Update is called once per frame
//    void Update()
//    {
        
//    }

//    private static bool PointWithinRadius(Vector2 a, Vector2 b, float radius)
//    {
//        float dx = (a.x - b.x),
//            dy = (a.y - b.y);
//        return dx * dx + dy * dy < radius * radius;
//    }

//    public override void Deform(Vector2 localPos, float radius, float strength)
//    {
//        bool trimming = false;
//        int startTrim = -1;

//        for (int i = 0; i < points.Count; i++)
//        {
//            Vector2 point = points[i];
//            if (PointWithinRadius(point, localPos, radius))
//            {
//                if (!trimming)
//                {
//                    trimming = true;
//                    startTrim = i;
//                }
//            }
//            else if (trimming)
//            {
//                trimming = false;
//            }
//        }
//    }
//}

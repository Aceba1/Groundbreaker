using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Deformable: MonoBehaviour
{
    //TODO: Identify possibility of alternate deformable types. Generic type?

    public abstract void Deform(IPointCloudBrush brush, Vector2 worldPos);

    //public abstract void Deform(Vector2 localPos, float radius, float strength);

    //public void Deform(Vector2 localPos, float radius, bool additive) => Deform(localPos, radius, additive ? 1f : -1f);
}

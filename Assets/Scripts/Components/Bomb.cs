using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Explosion), typeof(SpriteRenderer), typeof(Rigidbody2D))]
public class Bomb : MonoBehaviour
{
    //[SerializeField]
    //private Explosion explosion;

    [SerializeField]
    private bool autoFuse = true;
    [SerializeField]
    private float fuseTime = 3f;

    Material material;
    Explosion explosion;
    Rigidbody2D rbody;

    // Start is called before the first frame update
    public bool FuseOn { get; private set; }
    public float FuseTimeRemaining { get; private set; }
    
    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        rbody.simulated = true;

        material = GetComponent<Renderer>().material;
        explosion = GetComponent<Explosion>();

        UpdateFuse(1f);

        if (autoFuse)
        {
            FuseOn = true;
            FuseTimeRemaining = fuseTime;
        }
    }

    private void Explode()
    {
        FuseOn = false;

        UpdateFuse(0f);
        rbody.simulated = false;
        explosion.Explode(Core.DeformWorld);

        Destroy(gameObject, 0.55f);
    }

    private void UpdateFuse(float progress)
    {
        material.SetFloat("_Clip", progress);
    }

    private void OnCollisionEnter(Collision collision)
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (FuseOn)
        {
            FuseTimeRemaining -= Time.deltaTime;
            if (FuseTimeRemaining <= 0f)
                Explode();
            else
                UpdateFuse(FuseTimeRemaining / fuseTime);

        }
    }
}

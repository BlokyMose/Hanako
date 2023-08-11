using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningTorchController : MonoBehaviour
{
    public Material MeshMaterial;
    [Range(0.01f, 1f)] public float MeshSpeed = 0.5f;    
    
    [HideInInspector] public Color ColorTop;
    [HideInInspector] public Color ColorMiddle;
    [HideInInspector] public Color ColorBottom;
    [HideInInspector] public Vector3[] Vertices;
    [HideInInspector] [Range(1f, 10.0f)] public float FlameScale = 4f;
    [HideInInspector] [Range(0.1f, 1.0f)] public float FlameThreshold = 0.3f;
    [HideInInspector] [Range(0.1f, 5.0f)] public float FlameSpeed = 1.5f;
    [HideInInspector] [Range(0.0f, 0.5f)] public float FlameVOffset = 0.15f;
    [HideInInspector] [Range(0.0f, 0.5f)] public float FlameUOffset = 0.0f;
    [HideInInspector] [Range(0.0f, 0.5f)] public float FlameVolume = 0.5f;

    [HideInInspector] public bool ShowVertices;

    private int[] _triangles;
    private Vector3[] _verticesCopy;
    private Vector2[] _uvs;
    private Mesh _mesh;
    private MeshRenderer _renderer;
    private float _time = 0;
    private ParticleSystem _sparks;

    private LightningTorchController()
    {
        Vertices = new Vector3[]
        {
            new Vector3(-0.5f, 0.00f,  0), // 0
            new Vector3(-0.5f, 0.25f,  0), // 1
            new Vector3( 0.5f, 0.00f,  0), // 2
            new Vector3( 0.5f, 0.25f,  0), // 3
            new Vector3(-0.5f, 0.50f,  0), // 4
            new Vector3( 0.5f, 0.50f,  0), // 5
            new Vector3(-0.5f, 0.75f,  0), // 6
            new Vector3( 0.5f, 0.75f,  0), // 7
            new Vector3(-0.5f, 1.00f,  0), // 8
            new Vector3( 0.5f, 1.00f,  0)  // 9
        };

        _triangles = new int[]
        {
            0, 1, 2,
            1, 3, 2,
            1, 4, 3,
            4, 5, 3,
            4, 6, 5,
            6, 7, 5,
            6, 8, 7,
            8, 9, 7
        };

        _uvs = new Vector2[]
        {
            new Vector2(-0.5f, 0f),
            new Vector2(-0.5f, 0.25f),
            new Vector2( 0.5f, 0f),
            new Vector2( 0.5f, 0.25f),
            new Vector2(-0.5f, 0.5f),
            new Vector2( 0.5f, 0.5f),
            new Vector2(-0.5f, 0.75f),
            new Vector2( 0.5f, 0.75f),
            new Vector2(-0.5f, 1f),
            new Vector2( 0.5f, 1f),
        };
    }

    // Start is called before the first frame update
    void Start()
    {
        InitMesh();
        SetMeshShape();
        SetMeshColor();
    }    

    private void Update()
    {
        SetMeshShape();
    }

    private Vector3 OnLerp(Vector3 p0, Vector3 p1, float time)
    {
        return p0 + time * (p1 - p0);
    }

    Vector3 OnRotation(Vector3 p, Vector3 v)
    {   
        return p + transform.rotation * v;
    }

    public void SetMeshColor()
    {
        if (MeshMaterial != null)
        {
#if UNITY_EDITOR
            _renderer.sharedMaterial.SetColor("_cBottom", ColorBottom);
            _renderer.sharedMaterial.SetColor("_cMiddle", ColorMiddle);
            _renderer.sharedMaterial.SetColor("_cTop", ColorTop);
            _renderer.sharedMaterial.SetFloat("_fScale", FlameScale);
            _renderer.sharedMaterial.SetFloat("_fThreshold", FlameThreshold);
            _renderer.sharedMaterial.SetFloat("_fSpeed", FlameSpeed);
            _renderer.sharedMaterial.SetFloat("_fVolume", FlameVolume);
            _renderer.sharedMaterial.SetFloat("_vOffset", FlameVOffset);
            _renderer.sharedMaterial.SetFloat("_uOffset", FlameUOffset);
#else
            _renderer.material.SetColor("_cBottom", ColorBottom);
            _renderer.material.SetColor("_cMiddle", ColorMiddle);
            _renderer.material.SetColor("_cTop", ColorTop);
            _renderer.material.SetFloat("_fScale", FlameScale);
            _renderer.material.SetFloat("_fThreshold", FlameThreshold);
            _renderer.material.SetFloat("_fSpeed", FlameSpeed);
            _renderer.material.SetFloat("_fVolume", FlameVolume);
            _renderer.material.SetFloat("_vOffset", FlameVOffset);
            _renderer.material.SetFloat("_uOffset", FlameUOffset);
#endif
        }

        if (_sparks == null)
        {
            _sparks = GetComponentInChildren<ParticleSystem>();
        }

        if (_sparks)
        {
            var main = _sparks.main;
            main.startColor = ColorMiddle;
        }
    }    

    public void SetMeshShape()
    {
        _time += Time.deltaTime;
        _time = Mathf.Clamp01(_time);

        float e0 = _time * 1.00f;
        float e2 = _time * 0.95f * MeshSpeed;
        float e3 = _time * 0.50f * MeshSpeed;
        float e4 = _time * 0.30f * MeshSpeed;
        float e5 = _time * 0.20f * MeshSpeed;

        Vector3 p0 = OnRotation(transform.position, Vector3.Scale(new Vector3(-0.5f, 0.00f, 0), transform.localScale));
        Vector3 p1 = OnRotation(transform.position, Vector3.Scale(new Vector3(-0.5f, 0.25f, 0), transform.localScale));
        Vector3 p2 = OnRotation(transform.position, Vector3.Scale(new Vector3( 0.5f, 0.00f, 0), transform.localScale));
        Vector3 p3 = OnRotation(transform.position, Vector3.Scale(new Vector3( 0.5f, 0.25f, 0), transform.localScale));
        Vector3 p4 = OnRotation(transform.position, Vector3.Scale(new Vector3(-0.5f, 0.50f, 0), transform.localScale));
        Vector3 p5 = OnRotation(transform.position, Vector3.Scale(new Vector3( 0.5f, 0.50f, 0), transform.localScale));
        Vector3 p6 = OnRotation(transform.position, Vector3.Scale(new Vector3(-0.5f, 0.75f, 0), transform.localScale));
        Vector3 p7 = OnRotation(transform.position, Vector3.Scale(new Vector3( 0.5f, 0.75f, 0), transform.localScale));
        Vector3 p8 = OnRotation(transform.position, Vector3.Scale(new Vector3(-0.5f, 1.00f, 0), transform.localScale));
        Vector3 p9 = OnRotation(transform.position, Vector3.Scale(new Vector3( 0.5f, 1.00f, 0), transform.localScale));

        Vector3 v0 = OnLerp(Vertices[0], p0, _time * e0);
        Vector3 v1 = OnLerp(Vertices[1], p1, _time * e2);
        Vector3 v2 = OnLerp(Vertices[2], p2, _time * e0);
        Vector3 v3 = OnLerp(Vertices[3], p3, _time * e2);
        Vector3 v4 = OnLerp(Vertices[4], p4, _time * e3);
        Vector3 v5 = OnLerp(Vertices[5], p5, _time * e3);
        Vector3 v6 = OnLerp(Vertices[6], p6, _time * e4);
        Vector3 v7 = OnLerp(Vertices[7], p7, _time * e4);
        Vector3 v8 = OnLerp(Vertices[8], p8, _time * e5);
        Vector3 v9 = OnLerp(Vertices[9], p9, _time * e5);

        Vertices[0] = v0;
        Vertices[1] = v1;
        Vertices[2] = v2;
        Vertices[3] = v3;
        Vertices[4] = v4;
        Vertices[5] = v5;
        Vertices[6] = v6;
        Vertices[7] = v7;
        Vertices[8] = v8;
        Vertices[9] = v9;        

        for (int i = 0; i < Vertices.Length; i++)
        {
            _verticesCopy[i] = transform.InverseTransformPoint( Vertices[i]);
        }        

        _mesh.MarkDynamic();
        _mesh.SetVertices(_verticesCopy);
        _mesh.SetTriangles(_triangles, 0);
        _mesh.SetUVs(0, _uvs);
        _mesh.RecalculateBounds();
    }

    public void InitMesh()
    {
        _verticesCopy = new Vector3[Vertices.Length];

        if (_mesh == null)
        {
            _mesh = new Mesh();
        }
        
        _renderer = GetComponent<MeshRenderer>();

#if UNITY_EDITOR
        GetComponent<MeshFilter>().sharedMesh = _mesh;
        _renderer.sharedMaterial = MeshMaterial;

#else
        GetComponent<MeshFilter>().mesh = _mesh;
        _renderer.material = MeshMaterial;
#endif
        _mesh.Clear();
    }
}

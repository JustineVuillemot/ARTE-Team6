using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FillLine : MonoBehaviour
{
    private MeshRenderer _renderer;
    private MeshFilter _filter;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    { 
    }

    private void OnEnable()
    {
        _renderer = GetComponent<MeshRenderer>();
        _filter = GetComponent<MeshFilter>();
    }

    public void CreateFilledShape(LineRenderer line)
    {
        Vector3[] vertices3D = new Vector3[line.positionCount];
        int result = line.GetPositions(vertices3D);

        if(result != line.positionCount)
        {
            Debug.LogError("couldn't fill array with all line positions.");
            return;
        }

        Vector2[] vertices2D = System.Array.ConvertAll<Vector3, Vector2>(vertices3D, v => new Vector2(v.x, v.y));
        GraphToMesh(vertices2D);
    }

    private void GraphToMesh(Vector2[] linePoints)
    {
        Vector3[] vertices3D = System.Array.ConvertAll<Vector2, Vector3>(linePoints, v => v);

        // Use the triangulator to get indices for creating triangles
        Triangulator triangulator = new Triangulator(linePoints);
        int[] indices = triangulator.Triangulate();

        List<Color> colors = new List<Color>(vertices3D.Length);
        colors.ForEach(c => c = Color.black);

        // Create the mesh
        Mesh mesh = new Mesh
        {
            vertices = vertices3D,
            triangles = indices,
            colors = colors.ToArray()
        };

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        // Set up game object with mesh;
        _renderer.material = new Material(Shader.Find("Sprites/Default"));
        _filter.mesh = mesh;
    }
}

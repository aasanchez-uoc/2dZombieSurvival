using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    public float FOV = 360f;
    public int EdgeCount = 360;
    public float InitialAngle = 0f;
    public float distance = 8f;
    public LayerMask layer;

    private Mesh mesh;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    // Update is called once per frame
    void Update()
    {
        GenerateMesh();
    }

    private void GenerateMesh()
    {
        Vector3[] vertices = new Vector3[EdgeCount + 2];
        int[] triangles = new int[(EdgeCount + 1) * 3 ];

        float currentAngle = InitialAngle;
        float deltaAngle = FOV / EdgeCount;

        vertices[0] = transform.localPosition;


        for(int i = 0; i <= EdgeCount; i++)
        {
            Vector3 dirAnlge = GetVectorFromAngle(currentAngle);

            RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, dirAnlge, distance, layer);
            Vector3 currentVertex;

            if (raycastHit.collider == null)
            {
                currentVertex = transform.localPosition + dirAnlge * distance;
            }
            else
            {
                
                currentVertex = transform.InverseTransformPoint(raycastHit.point);
            }



            vertices[i + 1] = currentVertex;

            if ( i != 0)
            {
                triangles[3*i] = 0;
                triangles[3*i + 1] = i;
                triangles[3*i + 2] = i + 1;
            }

            currentAngle -= deltaAngle;

        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
    }

    Vector3 GetVectorFromAngle(float angle)
    {
        float rads = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(rads), Mathf.Sin(rads));
    }
}

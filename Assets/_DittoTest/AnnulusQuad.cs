using UnityEngine;
using System.Collections;

public class AnnulusQuad : MonoBehaviour
{
    public enum RingType
    {
        FullLine,//实线
        DottedLine,//虚线
        Pentagon,//五边形
        Square,//正方形
        Triangle,//三角形
        Disk,//圆盘
    }
    //圈的步长
    public int depth = 4;
    //圈的半径，最大是1
    public float radius = 1;
    //内圈半径
    private float radius1;
    public float width = 2.5f;

    public RingType ringType = RingType.DottedLine;
    private MeshFilter _meshFilter;

    Mesh mesh;
    void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        Rebuild();
    }
    private void Rebuild()
    {
        if (mesh == null)
            mesh = new Mesh();
        switch (ringType)
        {
            case RingType.DottedLine:
                depth = 4;
                radius = -3;
                width = 0.2f;
                RebuildRingDataDotted();
                break;
            case RingType.Disk:
                depth = 4;
                radius = 1;
                width = 2.5f;
                RebuildRingDataFull();
                break;
            case RingType.FullLine:
                depth = 4;
                radius = -1;
                width = 0.2f;
                RebuildRingDataFull();
                break;
            case RingType.Pentagon:
                depth = 72;
                radius = -3;
                width = 0.2f;
                RebuildRingDataFull();
                break;
            case RingType.Square:
                depth = 90;
                radius = -3;
                width = 0.2f;
                RebuildRingDataFull();
                break;
            case RingType.Triangle:
                depth = 120;
                radius = -3;
                width = 0.2f;
                RebuildRingDataFull();
                break;
        }
        _meshFilter.sharedMesh = mesh;

    }
    private float Deg2Rad;

    private Vector3[] vertices;
    private int[] indexes;
    private Vector2[] uvs;
    private void RebuildRingDataDotted()
    {
        mesh.Clear();
        Deg2Rad = Mathf.Deg2Rad;
        radius1 = radius - width;
        int count = 360 / depth;
        int totalTringleCount = count;
        int totalVertexCount = totalTringleCount * 3;
        int totalIndexCount = totalVertexCount;
        if (vertices == null || vertices.Length < totalVertexCount)
            vertices = new Vector3[totalVertexCount];
        if (indexes == null || indexes.Length < totalIndexCount)
            indexes = new int[totalIndexCount];
        if (uvs == null || uvs.Length < totalVertexCount)
            uvs = new Vector2[totalVertexCount];
        int vertexIndex = 0;
        for (int i = 0; i < count; i += 2)
        {
            Vector3 v0 = GetPos(radius, i * depth);
            Vector3 v1 = GetPos(radius1, i * depth);
            Vector3 v2 = GetPos(radius, (i + 1) * depth);
            Vector3 v3 = GetPos(radius1, (i + 1) * depth);
            vertices[vertexIndex++] = v0;
            vertices[vertexIndex++] = v1;
            vertices[vertexIndex++] = v2;
            vertices[vertexIndex++] = v2;
            vertices[vertexIndex++] = v1;
            vertices[vertexIndex++] = v3;
        }
        for (int i = 0; i < totalIndexCount; i++)
        {
            indexes[i] = i;
            uvs[i] = Vector2.zero;
        }
        if (vertices.Length > totalVertexCount)
        {
            for (int i = vertexIndex, il = vertices.Length; i < il; i++)
            {
                vertices[i] = Vector3.zero;
                indexes[i] = 0;
            }
        }
        mesh.vertices = vertices;
        mesh.SetIndices(indexes, MeshTopology.Triangles, 0);
        mesh.uv = uvs;
        mesh.MarkDynamic();
    }

    private void RebuildRingDataFull()
    {
        mesh.Clear();
        Deg2Rad = Mathf.Deg2Rad;
        radius1 = radius - width;
        int count = 360 / depth;
        int totalTringleCount = count * 2;
        int totalVertexCount = totalTringleCount * 3;
        int totalIndexCount = totalVertexCount;
        if (vertices == null || vertices.Length != totalVertexCount)
            vertices = new Vector3[totalVertexCount];
        if (indexes == null || indexes.Length != totalIndexCount)
            indexes = new int[totalIndexCount];
        if (uvs == null || uvs.Length < totalVertexCount)
            uvs = new Vector2[totalVertexCount];
        int vertexIndex = 0;
        for (int i = 0; i < count; i++)
        {
            Vector3 v0 = GetPos(radius, i * depth);
            Vector3 v1 = GetPos(radius1, i * depth);
            Vector3 v2 = GetPos(radius, (i + 1) * depth);
            Vector3 v3 = GetPos(radius1, (i + 1) * depth);
            vertices[vertexIndex++] = v0;
            vertices[vertexIndex++] = v1;
            vertices[vertexIndex++] = v2;
            vertices[vertexIndex++] = v2;
            vertices[vertexIndex++] = v1;
            vertices[vertexIndex++] = v3;
        }
        for (int i = 0; i < totalIndexCount; i++)
        {
            indexes[i] = i;
            uvs[i] = Vector2.zero;
        }
        if (vertices.Length > totalVertexCount)
        {
            for (int i = vertexIndex, il = vertices.Length; i < il; i++)
            {
                vertices[i] = Vector3.zero;
                indexes[i] = 0;
            }
        }
        mesh.vertices = vertices;
        mesh.SetIndices(indexes, MeshTopology.Triangles, 0);
        mesh.uv = uvs;
        mesh.MarkDynamic();
    }

    private Vector3 GetPos(float radius, float angle)
    {
        float x = radius * Mathf.Sin(angle * Deg2Rad);
        float y = radius * Mathf.Cos(angle * Deg2Rad);
        return new Vector3(x, y, 0);
    }
    void Update()
    {
        //Rebuild();
    }

}
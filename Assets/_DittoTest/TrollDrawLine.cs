using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class TrollDrawLine : MonoBehaviour
{

    public int pointCount = 10;
    //public float radius = 10f;
    private float angle;
    public int[] radiusList;
    public Material material; 
    void Start()
    {
        angle = 360f / pointCount;

        for (int i = 0; i <radiusList.Length; i++) {
            GameObject gameObject = Instantiate(new GameObject());
            LineRenderer renderer =  gameObject.AddComponent<LineRenderer>();
            renderer.material = material;
            renderer.useWorldSpace = true;
            renderer.positionCount = pointCount + 1;  ///这里是设置圆的点数，加1是因为加了一个终点（起点）
            DrowPoints(renderer, CalculationPoints(radiusList[i]));
        }
    }

    List<Vector3> CalculationPoints(float radius)
    {
        List<Vector3> points = new List<Vector3>();
        Vector3 v = transform.position + transform.forward * radius;
        points.Add(v);
        Quaternion r = transform.rotation;
        for (int i = 1; i < pointCount; i++)
        {
            Quaternion q = Quaternion.Euler(r.eulerAngles.x, r.eulerAngles.y - (angle * i), r.eulerAngles.z);
            v = transform.position + (q * Vector3.forward) * radius;
            points.Add(v);
        }

        return points;
    }
    void DrowPoints(LineRenderer lineRenderer , List<Vector3> points)
    {
        for (int i = 0; i < points.Count; i++)
        {
            //  Debug.DrawLine(transform.position, points[i], Color.green);
            lineRenderer.SetPosition(i, points[i]);  //把所有点添加到positions里
        }
        if (points.Count > 0)   //这里要说明一下，因为圆是闭合的曲线，最后的终点也就是起点，
            lineRenderer.SetPosition(pointCount, points[0]);
    }
   
    // Update is called once per frame
    void Update()
    {
    }
}
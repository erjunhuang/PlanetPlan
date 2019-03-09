using UnityEngine;
public class Circle : MonoBehaviour
{
    public Transform parent;
    public GameObject circleModel;
    //旋转改变的角度
    public int changeAngle = 10;
    //旋转一周需要的预制物体个数
    private int count;
    private float angle = 0;
    public float r = 5;
    void Start()
    {
        if (circleModel == null)
            circleModel = GameObject.CreatePrimitive(PrimitiveType.Cube);
        count = (int)360 / changeAngle;
        for (int i = 0; i < count; i++)
        {
            Vector3 center = circleModel.transform.position;
            GameObject cube = (GameObject)Instantiate(circleModel);
            float hudu = (angle / 180) * Mathf.PI;
            float xx = center.x + r * Mathf.Cos(hudu);
            float yy = center.y + r * Mathf.Sin(hudu);
            cube.transform.SetParent(parent);
            cube.transform.position = new Vector3(xx, yy, 0);
            cube.transform.LookAt(center);
             
            angle += changeAngle;
        }
    }
}
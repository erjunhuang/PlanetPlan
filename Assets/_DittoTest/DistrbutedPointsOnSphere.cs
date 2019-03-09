using UnityEngine;
using System.Collections;

public class DistrbutedPointsOnSphere : MonoBehaviour
{

    public Camera camera;
    /// <summary>
    /// 父级 -- 球
    /// </summary>
    public Transform parent;
    /// <summary>
    /// 预设 -- 球
    /// </summary>
    public GameObject prefab;
    public Transform[] kids;

    // Use this for initialization
    void Start()
    {
        CalualteSphere();
    }

    // Update is called once per frame
    void Update()
    {
        //if (!Input.GetMouseButton(0))
        //    return;
        //float fMouseX = Input.GetAxis("Mouse X");
        //float fMouseY = Input.GetAxis("Mouse Y");
        //parent.Rotate(Vector3.up, -fMouseX * 2, Space.World);
        //parent.Rotate(Vector3.right, fMouseY * 2, Space.World);
        //for (int i = 0; i < kids.Length; i++)
        //    kids[i].LookAt(camera.transform);
    }
    /// <summary>
    /// 平均分成的等份
    /// </summary>
    int N = 50;
    /// <summary>
    /// 小球的半径
    /// </summary>
    float size = 5f;

    /// <summary>
    /// 球体表面平均分割点
    /// </summary>
    void CalualteSphere()
    {
        float inc = Mathf.PI * (3.0f - Mathf.Sqrt(5.0f));
        float off = 2.0f / N;//注意保持数值精度
        kids = new Transform[N];
        for (int i = 0; i < N; i++)
        {
            float y = (float)i * off - 1.0f + (off / 2.0f);
            float r = Mathf.Sqrt(1.0f - y * y);
            float phi = i * inc;
            Vector3 pos = new Vector3(Mathf.Cos(phi) * r * size, y * size, Mathf.Sin(phi) * r * size);
            GameObject tempGo = Instantiate(prefab) as GameObject;
            tempGo.transform.parent = parent;
            //tempGo.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            tempGo.transform.localPosition = pos;
            tempGo.SetActive(true);
            kids[i] = tempGo.transform;
        }
    }

    /// <summary>
    /// 点击小球，将他移动到中心点
    /// </summary>
    /// <param name="pos">小球的自身位置</param>
    void ClickLittleSphere(Vector3 pos)
    {
        Vector3 vec301 = pos - Vector3.zero;
        Vector3 vec302 = camera.transform.position - parent.transform.position;
        parent.rotation = Quaternion.FromToRotation(vec301, vec302);
    }
}
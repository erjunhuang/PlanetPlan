using UnityEngine;
using Core.Utilities;

public class ArcBallTest : Singleton<ArcBallTest>
{
    public float damping = 0.9f; //阻尼
    public float speed = 0.1f; //速度
    public float wheelSpeed = 5;

    private Vector3 vDown;  //开始的位置
    private Vector3 vDrag;  //结束的位置
    private bool dragging;   //是否拖拽(标志位)
    private float angularVelocity; //角速度
    private Vector3 rotationAxis; //旋转轴

    //public bool isSelfCode = false;
    public Camera cachedCamera;
    /// <summary>
    /// Current reusable floor plane
    /// </summary>
    Plane m_FloorPlane;
    public Plane floorPlane
    {
        get { return m_FloorPlane; }
    }
    /// <summary>
    /// Y-height of the floor the camera is assuming
    /// </summary>
    public float floorY;
    public Vector3 currentLookPosition { get; private set; }
    public Vector3 cameraPosition { get; private set; }
    public Vector3 lookPosition { get; private set; }
    //float yOffset = 0;
    void Start()
    {
        dragging = false;
        angularVelocity = 0;
        rotationAxis = Vector3.zero;
        cachedCamera = Camera.main;

        m_FloorPlane = new Plane(Vector3.forward, new Vector3(0.0f, floorY, 0.0f));
        // Set initial values
        var lookRay = new Ray(cachedCamera.transform.position, cachedCamera.transform.forward);
        float dist;
        if (m_FloorPlane.Raycast(lookRay, out dist))
        {
            currentLookPosition = lookPosition = lookRay.GetPoint(dist);
        }
        cameraPosition = cachedCamera.transform.position;
    }

    void Update()
    {

        /**
         * 1:当鼠标一直被点击状态，发射射线获取查看是否有目标，有即获取目标初始位置，通过标志位dragging=false得到结束位置，求出角速度和旋转轴
         * 2：当鼠标抬起，设置标志位dragging=false，停止寻找结束位置
         * 3：同步角速度，使目标旋转
         **/
         
        //if (isSelfCode)
        //{
        //    //当鼠标处于一直点击状态
        //    if (Input.GetMouseButton(0))
        //    {
        //        //且dragging=false
        //        if (!dragging)
        //        {
        //            OnStartedDrag();
        //            dragging = true;
        //        }
        //        else
        //        {
        //            OnDrag();
        //        }
        //    }
        //    else
        //        // 设置dragging为false 
        //        dragging = false;
        //}

        if (angularVelocity > 0)
        {
            //根据旋转轴和角速度*Time.deltaTime(缓动)旋转本身
            transform.RotateAround(rotationAxis, -angularVelocity * Time.deltaTime);
            //如果angularVelocity > 0.01f,angularVelocity=angularVelocity * damping ,否则angularVelocity=0;
            angularVelocity = (angularVelocity > 0.01f) ? angularVelocity * damping : 0;
        }
    }
    public void OnStartedDrag() {
        Ray ray = cachedCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            vDown = hit.point - transform.position;
        }
        dragging = true;
    }
    public void OnDrag() {
        Ray ray = cachedCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            vDrag = hit.point - transform.position;
            rotationAxis = Vector3.Cross(vDown, vDrag);
            angularVelocity = Vector3.Angle(vDown, vDrag) * speed;
        }
    }
    public void OnReleased() {
        dragging = false;
    }
    public void OnScrollWheel(float zoomAmount) {
        zoomAmount *= -1;
        float currentfieldOfView = cachedCamera.fieldOfView + wheelSpeed * zoomAmount;
        cachedCamera.fieldOfView = Mathf.Clamp(currentfieldOfView, 20, 60);
        //cameraPosition += Vector3.forward * zoomAmount;
        //cachedCamera.transform.position = cameraPosition;
    }
         
}
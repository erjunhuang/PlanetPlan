using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class ArcBall : MonoBehaviour {

    public bool useSmoothRotation = false;
    public float smoothDamping = 10f;

    public bool wrapAround = false;
    private Vector3 dragPosition;
    private Vector3 prevMousePosition;

    public bool constrainToAxis = false;
    public Vector3 constrainAxis = Vector3.up;
    /*
    public bool useInertia = false;
    public float inertia = 1f;
    */
    public bool useFixedRadius = true;
    public bool calcRadiusFromCollider = true;
    public float fixedRadius = 5f;

    private float radius;

    private float raycastDist = 1000;

    private Vector3 startDrag;
    private Vector3 endDrag;
    private Quaternion downR;
    private Quaternion currR;
    public bool useInertia = true;

    float yOffset = 0;

    private bool dragging;
    void Start()
    {
        if (useFixedRadius)
            radius = (calcRadiusFromCollider) ? GetRadiusFromCollider() : fixedRadius;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            OnMouseDown();
        }
        // on mouse down
        if (Input.GetMouseButton(0))
        {
             
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // if the object was clicked
            if (Physics.Raycast(ray, out hit))
            {
                if (!dragging)
                {
                    dragging = true;
                }
                else
                {
                    OnMouseDrag();
                }
            }
            else
                dragging = false;
        }

        // on mouse up stop dragging
        if (Input.GetMouseButtonUp(0))
            dragging = false;
    }
    void OnMouseDown()
    {
        if (!useFixedRadius)
            InitArcball();

        downR = transform.rotation;
        prevMousePosition = dragPosition = Input.mousePosition;
        startDrag = MapToSphere(dragPosition);
    }

    void OnMouseDrag()
    {
        dragPosition += Input.mousePosition - prevMousePosition;
        endDrag = MapToSphere(dragPosition);

        Vector3 axis = Vector3.Cross(startDrag, endDrag).normalized;

        float angle = Vector3.Angle(startDrag, endDrag);
        Quaternion dragR = Quaternion.AngleAxis(angle, axis);

        currR = dragR * downR;
        if (useSmoothRotation)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, currR, Time.deltaTime * smoothDamping);
        }
        else
        {
            transform.rotation = currR;
        }

        prevMousePosition = Input.mousePosition;

        //Dev drawing
        Debug.DrawLine(transform.position, transform.position + axis, Color.white);
        Debug.DrawLine(transform.position, transform.position + startDrag, Color.yellow);
        Debug.DrawLine(transform.position, transform.position + endDrag, Color.green);
    }

    /// <summary>
        /// Dynamic arcball radius. Radius is determined by distance from grap point to transform.position
        /// </summary>
    private void InitArcball()
    {
        Ray ray = GetCamera().ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (GetComponent<Collider>().Raycast(ray, out hit, raycastDist))
        { //called from OnMouseDown, so we always hit
            radius = Vector3.Distance(hit.point, transform.position);
        }

        radius = 5;
    }

    /// <summary>
        /// Maps mouse position onto the arcball sphere as local unit vector
        /// </summary>
    private Vector3 MapToSphere(Vector3 position)
    {
        Ray ray = GetCamera().ScreenPointToRay(position);

        Vector3 normal = (transform.position - GetCamera().transform.position).normalized;
        Plane plane = new Plane(normal, transform.position);
        float dist = 0;
        plane.Raycast(ray, out dist);
        Vector3 hitPoint = ray.GetPoint(dist);

        float length = Vector3.Distance(hitPoint, transform.position);
        if (length < radius)
        {//on arcball
            float k = (float)(Mathf.Sqrt(radius - length));
            hitPoint -= normal * k;
        }
        else
        {//otside. 

            if (wrapAround)
            {
                //Do wraparound here
            }
            Vector3 dir = (hitPoint - transform.position).normalized;
            hitPoint = transform.position + dir * radius;
        }
        if (constrainToAxis)
            return ConstrainVector((hitPoint - transform.position));

        return (hitPoint - transform.position).normalized;
    }

    //works ok for primitives. use bounds instead?
    private float GetRadiusFromCollider()
    {
        switch (GetComponent<Collider>().GetType().ToString())
        {
            case "UnityEngine.BoxCollider":
                BoxCollider bCol = ((BoxCollider)GetComponent<Collider>());
                return Vector3.Scale(bCol.size, transform.localScale).magnitude * 0.5f;

            case "UnityEngine.SphereCollider":
                SphereCollider sCol = ((SphereCollider)GetComponent<Collider>());
                Vector3 s = transform.localScale;
                return Mathf.Max(s.x, Mathf.Max(s.y, s.z)) * sCol.radius;
                /*
                case "UnityEngine.CapsuleCollider":
                    CapsuleCollider cCol = ((CapsuleCollider)collider);
                    return Mathf.Max(cCol.height*0.5f, cCol.radius);
                    break;
                
                case "UnityEngine.MeshCollider":
                    return fixedRadius;///FIx
                    break;
                    */
        }
        return fixedRadius; //fallback
    }

    private Vector3 ConstrainVector(Vector3 vector)
    {
        vector = vector - constrainAxis * Vector3.Dot(constrainAxis, vector);
        return vector.normalized;
    }

    private Camera GetCamera()
    {
        return Camera.main;
    }
}

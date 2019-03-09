using UnityEngine;

public class SelfRotate : MonoBehaviour {
    public Transform target;
    public float speed = 0.2f;
    public float _RotationSpeed = 10;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.up * _RotationSpeed, Space.Self);

        //Vector3 axis = new Vector3(0, -360, 0);
        //transform.RotateAround(target.position, axis, speed * Time.deltaTime);
    }
}

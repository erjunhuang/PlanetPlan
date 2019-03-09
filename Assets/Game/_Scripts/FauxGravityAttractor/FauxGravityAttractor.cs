using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FauxGravityAttractor : MonoBehaviour {
	public SphereCollider col;

    void Awake ()
	{
		col = GetComponent<SphereCollider>();
	}

	public float gravity = -10f;
    //正常的重力效果 速度过快可能会飞出去 重力过小也有可能
    public void Attract (Rigidbody body)
	{
		Vector3 gravityUp = (body.position - transform.position).normalized;
        body.AddForce(gravityUp * gravity);
		RotateBody(body);
	}
    //如果想让玩家紧紧的贴着球跑 可通过得到球的Collider大小进行计算
    public void PlaceOnSurface (Rigidbody body)
	{
        body.MovePosition((body.position - transform.position).normalized * (transform.localScale.x * col.radius));
		RotateBody(body);
	}

	void RotateBody (Rigidbody body)
	{
		Vector3 gravityUp = (body.position - transform.position).normalized;
		Quaternion targetRotation = Quaternion.FromToRotation(body.transform.up, gravityUp) * body.rotation;
		body.MoveRotation (Quaternion.Slerp(body.rotation, targetRotation, 50f * Time.deltaTime));
	}

}

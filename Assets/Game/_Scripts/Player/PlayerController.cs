using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float moveSpeed = 10f;
	public float rotationSpeed = 10f;

    public bool isAI = false;
    public float AiIntervalTime = 3f;
    private Rigidbody rb;
    private float startTime = 0f;
    void Start ()
	{
		rb = GetComponent<Rigidbody>();
    }
    public void move(Vector3 direction,float fixedDeltaTime) {
        rb.MovePosition(rb.position + transform.TransformDirection(direction) * moveSpeed* fixedDeltaTime);
    }
    public void rotation(float rotation, float fixedDeltaTime) {
        Vector3 yRotation = Vector3.up * rotation * rotationSpeed* fixedDeltaTime;
        Quaternion deltaRotation = Quaternion.Euler(yRotation);
        Quaternion targetRotation = rb.rotation * deltaRotation;
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, 50f * fixedDeltaTime));
    }
    //AI
    void FixedUpdate()
    {
        if (isAI)
        {
            move(Vector3.forward, Time.fixedDeltaTime);

            startTime += Time.fixedDeltaTime;
            if (startTime > AiIntervalTime)
            {
                rotation(Random.Range(-1,1), Time.fixedDeltaTime);
                if (startTime > AiIntervalTime * 1.5f)
                    startTime = 0;
            }
        }
    }
}

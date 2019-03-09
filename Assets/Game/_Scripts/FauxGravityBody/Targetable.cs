using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Targetable : MonoBehaviour
{
    private Rigidbody rb;
    public bool placeOnSurface = false;
    public bool isStartFauxGravity = true;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        //去掉系统的重力且冻结旋转
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }


    public virtual void FixedUpdate()
    {
        if (!isStartFauxGravity) return;
        if (placeOnSurface)
            FauxGravityManger.instance.currentFauxGravityAttractor.PlaceOnSurface(rb);
        else
            FauxGravityManger.instance.currentFauxGravityAttractor.Attract(rb);
    }
}

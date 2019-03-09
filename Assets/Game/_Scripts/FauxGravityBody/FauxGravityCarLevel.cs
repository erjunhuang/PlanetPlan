using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FauxGravityCarLevel : FauxGravityBodyLevel
{
    public float speed=1;
    // Use this for initialization
    protected override void Start () {
        base.Start();
        if (transform.parent) {
            PlayerController playerController = transform.parent.GetComponent<PlayerController>();
            playerController.moveSpeed = speed;
        }
    }
}

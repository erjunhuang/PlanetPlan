using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FauxGravityBodyLevel : MonoBehaviour {
    public FauxGravityBodyGhost towerGhostPrefab;
    public string description;
    public int cost;
    public string FauxGravityBodyName;
    public string upgradeDescription;
    // Use this for initialization
    protected virtual  void Start () {
         
    }

    // Update is called once per frame
    protected virtual void Update () {
		
	}

    public float GetTowerDps() {

        return 0;
    }
}

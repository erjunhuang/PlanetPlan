using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FauxGravityBodyDisplay : MonoBehaviour {
    public Text fauxGravityBodyName, description, dps;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Show(FauxGravityBody tower)
    {
        int levelOfTower = tower.currentLevel;
        Show(tower, levelOfTower);
    }

    public void Show(FauxGravityBody fauxGravityBody, int levelOfTower)
    {
        if (levelOfTower >= fauxGravityBody.levels.Length)
        {
            return;
        }
        FauxGravityBodyLevel level = fauxGravityBody.levels[levelOfTower];
        DisplayText(fauxGravityBodyName, level.FauxGravityBodyName);
        DisplayText(description, level.description);
        DisplayText(dps, level.GetTowerDps().ToString("f2"));
    }

    static void DisplayText(Text textBox, string text)
    {
        if (textBox != null)
        {
            textBox.text = text;
        }
    }

}

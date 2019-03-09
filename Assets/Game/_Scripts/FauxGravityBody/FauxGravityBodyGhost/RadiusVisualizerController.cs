﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadiusVisualizerController : MonoBehaviour {
    public GameObject radiusVisualizerPrefab;
    public float radiusVisualizerHeight = 0.02f;
    public Vector3 localEuler;
    readonly List<GameObject> m_RadiusVisualizers = new List<GameObject>();
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetupRadiusVisualizers(FauxGravityBody fauxGravityBody, Transform ghost = null) {
        int length = 1;
        for (int i = 0; i < length; i++)
        {
            if (m_RadiusVisualizers.Count < i + 1)
            {
                m_RadiusVisualizers.Add(Instantiate(radiusVisualizerPrefab));
            }
            GameObject radiusVisualizer = m_RadiusVisualizers[i];
            radiusVisualizer.SetActive(true);
            radiusVisualizer.transform.SetParent(ghost == null ? fauxGravityBody.transform : ghost);
            radiusVisualizer.transform.localPosition = new Vector3(0, radiusVisualizerHeight, 0);
            radiusVisualizer.transform.localScale = Vector3.one*2;
            radiusVisualizer.transform.localRotation = new Quaternion { eulerAngles = localEuler };

            var visualizerRenderer = radiusVisualizer.GetComponent<Renderer>();
            if (visualizerRenderer != null)
            {
                //visualizerRenderer.material.color = provider.effectColor;
            }
        }
    }

    public void HideRadiusVisualizers()
    {
        foreach (GameObject radiusVisualizer in m_RadiusVisualizers)
        {
            radiusVisualizer.transform.parent = transform;
            radiusVisualizer.SetActive(false);
        }
    }
}

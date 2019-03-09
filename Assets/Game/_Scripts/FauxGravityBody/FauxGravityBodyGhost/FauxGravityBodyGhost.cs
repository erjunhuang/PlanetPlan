using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FauxGravityBodyGhost : Targetable
{
    protected MeshRenderer[] m_MeshRenderers;
    public FauxGravityBody controller { get; private set; }
    public Collider ghostCollider { get; private set; }

    protected override void Start()
    {
        base.Start();
    }

    public virtual void Initialize(FauxGravityBody fauxGravityBody)
    {
        m_MeshRenderers = GetComponentsInChildren<MeshRenderer>();
        controller = fauxGravityBody;
        if (GameUIManager.instanceExists)
        {
           GameUIManager.instance.SetupRadiusVisualizer(controller, transform);
        }
        ghostCollider = GetComponent<Collider>();
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Show this ghost
    /// </summary>
    public virtual void Show()
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

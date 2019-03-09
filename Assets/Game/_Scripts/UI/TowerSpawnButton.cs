using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// A button controller for spawning towers
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class TowerSpawnButton : MonoBehaviour, IDragHandler
{
    /// <summary>
    /// The text attached to the button
    /// </summary>
    public Text buttonText;

    public Image towerIcon;

    public Button buyButton;

    public Image energyIcon;

    public Color energyDefaultColor;

    public Color energyInvalidColor;

    public event Action<FauxGravityBody> buttonTapped;

    public event Action<FauxGravityBody> draggedOff;

  
    FauxGravityBody m_fauxGravityBody;

    RectTransform m_RectTransform;

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (!RectTransformUtility.RectangleContainsScreenPoint(m_RectTransform, eventData.position))
        {
            if (draggedOff != null)
            {
                draggedOff(m_fauxGravityBody);
            }
        }
    }

    public void InitializeButton(FauxGravityBody fauxGravityBody)
    {
        m_fauxGravityBody = fauxGravityBody;

        if (fauxGravityBody.levels.Length > 0)
        {
            FauxGravityBodyLevel fauxGravityBodyLevel = fauxGravityBody.levels[0];
            buttonText.text = fauxGravityBodyLevel.cost.ToString();
        }
        else
        {
            Debug.LogWarning("[Tower Spawn Button] No level data for tower");
        }
        UpdateButton();
    }

    /// <summary>
    /// Cache the rect transform
    /// </summary>
    protected virtual void Awake()
    {
        m_RectTransform = (RectTransform)transform;
    }

    /// <summary>
    /// Unsubscribe from events
    /// </summary>
    protected virtual void OnDestroy()
    {
    }

    /// <summary>
    /// The click for when the button is tapped
    /// </summary>
    public void OnClick()
    {
        if (buttonTapped != null)
        {
            buttonTapped(m_fauxGravityBody);
        }
    }

    /// <summary>
    /// Update the button's button state based on cost
    /// </summary>
    void UpdateButton()
    {
        buyButton.interactable = true;
        energyIcon.color = energyDefaultColor;
    }
}

using UnityEngine;
using UnityEngine.UI;

public class FauxGravityBodyUI : MonoBehaviour {
    private FauxGravityBodyDisplay fauxGravityBodyDisplay;

    private Canvas canvas;
    private FauxGravityBody m_fauxGravityBody;
    public RectTransform panelRectTransform;
    public Button upgradeButton;
    public Text upgradeDescription;
    // Use this for initialization
    void Start () {
        fauxGravityBodyDisplay = GetComponent<FauxGravityBodyDisplay>();
        canvas = GetComponent<Canvas>();
        if (GameUIManager.instanceExists) {
            GameUIManager.instance.selectionChanged += OnUISelectionChanged;
            GameUIManager.instance.stateChanged += OnGameUIStateChanged;
        }

    }
    public virtual void Show(FauxGravityBody fauxGravityBody)
    {
        if (fauxGravityBody == null)
        {
            return;
        }
        AdjustPosition();
        this.m_fauxGravityBody = fauxGravityBody;


        if (upgradeButton != null)
        {
            bool maxLevel = fauxGravityBody.isAtMaxLevel;
            upgradeButton.gameObject.SetActive(!maxLevel);
            if (!maxLevel)
            {
                upgradeDescription.text =
                    fauxGravityBody.levels[fauxGravityBody.currentLevel + 1].upgradeDescription.ToUpper();
            }
        }

        fauxGravityBodyDisplay.Show(fauxGravityBody);
        canvas.enabled = true;
    }

    public virtual void Hide()
    {
        canvas.enabled = false;
        if (GameUIManager.instanceExists)
        {
            GameUIManager.instance.HideRadiusVisualizer();
        }

        this.m_fauxGravityBody = null;
    }
    protected virtual void OnUISelectionChanged(FauxGravityBody fauxGravityBody)
    {
        if (fauxGravityBody != null)
        {
            Show(fauxGravityBody);
        }
        else
        {
            Hide();
        }
    }

    void OnGameUIStateChanged(GameUIManager.State oldState, GameUIManager.State newState)
    {
        if (newState == GameUIManager.State.GameOver)
        {
            Hide();
        }
    }

    public void UpgradeButtonClick()
    {
        GameUIManager.instance.UpgradeSelectedTower();
    }
    public void SellButtonClick()
    {
        GameUIManager.instance.SellSelectedTower();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        AdjustPosition();
    }

    protected void AdjustPosition()
    {
        if (m_fauxGravityBody == null)
        {
            return;
        }
        Vector3 point = Camera.main.WorldToScreenPoint(m_fauxGravityBody.transform.position);
        point.z = 0;
        panelRectTransform.transform.position = point;
    }

    void OnDestroy()
    {
        if (GameUIManager.instanceExists)
        {
            GameUIManager.instance.selectionChanged -= OnUISelectionChanged;
            GameUIManager.instance.stateChanged -= OnGameUIStateChanged;
        }
    }
}

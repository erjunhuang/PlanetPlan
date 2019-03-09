using Core.Input;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Core.Utilities;
using JetBrains.Annotations;

public struct UIPointer
{
    public PointerInfo pointer;
    public Ray ray;
    public RaycastHit? raycast;
    public bool overUI;
}

 
public class GameUIManager : Singleton<GameUIManager>
{
    public enum State
    {
        Normal,
        Building,
        Paused,
        GameOver,
        BuildingWithDrag
    }


    public LayerMask ghostWorldPlacementMask;
    public LayerMask planetLayer;
    public LayerMask ghostMask;
    public LayerMask fauxGravityBobyLayer;
    public FauxGravityBodyUI fauxGravityBodyUI;
    private FauxGravityBody currentSelectedBody;
    public Action<FauxGravityBody> selectionChanged;
    public Action<State, State> stateChanged;
    public State state { get; private set; }

    private FauxGravityBodyGhost m_CurrentFauxGravityBodyGhost;

    public bool isBuilding
    {
        get
        {
            return state == State.Building || state == State.BuildingWithDrag;
        }
    }
    public RadiusVisualizerController radiusVisualizerController;
    // Use this for initialization
    void Start()
    {
        //测试
        SetState(State.Normal);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TrySelectTower(PointerInfo info)
    {
        if (state != State.Normal)
        {
            throw new InvalidOperationException("Trying to select towers outside of Normal state");
        }
        UIPointer uiPointer = WrapPointer(info);
        RaycastHit output;
        // bool hasHit = Physics.Raycast(uiPointer.ray, out output, float.MaxValue, fauxGravityBobyLayer);
        //解决能穿过物体点到后面的物体这种bug
        bool hasHit = Physics.Raycast(uiPointer.ray, out output);
        if (!hasHit || uiPointer.overUI)
        {
            return;
        }
        if (output.collider.transform.parent ) {
            var fauxGravityBody = output.collider.transform.parent.GetComponent<FauxGravityBody>();
            if (fauxGravityBody != null)
            {
                SelectTower(fauxGravityBody);
            }
        }
    }

    public void SelectTower(FauxGravityBody fauxGravityBody)
    {
        if (state != State.Normal)
        {
            throw new InvalidOperationException("Trying to select whilst not in a normal state");
        }
        DeselectFauxGravityBody();
        currentSelectedBody = fauxGravityBody;
        if (currentSelectedBody != null)
        {
            currentSelectedBody.removed += OnTowerDied;
        }
        radiusVisualizerController.SetupRadiusVisualizers(fauxGravityBody);
        if (selectionChanged != null)
        {
            selectionChanged(fauxGravityBody);
        }
    }

    public void UpgradeSelectedTower()
    {
        if (state != State.Normal)
        {
            throw new InvalidOperationException("Trying to upgrade whilst not in Normal state");
        }
        if (currentSelectedBody == null)
        {
            throw new InvalidOperationException("Selected Tower is null");
        }
        if (currentSelectedBody.isAtMaxLevel)
        {
            return;
        }

        currentSelectedBody.UpgradeTower();
        fauxGravityBodyUI.Hide();
        DeselectFauxGravityBody();
    }

    public void SellSelectedTower()
    {
        if (state != State.Normal)
        {
            throw new InvalidOperationException("Trying to sell tower whilst not in Normal state");
        }
        if (currentSelectedBody == null)
        {
            throw new InvalidOperationException("Selected Tower is null");
        }
        currentSelectedBody.Sell();
        DeselectFauxGravityBody();
    }

    public void DeselectFauxGravityBody()
    {
        if (state != State.Normal)
        {
            throw new InvalidOperationException("Trying to deselect tower whilst not in Normal state");
        }

        if (currentSelectedBody != null)
        {
            currentSelectedBody.removed -= OnTowerDied;
        }

        currentSelectedBody = null;
        if (selectionChanged != null)
        {
            selectionChanged(null);
        }
    }

    protected void OnTowerDied(FauxGravityBody targetable)
    {
        fauxGravityBodyUI.enabled = false;
        radiusVisualizerController.HideRadiusVisualizers();
        DeselectFauxGravityBody();
    }

    public void GameOver()
    {
        SetState(State.GameOver);
    }

    public void Pause()
    {
        SetState(State.Paused);
    }

    public void Unpause()
    {
        SetState(State.Normal);
    }

    void SetState(State newState)
    {
        if (state == newState)
        {
            return;
        }
        State oldState = state;
        if (oldState == State.Paused || oldState == State.GameOver)
        {
            Time.timeScale = 1f;
        }

        switch (newState)
        {
            case State.Normal:
                break;
            case State.Building:
                break;
            case State.BuildingWithDrag:
                break;
            case State.Paused:
            case State.GameOver:
                if (oldState == State.Building)
                {
                    CancelGhostPlacement();
                }
                Time.timeScale = 0f;
                break;
            default:
                throw new ArgumentOutOfRangeException("newState", newState, null);
        }
        state = newState;
        if (stateChanged != null)
        {
            stateChanged(oldState, state);
        }
    }

    protected UIPointer WrapPointer(PointerInfo pointerInfo)
    {
        return new UIPointer
        {
            overUI = IsOverUI(pointerInfo),
            pointer = pointerInfo,
            ray = Camera.main.ScreenPointToRay(pointerInfo.currentPosition)
        };
    }

    protected bool IsOverUI(PointerInfo pointerInfo)
    {
        int pointerId;
        EventSystem currentEventSystem = EventSystem.current;

        // Pointer id is negative for mouse, positive for touch
        var cursorInfo = pointerInfo as MouseCursorInfo;
        var mbInfo = pointerInfo as MouseButtonInfo;
        var touchInfo = pointerInfo as TouchInfo;

        if (cursorInfo != null)
        {
            pointerId = PointerInputModule.kMouseLeftId;
        }
        else if (mbInfo != null)
        {
            // LMB is 0, but kMouseLeftID = -1;
            pointerId = -mbInfo.mouseButtonId - 1;
        }
        else if (touchInfo != null)
        {
            pointerId = touchInfo.touchId;
        }
        else
        {
            throw new ArgumentException("Passed pointerInfo is not a TouchInfo or MouseCursorInfo", "pointerInfo");
        }

        return currentEventSystem.IsPointerOverGameObject(pointerId);
    }


    //设置Ghost
    public void SetToDragMode([NotNull] FauxGravityBody fauxGravityBody)
    {
        if (state != State.Normal)
        {
            throw new InvalidOperationException("Trying to enter drag mode when not in Normal mode");
        }

        if (m_CurrentFauxGravityBodyGhost != null)
        {
            // Destroy current ghost
            CancelGhostPlacement();
        }
        SetUpGhostTower(fauxGravityBody);
        SetState(State.BuildingWithDrag);
    }

    public void SetToBuildMode([NotNull] FauxGravityBody fauxGravityBody)
    {
        if (state != State.Normal)
        {
            throw new InvalidOperationException("Trying to enter Build mode when not in Normal mode");
        }

        if (m_CurrentFauxGravityBodyGhost != null)
        {
            // Destroy current ghost
            CancelGhostPlacement();
        }
        SetUpGhostTower(fauxGravityBody);
        SetState(State.Building);
    }

    void SetUpGhostTower([NotNull] FauxGravityBody fauxGravityBody)
    {
        if (fauxGravityBody == null)
        {
            throw new ArgumentNullException("towerToBuild");
        }

        m_CurrentFauxGravityBodyGhost = Instantiate(fauxGravityBody.towerGhostPrefab);
        m_CurrentFauxGravityBodyGhost.Initialize(fauxGravityBody);
       // m_CurrentFauxGravityBodyGhost.Hide();
    }

    public void SetupRadiusVisualizer(FauxGravityBody fauxGravityBody, Transform ghost = null)
    {
        radiusVisualizerController.SetupRadiusVisualizers(fauxGravityBody, ghost);
    }

    /// <summary>
    /// Hides the radius visualizer
    /// </summary>
    public void HideRadiusVisualizer()
    {
        radiusVisualizerController.HideRadiusVisualizers();
    }

    public void CancelGhostPlacement()
    {
        if (!isBuilding)
        {
            throw new InvalidOperationException("Can't cancel out of ghost placement when not in the building state.");
        }
        Destroy(m_CurrentFauxGravityBodyGhost.gameObject);
        m_CurrentFauxGravityBodyGhost = null;
        SetState(State.Normal);
        DeselectFauxGravityBody();
    }
    //移动Ghost
    public void TryMoveGhost(PointerInfo pointerInfo, bool hideWhenInvalid = false)
    {
        if (m_CurrentFauxGravityBodyGhost == null)
        {
            throw new InvalidOperationException("Trying to move the FauxGravityBody ghost when we don't have one");
        }

        UIPointer pointer = WrapPointer(pointerInfo);
        // Do nothing if we're over UI
        if (pointer.overUI && hideWhenInvalid)
        {
            m_CurrentFauxGravityBodyGhost.Hide();
            return;
        }
        MoveGhost(pointer, hideWhenInvalid);
    }

    protected void MoveGhost(UIPointer pointer, bool hideWhenInvalid = false)
    {
        if (m_CurrentFauxGravityBodyGhost == null || !isBuilding)
        {
            throw new InvalidOperationException(
                "Trying to position a FauxGravityBody ghost while the UI is not currently in the building state.");
        }

        PlacementAreaRaycast(ref pointer);
        if (pointer.raycast != null)
        {   
            MoveGhostWithRaycastHit(pointer.raycast.Value);
        }
        else
        {
            MoveGhostOntoWorld(pointer.ray, hideWhenInvalid);
        }
    }

    protected void PlacementAreaRaycast(ref UIPointer pointer)
    {
        pointer.raycast = null;
         
        if (pointer.overUI)
        {
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(pointer.ray, out hit, float.MaxValue, planetLayer))
        {
            pointer.raycast = hit;
        }
    }

    protected virtual void MoveGhostWithRaycastHit(RaycastHit hit)
    {
        m_CurrentFauxGravityBodyGhost.Show();
        m_CurrentFauxGravityBodyGhost.transform.position = hit.point;
        m_CurrentFauxGravityBodyGhost.isStartFauxGravity = true;
    }

    protected virtual void MoveGhostOntoWorld(Ray ray, bool hideWhenInvalid)
    {
        if (!hideWhenInvalid)
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, float.MaxValue, ghostWorldPlacementMask))
            {
                m_CurrentFauxGravityBodyGhost.Show();
                m_CurrentFauxGravityBodyGhost.transform.position = hit.point;
                m_CurrentFauxGravityBodyGhost.isStartFauxGravity = false;
                m_CurrentFauxGravityBodyGhost.transform.LookAt(Camera.main.transform);
            }
        }
        else
        {
            m_CurrentFauxGravityBodyGhost.Hide();
        }
    }

    //放置

    public void TryPlaceTower(PointerInfo pointerInfo)
    {
        UIPointer pointer = WrapPointer(pointerInfo);

        // Do nothing if we're over UI
        if (pointer.overUI)
        {
            return;
        }
        BuyTower(pointer);
    }

    public void BuyTower(UIPointer pointer)
    {
        if (!isBuilding)
        {
            throw new InvalidOperationException("Trying to buy towers when not in a Build Mode");
        }
        if (m_CurrentFauxGravityBodyGhost == null)
        {
            return;
        }
        PlacementAreaRaycast(ref pointer);
        if (!pointer.raycast.HasValue || pointer.raycast.Value.collider == null)
        {
            CancelGhostPlacement();
            return;
        }
        int cost = m_CurrentFauxGravityBodyGhost.controller.purchaseCost;
        PlaceGhost(pointer);
    }
    protected void PlaceGhost(UIPointer pointer)
    {
        if (m_CurrentFauxGravityBodyGhost == null || !isBuilding)
        {
            throw new InvalidOperationException(
                "Trying to position a FauxGravityBody ghost while the UI is not currently in a building state.");
        }

        MoveGhost(pointer);

        RaycastHit hit;
        if (Physics.Raycast(pointer.ray, out hit, float.MaxValue, planetLayer))
        {
            // Place the ghost
            FauxGravityBody createdFauxGravityBody = Instantiate(m_CurrentFauxGravityBodyGhost.controller);
            createdFauxGravityBody.Initialize();
            createdFauxGravityBody.transform.position = hit.point;
            CancelGhostPlacement();
        }
    }
}

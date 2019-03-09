using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Input;
public class DefenseKeyboardMouseInput : KeyboardMouseInput
{
    public GameUIManager gameUIManager;
    protected override void OnEnable()
    {
        base.OnEnable();
        if (!InputController.instanceExists)
        {
            Debug.LogError("[UI] Keyboard and Mouse UI requires InputController");
            return;
        }
        
        InputController controller = InputController.instance;

        controller.tapped += OnTap;
        controller.mouseMoved += OnMouseMoved;
    }

    /// <summary>
    /// Deregister input events
    /// </summary>
    protected override void OnDisable()
    {
        if (!InputController.instanceExists)
        {
            return;
        }

        InputController controller = InputController.instance;

        controller.tapped -= OnTap;
        controller.mouseMoved -= OnMouseMoved;
    }
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    /// <summary>
    /// Ghost follows pointer
    /// </summary>
    void OnMouseMoved(PointerInfo pointer)
    {
        // We only respond to mouse info
        var mouseInfo = pointer as MouseCursorInfo;

        if (mouseInfo != null)
        {
            if (gameUIManager.isBuilding)
            {
                gameUIManager.TryMoveGhost(pointer);
            }
        }
    }

    /// <summary>
    /// Select towers or position ghosts
    /// </summary>
    void OnTap(PointerActionInfo pointer)
    {
        // We only respond to mouse info
        var mouseInfo = pointer as MouseButtonInfo;

        if (mouseInfo != null && !mouseInfo.startedOverUI)
        {
            if (gameUIManager.isBuilding)
            {
                if (mouseInfo.mouseButtonId == 0) 
                {
                    //放置
                    gameUIManager.TryPlaceTower(pointer);
                }
                else // RMB cancels
                {
                    //取消动作
                    gameUIManager.CancelGhostPlacement();
                }
            }
            else {
                if (mouseInfo.mouseButtonId == 0) // LMB confirms
                {   
                    //尝试选择
                    gameUIManager.TrySelectTower(pointer);
                }
            }
        }
    }

    protected override void OnStartedDrag(PointerActionInfo pointer)
    {
        base.OnStartedDrag(pointer);
        var mouseInfo = pointer as MouseButtonInfo;
        if ((mouseInfo != null) &&
            (mouseInfo.mouseButtonId == 0))
        {
            if (arcBallTest && !gameUIManager.isBuilding)
            {
                arcBallTest.OnStartedDrag();
            }
        }
    }
    protected override void OnDrag(PointerActionInfo pointer)
    {
        base.OnStartedDrag(pointer);
        if (arcBallTest && !gameUIManager.isBuilding)
        {
            arcBallTest.OnDrag();
        }
    }

    protected override void OnRelease(PointerActionInfo pointer)
    {
        base.OnRelease(pointer);
        var mouseInfo = pointer as MouseButtonInfo;
        if (mouseInfo != null)
        {
            if (arcBallTest && !gameUIManager.isBuilding)
            {
                arcBallTest.OnReleased();
            }
        }
    }

}

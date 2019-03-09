using Core.Input;
using UnityEngine;

namespace TowerDefense.Input
{
    public class DefenseTouchInput : TouchInput
    {
        public GameUIManager gameUIManager;
        protected override void OnEnable()
        {
            base.OnEnable();
            if (InputController.instanceExists)
            {
                InputController.instance.tapped += OnTap;
                InputController.instance.startedDrag += OnStartDrag;
            }
        }

        /// <summary>
        /// Deregister input events
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            if (InputController.instanceExists)
            {
                InputController.instance.tapped -= OnTap;
                InputController.instance.startedDrag -= OnStartDrag;
            }
        }

        /// <summary>
        /// Hide UI 
        /// </summary>
        protected virtual void Awake()
        {
        }

        /// <summary>
        /// Decay flick
        /// </summary>
        protected override void Update()
        {
            base.Update();
        }


        /// <summary>
        /// Called on tap,
        /// calls confirmation of tower placement
        /// </summary>
        protected virtual void OnTap(PointerActionInfo pointerActionInfo)
        {
            var touchInfo = pointerActionInfo as TouchInfo;
            if (touchInfo != null)
            {
                if (gameUIManager.state == GameUIManager.State.Normal && !touchInfo.startedOverUI)
                {
                    gameUIManager.TrySelectTower(touchInfo);
                }
            }
        }

        /// <summary>
        /// Assigns the drag pointer and sets the UI into drag mode
        /// </summary>
        /// <param name="pointer"></param>
        protected virtual void OnStartDrag(PointerActionInfo pointer)
        {
            var touchInfo = pointer as TouchInfo;
            if (touchInfo != null&& !touchInfo.startedOverUI)
            {
                if (arcBallTest && !gameUIManager.isBuilding) {
                    arcBallTest.OnStartedDrag();
                }
            }
        }

        protected override void OnDrag(PointerActionInfo pointer)
        {
            base.OnDrag(pointer);
            var touchInfo = pointer as TouchInfo;
            if (touchInfo != null && !touchInfo.startedOverUI)
            {   
                if (gameUIManager.state == GameUIManager.State.Building)
                {
                    gameUIManager.TryMoveGhost(touchInfo);
                }

                if (arcBallTest && !gameUIManager.isBuilding)
                {
                    arcBallTest.OnDrag();
                }
            }
        }
        protected override void OnRelease(PointerActionInfo pointer)
        {
            base.OnRelease(pointer);
            var touchInfo = pointer as TouchInfo;
            if (touchInfo != null && !touchInfo.startedOverUI)
            {
                if (gameUIManager.state == GameUIManager.State.Building)
                {
                    gameUIManager.TryPlaceTower(pointer);
                }

                if (arcBallTest && !gameUIManager.isBuilding)
                {
                    arcBallTest.OnReleased();
                }
            }
        }
         
    }
}
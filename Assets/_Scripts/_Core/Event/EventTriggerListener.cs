using UnityEngine;
using UnityEngine.EventSystems;
using Core.Utilities;
namespace Core.Event
{
    public class TouchHandle
    {
        private event OnTouchEventHandle eventHandle = null;
        private object[] handleParmas;

        public TouchHandle(OnTouchEventHandle _handle, params object[] _params)
        {
            SetHandle(_handle, _params);
        }

        public TouchHandle()
        {
        }

        public void SetHandle(OnTouchEventHandle _handle, params object[] _params)
        {
            DestroyHandle();
            eventHandle += _handle;
            handleParmas = _params;
        }

        public void CallEventHandle(GameObject _sender, object _args)
        {
            if (null != eventHandle)
            {
                eventHandle(_sender, _args, handleParmas);
            }
        }

        public void DestroyHandle()
        {
            if (null != eventHandle)
            {
                eventHandle -= eventHandle;
                eventHandle = null;
            }
        }
    }

    public class EventTriggerListener : EventTrigger
    {
        public TouchHandle onBeginDrag, onCancel, onDeselect, onDrag, onDrop, onEndDrag, onInitializePotentialDrag, onMove,
            onPointerClick, onPointerDown, onPointerEnter, onPointerExit, onPointerUp, onScroll, onSelect, onSubmit, onUpdateSelected;

        static public EventTriggerListener Get(GameObject go)
        {
            return go.GetOrAddComponent<EventTriggerListener>();
        }

        void OnDestroy()
        {
            this.RemoveAllHandle();
        }

        private void RemoveAllHandle()
        {
            this.RemoveHandle(onBeginDrag);
            this.RemoveHandle(onCancel);
            this.RemoveHandle(onDeselect);
            this.RemoveHandle(onDrag);
            this.RemoveHandle(onDrop);
            this.RemoveHandle(onEndDrag);
            this.RemoveHandle(onInitializePotentialDrag);
            this.RemoveHandle(onMove);
            this.RemoveHandle(onPointerClick);
            this.RemoveHandle(onPointerDown);
            this.RemoveHandle(onPointerEnter);
            this.RemoveHandle(onPointerExit);
            this.RemoveHandle(onPointerUp);
            this.RemoveHandle(onScroll);
            this.RemoveHandle(onSelect);
            this.RemoveHandle(onSubmit);
            this.RemoveHandle(onUpdateSelected);
        }

        private void RemoveHandle(TouchHandle _handle)
        {

            if (null != _handle)
            {

                _handle.DestroyHandle();

                _handle = null;

            }

        }
        #region  事件
        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (null != onBeginDrag)
            {
                onBeginDrag.CallEventHandle(this.gameObject, eventData);
            }
        }
        public override void OnCancel(BaseEventData eventData)
        {
            if (null != onCancel)
            {
                onCancel.CallEventHandle(this.gameObject, eventData);
            }
        }
        public override void OnDeselect(BaseEventData eventData)
        {
            if (null != onDeselect)
            {
                onDeselect.CallEventHandle(this.gameObject, eventData);
            }
        }
        public override void OnDrag(PointerEventData eventData)
        {
            if (null != onDrag)
            {
                onDrag.CallEventHandle(this.gameObject, eventData);
            }
        }
        public override void OnDrop(PointerEventData eventData)
        {
            if (null != onDrop)
            {
                onDrop.CallEventHandle(this.gameObject, eventData);
            }
        }
        public override void OnEndDrag(PointerEventData eventData)
        {
            if (null != onEndDrag)
            {
                onEndDrag.CallEventHandle(this.gameObject, eventData);
            }
        }
        public override void OnInitializePotentialDrag(PointerEventData eventData)
        {
            if (null != onInitializePotentialDrag)
            {
                onInitializePotentialDrag.CallEventHandle(this.gameObject, eventData);
            }
        }
        public override void OnMove(AxisEventData eventData)
        {
            if (null != onMove)
            {
                onMove.CallEventHandle(this.gameObject, eventData);
            }
        }
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (null != onPointerClick)
            {
                AudioManager.instance.Play("Click");
                onPointerClick.CallEventHandle(this.gameObject, eventData);
            }
        }
        public override void OnPointerDown(PointerEventData eventData)
        {
            if (null != onPointerDown)
            {
                onPointerDown.CallEventHandle(this.gameObject, eventData);
            }
        }
        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (null != onPointerEnter)
            {
                onPointerEnter.CallEventHandle(this.gameObject, eventData);
            }
        }
        public override void OnPointerExit(PointerEventData eventData)
        {
            if (null != onPointerExit)
            {
                onPointerExit.CallEventHandle(this.gameObject, eventData);
            }
        }
        public override void OnPointerUp(PointerEventData eventData)
        {
            if (null != onPointerUp)
            {
                onPointerUp.CallEventHandle(this.gameObject, eventData);
            }
        }
        public override void OnScroll(PointerEventData eventData)
        {
            if (null != onScroll)
            {
                onScroll.CallEventHandle(this.gameObject, eventData);
            }
        }
        public override void OnSelect(BaseEventData eventData)
        {
            if (null != onSelect)
            {
                onSelect.CallEventHandle(this.gameObject, eventData);
            }
        }
        public override void OnSubmit(BaseEventData eventData)
        {
            if (null != onSubmit)
            {
                onSubmit.CallEventHandle(this.gameObject, eventData);
            }
        }
        public override void OnUpdateSelected(BaseEventData eventData)
        {
            if (null != onUpdateSelected)
            {
                onUpdateSelected.CallEventHandle(this.gameObject, eventData);
            }
        }
        #endregion
    }
}

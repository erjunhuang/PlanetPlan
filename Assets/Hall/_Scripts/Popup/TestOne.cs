using UnityEngine;
using Core.Message;
using Core.Event;
using Core.Popup;
public class TestOne : Panel
{
    protected override void OnAwake()
    {
        base.OnAwake();

        EventTriggerListener.Get(this.gameObject).onPointerClick = new TouchHandle(OnClickBtn);
        MessageCenter.Instance.AddListener(MessageType.Net_MessageTestOne, UpdateGold);
    }

    protected override void OnStart()
    {
        base.OnStart();
    }
    protected override void OnLoadData()
    {
        base.OnLoadData();
    }
     
    protected override void OnUpdate(float delatTime)
    {
        base.OnUpdate(delatTime);
    }

    protected override void OnClose()
    {
        MessageCenter.Instance.RemoveListener(MessageType.Net_MessageTestOne, UpdateGold);
        base.OnClose();
    }

    public void OnClickBtn(GameObject _sender, object _args, params object[] _params)
    {
        Close();
    }
    private void UpdateGold(Message message)
    {
        int gold = (int)message["gold"];
        Debug.Log("TestOne UpdateGold:" + gold);
    }
}

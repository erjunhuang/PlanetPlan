using System;
using System.Collections;
using UnityEngine;
using Core.Message;
using Core.ResourcesLoad;
public class GameController : DDOLSingleton<GameController>
{
    // Use this for initialization
    void Awake () {
        //显示UI
        new TestOne().showPanel_();
        //事件模拟
        StartCoroutine(NetUpdateGold());
        //获取公共方法
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private IEnumerator NetUpdateGold()
    {
        int gold = 0;
        while (true)
        {
            gold++;
            yield return new WaitForSeconds(1.0f);
            Message message = new Message(MessageType.Net_MessageTestOne, this);
            message["gold"] = gold;
            message.Send();
        }
    }

    void TestResManager()
    {

        float time = Environment.TickCount;
        for (int i = 1; i < 1000; i++)
        {

            GameObject go = null;
            //直接加载
            //go = Instantiate(Resources.Load<GameObject>("Prefabs/Cube"));  //1
            //go.transform.position = UnityEngine.Random.insideUnitSphere * 20;

            //2正常加载
            //,()=> {Debug.Log("加载进度成功");}
            //go = Instantiate(ResManager.Instance.LoadInstance("Prefabs/Cube")) as GameObject;
            //go.transform.position = UnityEngine.Random.insideUnitSphere * 20;

            //3、异步加载
            ResManager.Instance.LoadAsync("Prefabs/Cube", (_obj) =>
            {
                go = Instantiate(_obj) as GameObject;
                go.transform.position = UnityEngine.Random.insideUnitSphere * 20;
            }, (_progress) =>
            {
                Debug.Log("加载进度" + _progress);
            });

            ////4、协程加载
            //ResManager.Instance.LoadCoroutine("Prefabs/Cube", (_obj) =>
            //{
            //    go = Instantiate(_obj) as GameObject;
            //    go.transform.position = UnityEngine.Random.insideUnitSphere * 20;
            //});
        }
    }

}

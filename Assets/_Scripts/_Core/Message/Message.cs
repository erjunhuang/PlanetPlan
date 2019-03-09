using System.Collections;
using System.Collections.Generic;

namespace Core.Message
{
    public class Message : IEnumerable<KeyValuePair<string, object>>
    {
        private Dictionary<string, object> dicDatas = null;
        public string Name { get; private set; }
        public object Sender { get; private set; }
        public object Content { get; set; }

        #region message[key] = value or data = message[key]
        //实现类似索引器方法：message[key] = value功能
        public object this[string key]
        {
            get
            {
                if (null == dicDatas || !dicDatas.ContainsKey(key))
                {
                    return null;
                }
                return dicDatas[key];
            }
            set
            {
                if (null == dicDatas)
                {
                    dicDatas = new Dictionary<string, object>();
                }
                if (dicDatas.ContainsKey(key))
                {
                    dicDatas[key] = value;
                }
                else
                {
                    dicDatas.Add(key, value);
                }
            }
        }

        #endregion

        #region IEnumerable implementation
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            if (null == dicDatas)
            {
                yield break;
            }
            foreach (KeyValuePair<string, object> kvp in dicDatas)
            {
                yield return kvp;
            }
        }
        #endregion

        #region IEnumerable implementation
        IEnumerator IEnumerable.GetEnumerator()
        {
            return dicDatas.GetEnumerator();  //借用Dictionary接口
        }
        #endregion

        #region Message Construction Function（构造函数）
        public Message(string name, object sender)
        {
            Name = name;
            Sender = sender;
            Content = null;
        }

        public Message(string name, object sender, object content)
        {
            Name = name;
            Sender = sender;
            Content = content;
        }

        public Message(string name, object sender, object content, params object[] _dicParams)
        {
            Name = name;
            Sender = sender;
            Content = content;
            if (_dicParams.GetType() == typeof(Dictionary<string, object>))
            {
                foreach (object _dicParam in _dicParams)
                {
                    foreach (KeyValuePair<string, object> kvp in _dicParam as Dictionary<string, object>)
                    {
                        this[kvp.Key] = kvp.Value;    //利用索引器
                    }
                }
            }
        }

        //类似C++拷贝构造函数
        public Message(Message message)
        {
            Name = message.Name;
            Sender = message.Sender;
            Content = message.Content;
            foreach (KeyValuePair<string, object> kvp in message.dicDatas)
            {
                this[kvp.Key] = kvp.Value;
            }
        }
        #endregion

        #region Add & Remove
        public void Add(string key, object value)
        {
            this[key] = value;
        }

        public void Remove(string key)
        {
            if (null != dicDatas && dicDatas.ContainsKey(key))
            {
                dicDatas.Remove(key);
            }
        }
        #endregion

        #region Send()
        public void Send()
        {
            MessageCenter.Instance.SendMessage(this);
        }
        #endregion
    }

}

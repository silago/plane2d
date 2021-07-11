using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Events;
using Modules.Resources;
using UnityEngine;
using Yarn;
using Yarn.Unity;
using Type = Yarn.Type;
public class DialogueDataStorage : IVariableStorage
{
    private readonly DataProvider _dataProvider;
    private readonly ResourceSettings _resourceSettings;
    private List<DialogueResourceChanged<int>> IntMessageStash = new List<DialogueResourceChanged<int>>();
    private string Trim(string s) => s.Trim('$');
    public  DialogueDataStorage(DataProvider dataProvider, ResourceSettings resourceSettings)
    {
        _resourceSettings = resourceSettings;
        _dataProvider = dataProvider;
    }
    public void SetValue(string variableName, string stringValue) => _dataProvider.Set(Trim(variableName), stringValue);
    
    // we set value through dialogue and want to notify
    public void SetValue(string variableName, float floatValue)
    {
        int value = (int)floatValue;
        variableName = variableName.Trim('$');
        var msg = new DialogueResourceChanged<int>() {
            ResourceInfo = _resourceSettings[variableName],
            Prev = _dataProvider[variableName]
        };
        msg.Current = msg.Prev + value;
        _dataProvider[variableName] = value; 
        IntMessageStash.Add(msg);
        SendMessagesLater();
        //this.SendEvent(msg);
    }
    public void SetValue(string variableName, bool boolValue)     => _dataProvider[Trim(variableName)] = boolValue ? 1 : 0;
    public bool TryGetValue<T>(string variableName, out T result)
    {
        return _dataProvider.TryGetValue(Trim(variableName), out result);
    }

    async void SendMessagesLater()
    {
        await Task.Yield();
        //this.SendEvent(new DialogueResourceChangedMessagePack<int>() {
        //    Data = IntMessageStash.ToArray()
        //});
        foreach (var msg in IntMessageStash)
        {
            this.SendEvent(msg);
        }
        IntMessageStash.Clear();
    }
    
    public void Clear()
    {
        //throw new NotImplementedException();
    }
    

    public class DialogueResourceChangedMessagePack<T> : IMessage
    {
        public DialogueResourceChanged<T>[] Data;
    }
    public class DialogueResourceChanged<T> : IMessage
    {
        public ResourceInfo ResourceInfo;
        public T Prev;
        public T Current;
    }
}

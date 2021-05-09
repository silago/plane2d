using System;
using Modules.Resources;
using Yarn;
using Yarn.Unity;
using Type = Yarn.Type;
public class DialogueDataStorage : IVariableStorage
{
    private readonly UserDataProvider _userDataProvider;
    private string Trim(string s) => s.Trim('$');
    public  DialogueDataStorage(UserDataProvider userDataProvider)
    {
        _userDataProvider = userDataProvider;
    }
    public void SetValue(string variableName, string stringValue) => _userDataProvider.Set(Trim(variableName), stringValue);
    public void SetValue(string variableName, float floatValue) => _userDataProvider[Trim(variableName)] = (int)floatValue;
    public void SetValue(string variableName, bool boolValue) => _userDataProvider[Trim(variableName)] = boolValue ? 1 : 0;
    public bool TryGetValue<T>(string variableName, out T result)
    {
        return _userDataProvider.TryGetValue(Trim(variableName), out result);
    }
    
    public void Clear()
    {
        //throw new NotImplementedException();
    }
}

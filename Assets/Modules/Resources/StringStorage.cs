using UnityEngine;
public class StringStorage<T>
{
    public string Prefix = "";
    private string Key(T k) => $"{Prefix}_{typeof(T)}_{k}";
    public string this[T index]
    {
        get => PlayerPrefs.GetString(Key(index), string.Empty);
        set => PlayerPrefs.SetString(Key(index), value);
    }
}

public class StringStorage : StringStorage<string>
{
}

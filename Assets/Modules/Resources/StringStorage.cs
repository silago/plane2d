using UnityEngine;
public class StringStorage<T>
{
    private string _prefix = typeof(T).ToString();
    private string Key(T k) => $"{_prefix}_{k}";
    public string this[T index]
    {
        get => PlayerPrefs.GetString(Key(index), string.Empty);
        set => PlayerPrefs.SetString(Key(index), value);
    }
}

public class StringStorage : StringStorage<string>
{
}

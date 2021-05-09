using UnityEngine;
public class FloatStorage<T> 
{
    private string Key(T k) => $"{typeof(T)}_{k}";
    public float this[T index]
    {
        get => PlayerPrefs.GetFloat(Key(index), 0f);
        set => PlayerPrefs.SetFloat(Key(index), value);
    }
    public bool Check(T key, string op, float value)
    {
        return Check(key, op.FromText(), value);
    }
    public bool Check(T key, Operator op, float value)
    {
        return op switch {
            Operator.Eq => this[key] == value,
            Operator.Lt => this[key] < value,
            Operator.Gt => this[key] > value,
            _ => false
        };
    }
}

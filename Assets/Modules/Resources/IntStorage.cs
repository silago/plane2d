using System;
using UnityEngine;

public class IntStorage<T>
{
    public string Prefix = "";
    private string Key(T k) => $"{Prefix}_{typeof(T)}_{k}";
    public int this[T index]
    {
        get => PlayerPrefs.GetInt(Key(index), 0);
        set => PlayerPrefs.SetInt(Key(index), value);
    }
    public bool Check(T key, string op, int value)
    {
        return Check(key, op.FromText(), value);
    }
    public bool Check(T key, Operator op, int value)
    {
        return op switch {
            Operator.Eq => this[key] == value,
            Operator.Lt => this[key] < value,
            Operator.Gt => this[key] > value,
            _ => false
        };
    }
}

//public static class storageext
//{
//    public static bool eq(this icomparable a, icomparable b) => a.compareto(b) == 0;
//    public static bool gt(this icomparable a, icomparable b) => a.compareto(b) >  0;
//    public static bool lt(this icomparable a, icomparable b) => a.compareto(b) <  0;
//}

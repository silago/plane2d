using System.Collections.Generic;
using JetBrains.Annotations;
namespace Modules.Common
{
    public class TwoSideDictionary<T1, T2>
    {
        private Dictionary<T1, T2> _dictionary1 = new Dictionary<T1, T2>();
        private Dictionary<T2, T1> _dictionary2 = new Dictionary<T2, T1>();
        public void Add(T1 key, T2 value)
        {
            _dictionary1.Add(key, value);
            _dictionary2.Add(value, key);
        }
    }
}

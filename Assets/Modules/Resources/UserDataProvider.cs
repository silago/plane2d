using System;
using System.Collections.Generic;
using System.Linq;
using Events;
using Modules.YarnPlayer;

public class ResourceChanged : IMessage
{
    public string Name;
    public int Value;
}

namespace Modules.Resources
{
    public class UserDataProvider
    {
        private ResourceSettings _resourceSettings;
        private IntStorage<string>    _intStorage   = new IntStorage<string>();
        private FloatStorage<string>  _floatStorage  = new FloatStorage<string>();
        private StringStorage<string> _stringStorage = new StringStorage<string>();
        public UserDataProvider(ResourceSettings resourceSettings)
        {
            _resourceSettings = resourceSettings;
        }
        public int this[string index]
        {
            get => _intStorage[index];
            set
            {
                _intStorage[index] = value;
                this.SendEvent(new ResourceChanged() {Name = index, Value = value} );
            }
        }

        public void SubscribeResourceChange(Action<int> action)
        {
        }

        public ResourceInfo GetResourceInfo(string key) => _resourceSettings[key.Trim('$')];

        public LineRequirement ResolveLineRequirement(ResourceCondition x)
        {
            return new LineRequirement(
                x, GetResourceInfo(x.resource),
                this[x.resource],
                Check(x)
            );
        }
        public void Set(string key, int value) => this[key] = value;
        public void Set(string key, float value) => _floatStorage[key] = value;
        public void Set(string key, string value) => _stringStorage[key] = value;

        public int GetInt(string key) => this[key];
        public float GetFloat(string key) => _floatStorage[key];
        public string GetString(string key) => _stringStorage[key];
    
        public bool TryGetValue<T>(string key, out T o)
        {
            var result = true;
            if (_intStorage[key] != 0 && _intStorage[key] is T vi)
            {
                o = vi;
            } else if (_floatStorage[key] != 0f && _floatStorage[key] is T vf)
            {
                o = vf;
            } else if (_stringStorage[key] != "" && _stringStorage[key] is T vs )
            {
                o = vs;
            }
            else
            {
                o = default;
                result = false;
            }
            return result;
        }
    
        public bool Check(ResourceCondition[] items)
        {
            return items.All(x => Check(x.resource, x.op, x.value));
        }

        public bool Check(ResourceCondition condition) => Check(condition.resource, condition.op, condition.value);
        public bool Check(string resource, Operator op, int value)
        {
            return op switch {
                Operator.Eq => _intStorage[resource] == value,
                Operator.Lt => _intStorage[resource] < value,
                Operator.Gt => _intStorage[resource] > value,
                _ => false
            };
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace Smart
{
    public class LevelEditorData : ScriptableObject
    {
        public LevelEditorGroup selected;
        [SerializeField]
        private List<LevelEditorGroup> groups = new List<LevelEditorGroup>();

        public LevelEditorGroup this[int i]
        {
            get => groups[i];
        }

        public int count
        {
            get => groups.Count;
        }

        public void Add(LevelEditorGroup value)
        {
            groups.Add(value);
        }

        public void Remove(LevelEditorGroup value)
        {
            groups.Remove(value);
        }

        public bool Contains(LevelEditorGroup value)
        {
            return groups.Contains(value);
        }
    }

    // I tried inhereting from List<Object> but serialization won't work
    [System.Serializable]
    public class LevelEditorGroup
    {
        //
        // Values
        //

        public const int maxElements = 100;

        public string name;
        public bool isExpanded;
        public List<Object> list = new List<Object>();

        //
        // Properties
        //

        public int Count
        {
            get => null == list ? 0 : list.Count;
        }

        public bool full
        {
            get => Count == maxElements;
        }

        public Object this[int i]
        {
            get => list[i];
        }

        public LevelEditorGroup(string name)
        {
            this.name = name;
        }

        public bool Contains(Object value)
        {
            if(null == list) { return false; }

            return list.Contains(value);
        }

        public void Add(Object value)
        {
            if (null == list) { return; }

            list.Add(value);
        }

        public void Remove(Object value)
        {
            if (null == list) { return; }

            list.Remove(value);
        }

        public void Remove(int atIndex)
        {
            if (null == list) { return; }

            list.RemoveAt(atIndex);
        }
    }
}

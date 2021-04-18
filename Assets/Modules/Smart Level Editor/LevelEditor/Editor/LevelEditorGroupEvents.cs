using UnityEditor;
using UnityEngine;

namespace Smart
{
	public partial class LevelEditor
    {
        //
        // Group Events
        //

        void GROUP_EVENTS(Rect rect)
        {
            if (null == data.selected) { return; }

            if (!rect.Contains(e.mousePosition)) { return; }

            if (data.selected.full) { return; }

            switch (e.type)
            {
                case EventType.DragUpdated:
                    DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                    break;
                case EventType.DragPerform:
                    GROUP_DRAG_PERFORM();
                    break;
            }
        }

        void GROUP_DRAG_PERFORM()
        {
            Object[] others = DragAndDrop.objectReferences;

            for (int i = 0; i < others.Length; i++)
            {
                if (data.selected.full) { break; }

                if (data.selected.Contains(others[i])) { continue; }

                data.selected.Add(others[i]);
            }

            EditorUtility.SetDirty(data);

            e.Use();
        }

        //
        // Objects
        //

        void ObjectEvent(Object value, Rect rect)
        {
            if (!rect.Contains(e.mousePosition)) { return; }

            switch (e.type)
            {
                case EventType.MouseDown:
                    ObjectMouseDown(value);
                    break;
            }
        }

        void ObjectMouseDown(Object value)
        {
            if (0 == e.button) { ObjectLeftClick(value); }
            if (1 == e.button) { ObjectRightClick(value); }
        }

        void ObjectLeftClick(Object value)
        {
            if (e.control)
            {
                ObjectSelectMultiple(value);
            }
            else
            {
                ObjectSelectSingle(value);
            }

            e.Use();
        }

        void ObjectRightClick(Object value)
        {
            GenericMenu menu = new GenericMenu();

            Object[] objs = new Object[] { value };
            GUIContent content = new GUIContent("Select");
            
            if(selected.Count > 0 && selected.Contains(value))
            {
                objs = selected.ToArray();
                string label = string.Format("Select ({0})", selected.Count);
                content.text = label;
            }

            menu.AddItem(content, false, () => PingObjects(objs));
            menu.ShowAsContext();

            e.Use();
        }

        void PingObjects(Object[] objs)
        {
            Selection.objects = objs;

            for(int i = 0; i < objs.Length; i++)
            {
                EditorGUIUtility.PingObject(objs[i]);
            }
        }

        void ObjectSelectMultiple(Object value)
        {
            if (selected.Contains(value))
            {
                selected.Remove(value);
            }
            else
            {
                selected.Add(value);
            }

            if (!active || mode != Mode.Create) { return; }

            CreatePreviews();
        }

        void ObjectSelectSingle(Object value)
        {
            if(selected.Contains(value))
            {
                selected.Clear();
            }
            else
            {
                selected.Clear();
                selected.Add(value);
            }

            if (!active || mode != Mode.Create) { return; }

            CreatePreviews();
        }
    }
}

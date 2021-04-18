using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Smart
{
    public partial class LevelEditor
    {
        bool LeftClick(Action action)
        {
            // E x i t
            if (e.alt) { return false; }

            bool condition = false;

            switch (paint)
            {
                case Paint.Single:
                    condition = MouseButtonDown(0);
                    break;
                case Paint.Brush:
                    condition = MoseButtonDrag(0) || MouseButtonDown(0);
                    break;
            }

            return LeftClickAction(action, condition);
        }

        bool LeftClickAction(Action action, bool condition)
        {
            if (!condition) { return false; }

            action();

            return true;
        }

        bool MoseButtonDrag(int button)
        {
            if (null == e) { return false; }
            // Exit
            if (false == e.isMouse) { return false; }
            // Exit
            if (e.type != EventType.MouseDrag) { return false; }

            return button == e.button;
        }

        bool MouseButtonUp(int button)
        {
            // Exit
            if (null == e) { return false; }
            // Exit
            if (false == e.isMouse) { return false; }
            // Exit
            if (e.type != EventType.MouseUp) { return false; }

            return button == e.button;
        }

        bool MouseButtonDown(int button)
        {
            // Exit
            if (null == e) { return false; }
            // Exit
            if (false == e.isMouse) { return false; }
            // Exit
            if (e.type != EventType.MouseDown) { return false; }

            return button == e.button;
        }

        bool LeftButtonUp()
        {
            if (null == e) { return false; }

            // E x i t
            if (false == e.isMouse) { return false; }
            // E x i t
            if (0 != e.button) { return false; }
            // E x i t
            if (e.type != EventType.MouseUp) { return false; }

            return true;
        }

        public bool KeyUp(KeyCode keyCode)
        {
            if (null == e) { return false; }
        
            // E x i t
            if (!e.isKey) { return false; }
            // E x i t
            if (e.type != EventType.KeyUp) { return false; }

            return e.keyCode == keyCode;
        }

        public bool KeyDown(KeyCode keyCode)
        {
            if (null == e) { return false; }

            // E x i t
            if (!e.isKey) { return false; }
            // E x i t
            if (e.type != EventType.KeyDown) { return false; }

            return e.keyCode == keyCode;
        }
    }
}
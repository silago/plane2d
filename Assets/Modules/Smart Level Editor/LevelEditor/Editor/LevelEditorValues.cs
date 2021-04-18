using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Smart
{
    public partial class LevelEditor
    {
        //
        // Draw values
        //

        const float imgWidth1 = 80;
        const float imgHeight1 = 80;
        const float imgWidth2 = 100;
        const float imgHeight2 = 100;

        Vector2 defaultIconSize;
        Vector3 rotationAxis = new Vector3(0, 1, 0);
        Vector3 angle = Vector3.zero;

        bool drawSettings = true;
        bool drawSelection = true;
        bool drawGroups = true;

        //
        // Edit values
        //

        private Vector3 pickPosition = Vector3.zero;
        private Vector3 pickNormal = Vector3.zero;
        private Vector3 labelMouseOffset = new Vector3(0f, 0.5f, 0f);
        private Vector3 labelObjectOffset = new Vector3(0, -1, 0);
        private Pick pick = new Pick();
        private Transform[] transforms = null;

        //
        // Properties
        //

        // Targets to delete
        private Object[] targets;

        // The data to store the groups
        private LevelEditorData data;

        // The active scene view
        private SceneView view;

        // Is window focused
        private bool isFocused = false;

        // Is this level editor active
        private bool active = false;

        // The height of an invisible plane where objects are placed
        private float height = 0;

        // Mode
        private Mode mode;

        // Paint
        private Paint paint;

        // Current unity editor tool
        private Tool tool;

        // The size of the brush
        private Vector3Int size = Vector3Int.one;

        // Snap move value
        private Vector3 move = Vector3.one;

        // Snap rotate value
        private Vector3 rotate = new Vector3(15, 15, 15);

        // Snap scale value
        private Vector3 scale = new Vector3(0.5f, 0.5f, 0.5f);
        private Vector3 objectScale = new Vector3(0.5f, 0.5f, 0.5f);

        // Current event
        private Event e;

        // Layer mask
        private LayerMask layer;

        // Selected objects
        private List<Object> selected = new List<Object>();

        // Search filter
        private string filter = "";

        //
        // Default properties
        //

        private readonly Vector3Int defaultSize = Vector3Int.one;
        private readonly Vector3 defaultMove = Vector3.one;
        private readonly Vector3 defaultRotate = new Vector3(15, 15, 15);
        private readonly Vector3 defaultScale = new Vector3(0.5f, 0.5f, 0.5f);
        private readonly int defaultLayer = 0;
        private readonly float defaultHeight = 0;

        //
        // Methods
        //

        void Load()
        {
            size = GetPrefsVector("sle.size", defaultSize);
            move = GetPrefsVector("sle.move", defaultMove);
            rotate = GetPrefsVector("sle.rotate", defaultRotate);
            scale = GetPrefsVector("sle.scale", defaultScale);
            layer = EditorPrefs.GetInt("sle.layer", defaultLayer);
            height = EditorPrefs.GetFloat("sle.height", defaultHeight);
        }

        void Save()
        {
            SetPrefsVector("sle.size", size);
            SetPrefsVector("sle.move", move);
            SetPrefsVector("sle.rotate", rotate);
            SetPrefsVector("sle.scale", scale);
            EditorPrefs.SetInt("sle.layer", layer);
            EditorPrefs.SetFloat("sle.height", height);
        }

        Vector3 GetPrefsVector(string key, Vector3 defaultValue)
        {
            Vector3 result = defaultValue;

            result.x = EditorPrefs.GetFloat(string.Format("{0}.x", key), defaultValue.x);
            result.y = EditorPrefs.GetFloat(string.Format("{0}.y", key), defaultValue.y);
            result.z = EditorPrefs.GetFloat(string.Format("{0}.z", key), defaultValue.z);

            return result;
        }

        Vector3Int GetPrefsVector(string key, Vector3Int defaultValue)
        {
            Vector3Int  result = defaultValue;

            result.x = EditorPrefs.GetInt(string.Format("{0}.x", key), defaultValue.x);
            result.y = EditorPrefs.GetInt(string.Format("{0}.y", key), defaultValue.y);
            result.z = EditorPrefs.GetInt(string.Format("{0}.z", key), defaultValue.z);

            return result;
        }

        void SetPrefsVector(string key, Vector3 value)
        {
            EditorPrefs.SetFloat(string.Format("{0}.x", key), value.x);
            EditorPrefs.SetFloat(string.Format("{0}.y", key), value.y);
            EditorPrefs.SetFloat(string.Format("{0}.z", key), value.z);
        }

        void SetPrefsVector(string key, Vector3Int value)
        {
            EditorPrefs.SetInt(string.Format("{0}.x", key), value.x);
            EditorPrefs.SetInt(string.Format("{0}.y", key), value.y);
            EditorPrefs.SetInt(string.Format("{0}.z", key), value.z);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.XR;
using Object = UnityEngine.Object;

namespace Smart
{
    using GL = GUILayout;

    public partial class LevelEditor
    {
        //
        // Mode & Paint
        //

        public enum Paint
        {
            Single,
            Brush
        }

        public enum Mode
        {
            Create,
            Delete,
            Move,
            Rotate,
            Scale
        }

        //
        // Passive Controls
        //

        void PassiveControls()
        {
            int passive = GUIUtility.GetControlID(FocusType.Passive);
            HandleUtility.AddDefaultControl(passive);
        }

        //
        // Snap
        //

        public float Snap(float val, float snap)
        {
            if(snap > 0)
            {
                val = Mathf.Round(val / snap) * snap;
            }

            return val;
        }

        public Vector3 Snap(Vector3 v, float x, float y, float z)
        {
            v.x = Snap(v.x, x);
            v.y = Snap(v.y, y);
            v.z = Snap(v.z, z);

            return v;
        }

        public Vector3 Snap(Vector3 v, Vector3 snap)
        {
            v.x = Snap(v.x, snap.x);
            v.y = Snap(v.y, snap.y);
            v.z = Snap(v.z, snap.z);
        

            return v;
        }

        //
        // Labels
        //

        void SceneLabel(Vector3 point, string text)
        {
            Handles.Label(point, text, Styles.sceneLabel);
        }

        void SceneLabel(Vector3 point, string format, object arg)
        {
            Handles.Label(point, string.Format(format, arg), Styles.sceneLabel);
        }

        void SceneLabel(Vector3 point, string format, params object[] args)
        {
            Handles.Label(point, string.Format(format, args), Styles.sceneLabel);
        }

        //
        // Draws
        // 

        void DrawBrush(Vector3 point, Vector3 rotation, Color color)
        {
            Handles.color = new Color(color.r,color.g,color.b,0.1f);
            Handles.DrawSolidDisc(point, rotation, 0.1f);
            //Handles.RotationHandle(rot,Vector3.zero);
            //Handles.matrix = rotationMatrix;
            //Handles.DrawWireCube(point, size);
        }

        //void DrawDisc()
        //{
        //    Handles.color = new Color(0.2f, 0.7f, 1f, 0.4f);
        //    Handles.DrawSolidDisc(pickPosition, Vector3.up, snap.magnitude * 0.3f);
        //}

        Texture2D GetPreview(Object value)
        {
            return AssetPreview.GetAssetPreview(value);
        }

        Texture2D GetThumbnail(Object value)
        {
            return AssetPreview.GetMiniThumbnail(value);
        }

        Vector3[] GetPoints()
        {
            Vector3[] result = new Vector3[size.x * size.y * size.z];

            // Offset
            Vector3 point = Vector3.zero;
            Vector3 fsize = (Vector3)size;

            int index = 0;

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    for (int z = 0; z < size.z; z++)
                    {
                        point.x = x - (fsize.x / 2) + 0.5f;
                        point.y = y - (fsize.y / 2) + 0.5f;
                        point.z = z - (fsize.z / 2) + 0.5f;
                    
                        result[index] = pickPosition + point;

                        index++;
                    }
                }
            }

            return result;
        }

        void FindData()
        {
            if (data) { return; }

            string filter = string.Format("t:{0}", typeof(LevelEditorData));
            //Debug.Log(filter);
            string[] guids = AssetDatabase.FindAssets(filter);
            
            if (null == guids || 0 == guids.Length) { return; }

            string path = AssetDatabase.GUIDToAssetPath(guids[0]);

            data = AssetDatabase.LoadAssetAtPath<LevelEditorData>(path);
        }

        public static LayerMask LayerField(string label, LayerMask layer)
        {
            int lval = InternalEditorUtility.LayerMaskToConcatenatedLayersMask(layer);

            LayerMask ltmp = EditorGUILayout.MaskField(label, lval, InternalEditorUtility.layers);

            return InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(ltmp.value);
        }
    }
}
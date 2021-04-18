using UnityEditor;
using UnityEngine;

namespace Smart
{
    using GL = GUILayout;
    using static EditorGUI;
    using static EditorGUILayout;
    using static EditorGUIUtility;

    public partial class LevelEditor : EditorWindow
    {
        const string headerStyle = "DD HeaderStyle";

        GUIStyle titleStyle
        {
            get
            {
                GUIStyle style = new GUIStyle("boldLabel")
                {
                    margin = new RectOffset(0, 0, 0, 0)
                };
                return style;
            }
        }

        void OnDrawGUI()
        {
            if (null == data) { DRAW_CREATE_DATA(); return; }

            e = Event.current;

            wideMode = true;
            
            BeginHorizontal();
            drawSettings = DrawTab(drawSettings, "Settings", "EditorSettings Icon", "ButtonLeft");
            drawSelection = DrawTab(drawSelection, "Selection", "GameObject Icon", "ButtonMid");
            drawGroups = DrawTab(drawGroups, "Groups", "LODGroup Icon", "ButtonRight");
            EndHorizontal();

            mainScroll = BeginScrollView(mainScroll);
            if (drawSettings) { DRAW_SETTINGS(); } else { ActiveField(); }
            if (drawSelection) { DRAW_SELECTED(); }
            if (drawGroups) { DRAW_GROUPS(); }
            EndScrollView();
        }

        bool DrawTab(bool toggle, string label, string icon, string style)
        {
            GUIContent content = IconContent(icon);
            content.text = label;
            
            toggle = GL.Toggle(toggle, content, style, GL.Height(singleLineHeight * 2));
            return toggle;
        }

        void DRAW_SETTINGS()
        {
            BeginVertical();
            DrawHeader("Settings", "EditorSettings Icon");

            BeginVertical("helpbox");
            SETTINGS_FIELDS();
            EndVertical();

            EndVertical();
        }

        void SETTINGS_FIELDS()
        {
            // Label
            GL.Label("Main", "boldlabel");
            // Active
            ActiveField();
            // Mode
            ModeField();
            // Paint
            paint = (Paint)EnumPopup("Paint (B)", paint);
            // Layer
            layer = LayerField("Layer", layer);
            // Height
            height = FloatField("Height(+/ -)", height);
            // Size
            SizeField();

            // Label
            GL.Label("Snap", "boldlabel");
            // Move
            move = Vector3Field("Move", move);
            // Rotate
            rotate = Vector3Field("Rotate", rotate);
            // Scale
            scale = Vector3Field("Scale", scale);
            objectScale = Vector3Field("object Scale", objectScale);
        }

        void ActiveField()
        {
            BeginChangeCheck();
            active = GL.Toggle(active, "Edit (ESC)");
            if (EndChangeCheck())
            {
                PreviewGUIChanged();
            }
        }

        void SizeField()
        {
            BeginChangeCheck();
            size = Vector3IntField("Size", size);
            if (EndChangeCheck())
            {
                size.x = Mathf.Max(0, size.x);
                size.y = Mathf.Max(0, size.y);
                size.z = Mathf.Max(0, size.z);

                PreviewGUIChanged();
            }
        }

        void ModeField()
        {
            BeginChangeCheck();
            mode = (Mode)EnumPopup("Mode (C)", mode);
            if (EndChangeCheck())
            {
                PreviewGUIChanged();
            }
        }

        void LayerField()
        {
            BeginChangeCheck();
            layer = LayerField("Layer", layer);
            if(EndChangeCheck())
            {
                Tools.visibleLayers = 1 << layer;
                SceneView.RepaintAll();
            }
        }
    }
}
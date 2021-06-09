using System;
using UnityEditor;
using UnityEngine;

namespace Smart
{
    public partial class LevelEditor : EditorWindow
    {
        //
        // Unity
        //

        [MenuItem("Tools/Smart/Level Editor")]
        static void Init()
        {
            GetWindow<LevelEditor>("Level Editor");
        }

        private void OnEnable()
        {
            AssetPreview.SetPreviewTextureCacheSize(LevelEditorGroup.maxElements + 10);

            Load();

            FindData();

            tool = Tools.current;

            SceneView.duringSceneGui += OnSceneGUI;

            Selection.selectionChanged += SelectionChanged;

            PreviewGUIChanged();
        }

        private void OnDisable()
        {
            DeletePreviews();

            Save();

            SceneView.duringSceneGui -= OnSceneGUI;

            Selection.selectionChanged -= SelectionChanged;
        }

        private void OnFocus()
        {
            tool = Tools.current;

            isFocused = true;

            FindData();
        }

        private void OnLostFocus()
        {
            isFocused = false;
        }

        private void OnHierarchyChange()
        {
            if (isFocused)
            {
                Repaint();
            }
        }

        private void OnGUI()
        {
            OnDrawGUI();
        }

        //
        // Methods
        //  

        void OnSceneGUI(SceneView view)
        {
            e = Event.current;
            
            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.A)
            {
                for (int i = 0; i < previews.Count; i++)
                    previews[i].transform.Rotate(0f,15f,0);
            }
            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.D)
            {
                for (int i = 0; i < previews.Count; i++)
                    previews[i].transform.Rotate(0f,-15f,0f);
            }

            this.view = view;
            
            ControlActive();

            if (!active) { return; }

            ControlMode();
            ControlTools();
            ControlPaint();
            ControlHeight();
            SceneGUIButtons();
            EditUpdate();
            
            if(isFocused)
            {
                Repaint();
            }
            
            
            
        }
        
        

        void SelectionChanged()
        {
            transforms = Selection.transforms;

            if(null != transforms && transforms.Length > 0)
            {
                for(int i = 0; i < transforms.Length; i++)
                {
                    transforms[i].hasChanged = false;
                }
            }
        }
    }
}
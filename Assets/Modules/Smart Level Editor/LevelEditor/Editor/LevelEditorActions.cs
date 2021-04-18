using UnityEditor;
using UnityEngine;
using Rand = UnityEngine.Random;
using Object = UnityEngine.Object;

namespace Smart
{
    public partial class LevelEditor
    {
        Object GetFirst()
        {
            if(selected.Count <= 0) { return null; }

            return selected[0];
        }

        Object GetObject()
        {
            if (selected.Count == 0) { return null; }

            if (selected.Count == 1) { return selected[0]; }

            int index = Rand.Range(0, selected.Count);

            return selected[index];
        }

        void Create()
        {
            if (paint == Paint.Brush)
            {
                // Exit
                if (pick.Overlap(pickPosition, size, previews.ToArray(), layer)) { return; }
            }

            // 1 - We are going to create the objects indicated by our previews
            // 2 - Why not just removed them from the preview list? they are already there
            // 3 - Because we need to be able to use Ctrl-z, to undo

            GameObject result;
            GameObject prefab;
            for (int i = 0; i < previews.Count; i++)
            {
                prefab = PrefabUtility.GetCorrespondingObjectFromSource(previews[i]);

                if(null == prefab) { continue; }

                result = PrefabUtility.InstantiatePrefab(prefab) as GameObject;

                if(null == result) { continue; }

                result.transform.SetParent(GetRoot());
                result.transform.position = previews[i].transform.position;
                result.transform.localScale = previews[i].transform.localScale;
                result.transform.rotation = previews[i].transform.rotation;
                Undo.RegisterCreatedObjectUndo(result, "LeveEditor.CreateObject");
            }
            // 4 - lastly we are going to create the previews again so we can randomize the objs
            CreatePreviews();
        }

        Transform root;

        Transform GetRoot()
        {
            root = Selection.activeTransform;
            // E x i t
            if (root) { return root; }
            // Create new section
            GameObject newSection = new GameObject("[NewSection]");
            // Get Transform
            root = newSection.transform;
            // Register undo
            Undo.RegisterCreatedObjectUndo(newSection, "Smart.Root.GO.Created");
            // Select transform
            Selection.activeTransform = root;

            return root;
        }
    
        void Delete()
        {
            targets = pick.GetGameObjects(pickPosition, size, layer);

            if(null == targets) { return; }

            GameObject prefab;

            for(int i = 0; i < targets.Length; i++)
            {
                //if (!IsPrefabAndSelected(targets[i])) { continue; }
                if(null == targets[i]) { continue; }

                prefab = PrefabUtility.GetOutermostPrefabInstanceRoot(targets[i]);

                if(prefab)
                {
                    Undo.DestroyObjectImmediate(prefab);
                    prefab = null;
                }
                else
                {
                    Undo.DestroyObjectImmediate(targets[i]);
                    targets[i] = null;
                }
            }
        }
    }
}
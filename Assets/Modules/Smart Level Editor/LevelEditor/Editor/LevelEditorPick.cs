
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace Smart
{
    public partial class LevelEditor
    {
        public class Pick
        {
            //
            // Variables
            //

            private float distance;
            private float height;
            private Vector3 mouse;
            private UnityEngine.Plane plane;
            private Bounds bounds;
            private Transform[] transforms;
            private List<GameObject> gameObjects;

            private Ray ray;
            private RaycastHit rayInfo;

            public Event e;
            public Camera camera;
        
            //
            // Properties
            //

            public bool Hit
            { get; protected set; }

            public GameObject Target
            { get; protected set; }

            public Vector3 Position
            { get; protected set; }

            public Vector3 Point
            { get; protected set; }
            
            public Vector3 HitNormal
            { get; protected set; }

            public Vector3 TargetPosition
            {
                get { return Target.transform.position; }
            }

            //
            // Methods
            //

            public void Set(float height, Event e, Camera camera)
            {
                this.height = height;
                this.e = e;
                this.camera = camera;
            }

        
            public void UpdateRay()
            {
                mouse = e.mousePosition;
                mouse.y = camera.pixelHeight - mouse.y;
                ray = camera.ScreenPointToRay(mouse);
            }

            public void UpdatePosition()
            {
                
                
                //plane = new UnityEngine.Plane(Vector3.up, new Vector3(0, height, 0));
                //plane.Raycast(ray, out distance);
                //Position = ray.GetPoint(distance);
                //Position = new Vector3(2, 0, 0);

                Ray ray = HandleUtility.GUIPointToWorldRay (Event.current.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    //Debug.Log($"hit on {hit.transform.name}");
                    Position = hit.transform.position;
                    HitNormal = hit.normal;
                }
                

                //Position = ray.GetPoint(distance);
            }

            public void UpdateCast()
            {
                Hit = Physics.Raycast(ray, out rayInfo);
                Point = rayInfo.point;
                Target = rayInfo.collider ? rayInfo.collider.gameObject : null;
            }

            public void Update()
            {
                UpdateRay();
                UpdatePosition();
                UpdateCast();
            }

            public void GetGameObject(LayerMask layer)
            {
                Target = HandleUtility.PickGameObject(e.mousePosition, true);

                if(!WorkingLayers(Target, layer))
                {
                    Target = null;
                }
            }

            public bool Overlap(Vector3 center, Vector3 size, GameObject[] previews, LayerMask layer)
            {
                transforms = FindObjectsOfType<Transform>();

                RemovePreviews(previews);

                bounds = new Bounds(center, size);
                
                for (int i = 0; i < transforms.Length; i++)
                {
                    if (null == transforms[i])
                    {
                        continue;
                    }

                    if (!WorkingLayers(transforms[i].gameObject, layer))
                    {
                        continue;
                    }

                    if (bounds.Contains(transforms[i].position)) { return true; }
                }

                return false;
            }

            void RemovePreviews(GameObject[] previews)
            {
                Transform[] pt;

                for (int i = 0; i < transforms.Length; i++)
                {
                    for (int j = 0; j < previews.Length; j++)
                    {
                        pt = previews[j].GetComponentsInChildren<Transform>();

                        for (int w = 0; w < pt.Length; w++)
                        {
                            if (transforms[i] == pt[w])
                            {
                                transforms[i] = null;
                            }
                        }
                    }
                }
            }

            // No need to remove previews here, since this func is used in the delte mode
            // at that point we are not rendering previews
            public GameObject[] GetGameObjects(Vector3 center, Vector3 size, LayerMask layer)
            {
                gameObjects = new List<GameObject>();

                transforms = FindObjectsOfType<Transform>();

                bounds = new Bounds(center, size);
            
                for (int i = 0; i < transforms.Length; i++)
                {
                    if (!WorkingLayers(transforms[i].gameObject, layer))
                    {
                        continue;
                    }

                    if (bounds.Contains(transforms[i].position))
                    {
                        gameObjects.Add(transforms[i].gameObject);
                    }
                }

                return gameObjects.ToArray();
            }

            bool WorkingLayers(GameObject gameObject, LayerMask layer)
            {
                return layer == (layer | (1 << gameObject.layer));
            }
        }
    }
}
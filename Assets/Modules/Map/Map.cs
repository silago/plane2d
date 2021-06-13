using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Events;
using Modules.Common;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;
namespace Modules.Map
{
    public class Map : BaseScreen
    {
        [SerializeField]
        private MapElementUI mapElementPrefab;
        [SerializeField]
        private float scale = 1;
        [SerializeField]
        private Transform target; //player
        [SerializeField]
        int clearRadius = 100;
        [SerializeField]
        private Image mask;
        [SerializeField]
        private Transform Container;
        [SerializeField]
        private float scaleSpeed = 0.1f;
        private List<(Transform, RectTransform)> _mapElements = new List<(Transform, RectTransform)>();
        private Vector3 WorldCenter => Vector3.zero; 
        private static Vector3 MapCenter => Vector3.zero;
        private Vector2 _maskScale;
        private Rect MapBounds()
        {
            var r = new Rect {
                position = Vector2.zero//,
            };

            var size = new Vector2(
                mask.rectTransform.sizeDelta.x * mask.rectTransform.localScale.x,
                mask.rectTransform.sizeDelta.y * mask.rectTransform.localScale.y
            );
            size-= (new Vector2(Screen.width , Screen.height));
            r.min = -size / 2;
            r.max = size / 2;
            return r;
        }
        private void Awake()
        {
            _maskScale = mask.rectTransform.localScale;
            this.Subscribe<MapElement>(OnMapElement);
            base.Awake();
        }

        private Vector3 ScaleToMask(Vector3 from) =>
            new Vector3(
                @from.x / _maskScale.x,
                @from.y / _maskScale.y
            );
        private void Start()
        {
            
            var x = MapBounds();
           StartCoroutine(UpdateMap());
        }

        private void OnMapElement(MapElement obj)
        {
            var item = Instantiate(mapElementPrefab, Container);
            item.transform.SetAsFirstSibling();
            item.text.text = obj.caption; 
            item.image.sprite = obj.sprite;
            item.image.SetNativeSize();
            item.gameObject.SetActive(true);
            _mapElements.Add((obj.transform,item.image.rectTransform));
        }

        
        protected override ScreenId ScreenId => ScreenId.Map;
        private void Update()
        {
            foreach (var (worldTransform, mapTransform) in _mapElements)
            {
                //relative to player
                var dir = worldTransform.position - target.position;
                mapTransform.anchoredPosition = MapCenter + (dir * scale);
            }
            var maskPosition = Vector3.zero - target.position;
            var pos = MapCenter + (maskPosition) * scale;
            mask.rectTransform.anchoredPosition = pos;

            var scrollDelta = Input.mouseScrollDelta.y;
            if (scrollDelta != 0)
            {
                scrollDelta *= scaleSpeed;
                var newScale  = Container.localScale + new Vector3(scrollDelta, scrollDelta);
                if (newScale.x > 0.35f && newScale.x < 10f)
                {
                    Container.localScale = newScale;
                }
            }
            if (!MapBounds().Contains(mask.rectTransform.anchoredPosition))
            {
                Debug.Log("OUT OF BOUNDS");
            }

            ProcessDrag();
        }

        [SerializeField]
        private float dragSpeed = 1f;
        [SerializeField]
        private float maxDragSpeed = 7f;
        private void ProcessDrag()
        {
            if (Input.GetMouseButton(0))
            {
                var delta = new Vector3(
                    Input.GetAxis("Horizontal"),
                    Input.GetAxis("Vertical")
                ) * dragSpeed;
                delta = Vector3.ClampMagnitude(delta,maxDragSpeed);
                Container.localPosition += delta;
            }
        }

        IEnumerator UpdateMap()
        {
            var pivot = mask.rectTransform.pivot;
            
            // we have to consider scale
            var center = new Vector2(
                mask.sprite.texture.width * pivot.x,
                mask.sprite.texture.height * pivot.y
            );
            int radius = (int)(clearRadius / _maskScale.x); 
            var color = new Color(0, 0, 0, 0);
            Texture2D texture = mask.mainTexture as Texture2D;
            if (texture == null) yield break;
            for (;;)
            {
                var pos  = center  + (Vector2) ScaleToMask(target.position * scale);
               DrawCircle(texture, color, (int)pos.x, (int)pos.y,radius);
               texture.Apply();
               yield return new WaitForSeconds(0.3f);
            } 
        }
        
        public static float PointDistance(int x, int y)
        {
          float num1 = - x;
          float num2 = - y;
          return (float) Math.Sqrt((double) num1 * (double) num1 + (double) num2 * (double) num2);
        }
        
        public void DrawCircle(Texture2D tex, Color color, int x, int y, int radius = 3)
        {
            var c = color;
            float rSquared = radius * radius;
            for (int u = x - radius; u < x + radius + 1; u++)
            for (int v = y - radius; v < y + radius + 1; v++)
            {

                var d = (x - u) * (x - u) + (y - v) * (y - v);
                if ( d < rSquared)
                {
                    var p = tex.GetPixel(u, v);
                    c.a = Mathf.Min(Mathf.Pow(d/rSquared,5),p.a);
                    
                    tex.SetPixel(u, v, c);
                }
            }
        } 
    }

}

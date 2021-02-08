using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Modules.Environment
{
    [RequireComponent(typeof(Camera))]
    public class ScrollingBackground : MonoBehaviour 
    {
        [SerializeField] private float speed = 1f;
        private Vector3 _startPos;
        protected List<Vector3> points = new List<Vector3>();
        
        [SerializeField]
        SpriteRenderer bg;
        //upper left, upper right; lower left, lower right;
        private SpriteRenderer _ul, _ur, _ll, _lr;

        private Vector3 _halfDist;
        private Vector3 _bgSize;
        private Camera _cam;

        private bool toStop = false;

        void Start() {
            var ul = bg;
            var ur = Instantiate(bg,transform);
            var ll = Instantiate(bg,transform);
            var lr = Instantiate(bg,transform);

            _cam = Camera.main;
            _bgSize = bg.size;//cam.ScreenToWorldPoint(_size);
            _halfDist = _bgSize * 0.5f;
            ul.transform.position+= new Vector3(-_halfDist.x,  _halfDist.y); 
            ur.transform.position+= new Vector3( _halfDist.x,  _halfDist.y); 
            ll.transform.position+= new Vector3(-_halfDist.x, -_halfDist.y); 
            lr.transform.position+= new Vector3( _halfDist.x, -_halfDist.y);
            
            _startPos = transform.position = _cam.transform.position;
        }
    
        Vector3 GetDirection() {
            //var dist = cam.transform.position - /*transform.position*/ startPos;
            //var dist = cam.transform.position - transform.position;// startPos;
            var dist = _cam.transform.position -  transform.position;
            //dist *= speed;
            var x = dist.x > _halfDist.x ? 1 : dist.x < -_halfDist.x ? -1 : 0;
            var y = dist.y > _halfDist.y ? 1 : dist.y < -_halfDist.y ? -1 : 0;
            return new Vector3(
                x,
                y,
                0
            );
        }
        //well, we may move UpdatePos and LateUpdate stuff to the different components 

        void UpdatePos(float dt)
        {
            var camPosition = _cam.transform.position;
            var offset = camPosition - _startPos;
            var taret = _startPos + (offset) * speed;
            
            //taret = camPosition * 0.3f;
            taret.z = 2;
            transform.position = taret;
        }

        private void LateUpdate()
        {
            UpdatePos(Time.deltaTime);
            if (toStop) return;
         
            var direction = GetDirection(); 
            if (direction == Vector3.zero) {
                return;
            }
            var offset = new Vector3(
                _bgSize.x * direction.x,
                _bgSize.y * direction.y
            )*(1+speed);
            _startPos = _startPos + offset ;//+ o*2  ;  
        }

        void S( Vector3 s, string name = "")
        {
            points.Add(_startPos);
            var g = Instantiate(this);
            g.enabled = false;
            /*
            foreach (Transform item in g.transform)
            {
                if (g.bg.gameObject != item.gameObject)
                {
                    Destroy(item.gameObject);
                }
            }
            */
            
            g.transform.position = s;
            if (name != "")
            {
                g.name = name;
            }
        }

        private void OnDrawGizmos()
        {
            foreach (var point in points)
            {

                Gizmos.color = Color.red;
            Gizmos.DrawSphere(point, 1);
            }
        }
    }
}

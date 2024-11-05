using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

namespace XMeta.Foundation.UI.TaskUI
{
    public class TutorialArrows:MonoBehaviour
    {
        [SerializeField]
        private Vector3 targetPosition;

        [SerializeField]
        private float offsetHeight = 1f;

        public float top = 0.1f;
        public float bottom = 0.1f;
        public float left = 0.1f;
        public float right = 0.1f;
        
        /// <summary>
        /// 距离 
        /// </summary>
        /// <returns></returns>
        public float distance = 30; 
        
        
        
        public void SetTargetPosition(Vector3 position, float height)
        {
            targetPosition = position;
            offsetHeight = height;
        }

        private CinemachineBrainEvents _cinemachineBrainEvents;
        private void Start()
        {
            if (Camera.main is { } c)
            {
                _cinemachineBrainEvents = c.GetComponent<CinemachineBrainEvents>();
                if (_cinemachineBrainEvents is not null)
                {
                    _cinemachineBrainEvents.BrainUpdatedEvent.AddListener(CameraUpdated);
                }
            }
        }

        private void CameraUpdated(CinemachineBrain arg0)
        {
            UpdatePosition();
        }

        // private void Update()
        // {
        //     var go = GameObject.Find("Stickman-Casual-Man-2 (1)");
        //     _position = go.transform.position;
        //     // targetPosition = Vector3.Lerp(targetPosition, _position,0.5f);
        //      SetTargetPosition(go.transform.position, 1f);
        //     
        // }
        
        

        
        private void LateUpdate()
        {
            if (_cinemachineBrainEvents is null)
            {
                UpdatePosition();
            }
        }

        private void OnDestroy()
        {
            if (_cinemachineBrainEvents is not null)
            {
                _cinemachineBrainEvents.BrainUpdatedEvent.RemoveListener(CameraUpdated);
            }
        }


        private void UpdatePosition()
        {
            if (!enabled) return;
            // 世界坐标转换为屏幕坐标
            if (Camera.main is {} c)
            {
                var screenPosition = c.WorldToViewportPoint(targetPosition);
                var cameraEulerAnglesX = c.transform.rotation.eulerAngles.x;
                if (screenPosition.x > left && screenPosition.x < 1 - right && screenPosition.y > bottom && screenPosition.y < 1 - top)
                {
                    transform.position = targetPosition + new Vector3(0, offsetHeight, 0);
                    transform.rotation = Quaternion.Euler(cameraEulerAnglesX,0,0);// Quaternion.identity;
                }
                else
                {
                    var rect = new Rect(left, bottom, 1 - left - right, 1 - top - bottom);
                    var pos = GetIntersectionWithRectangle(rect, screenPosition);
                    transform.position = c.ViewportToWorldPoint(new Vector3(pos.x, pos.y, distance));
                    
                    
                    // //x y 相对于屏幕中心的旋转
                    var angle = Mathf.Atan2(Mathf.Clamp(screenPosition.y,0f,1f)-0.5f, Mathf.Clamp(screenPosition.x,0f,1f) - 0.5f) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(cameraEulerAnglesX, 0, angle+90);
                }
            }
        }
        
        
        public static Vector2 GetIntersectionWithRectangle(Rect rectangle, Vector2 outsidePoint)
        {
            Vector2 center = rectangle.center;

            // 计算斜率
            float slope = (outsidePoint.y - center.y) / (outsidePoint.x - center.x);

            // 计算y轴截距
            float yIntercept = center.y - slope * center.x;

            // 计算与矩形四个边界的交点
            float leftIntersectionY = slope * rectangle.xMin + yIntercept;
            float rightIntersectionY = slope * rectangle.xMax + yIntercept;
            float topIntersectionX = (rectangle.yMax - yIntercept) / slope;
            float bottomIntersectionX = (rectangle.yMin - yIntercept) / slope;

            var returnVector = center;
            var minDistance = float.MaxValue;
            

            // 判断哪个交点在矩形内
            if (rectangle.yMin <= leftIntersectionY && leftIntersectionY <= rectangle.yMax)
            {
                var v = new Vector2(rectangle.xMin, leftIntersectionY);
                MinDistance(outsidePoint, v, ref minDistance, ref returnVector);
            }

            if (rectangle.yMin <= rightIntersectionY && rightIntersectionY <= rectangle.yMax)
            {
                var v = new Vector2(rectangle.xMax, rightIntersectionY);
                MinDistance(outsidePoint, v, ref minDistance, ref returnVector);
            }

            if (rectangle.xMin <= topIntersectionX && topIntersectionX <= rectangle.xMax)
            {
                var v =  new Vector2(topIntersectionX, rectangle.yMax);
                MinDistance(outsidePoint, v, ref minDistance, ref returnVector);
            }

            if (rectangle.xMin <= bottomIntersectionX && bottomIntersectionX <= rectangle.xMax)
            {
                var v = new Vector2(bottomIntersectionX, rectangle.yMin);
                MinDistance(outsidePoint, v, ref minDistance, ref returnVector);
            }
            // 如果没有找到交点，返回矩形中心点
            return returnVector;
        }

        private static void MinDistance(Vector2 outsidePoint, Vector2 v, ref float minDistance, ref Vector2 returnVector)
        {
            var sqrM = (outsidePoint - v).sqrMagnitude;
            if (sqrM < minDistance)
            {
                minDistance = sqrM;
                returnVector = v;
            }
        }
        
    }
    
    
    
}
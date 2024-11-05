using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace XMeta.Foundation.UI.TaskUI
{
    public static class AllUguiTutorialAnchor
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Reset()
        {
            Anchors.Clear();
            Layers.Clear();
            OnChange = null;
        }
        
        private static readonly List<UguiTutorialAnchor> Anchors = new List<UguiTutorialAnchor>();
        private static readonly List<UguiTutorialLayer> Layers = new List<UguiTutorialLayer>();

        public static event Action OnChange;


        public static void Add(UguiTutorialAnchor anchor)
        {
            Anchors.Add(anchor);
            OnChange?.Invoke();
        }

        public static void Remove(UguiTutorialAnchor anchor)
        {
            Anchors.Remove(anchor);
            OnChange?.Invoke();
        }
        
        
        public static void Add(UguiTutorialLayer layer)
        {
            Layers.Add(layer);
            OnChange?.Invoke();
        }
        
        public static void Remove(UguiTutorialLayer layer)
        {
            Layers.Remove(layer);
            OnChange?.Invoke();
        }


        public static void UpdateAnchorName()
        {
            OnChange?.Invoke();
        }
        
        
        public static void UpdateRectTransform()
        {
            OnChange?.Invoke();
        }
        


        public static UguiTutorialAnchor GetAnchor(string[] names)
        {
            UguiTutorialAnchor anchor = null;
            var index = -1;


            foreach (var a in Anchors)
            {
                if (!a) continue;
                var i = Array.IndexOf(names, a.AnchorName);
                if (i != -1)
                {
                    if (anchor == null) anchor = a;
                    else
                    {
                        // 名字匹配的UI中，选择sortingOrder最大的或 sortingOrder相同的选择后面的
                        if (a.sortingOrder > anchor.sortingOrder)
                        {
                            anchor = a;
                        }
                        else if (a.sortingOrder == anchor.sortingOrder && i > index)
                        {
                            anchor = a;
                        }
                    }

                    index = i;
                }
            }

            if (anchor!=null)
            {
                for (var i = 0; i < Layers.Count; i++)
                {
                    var layer = Layers[i];
                    if (!layer) continue;
                    var screenPoint = RectTransformUtility.WorldToScreenPoint(null,anchor.transform.position);
                    var b = RectTransformUtility.RectangleContainsScreenPoint(layer.transform as RectTransform, screenPoint);
                    if (b)
                    {
                        var v = CompareSiblingIndex(anchor.transform , layer.transform );
                        if (v < 0) return null;
                    }
                }
                
            }
            return anchor;
        }


        public static int CompareSiblingIndex(Transform a, Transform b)
        {
            var aList = ListPool<Transform>.Get();
            var bList = ListPool<Transform>.Get();
            AllParent(a, aList);
            AllParent(b, bList);
            var value = 0;
            for (var i = 0; i < aList.Count; i++)
            {
                if (i >= bList.Count) break;
                if (aList[i] != bList[i])
                {
                    value = aList[i].GetSiblingIndex().CompareTo(bList[i].GetSiblingIndex());
                    break;
                }
            }
            
            ListPool<Transform>.Release(aList);
            ListPool<Transform>.Release(bList);
            return value;
        }

        private static void AllParent(Transform a, List<Transform> parents)
        {
            parents.Add(a);
            var tfParent = a.parent;
            if (tfParent != null)
            {
                parents.Add(tfParent);
                while (tfParent.parent != null)
                {
                    tfParent = tfParent.parent;
                    parents.Add(tfParent);
                }
            }
            parents.Reverse();
        }
        
    }
}
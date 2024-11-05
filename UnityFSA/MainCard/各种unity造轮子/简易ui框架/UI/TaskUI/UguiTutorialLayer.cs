using UnityEngine;

namespace XMeta.Foundation.UI.TaskUI
{
    
    /// <summary>
    /// UI的遮挡层
    /// </summary>
    public class UguiTutorialLayer:MonoBehaviour
    {
        
        
        
        public void OnEnable()
        {
            AllUguiTutorialAnchor.Add(this);
        }
        
        public void OnDisable()
        {
            AllUguiTutorialAnchor.Remove(this);
        }
        
        public void OnRectTransformDimensionsChange()
        {
            AllUguiTutorialAnchor.UpdateRectTransform();
        }
    }
}
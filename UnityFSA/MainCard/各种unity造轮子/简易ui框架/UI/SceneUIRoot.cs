using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace XMeta.Foundation.UI
{
    public class SceneUIRoot:MonoBehaviour
    {
        
        public enum SceneUIRootSpace
        {
            None,
            Main,
            Overlay
        }

        public SceneUIRootSpace space = SceneUIRootSpace.Main;
        
        
        public void Awake()
        {
            if (space == SceneUIRootSpace.Main)
            {
                SceneUIManager.MainSpace.InitRoot(this.transform);
            }
            else
            {
                SceneUIManager.OverlaySpace.InitRoot(this.transform);
            }
            
        }
    }
}
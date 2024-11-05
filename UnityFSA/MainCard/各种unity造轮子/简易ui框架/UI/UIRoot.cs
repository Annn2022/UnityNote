using UnityEngine;

namespace XMeta.Foundation.UI
{
    public class UIRoot:MonoBehaviour
    {
        public enum UIRootSpace
        {
            Main,
            Second
        }
        
        public UIRootSpace space = UIRootSpace.Main;
        
        public void Awake()
        {
            if (space == UIRootSpace.Main)
            {
                UIManager.MainSpace.InitRoot(transform);
            }
            else
            {
                UIManager.SecondSpace.InitRoot(this.transform);
            }
           
        }
    }
}
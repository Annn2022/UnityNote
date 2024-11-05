using UnityEngine;

namespace XMeta.Foundation.UI
{
    public class SceneUIManager
    {
        /// <summary>
        /// UI根节点
        /// </summary>
        private Transform _root;
        private static SceneUIManager _mainSpace;
        public static SceneUIManager MainSpace
        {
            get { return _mainSpace ??= new SceneUIManager(); }
        }
        private static SceneUIManager _overlaySpace;
        public static SceneUIManager OverlaySpace
        {
            get { return _overlaySpace ??= new SceneUIManager(); }
        }
        public void InitRoot(Transform root)
        {
            _root = root;
        }
        public void Add(GameObject uiInst)
        {
            uiInst.transform.SetParent(_root);
        }
    }
}


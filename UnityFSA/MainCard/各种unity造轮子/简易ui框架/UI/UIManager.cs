using System.Collections.Generic;
using UnityEngine;

namespace XMeta.Foundation.UI
{
    public class UIManager
    {
        private Transform _root;

        private static UIManager _mainSpace;

        public static UIManager MainSpace
        {
            get { return _mainSpace ??= new UIManager(); }
        }

        private static UIManager _secondSpace;

        public static UIManager SecondSpace
        {
            get { return _secondSpace ??= new UIManager(); }
        }
        
        public void InitRoot(Transform root)
        {
            _singleton.Clear();
            _root = root;
        }

        public void Add(GameObject uiInst)
        {
            uiInst.transform.SetParent(_root, false);
        }
        
        private Dictionary<string, GameObject> _singleton = new Dictionary<string, GameObject>();
        
        
        public Dictionary<string, GameObject> Singleton => _singleton;
        
    }
}
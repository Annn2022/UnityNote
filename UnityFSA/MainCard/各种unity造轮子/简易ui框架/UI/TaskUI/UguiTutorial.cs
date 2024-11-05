using System;
using UnityEngine;

namespace XMeta.Foundation.UI.TaskUI
{
    public class UguiTutorial:MonoBehaviour
    {
        /// <summary>
        /// 指示类UI 
        /// </summary>
        public GameObject hintUI;

        private string[] _anchorName;


        public void Start()
        {
            AllUguiTutorialAnchor.OnChange += OnChange;
            SetHintUIPosition();
        }

        /// <summary>
        /// 场景中的UI发生变化 (打开或关闭)
        /// </summary>
        private void OnChange()
        {
            if (_anchorName is { Length: > 0 })
            {
                _currentAnchor = AllUguiTutorialAnchor.GetAnchor(_anchorName);
                SetHintUIPosition();
            }
        }
        
        public void SetTutorialAnchor(string[] anchorName)
        {
            if (anchorName != _anchorName)
            {
                _anchorName = anchorName;
                _currentAnchor = AllUguiTutorialAnchor.GetAnchor(_anchorName);
            }
           
            SetHintUIPosition();
        }

        // public void Hide()
        // {
        //     _anchorName = null;
        // }

        private UguiTutorialAnchor _currentAnchor;

        private void SetHintUIPosition()
        {
            if (_currentAnchor != null)
            {
                if (hintUI != null)
                {
                    hintUI.gameObject.SetActive(true);
                    hintUI.transform.position = _currentAnchor.transform.position;
                }
            }
            else
            {
                if (hintUI != null)
                {
                    hintUI.gameObject.SetActive(false);
                }
            }
        }
    }
}
using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace XMeta.Foundation.UI
{
    public class SceneUI : MonoBehaviour
    {
        // public GameObject sceneUI;
        // private GameObject _sceneUIInstance;
        public bool lookAtCamera = false;
        public Transform followTarget;
        public Vector3 offset = Vector3.zero;
        public SceneUIRoot.SceneUIRootSpace space = SceneUIRoot.SceneUIRootSpace.None;
        // private void Start()
        // {
        //     if (space == SceneUIRoot.SceneUIRootSpace.None) return;
        //     if (space == SceneUIRoot.SceneUIRootSpace.Main)
        //     {
        //         SceneUIManager.MainSpace.Add(gameObject);
        //     }
        //     else
        //     {
        //         SceneUIManager.OverlaySpace.Add(gameObject);
        //     }
        //
        //     if (followTarget)
        //     {
        //         transform.position = followTarget.position + offset;
        //         transform.rotation = followTarget.rotation;
        //     }
        // }

        private void OnEnable()
        {
            if (space == SceneUIRoot.SceneUIRootSpace.None) return;
            if (space == SceneUIRoot.SceneUIRootSpace.Main)
            {
                SceneUIManager.MainSpace.Add(gameObject);
            }
            else
            {
                SceneUIManager.OverlaySpace.Add(gameObject);
            }

            if (followTarget)
            {
                transform.position = followTarget.position + offset;
                transform.rotation = followTarget.rotation;
            }
        }

        private void LateUpdate()
        {
            if (followTarget != null)
            {
                transform.position = followTarget.position + offset;
                transform.rotation = followTarget.rotation;
            }

            if (lookAtCamera)
            {
                if (Camera.main != null) transform.rotation = Camera.main.transform.rotation;
            }
        }
    }

    public static class XuiExtSceneUI
    {
        public static Xui Follow(this Xui xui, Transform target)
        {
            if (xui.SceneUI != null)
            {
                xui.SceneUI.followTarget = target;
            }

            return xui;
        }

        public static Xui LookAtCamera(this Xui xui, bool value)
        {
            if (xui.SceneUI != null)
            {
                xui.SceneUI.lookAtCamera = value;
            }

            return xui;
        }


        public static Xui Offset(this Xui xui, Vector3 value)
        {
            if (xui.SceneUI != null)
            {
                xui.SceneUI.offset = value;
            }

            return xui;
        }
        
        
    }
}
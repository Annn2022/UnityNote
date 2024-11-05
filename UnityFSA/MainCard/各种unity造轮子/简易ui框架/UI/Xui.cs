using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitMotion;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace XMeta.Foundation.UI
{
    public struct Xui
    {
        // [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        // static void Init()
        // {
        //     _singleton = new Dictionary<string, GameObject>();
        // }
        //
        // private static Dictionary<string, GameObject> _singleton = new();
        
        
        public GameObject Ui{ private set; get; }
        private string UiName { set; get; }

        private XuiComponent<SceneUI> _sceneUI;
        public SceneUI SceneUI => XuiComponent<SceneUI>.GetComponet(Ui, ref _sceneUI);

        private XuiComponent<CanvasGroup> _canvasGroup;
        
        public CanvasGroup CanvasGroup => XuiComponent<CanvasGroup>.GetComponet(Ui, ref _canvasGroup);
 
        public static Xui Bind(GameObject ui)
        {
            return new Xui()
            {
                Ui = ui
            };
        }

        public Xui AddToSceneMainSpace()
        {
            SceneUIManager.MainSpace.Add(Ui);
            return this;
        }
        
        public Xui AddToSceneOverlaySpace()
        {
            SceneUIManager.OverlaySpace.Add(Ui);
            return this;
        }
        
        public Xui AddSceneUIComponent()
        {
            if (SceneUI == null)
            {
                var anchorSceneUI = Ui.AddComponent<SceneUI>();
                _sceneUI.Component = anchorSceneUI;
                _sceneUI.Mode = XuiComponentMode.Existing;
            }
            return this;
        }

        public Xui AddUIComponent()
        {
            UIManager.MainSpace.Add(Ui);
            return this;
        }
        public void ShowPanel()
        {
            Ui.transform.SetAsLastSibling();
            Ui.SetActive(true);
        }
        
        public void HidePanel()
        {
            Ui.SetActive(false);
        }

        public void Dispose()
        {
            Object.Destroy(Ui);
        }
        
        public void ReleaseToPool()
        {
            if (!string.IsNullOrEmpty(UiName))
            {
                AssetLoader.ReleaseInstanceToPool(UiName, Ui);
            }
        }
        
        public Xui SetPosition(Vector3 position)
        {
            Ui.transform.position = position;
            return this;
        }
        
        
        public async UniTask<Xui> FadeIn(float t, Ease ease = Ease.Linear)
        {
            var canvasGroup = CanvasGroup;
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0;
                var handle = LMotion.Create(0f, 1f, t).WithEase(ease).Bind(value =>
                {
                    canvasGroup.alpha = value;

                });
                await handle;
            }
            return this;
        }

        public async UniTask<Xui> FadeOut(float t, Ease ease = Ease.Linear)
        {
            var canvasGroup = CanvasGroup;
            if (canvasGroup != null)
            {
                var handle = LMotion.Create(1f, 0f, t).WithEase(ease).Bind(value =>
                {
                    canvasGroup.alpha = value;

                });
                await handle;
            }
            return this;
        }

        
        /// <summary>
        /// 从对象池中获取实例并绑定到Xui
        /// </summary>
        /// <param name="uiName"></param>
        /// <returns></returns>
        public static async UniTask<Xui> LoadFromPoolToBind(string uiName)
        {
            var ui = await AssetLoader.GetInstanceFromPool(uiName);
            if (ui == null)
            {
                return Bind((GameObject) null) ;
            }
            
            var xui = Bind(ui);
            xui.UiName = uiName;
            return xui;
        }
        
        
        
        public static async UniTask<Xui> LoadToBind(string uiName)
        {
            var ui = await Addressables.LoadAssetAsync<GameObject>(uiName);
            if (ui == null)
            {
                Debug.LogError($"UI {uiName} not found");
                return Bind((GameObject) null) ;
            }
            var uiInstance = Object.Instantiate(ui);
            return Bind(uiInstance);
        }

        public static async UniTask<Xui> LoadToBindSingleton(string uiName)
        {
            if (UIManager.MainSpace.Singleton.TryGetValue(uiName, out var go))
            {
                return Bind(go);
            }
            var ui = await Addressables.LoadAssetAsync<GameObject>(uiName);
            if (ui == null)
            {
                Debug.LogError($"UI {uiName} not found");
                return Bind((GameObject) null) ;
            }
            else
            {
                var uiInstance = Object.Instantiate(ui);
                UIManager.MainSpace.Singleton.Add(uiName, uiInstance);
                return Bind(uiInstance);
            
            }
        }
        
        public T Component<T>() where T: Component
        {
            if (!Ui) return null;
            return Ui.GetComponent<T>();
        }
        
    }

    public struct XuiComponent<T> where T: Component
    {
        public XuiComponentMode Mode;
        public T Component;

        
        public static T GetComponet(GameObject go, ref XuiComponent<T> component)
        {
            if (component.Mode == XuiComponentMode.Undefined)
            {
                if (go.TryGetComponent<T>(out var ui))
                {
                    component.Component = ui;
                    component.Mode = XuiComponentMode.Existing;
                }
                else
                {
                    component.Mode = XuiComponentMode.Null;
                }
            }

            return component.Component;
        }
    }

    public enum XuiComponentMode
    {
        Undefined = 0,
        Null,
        Existing,
    }
}
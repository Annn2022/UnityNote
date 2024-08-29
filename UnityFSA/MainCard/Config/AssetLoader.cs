using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace XMeta.Foundation.Config
{
    /// <summary>
    /// 资源管理。加载资源和对象池
    /// </summary>
    public static class AssetLoader
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()
        {
            _gameObjectPool = new Dictionary<int, GameObjectPool>();
            _gameObjectPoolByName = new Dictionary<string, GameObjectPool>();
            _gameObjectComponentPool = new Dictionary<int, GameObjectComponentPool>();
            
        }

        private static Dictionary<int, GameObjectPool> _gameObjectPool;
        private static Dictionary<int, GameObjectComponentPool> _gameObjectComponentPool;
        private static Dictionary<string, GameObjectPool> _gameObjectPoolByName;

        public static async UniTask<T> GetInstanceFromPool<T>(int id) where T : Component
        {
            if (_gameObjectComponentPool.TryGetValue(id, out var goPool) && goPool is GameObjectComponentPool<T> cPool)
            {
                return cPool.GetInstance();
            }
            else
            {
                var config = ConfigManager.GetObjectConfig(id);
                if (config is IConfigAsset configAsset)
                {
                    var go = await Addressables.LoadAssetAsync<GameObject>(configAsset.AssetPath);
                    _gameObjectComponentPool[id] = new GameObjectComponentPool<T>(go);
                    return ((GameObjectComponentPool<T>) _gameObjectComponentPool[id]).GetInstance();
                }
                return null;
            }
        }
        
        public static void ReleaseInstanceToPool<T>(int id, T c) where T:Component
        {
            if (_gameObjectComponentPool.TryGetValue(id, out var goPool) && goPool is GameObjectComponentPool<T> cPool)
            {
                cPool.Release(c);
            }
            else
            {
                Object.Destroy(c.gameObject);
            }
           
        }

        public static void ReleaseInstanceToPool<T>(int id, GameObject go) where T : Component
        {
            if (go.TryGetComponent<T>(out var c))
            {
                ReleaseInstanceToPool(id, c);
            }
        }
        
        public static async UniTask<GameObject> GetInstanceFromPool(int id)
        {
            if (_gameObjectPool.TryGetValue(id, out var goPool))
            {
                return goPool.GetInstance();
            }
            else
            {
                var config = ConfigManager.GetObjectConfig(id);
                if (config is IConfigAsset configAsset)
                {
                    var go = await Addressables.LoadAssetAsync<GameObject>(configAsset.AssetPath);
                    _gameObjectPool[id] = new GameObjectPool(go);
                    return _gameObjectPool[id].GetInstance();
                }
#if UNITY_EDITOR
                if (config != null)
                {
                    Debug.LogError($"id:{id} config:{config} is not IConfigAsset");
                }
#endif
                return null;
            }
        }
        
        public static async UniTask<GameObject> GetInstanceFromPool(string name)
        {
            if (_gameObjectPoolByName.TryGetValue(name, out var goPool))
            {
                return goPool.GetInstance();
            }
            else
            {
                var go = await Addressables.LoadAssetAsync<GameObject>(name);
                _gameObjectPoolByName[name] = new GameObjectPool(go);
                return _gameObjectPoolByName[name].GetInstance();
            }
        }
        
        
        public static void ReleaseInstanceToPool(int id, GameObject go)
        {
            if (_gameObjectPool.TryGetValue(id, out var goPool))
            {
                goPool.Release(go);
            }
            else
            {
                Object.Destroy(go);
            }
        }
        
        public static void ReleaseInstanceToPool(string name, GameObject go)
        {
            if (_gameObjectPoolByName.TryGetValue(name, out var goPool))
            {
                goPool.Release(go);
            }
            else
            {
                Object.Destroy(go);
            }
        }


        private static void MoveToCurrentScene(GameObject gameObject)
        {
            if (gameObject.transform.parent) return;
            SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
        }
        
        private static void MoveToPoolScene(GameObject gameObject)
        {
            if (gameObject.transform.parent)
            {
                gameObject.transform.SetParent(null, true);
            }
            Object.DontDestroyOnLoad(gameObject);
            // SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetSceneAt(0));
        }
        

        private class GameObjectComponentPool<T>:GameObjectComponentPool where T : Component
        {
            private readonly ObjectPool<T> _pool;
            public GameObjectComponentPool(GameObject prefab):base(prefab)
            {
                #if UNITY_EDITOR
                if (!prefab.TryGetComponent<T>(out _))
                {
                    Debug.LogError($"prefab:{prefab} has no component:{typeof(T)}");
                }
                #endif
                
                _pool = new ObjectPool<T>(_createInstance, _onGet, _onRelease, _onDestroy);
            }
            
            private T _createInstance()
            {
                var inst = Object.Instantiate(Prefab);
                return inst.GetComponent<T>();
            }
            
            private void _onGet(T c)
            {
                MoveToCurrentScene(c.gameObject);
                c.gameObject.SetActive(true);
            }
            
            private void _onRelease(T c)
            {
                c.gameObject.SetActive(false);
                MoveToPoolScene(c.gameObject);
            }
            
            private void _onDestroy(T c)
            {
                Object.Destroy(c.gameObject);
            }
            
            
            public T GetInstance()
            {
                return _pool.Get();
            }
            
            public void Release(T c)
            {
                _pool.Release(c);
            }
        }
        
        private class GameObjectComponentPool
        {
            protected readonly GameObject Prefab;

            protected GameObjectComponentPool(GameObject prefab)
            {
                Prefab = prefab;
            }
        }
        
        

        private class GameObjectPool
        {
            private readonly GameObject _prefab;
            private readonly ObjectPool<GameObject> _pool;

            public GameObjectPool(GameObject prefab)
            {
                _prefab = prefab;
                _pool = new ObjectPool<GameObject>(_createInstance, _onGet, _onRelease, _onDestroy);
            }

            private GameObject _createInstance()
            {
                return Object.Instantiate(_prefab);
            }

            private void _onGet(GameObject go)
            {
                MoveToCurrentScene(go);
                go.SetActive(true);
            }

            private void _onRelease(GameObject go)
            {
                if (go)
                {
                    MoveToPoolScene(go);
                    go.SetActive(false);
                }
            }

            private void _onDestroy(GameObject go)
            {
                Object.Destroy(go);
            }


            public GameObject GetInstance()
            {
                return _pool.Get();
            }
            
            public void Release(GameObject go)
            {
                _pool.Release(go);
            }
        }
    }
}
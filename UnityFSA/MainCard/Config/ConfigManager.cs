using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace XMeta.Foundation.Config
{
    public class ConfigManager:MonoBehaviour
    {
        /// <summary>
        /// 默认的初始化配置文件
        /// </summary>
        public AllConfig configs;
        
        private void Awake()
        {
            if (configs) Init(configs);
        }
        
        
        
        
        private static AllConfig _configs;
        
        
        public static void Init(AllConfig configs)
        {
            _configs = configs;
        }
        
        
        public static IObjectTableConfig.IItem GetObjectConfig(int id)
        {
            if (_configs == null)
            {
                return null;
            }

            var head = id / AllConfig.IDHead;

            foreach (var table in _configs.objectConfigs)
            {
                if (table.id != head) continue;

                if (table.config is not IObjectTableConfig tableBasedConfig)
                {
                    throw new Exception("物品配置表需要继承IObjectTableConfig");
                }
                
                var items = tableBasedConfig.GetItems();
                foreach (var item in items)
                {
                    if (item is IObjectTableConfig.IItem t )
                    {
                        if (t.Id == id) return t;
                    }
                }
            }
            return null;
        }
        
        public static T GetObjectConfig<T>(int id) where T:class,IObjectTableConfig.IItem 
        {
            return GetObjectConfig(id) as T;
        }
        
        public static T GetGameConfig<T>() where T : ScriptableObject
        {
            foreach (var config in _configs.gameConfigs)
            {
                if (config is T t)
                {
                    return t;
                }
            }

            return null;
        }
        
       


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void InitAllObjectConfigs()
        {
            _allObjectConfigs = null;
            _gameConfigs = null;
        }
        
        private static List<object> _allObjectConfigs = null;
        private static List<ScriptableObject> _gameConfigs = null;
        
        public static T GetGameConfigInEditor<T>() where T : ScriptableObject
        {
            GetAllObjectConfigs();
            if (_gameConfigs == null)
            {
                return null;
            }
            foreach (var config in _gameConfigs)
            {
                if (config is T t)
                {
                    return t;
                }
            }
            return null;
        }
        
        public static object GetObjectConfigInEditor(int id)
        {
            var all = GetAllObjectConfigs();
            foreach (var item in all)
            {
                if (item is IObjectTableConfig.IItem cItem)
                {
                    if (cItem.Id == id)
                    {
                        return item;
                    }
                }
            }
            return null;
        } 

        public static List<object> GetAllObjectConfigs()
        {

            if (_allObjectConfigs != null)
            {
                return _allObjectConfigs;
            }
            _allObjectConfigs = new List<object>();
#if UNITY_EDITOR    
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:" + nameof(AllConfig));  // Find all asset GUIDs
            foreach (var guid in guids)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);  // Convert GUID to asset path
                AllConfig allConfig = UnityEditor.AssetDatabase.LoadAssetAtPath<AllConfig>(path);  // Load the asset
                if (allConfig != null)
                {
                    foreach (var table in allConfig.objectConfigs)
                    {
                        if (table.config is IObjectTableConfig tableBasedConfig)
                        {
                            var items = tableBasedConfig.GetItems();
                            foreach (var item in items)
                            {
                                _allObjectConfigs.Add(item);
                            }
                        }
                    }
                    _gameConfigs = allConfig.gameConfigs;
                }
            }
            
            
#endif
            return _allObjectConfigs;

        }

        public static ValueDropdownList<int> GetIdList()
        {
            var objects = ConfigManager.GetAllObjectConfigs();
            var vd = new ValueDropdownList<int>();
            foreach (var item in objects)
            {
                if (item is IObjectTableConfig.IItem cItem)
                {
                    if (item is ILocalizationName localizationName)
                    {
                        var n = $"{cItem.Id} {localizationName.LocalizationName}";
                        vd.Add(n, cItem.Id);
                    }
                    else if (item is ScriptableObject so)
                    {
                        vd.Add(so.name, cItem.Id);
                    }
                }
            }
            return vd;
        }

        public static ValueDropdownList<int> GetIdList<T>()
        {
            var objects = ConfigManager.GetAllObjectConfigs();
            var vd = new ValueDropdownList<int>();
            foreach (var item in objects)
            {
                if (item is not T) continue;
                if (item is IObjectTableConfig.IItem cItem)
                {
                    if (item is ILocalizationName localizationName)
                    {
                        var n = $"{cItem.Id} {localizationName.LocalizationName}";
                        vd.Add(n, cItem.Id);
                    }
                    else if (item is ScriptableObject so)
                    {
                        vd.Add(so.name, cItem.Id);
                    }
                }
            }
            return vd;
        }
        
        
        

    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace XMeta.Foundation.Config
{
    [CreateAssetMenu(fileName = "AllConfig", menuName = "XMeta/AllConfig")]
    public class AllConfig:ScriptableObject
    {
        public const int IDHead = 10000; 
        
        /// <summary>
        /// 对象级别的配置表。道具、怪物、NPC等
        /// </summary>
        public List<ConfigTable> objectConfigs;
        
        
        /// <summary>
        /// 游戏级别的配置表。游戏的一些基础配置，全局玩家经验等
        /// </summary>
        public List<ScriptableObject> gameConfigs;

    }
    
    
    

    /// <summary>
    /// 带有ID的配置表
    /// </summary>
    [System.Serializable]
    public class ConfigTable
    {
        public string name;
        public int id;
        public ScriptableObject config;
    }
}
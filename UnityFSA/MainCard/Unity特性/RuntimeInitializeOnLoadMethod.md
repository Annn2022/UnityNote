<span style="color:rgb(255, 255, 0)">2024-08-29  10:26</span>
Status: #idea
Tag:

## Content

`RuntimeInitializeOnLoadMethod` 是 Unity 引擎中的一个特性（attribute），用于标记某个静态方法，使其在场景加载或游戏启动时自动执行。这个特性非常适合用于初始化操作、设置全局状态或注册事件等任务。

以下是 `RuntimeInitializeOnLoadMethod` 的基本用法和可选参数的解释。

### 基本用法

csharp复制

```cs 
using UnityEngine;  public class Example {     [RuntimeInitializeOnLoadMethod]     static void OnRuntimeMethodLoad()     {         // 这是一个在场景加载或游戏启动时会被调用的静态方法         Debug.Log("This runs on game start or when a scene is loaded");     } }`

### 可选参数

`RuntimeInitializeOnLoadMethod` 特性可以接受一个可选参数，用于指定方法的调用时机。这个参数是 `RuntimeInitializeLoadType` 枚举类型，包含以下几种选项：

1. **`RuntimeInitializeLoadType.AfterSceneLoad`** - 默认值。在场景加载完成之后调用。
2. **`RuntimeInitializeLoadType.BeforeSceneLoad`** - 在场景加载之前调用。
3. **`RuntimeInitializeLoadType.BeforeSplashScreen`** - 在显示启动画面之前调用（仅适用于 Unity 2019.2 及以上版本）。
4. **`RuntimeInitializeLoadType.AfterAssembliesLoaded`** - 在程序集加载完成之后调用（仅适用于 Unity 2019.3 及以上版本）。
5. **`RuntimeInitializeLoadType.SubsystemRegistration`** - 在子系统注册之前调用（仅适用于 Unity 2019.4 及以上版本）。

### 参数示例

csharp复制

`using UnityEngine;  
public class Example {     // 在场景加载完成之后调用（默认行为）     [RuntimeInitializeOnLoadMethod]     
static void OnAfterSceneLoad()     
{         Debug.Log("This runs after the scene has loaded.");     }     
// 在场景加载之前调用     [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]     static void OnBeforeSceneLoad()     
{         Debug.Log("This runs before the scene loads.");     }      
// 在显示启动画面之前调用（Unity 2019.2+）     [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]     static void BeforeSplashScreen()     
{         Debug.Log("This runs before the splash screen shows.");     }      
// 在程序集加载完成之后调用（Unity 2019.3+）     [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]   static void AfterAssembliesLoaded()     
{         Debug.Log("This runs after assemblies are loaded.");     }      // 在子系统注册之前调用（Unity 2019.4+）     [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]     static void SubsystemRegistration()     
{         Debug.Log("This runs before subsystem registration.");     } }
```

### 注意事项

- 标记的方法必须是静态的。
- `RuntimeInitializeOnLoadMethod` 特性通常用于需要在游戏启动或场景加载时进行一些全局配置或初始化操作的场景。
- 如果你使用了多个 `RuntimeInitializeOnLoadMethod`，它们的执行顺序并不保证，因此不要依赖不同方法的执行顺序。

通过了解和使用 `RuntimeInitializeOnLoadMethod` 特性，你可以更加灵活地控制游戏初始化和场景加载时的行为，从而提高项目的可维护性和合理性。

# References
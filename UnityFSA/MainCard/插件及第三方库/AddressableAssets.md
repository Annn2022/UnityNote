<span style="color:rgb(255, 255, 0)">2024-07-17  11:39</span>
Status: #idea
Tag: [[plugin]] [[资源加载和卸载]]

## Overview 概述
可寻址资产系统 Unity 目前最常使用的资源加载卸载工具

## Getting Started 入门

```cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine;

public class AddressablesExample : MonoBehaviour {

    GameObject myGameObject;

        ...
        Addressables.LoadAssetAsync<GameObject>("AssetAddress").Completed += OnLoadDone;
    }

    private void OnLoadDone(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> obj)
    {
        // In a production environment, you should add exception handling to catch scenarios such as a null result.
        myGameObject = obj.Result;
    }
}
```





## Setup


- Addressable Assets package (primary package)

# References
[ 官网手册](https://docs.unity3d.com/Packages/com.unity.addressables@1.1/manual/AddressableAssetsOverview.html)


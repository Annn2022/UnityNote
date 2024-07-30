<span style="color:rgb(255, 255, 0)">2024-07-17  11:39</span>
Status: #idea
Tag: [[plugin]]  

## ## Overview 概述

LitMotion 是 Unity 的高性能补间库。 LitMotion 包含一组丰富的动画组件功能，例如 Transform、Material、TextMeshPro 和任何字段/属性，使创建动画变得轻松。

LitMotion 是继 Magic Tween 之后我创建的第二个补间库。 LitMotion 的设计基于 Magic Tween 的实现经验，以实现丰富的功能和极高的性能。在创建和更新补间等所有情况下，它都表现出压倒性的性能，比其他补间库快 2 到 20 倍。当然，创建补间时根本没有分配。

#### Getting Started 入门

```cs
using System;
using System.Threading;
using UnityEngine;
using UniRx; // UniRx
using Cysharp.Threading.Tasks; // UniTask
using LitMotion;
using LitMotion.Extensions;

public class Example : MonoBehaviour
{
    [SerializeField] Transform target1;
    [SerializeField] Transform target2;
    [SerializeField] TMP_Text tmpText;

    void Start()
    {
        LMotion.Create(Vector3.zero, Vector3.one, 2f) // Animate values from (0f, 0f, 0f) to (1f, 1f, 1f) over 2 seconds
            .BindToPosition(target1); // Bind to target1.position

        LMotion.Create(0f, 10f, 2f) // Animate from 0f to 10f over 2 seconds
            .WithEase(Ease.OutQuad) // Specify easing function
            .WithLoops(2, LoopType.Yoyo) // Specify loop count and type
            .WithDelay(0.2f) // Set delay
            .BindToUnityLogger(); // Bind to Debug.unityLogger and display values in Console on update

        var value = 0f;
        LMotion.Create(0f, 10f, 2f) // Animate from 0f to 10f over 2 seconds
            .WithScheduler(MotionScheduler.FixedUpdate) // Specify execution timing with Scheduler
            .WithOnComplete(() => Debug.Log("Complete!")) // Set a callback
            .WithCancelOnError() // Cancel motion if an exception occurs within Bind
            .Bind(x => value = x) // Bind to any variable, field, or property
            .AddTo(gameObject); // Cancel motion when the GameObject is destroyed
        
        LMotion.String.Create128Bytes("", "<color=red>Zero</color> Allocation <i>Text</i> Tween! <b>Foooooo!!</b>", 5f)
            .WithRichText() // Enable RichText tags
            .WithScrambleChars(ScrambleMode.All) // Fill unseen parts with random characters
            .BindToText(tmpText); // Bind to TMP_Text (update text with zero allocation without generating strings)

        LMotion.Punch.Create(0f, 5f, 2f) // Create a Punch motion (regular damping oscillation)
            .WithFrequency(20) // Specify oscillation count
            .WithDampingRatio(0f) // Specify damping ratio
            .BindToPositionX(target2); // Bind to transform.position.x

        // Control created motions via the `MotionHandle` struct
        var handle = LMotion.Create(0f, 1f, 2f).RunWithoutBinding();

        if (handle.IsActive()) // Returns true if the motion is playing
        {
            handle.Cancel(); // Cancel the motion
            handle.Complete(); // Complete the motion
        }
    }
    
    // Animate TMP_Text characters
    void TMPCharMotionExample()
    {
        // Get the number of characters from TMP_Text.textInfo.characterCount
        for (int i = 0; i < text.textInfo.characterCount; i++)
        {
            LMotion.Create(Color.white, Color.red, 1f)
                .WithDelay(i * 0.1f)
                .WithEase(Ease.OutQuad)
                .BindToTMPCharColor(text, i); // Bind to the i-th character

            LMotion.Punch.Create(Vector3.zero, Vector3.up * 30f, 1f)
                .WithDelay(i * 0.1f)
                .WithEase(Ease.OutQuad)
                .BindToTMPCharPosition(text, i);
        }
    }

    // Coroutine support
    IEnumerator CoroutineExample()
    {
        var handle = LMotion.Create(0f, 1f, 2f).BindToUnityLogger();
        yield return handle.ToYieldInteraction(); // Wait for completion in a coroutine
    }

    // async/await using UniTask
    async UniTask AsyncAwaitExample(CancellationToken cancellationToken)
    {
        var handle = LMotion.Create(0f, 1f, 2f).BindToUnityLogger();
        await handle; // Await MotionHandle directly
        await handle.ToUniTask(cancellationToken); // Await with passing CancellationToken
    }

    // Convert to IObservable<T> using UniRx
    void RxExample()
    {
        LMotion.Create(0f, 1f, 2f)
            .ToObservable() // Create motion as IObservable<T>
            .Where(x => x > 0.5f) // Utilize UniRx operators
            .Select(x => x.ToString())
            .Subscribe(x =>
            {
                tmpText.text = x;
            })
            .AddTo(this);
    }
}

```
## Setup

### Requirements

- Unity 2021.3 or later
- Burst 1.6.0 or later
- Collection 1.5.1 or later
- Mathematics 1.0.0 or later

### Installation

1. Open Package Manager from Window > Package Manager.
2. Click the "+" button > Add package from git URL.
3. Enter the following URL:

```
https://github.com/AnnulusGames/LitMotion.git?path=src/LitMotion/Assets/LitMotion
```

Alternatively, open Packages/manifest.json and add the following to the dependencies block:

```json
{
    "dependencies": {
        "com.annulusgames.lit-motion": "https://github.com/AnnulusGames/LitMotion.git?path=src/LitMotion/Assets/LitMotion"
    }
}

# References
[LitMotion 官网手册](https://annulusgames.github.io/LitMotion/articles/en/basic-concepts.html)

[LitMotion github 官方库](https://github.com/AnnulusGames/LitMotion)
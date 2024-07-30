<span style="color:rgb(255, 255, 0)">2024-07-17  15:39</span>
Status: #idea
Tag:[[MOC_框架  GameFramework]]

## Content

传统 MVC： 为Model， view, controller 三者之间的协作交互。
MVP 则为 MVC 的一种变体。
其中区别在于传统MVC 的view层 直接在运行时实时对于model数据的变化做监听。
而MVP则在运行时中间加入Persenter 中间层 管理两者的交互，使数据的view和model的互动 有更多的操作空间。

# References
[示例](https://unity.com/cn/how-to/build-modular-codebase-mvc-and-mvp-programming-patterns)
<span style="color:rgb(255, 255, 0)">2024-08-29  10:35</span>
Status: #idea
Tag:[[语法]]

## Content

### `using` 语句块

除了引用命名空间，`using` 关键字还可以用于管理资源的生命周期，确保在特定范围内使用资源，并在超出范围时自动释放。例如，在处理文件操作时：

csharp复制

```cs
using (System.IO.StreamWriter writer = new System.IO.StreamWriter("log.txt")) 
{    
writer.WriteLine("Hello, Unity!");
}
```

在这个例子中，`using` 语句块用来确保 `StreamWriter` 对象在写入操作完成后自动关闭和释放资源。

# References
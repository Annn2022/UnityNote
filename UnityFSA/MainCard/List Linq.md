<span style="color:rgb(255, 255, 0)">2024-09-03  18:35</span>
Status: #idea
Tag:[[C#]][[MOC_C Sharp]]

## Content

可监听的list
```cs
private ObservableCollection<GameObject> _actors = new();
_actors.CollectionChanged += OnActorChange;
```

# References
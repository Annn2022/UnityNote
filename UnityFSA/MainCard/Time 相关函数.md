<span style="color:rgb(255, 255, 0)">2024-07-30  17:16</span>
Status: #idea
Tag:[[Time]]

## Content

```cs
public static class TimeUtils  
{  
	//毫秒转为时间戳
    public static string GetTimeToString(float value)  
    {        TimeSpan ts = new TimeSpan(0, 0, (int)value);  
        string str = $"{ts.Minutes:00}:{ts.Seconds:00}";  
        str = ts.Hours > 0 ? $"{ts.Hours:00}:" + str:str;  
        return str;  
    }    
    
    
    /// DateTime 转 日期(2001-1-1 1:1:1)  
    public static string DateTime2Date(this DateTime dateTime)  
    {        string str = $"{dateTime.Year}-{dateTime.Month}-{dateTime.Day} {dateTime.Hour}:{dateTime.Minute}:{dateTime.Second}";  
        return str;  
    }  
    
   
    /// Ticks 转 日期(2001-1-1 1:1:1)  
       
    public static string Ticks2Date(this long ticks)  
    {        DateTime date = new DateTime(ticks);  
        return date.DateTime2Date();  
    }  
    
    /// 服务器时间毫秒转化成DateTime  
    public static DateTime MilliSecond2Ticks(this long milliSecond)  
    {        DateTime dateTime = DateTime.UnixEpoch.ToLocalTime().AddSeconds(milliSecond);  
        return dateTime;  
    }  
    
    /// 计算出当前时间距离晚上12点的剩余时间  
    public static TimeSpan CountdownToMidnight(this long milliSecond)  
    {        DateTime date = DateTime.UnixEpoch.ToLocalTime().AddSeconds(milliSecond / 1000);  
        // 计算今晚24点的时间  
        DateTime midnight = new DateTime(date.Year, date.Month, date.Day).AddDays(1);  
  
        // 计算倒计时  
        TimeSpan countdown = midnight - date;  
        return countdown;  
    }}
```

# References
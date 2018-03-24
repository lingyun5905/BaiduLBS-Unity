# BaiduLBS-Unity

该项目为百度定位Unity版本，支持坐标系（wgs84,gcj02,bd09,bd09ll）转换；Unity自带的定位不提供详细的地址位置描述等信息，只提供简单的经纬度信息，且只支持GPS定位，功能比较单一。
（关于百度定位sdk可以参考http://lbsyun.baidu.com/index.php?title=android-locsdk）


## 定位信息
~~~csharp
public class LocationInfo
{
    public string coorType;             // 坐标系类型(wgs84,gcj02,bd09,bd09ll)
    public double latitude;             // 纬度
    public double longitude;            // 经度
    public double altitude;             // 海拔
    public float radius;                // 半径
    public string countryCode;          // 国家码
    public string country;              // 国家名称
    public string province;             // 省份
    public string cityCode;             // 城市编码
    public string city;                 // 城市名称
    public string district;             // 区
    public string street;               // 街道
    public string streetNumber;         // 街道号码
    public string adCode;
    public string locationDescribe;     // 位置语义化信息
    public bool isInChina;               // 是否在中国, 国内使用gcj02坐标系, 国外使用wgs84坐标系
}

~~~

## 使用方法
~~~csharp
LocationManager.Instance.SetAppKey("8RIVQyTPRGevwgQnueUa3ubx");
LocationManager.Instance.ReqLocation(delegate (LocationInfo li)
{
    location = JsonUtility.ToJson(li);
    Debug.Log("data => " + location);
});
~~~
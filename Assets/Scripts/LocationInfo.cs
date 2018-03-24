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

using System;

/// <summary>
/// 定位坐标系转换;
/// WGS84坐标系：即地球坐标系，国际上通用的坐标系。设备一般包含GPS芯片或者北斗芯片获取的经纬度为WGS84地理坐标系,
/// 谷歌地图采用的是WGS84地理坐标系（中国范围除外）;
/// GCJ02坐标系：即火星坐标系，是由中国国家测绘局制订的地理信息系统的坐标系统。由WGS84坐标系经加密后的坐标系。
/// 谷歌中国地图和搜搜中国地图采用的是GCJ02地理坐标系; BD09坐标系：即百度坐标系，GCJ02坐标系经加密后的坐标系;
/// 搜狗坐标系、图吧坐标系等，估计也是在GCJ02基础上加密而成的。
/// </summary>
public class LocationUtil
{
    private const double PI = 3.1415926535897932384626;                     // 圆周率
    private const double X_PI = 3.14159265358979324 * 3000.0 / 180.0;
    private const double A = 6378245.0;                                     // 长半轴
    private const double EE = 0.00669342162296594323;                       // 扁率

    /// <summary>
    /// 是否在中国之外
    /// </summary>
    /// <param name="latLng"></param>
    /// <returns></returns>
    public static bool IsOutOfChina(LatLng latLng)
    {
        if (latLng.Longitude < 72.004 || latLng.Longitude > 137.8347)
            return true;
        if (latLng.Latitude < 0.8293 || latLng.Latitude > 55.8271)
            return true;
        return false;
    }

    /// <summary>
    /// WGS84坐标转Gcj02火星坐标
    /// </summary>
    /// <param name="latLng"></param>
    /// <returns></returns>
    public static LatLng Wgs84ToGcj02(LatLng latLng)
    {
        if (IsOutOfChina(latLng))
        {
            return latLng;
        }
        double dLat = TransformLat(latLng.Longitude - 105.0, latLng.Latitude - 35.0);
        double dLon = TransformLon(latLng.Longitude - 105.0, latLng.Latitude - 35.0);
        double radLat = latLng.Latitude / 180.0 * PI;
        double magic = Math.Sin(radLat);
        magic = 1 - EE * magic * magic;
        double sqrtMagic = Math.Sqrt(magic);
        dLat = (dLat * 180.0) / ((A * (1 - EE)) / (magic * sqrtMagic) * PI);
        dLon = (dLon * 180.0) / (A / sqrtMagic * Math.Cos(radLat) * PI);
        double mgLat = latLng.Latitude + dLat;
        double mgLon = latLng.Longitude + dLon;
        return new LatLng(mgLat, mgLon);
    }

    /// <summary>
    /// Gcj02火星坐标转WGS84坐标
    /// </summary>
    /// <param name="latLng"></param>
    /// <returns></returns>
    public static LatLng Gcj02ToWgs84(LatLng latLng)
    {
        LatLng gps = Transform(latLng);
        double latitude = latLng.Latitude * 2 - gps.Latitude;
        double lontitude = latLng.Longitude * 2 - gps.Longitude;
        return new LatLng(latitude, lontitude);
    }

    /// <summary>
    /// 火星坐标系 (GCJ-02)转百度坐标系(BD-09)
    /// </summary>
    /// <param name="latLng"></param>
    /// <returns></returns>
    public static LatLng Gcj02ToBd09(LatLng latLng)
    {
        double x = latLng.Longitude, y = latLng.Latitude;
        double z = Math.Sqrt(x * x + y * y) + 0.00002 * Math.Sin(y * X_PI);
        double theta = Math.Atan2(y, x) + 0.000003 * Math.Cos(x * X_PI);
        double tempLat = z * Math.Sin(theta) + 0.006;
        double tempLon = z * Math.Cos(theta) + 0.0065;
        return new LatLng(tempLat, tempLon);
    }

    /// <summary>
    /// BD-09坐标转换GCJ-02坐标
    /// </summary>
    /// <param name="latLng"></param>
    /// <returns></returns>
    public static LatLng Bd09ToGcj02(LatLng latLng)
    {
        double x = latLng.Longitude - 0.0065, y = latLng.Latitude - 0.006;
        double z = Math.Sqrt(x * x + y * y) - 0.00002 * Math.Sin(y * X_PI);
        double theta = Math.Atan2(y, x) - 0.000003 * Math.Cos(x * X_PI);
        double tempLat = z * Math.Sin(theta);
        double tempLon = z * Math.Cos(theta);
        return new LatLng(tempLat, tempLon);
    }

    /// <summary>
    /// WGS84坐标转为BD09坐标 
    /// </summary>
    /// <param name="latLng"></param>
    /// <returns></returns>
    public static LatLng Wgs84ToBd09(LatLng latLng)
    {
        LatLng gcj02 = Wgs84ToGcj02(latLng);
        LatLng bd09 = Gcj02ToBd09(gcj02);
        return bd09;
    }

    /// <summary>
    /// BD09坐标转为Wgs84坐标
    /// </summary>
    /// <param name="latLng"></param>
    /// <returns></returns>
    public static LatLng Bd09ToWgs84(LatLng latLng)
    {
        LatLng gcj02 = Bd09ToGcj02(latLng);
        LatLng wgs84 = Gcj02ToWgs84(gcj02);
        return new LatLng(Retain6(wgs84.Latitude), Retain6(wgs84.Longitude));
    }

    /// <summary>
    /// 保留小数点后六位
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    private static double Retain6(double num)
    {
        return Double.Parse(String.Format("%.6f", num));
    }

    private static double TransformLat(double x, double y)
    {
        double ret = -100.0 + 2.0 * x + 3.0 * y + 0.2 * y * y + 0.1 * x * y
                     + 0.2 * Math.Sqrt(Math.Abs(x));
        ret += (20.0 * Math.Sin(6.0 * x * PI) + 20.0 * Math.Sin(2.0 * x * PI)) * 2.0 / 3.0;
        ret += (20.0 * Math.Sin(y * PI) + 40.0 * Math.Sin(y / 3.0 * PI)) * 2.0 / 3.0;
        ret += (160.0 * Math.Sin(y / 12.0 * PI) + 320 * Math.Sin(y * PI / 30.0)) * 2.0 / 3.0;
        return ret;
    }

    private static double TransformLon(double x, double y)
    {
        double ret = 300.0 + x + 2.0 * y + 0.1 * x * x + 0.1 * x * y + 0.1
                     * Math.Sqrt(Math.Abs(x));
        ret += (20.0 * Math.Sin(6.0 * x * PI) + 20.0 * Math.Sin(2.0 * x * PI)) * 2.0 / 3.0;
        ret += (20.0 * Math.Sin(x * PI) + 40.0 * Math.Sin(x / 3.0 * PI)) * 2.0 / 3.0;
        ret += (150.0 * Math.Sin(x / 12.0 * PI) + 300.0 * Math.Sin(x / 30.0
                                                                   * PI)) * 2.0 / 3.0;
        return ret;
    }

    private static LatLng Transform(LatLng latLng)
    {
        if (IsOutOfChina(latLng))
        {
            return latLng;
        }
        double dLat = TransformLat(latLng.Longitude - 105.0, latLng.Latitude - 35.0);
        double dLon = TransformLon(latLng.Longitude - 105.0, latLng.Latitude - 35.0);
        double radLat = latLng.Latitude / 180.0 * PI;
        double magic = Math.Sin(radLat);
        magic = 1 - EE * magic * magic;
        double sqrtMagic = Math.Sqrt(magic);
        dLat = (dLat * 180.0) / ((A * (1 - EE)) / (magic * sqrtMagic) * PI);
        dLon = (dLon * 180.0) / (A / sqrtMagic * Math.Cos(radLat) * PI);
        double mgLat = latLng.Latitude + dLat;
        double mgLon = latLng.Longitude + dLon;
        return new LatLng(mgLat, mgLon);
    }
}

using System;

public interface ILocation
{
    void SetAppKey(string appKey);

    /// <summary>
    /// 请求定位
    /// </summary>
    /// <param name="callback">定位成功回调</param>
    void ReqLocation(Action<LocationInfo> callback);
}

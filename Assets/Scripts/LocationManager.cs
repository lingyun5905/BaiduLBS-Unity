using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class LocationManager : MonoBehaviour, ILocation
{
    private static ILocation _instance;
    public static ILocation Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("[LocationManager]").AddComponent<LocationManager>();
            }
            return _instance;
        }
    }

    private Action<LocationInfo> m_CallBack;

    protected void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(this.gameObject);

#if UNITY_IOS
        __PushUnity3DBridge_init(this.gameObject.name, "OnLocationResult");
#elif UNITY_ANDROID
        _androidPushManager.Call("init", this.gameObject.name, "OnLocationResult");
#endif
    }

    protected void OnLocationResult(string data)
    {
        if (m_CallBack != null)
        {
            if (string.IsNullOrEmpty(data))
            {
                m_CallBack(null);
            }
            else
            {
                m_CallBack(JsonUtility.FromJson<LocationInfo>(data));
            }
        }
        m_CallBack = null;
    }

#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern void __LocationUnity3DBridge_init(string gameobject, string calllback);

    [DllImport("__Internal")]
    private static extern void __LocationUnity3DBridge_setAppKey(string appID);

    [DllImport("__Internal")]
    private static extern void __LocationUnity3DBridge_reqLocation();

    private LocationManager()
    {
    }

    public void SetAppKey(string appKey)
    {
        __LocationUnity3DBridge_setAppKey(appKey);
    }

    public void ReqLocation(Action<LocationInfo> callback)
    {
        m_CallBack = callback;
        __LocationUnity3DBridge_reqLocation();
    }

#elif UNITY_ANDROID
    private static AndroidJavaObject _androidPushManager;

    private LocationManager()
    {
        try
        {
            var ajc = new AndroidJavaClass("com.hk.sdk.location.LocationManager");
            _androidPushManager = ajc.CallStatic<AndroidJavaObject>("getInstance");
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public void SetAppKey(string appKey)
    {
        // BaiduLBS Android的appkey 需要在AndroidManifest文件中设置
        // BaiduLocPreProcessBuild.cs 文件在打包时已经处理
    }

    public void ReqLocation(Action<LocationInfo> callback)
    {
        m_CallBack = callback;
        _androidPushManager.Call("reqLocation");
    }
#endif

}

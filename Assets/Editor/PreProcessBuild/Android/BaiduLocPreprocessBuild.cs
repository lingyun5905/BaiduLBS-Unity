#if UNITY_ANDROID
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

public class BaiduLocPreProcessBuild : IPreprocessBuild
{
    public int callbackOrder
    {
        get { return 1; }
    }

    public void OnPreprocessBuild(BuildTarget target, string path)
    {
        string manifest = Path.Combine(Application.dataPath, "Plugins/Android/Location/AndroidManifest.xml");
        XmlDocument doc = new XmlDocument();
        doc.Load(manifest);
        XmlElement node = AndroidManifestHelper.SearchNode(doc, "manifest/application/meta-data"
            , new KeyValuePair<string, string>("android:name", "com.baidu.lbsapi.API_KEY"));
        node.SetAttribute("android:value", Configs.AppKey);
        doc.Save(manifest);
    }
}
#endif
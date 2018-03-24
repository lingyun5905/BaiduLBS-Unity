#if UNITY_IPHONE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XFramework;
using UnityEditor.Callbacks;
using UnityEditor;
using UnityEditor.iOS.Xcode;
using System.IO;

public static class BaiduLocPostProcessBuild {
	private static SDKConfig _SDKConfig;

	[PostProcessBuildAttribute(90)]
	public static void OnPostprocessBuild(BuildTarget buildTarget, string pathToBuiltProject)
	{
		// BuildTarget需为iOS
		if (buildTarget != BuildTarget.iOS)
			return;
		_SDKConfig = Resources.Load<SDKConfig>("SDKConfig");
		if (_SDKConfig == null)
		{
			Debug.LogError("SDKConfig.asset not found!");
			return;
		}
		ModifyXCodeProj (pathToBuiltProject);
		ModifyInfoPlist (pathToBuiltProject);
	}

	/// <summary>
	/// 修改Xcode proj
	/// </summary>
	/// <param name="pathToBuiltProject"></param>
	static void ModifyXCodeProj(string pathToBuiltProject)
	{
		new XCodeProj (pathToBuiltProject)
			.AddFramework ("CoreLocation.framework")
			.AddFramework ("SystemConfiguration.framework")
			.AddFramework ("Security.framework")
			.AddFramework ("CoreTelephony.framework")
			.AddFramework ("AdSupport.framework")
			.AddFramework ("libsqlite3.tbd")
			.AddFramework ("libc++.tbd")
			.AddBuildProperty ("OTHER_LDFLAGS", "-ObjC")
			.Save ();
	}

	/// <summary>
	/// 修改PList文件
	/// </summary>
	/// <param name="pathToBuiltProject"></param>
	static void ModifyInfoPlist(string pathToBuiltProject)
	{
		// 修改Info.plist文件
		var plistPath = Path.Combine(pathToBuiltProject, "Info.plist");
		var plist = new PlistDocument();
		plist.ReadFromFile(plistPath);

		var dict = plist.root.AsDict();
		dict.SetString("NSLocationWhenInUseUsageDescription", "需要使用定位权限");
		dict.SetString("NSLocationAlwaysUsageDescription", "需要使用定位权限");
		dict.GetOrCreateDict ("NSAppTransportSecurity").SetBoolean ("NSAllowsArbitraryLoads", true);

		plist.WriteToFile(plistPath);
	}
}
#endif
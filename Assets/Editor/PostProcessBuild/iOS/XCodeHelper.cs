#if UNITY_IPHONE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.iOS.Xcode;
using System.IO;
using System.Linq;
using System;

public static class XCodeHelper {

	public static PlistElementDict GetOrCreateDict(this PlistElementDict dict, string key) {
		return dict.GetOrCreate<PlistElementDict>(key);
	}

	public static PlistElementArray GetOrCreateArray(this PlistElementDict dict, string key) {
		return dict.GetOrCreate<PlistElementArray>(key);
	}

	public static void AddString(this PlistElementArray array, string key, bool isKeepUnique) {
		if (!isKeepUnique) {
			array.AddString (key);
			return;
		}
		var keys = array.values.Where (x => x.AsString() == key);
		if (keys != null && keys.Count() != 0) {
			return;
		}
		array.AddString (key);
	}

	private static T GetOrCreate<T>(this PlistElementDict dict, string key){
		var values = dict.values.Where (x => x.Key == key).ToArray();
		if (values != null && values.Length != 0) {
			return (T)Convert.ChangeType (values [0].Value, typeof(T));
		} 
		if (typeof(T) == typeof(PlistElementDict)) {
			return (T)Convert.ChangeType (dict.CreateDict (key), typeof(T));
		} 
		if (typeof(T) == typeof(PlistElementArray)){
			return (T)Convert.ChangeType (dict.CreateArray (key), typeof(T));
		}
		return default(T);
	}



	/// <summary>
	/// 添加folder
	/// </summary>
	/// <param name="pbxProject"></param>
	/// <param name="targetGuid"></param>
	/// <param name="path"></param>
	/// <param name="projectPath"></param>
	public static void AddFolderToBuild(this PBXProject pbxProject, string targetGuid, string path, string projectPath)
	{
		// 添加文件到xcode并排除多余的文件
		DirectoryInfo di = new DirectoryInfo(path);
		var originFiles = di.GetFiles();
		var files = originFiles.Where(x => !x.Name.EndsWith(".DS_Store"));
		foreach (FileInfo f in files)
		{
			var fpath = projectPath + "/" + f.Name;
			pbxProject.AddFileToBuild(targetGuid, pbxProject.AddFile(fpath, fpath, PBXSourceTree.Source));
		}

		// 设置静态库的查找路径
		var libs = originFiles.Where(x => x.Name.EndsWith(".a"));
		foreach (FileInfo l in libs)
		{
			pbxProject.AddBuildProperty(targetGuid, "LIBRARY_SEARCH_PATHS", "$(SRCROOT)/" + projectPath);
		}

		// 添加framework及bundle
		var originDirectorys = di.GetDirectories();
		var frameworks = originDirectorys.Where(x => x.Name.EndsWith(".framework") || x.Name.EndsWith(".bundle"));
		foreach (DirectoryInfo f in frameworks)
		{
			var fpath = projectPath + "/" + f.Name;
			pbxProject.AddBuildProperty(targetGuid, "FRAMEWORK_SEARCH_PATHS", "$(SRCROOT)/" + projectPath);
			pbxProject.AddFileToBuild(targetGuid, pbxProject.AddFile(fpath, fpath, PBXSourceTree.Source));
		}

		// 递归遍历子目录
		var directorys = originDirectorys.Where(x => !x.Name.EndsWith(".framework") && !x.Name.EndsWith(".bundle"));
		foreach (DirectoryInfo d in directorys)
		{
			AddFolderToBuild(pbxProject, targetGuid, path + "/" + d.Name, projectPath + "/" + d.Name);
		}
	}
}
#endif
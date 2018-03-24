#if UNITY_IPHONE
using UnityEditor.iOS.Xcode;
using System.IO;


public class XCodeProj {

	private string m_ProBuildPath;
	private string m_ProjectPath;
	private PBXProject m_PBXProject;
	private string m_TargetGuid;

	public XCodeProj(string proBuildPath) {
		m_ProBuildPath = proBuildPath;
		m_ProjectPath = m_ProBuildPath + "/Unity-iPhone.xcodeproj/project.pbxproj";
		m_PBXProject = new PBXProject();
		m_PBXProject.ReadFromFile(m_ProjectPath);
		m_TargetGuid = m_PBXProject.TargetGuidByName("Unity-iPhone");
	}

	public XCodeProj AddFolder (string path, string projectPath){
		m_PBXProject.AddFolderToBuild(m_TargetGuid, m_ProBuildPath +"/"+ path, projectPath);
		return this;
	}

	public XCodeProj AddFramework(string framework) {
		m_PBXProject.AddFrameworkToProject(m_TargetGuid, framework, false);
		return this;
	}

	public XCodeProj AddBuildProperty(string name, string value) {
		m_PBXProject.AddBuildProperty(m_TargetGuid, name, value);
		return this;
	}

	public XCodeProj SetBuildProperty(string name, string value) {
		m_PBXProject.SetBuildProperty(m_TargetGuid, name, value);
		return this;
	}

	public void Save() {
		File.WriteAllText(m_ProjectPath, m_PBXProject.WriteToString());
	}
}
#endif
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using UnityEditor.iOS.Xcode;

public class ChocolateUnityIOSBuildHelper  {

  #region PostProcessBuild

  [PostProcessBuild]
  public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) {
    if (target == BuildTarget.iOS){
      specialSetupForGoogleAds(pathToBuiltProject);
      specialSetupForAdcolonyAds(pathToBuiltProject);
      processPodfile(pathToBuiltProject);
    }
  }

  #endregion

  #region Private methods
  //Google ads frameork requires a custom script to archive ios app
  internal static void specialSetupForGoogleAds(string pathToBuiltProject) {
    string scriptDir = pathToBuiltProject + "/Frameworks/Plugins/iOS/Mediation/Google";
    if(Directory.Exists(scriptDir)) {
      string[] assets = AssetDatabase.FindAssets("strip_for_archive");
      if(assets.Length > 0) {
        string scriptFile = AssetDatabase.GUIDToAssetPath(assets[0]);
        string scriptDest = scriptDir + "/" + System.IO.Path.GetFileName(scriptFile);
        System.IO.File.Copy(scriptFile,scriptDest);
      }
    }
  }

  //AdColony ads require optionally linked WatchConnectivity framework
  internal static void specialSetupForAdcolonyAds(string pathToBuiltProject) {
    string acDir = pathToBuiltProject + "/Frameworks/Plugins/iOS/Mediation/AdColony";
    if(Directory.Exists(acDir)) {
      var projectPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
      var project = new PBXProject();
      project.ReadFromFile(projectPath);
      string appTarget = project.TargetGuidByName(PBXProject.GetUnityTargetName());
      project.AddFrameworkToProject(appTarget, "WatchConnectivity.framework", true);
      project.WriteToFile(projectPath);
    }
  }

  internal static void processPodfile(string pathToBuiltProject) {
    if(canUseCocoaPodsSafely(pathToBuiltProject)) {
      CopyFileToDirectory(PodfileSource, pathToBuiltProject);
      CopyFileToDirectory(PodscriptSource, pathToBuiltProject);
			CopyFileToDirectory(OpenPodscriptSource, pathToBuiltProject);

			var proc = new System.Diagnostics.Process ();
			proc.StartInfo.FileName = Path.Combine (pathToBuiltProject,
				Path.GetFileName (OpenPodscriptSource));
			proc.Start ();
    }
  }

  internal static bool canUseCocoaPodsSafely(string pathToBuiltProject) {
    string medDir = pathToBuiltProject + "/Frameworks/Plugins/iOS/Mediation";
    string sdkDir = pathToBuiltProject + "/Frameworks/Plugins/iOS/SDK";
    if(Directory.Exists(medDir) || Directory.Exists(sdkDir)) {
      return false;
    }

    return File.Exists(PodfileSource);
  }

  internal static void CopyFileToDirectory(string srcFile, string dstDir) {
    CopyAndReplaceFile(srcFile,
      Path.Combine(dstDir,Path.GetFileName(srcFile)));
  }

  internal static void CopyAndReplaceFile (string srcPath, string dstPath)
  {
      if (File.Exists (dstPath))
          File.Delete (dstPath);

      File.Copy (srcPath, dstPath);
  }

  static string PodfileSource {
      get {
          return Path.Combine (".", "Podfile");
      }
  }

  static string PodscriptSource {
    get {
      return Path.Combine("./Assets/Editor", "pods.command");
    }
  }

  static string OpenPodscriptSource {
    get {
      return Path.Combine("./Assets/Editor", "open_pods.command");
    }
  }

  #endregion
}

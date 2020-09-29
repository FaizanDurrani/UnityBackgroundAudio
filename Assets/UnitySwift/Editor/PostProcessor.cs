using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

#if !UNITY_2019_4_OR_NEWER
namespace UnitySwift {
    public static class PostProcessor {
        [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath) {
            if(buildTarget == BuildTarget.iOS) {
                // So PBXProject.GetPBXProjectPath returns wrong path, we need to construct path by ourselves instead
                // var projPath = PBXProject.GetPBXProjectPath(buildPath);
                var projPath = buildPath + "/Unity-iPhone.xcodeproj/project.pbxproj";
                var proj = new PBXProject();
                proj.ReadFromFile(projPath);

#if UNITY_2019_3_OR_NEWER
                string targetGuid = proj.GetUnityFrameworkTargetGuid();
#else
                string targetGuid = proj.TargetGuidByName(PBXProject.GetUnityTargetName());
#endif
                //// Configure build settings
                proj.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");
                proj.SetBuildProperty(targetGuid, "SWIFT_OBJC_BRIDGING_HEADER", "Libraries/UnitySwift/UnitySwift-Bridging-Header.h");
                proj.SetBuildProperty(targetGuid, "SWIFT_OBJC_INTERFACE_HEADER_NAME", "unityswift-Swift.h");
                proj.AddBuildProperty(targetGuid, "LD_RUNPATH_SEARCH_PATHS", "@executable_path/Frameworks");

                proj.WriteToFile(projPath);
            }
        }
    }
}
#endif
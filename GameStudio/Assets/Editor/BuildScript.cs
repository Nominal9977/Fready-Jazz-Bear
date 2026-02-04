using UnityEditor;
using UnityEngine;
using System.IO;

public class BuildScript
{
    [MenuItem("Build/WebGL")]
    public static void BuildWeb()
    {
        // Get all enabled scenes
        string[] scenes = EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();

        if (scenes.Length == 0)
        {
            Debug.LogError("No scenes are enabled in Build Settings!");
            return;
        }

        string buildPath = Path.Combine(Application.dataPath, "../builds/WebGL");
        Directory.CreateDirectory(buildPath);

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = buildPath,
            target = BuildTarget.WebGL,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);

        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log("WebGL Build Succeeded!");
        }
        else
        {
            Debug.LogError("WebGL Build Failed!");
        }
    }
}
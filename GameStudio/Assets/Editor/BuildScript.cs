using UnityEditor;
using UnityEngine;
using System.IO;

public class BuildScript
{
    [MenuItem("Build/WebGL")]
    public static void BuildWebGL()
    {
        string[] scenes = EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .Select(s => s.path)
            .ToArray();

        string buildPath = Path.Combine(Application.dataPath, "../build/WebGL");

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = buildPath,
            target = BuildTarget.WebGL,
            options = BuildOptions.None
        };

        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }
}
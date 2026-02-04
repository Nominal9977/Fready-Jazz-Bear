using UnityEditor;
using UnityEngine;
using System.IO;

public class BuildScript
{
    [MenuItem("Build/WebGL")]
    public static void BuildWebGL()
    {
        string[] scenes = { "Assets/Scenes/MainGame.unity", "Assets/Scenes/MainMenu.unity", "Assets/Scenes/InitialPlayerMovement.unity" };
        string buildPath = Path.Combine(Application.dataPath, "../build/WebGL");

        BuildPipeline.BuildPlayer(scenes,
            buildPath,
            BuildTarget.WebGL,
            BuildOptions.None);
    }
}
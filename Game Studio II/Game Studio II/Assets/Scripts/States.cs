using UnityEngine;
using UnityEngine.SceneManagement;
using static FuntionLibray;
using System.Collections.Generic;
using UnityEngine.UI;

[System.Serializable]
public class AudioTextPair
{
    public AudioSource audio;
    [TextArea(3, 10)]
    public string text;
}
[System.Serializable]
public struct Diolog
{
    public string name;
    public List<AudioTextPair> logs;

}

public class sStartState : StateAuto<sStartState, TestofStates>
{
    public override bool IsDefault => true;
    public override void update()
    {

        if (Input.GetMouseButton(0))
        {
            //PlayPaw();
        }

    }
    void PlayPaw()
    {
      
    }
}

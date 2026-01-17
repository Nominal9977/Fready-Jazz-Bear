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

#region States For Control Script

public class sStartState : StateAuto<sStartState, ControlScript>
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
        if (script.Canvas == null)
        {
            script.Canvas = GameObject.FindAnyObjectByType<Canvas>();
        }


        var partical = Object.Instantiate(script?.Paw, Input.mousePosition, Quaternion.identity);
        partical.transform.parent = script.Canvas.transform;
        partical.GetComponent<ParticleSystem>().Play();

    }
}
public class sNailHit : StateAuto<sNailHit, ControlScript>
{

    override public void enter()
    {
        SceneManager.LoadScene("Nail");
        script.InvokeRepeating(nameof(script.TryNailBind), 0f, 0.1f);
    }
    override public void update()
    {
        // //PlayPaw();
        if (script.isMoving)
        {
            script.isMoving = PlayTransition(script.moveTimer, script.moveDuration, script.startPos, script.targetPos, script.scoreTransform);
        }
        int countleft = 0;
        if (script.nails != null)
        {
            foreach (Nails nail in script.nails)
            {
                if (nail.getDown())
                {
                    countleft++;
                }
            }
            if (countleft >= 16)
            {
                script.button.gameObject.SetActive(true);
            }
            if (countleft >= 8 && !script.isMoving)
            {
                GameObject scoreObj = GameObject.FindGameObjectWithTag("Mini");
                script.scoreTransform = scoreObj.transform;
                script.startPos = script.scoreTransform.localPosition;
                script.targetPos = new Vector3(0, 1393.6f, 0);
                script.isMoving = true;

            }

        }
    }
    void PlayPaw()
    {
        if (Input.GetMouseButton(0))
        {
            var partical = Object.Instantiate(script.Paw.gameObject, Input.mousePosition, Quaternion.identity);
            partical.GetComponent<ParticleSystem>().Play();
        }
    }
    public override void exit()
    {
        script.CancelInvoke(nameof(script.TryNailBind));
    }
}
public class sLevelSelect : StateAuto<sLevelSelect, ControlScript>
{

    override public void enter()
    {
        SceneManager.LoadScene("MainStart");
        script.ControlReset();
        script.currentLogIndex = 0;
    }
    public override void update()
    {
        // //PlayPaw();
    }
    void PlayPaw()
    {
        if (Input.GetMouseButton(0))
        {
            var partical = Object.Instantiate(script.Paw.gameObject, Input.mousePosition, Quaternion.identity);
            partical.GetComponent<ParticleSystem>().Play();
        }
    }
}
public class sDrag : StateAuto<sDrag, ControlScript>
{
    override public void enter()
    {
        SceneManager.LoadScene("Concret");

    }
    override public void update()
    {
        // //PlayPaw();
        if (script.button != null)
        {
            script.button.gameObject.SetActive(true);
        }

    }
    void PlayPaw()
    {
        if (Input.GetMouseButton(0))
        {
            var partical = Object.Instantiate(script.Paw.gameObject, Input.mousePosition, Quaternion.identity);
            partical.GetComponent<ParticleSystem>().Play();
        }
    }
}
public class sDiolog : StateAuto<sDiolog, ControlScript>
{

    [SerializeField] private float lettersPerSecond = 15f;

    public float timer;
    public int currentIndex;
    public bool isTyping = true;
    public int currentListIndex = 0;
    public void incresseDiologNumber()
    {
        currentListIndex += 1;
        script.textbox.text = "";
        isTyping = true;
        timer = 0;
        currentIndex = 0;
        script.button.gameObject.SetActive(false);
    }
    public override void enter()
    {
        SceneManager.LoadScene("Diolog");
        script.InvokeRepeating(nameof(script.TryDiologBind), 0f, 0.01f);
        currentListIndex = 0;
        isTyping = true;
        currentIndex = 0;
    }
    public override void update()
    {
        if (!isTyping || script.textbox == null) return;

        var currentLog = script.logs[script.currentLogIndex].logs[currentListIndex];

        if (!currentLog.audio.isPlaying && currentIndex == 0)
        {
            currentLog.audio.Play();


            float audioLength = currentLog.audio.clip.length;
            int textLength = currentLog.text.Length;
            lettersPerSecond = textLength / audioLength;

            var tmp = script.textbox.GetComponent<TMPro.TextMeshProUGUI>();
            string fullText = currentLog.text;

            if (tmp != null)
            {
                tmp.enableAutoSizing = true;
                tmp.text = fullText;

                Canvas.ForceUpdateCanvases();
                float calculatedSize = tmp.fontSize;

                tmp.enableAutoSizing = false;
                tmp.fontSize = calculatedSize;
                tmp.text = "";
            }
        }

        timer += Time.deltaTime;
        float delayBetweenLetters = 1f / lettersPerSecond;

        while (timer >= delayBetweenLetters && currentIndex < currentLog.text.Length)
        {
            script.textbox.text += currentLog.text[currentIndex];
            currentIndex++;
            timer -= delayBetweenLetters;
        }

        if (currentIndex >= currentLog.text.Length)
        {
            script.button.gameObject.SetActive(true);
        }

        if (currentListIndex == script.logs[script.currentLogIndex].logs.Count - 1)
        {
            script.button.GetComponent<ButtonLinker>().rebind(ButtonType.LevelBuild);
        }
    }

    public override void exit()
    {
        script.currentLogIndex += 1;
        currentListIndex = 0;
        isTyping = true;
        currentIndex = 0;
    }
    void PlayPaw()
    {
        if (Input.GetMouseButton(0))
        {
            var partical = Object.Instantiate(script.Paw.gameObject, Input.mousePosition, Quaternion.identity);
            partical.GetComponent<ParticleSystem>().Play();
        }
    }
}
public class sCrain : StateAuto<sCrain, ControlScript>
{
    public override void enter()
    {
        SceneManager.LoadScene("CraneButton");
        script.CancelInvoke(nameof(script.TryNailBind));
    }
    public override void update()
    {
        // //PlayPaw();
    }
    void PlayPaw()
    {
        if (Input.GetMouseButton(0))
        {
            var partical = Object.Instantiate(script.Paw.gameObject, Input.mousePosition, Quaternion.identity);
            partical.GetComponent<ParticleSystem>().Play();
        }
    }
}
public class sLevelBuilding : StateAuto<sLevelBuilding, ControlScript>
{
    public override void enter()
    {
        SceneManager.LoadScene("LevelBuiliding");
        script.InvokeRepeating(nameof(script.TrybuttonBind), 0.2f, 0.01f);
        script.buildIndex++;
    }
    public override void exit()
    {

    }
}
#endregion

#region States for Dragging
public class sNotDragable : StateAuto<sNotDragable, Drag>
{
    public override bool IsDefault => true;
    public override void enter()
    {
    }
    public override void update()
    {
        Debug.Log("not dragging");
    }
}
public class sDragable : StateAuto<sDragable, Drag>
{

    public override void enter()
    {
        script.Selected.targetPos.GetComponent<TargetPos>().TargetPost.GetComponent<UnityEngine.UI.Image>().color = Color.red;
        script.OneSelected = true;
        Debug.Log("dragging");
    }
    override public void update()
    {
        script.Selected.targetPos.transform.position = Input.mousePosition;
    }
    public override void exit()
    {
        script.Selected.targetPos.GetComponent<TargetPos>().TargetPost.GetComponent<UnityEngine.UI.Image>().color = Color.white;
        script.OneSelected = false;
    }
}
public class sNeverDragable : StateAuto<sNeverDragable, Drag>
{
    public override void enter()
    {
        script.Selected.snapped = true;

    }
    public override void update()
    {
        script.Selected.targetPos.transform.position = script.Selected.targetPos.GetComponent<TargetPos>().TargetPost.transform.position;
    }
    public override void exit()
    {
        script.Selected = null;
    }
}
#endregion 

#region States for Bucket Dragging
public class sBucketDefualt : StateAuto<sBucketDefualt, BucketDrag>
{
}
public class sBucketDragable : StateAuto<sBucketDragable, BucketDrag>
{
    public override void update()
    {
        script.transform.position = Input.mousePosition;
        script.newSpirte.gameObject.transform.position = Input.mousePosition;
    }

}
public class sBucketNotDragable : StateAuto<sBucketNotDragable, BucketDrag>
{
    public override bool IsDefault => true;
    public override void enter()
    {
        script.InvokeRepeating(nameof(script.TryButtonBind), 0f, 0.1f);
    }
    public override void update()
    {
        Debug.Log("Not Dragabel");
    }
}
public class sBucketPouring : StateAuto<sBucketPouring, BucketDrag>
{
    public override void enter()
    {
        script.GetComponent<Image>().enabled = false;
        script.newSpirte.gameObject.SetActive(true);

        script.transform.position = script.targetPos.transform.position;
    }
    public override void update()
    {
        if (script.Concrete.transform.localScale.x <= 3.15f)
        {
            script.Concrete.transform.localScale += new Vector3(2 * Time.deltaTime, 2 * Time.deltaTime, 0);
        }
        else
        {
            script.donePouring = true;
        }
    }
}
public class sDone : StateAuto<sDone, BucketDrag>
{
    public override void enter()
    {
        script.GetComponent<Image>().gameObject.SetActive(false);
        script.newSpirte.gameObject.SetActive(true);
        script.button.gameObject.SetActive(true);
    }
}

#endregion

#region States for LevelBuilding
public class sFirstState : StateAuto<sFirstState, TransitionScreen>
{
    public override bool IsDefault => true;
    public override void enter()
    {
        script.RedBars.enabled = false;
        script.BuildAnimation.enabled = false;
        script.CompleteBuilding.enabled = false;
        script.Stars.enabled = false;
    }
}
public class sSecondState : StateAuto<sSecondState, TransitionScreen>
{

    public override void enter()
    {
        script.RedBars.enabled = true;
        script.BuildAnimation.enabled = false;
        script.CompleteBuilding.enabled = false;
        script.Stars.enabled = false;
    }
}
public class sThirdState : StateAuto<sThirdState, TransitionScreen>
{
    public override void enter()
    {
        script.RedBars.enabled = true;
        script.BuildAnimation.enabled = true;
        script.CompleteBuilding.enabled = false;
        script.Stars.enabled = false;
    }
}
public class sFourthState : StateAuto<sFourthState, TransitionScreen>
{
    public override void enter()
    {
        script.RedBars.enabled = false;
        script.BuildAnimation.enabled = false;
        script.CompleteBuilding.enabled = true;
        script.Stars.enabled = true;
    }
}
#endregion

using System.Collections;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class BeatCombo : MonoBehaviour
{
    [SerializeField] PlayerScore pScore; //THIS WILL GET REMOVED THIS IS FOR TESTING SMILE 

    [SerializeField] float testBPM;

    [SerializeField] float lastBeatTime;
    [SerializeField] float lastHitTime;

    [SerializeField] float hitInterval;
    [SerializeField] float comboDecayTime;
    bool shouldScoreDecay = true;

    [SerializeField] int[] comboMults;
    [SerializeField] int curMultIndex = 0;

    [SerializeField] TextMeshProUGUI comboTF;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(BPMTimer());
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
  
            if (CheckBeat())
            {
                shouldScoreDecay = false;
                StartCoroutine(ComboDecayTimer());
            }
            pScore.UpdateScore(pScore.attackHitScoreIncrease);
        }
    }

    bool CheckBeat()
    {
        lastHitTime = Time.time;
        if (Mathf.Abs(lastBeatTime - lastHitTime) <= hitInterval )
        {
            //Debug.Log(Mathf.Abs(lastBeatTime - lastHitTime));
            //Debug.Log(" HIT!! LBT: " + lastBeatTime + ", " + "LHT: " + lastHitTime);

            IncreaseCombo();
            //Debug.Log(curMultIndex);
            pScore.UpdateScore((int)(pScore.attackHitScoreIncrease * comboMults[curMultIndex]));
            return true; //ONBEAT
        }

        ResetCombo();
        return false; //OFFBEAT
    }

    void ResetCombo()
    {
        curMultIndex = 0;
        comboTF.text = "x" + comboMults[curMultIndex].ToString();
        //Debug.Log("resetting combo");
    }

    void IncreaseCombo()
    {
        curMultIndex++;
        if (curMultIndex >= comboMults.Count())
        {
            curMultIndex = comboMults.Count() - 1;
        }
        comboTF.text = "x" + comboMults[curMultIndex].ToString();
    }

    IEnumerator ComboDecayTimer()
    {
        shouldScoreDecay = true;
        //Debug.Log("Starting Decay Timer");
        yield return new WaitForSeconds(comboDecayTime);
        if (shouldScoreDecay)
        {
            //Debug.Log("combo decayed");
            ResetCombo();
        }
    }

    //Change this to use FMOD's system
    IEnumerator BPMTimer()
    {
        yield return new WaitForSeconds(testBPM / 60.0f);
        lastBeatTime = Time.time;
        StartCoroutine(BPMTimer());
    }
}

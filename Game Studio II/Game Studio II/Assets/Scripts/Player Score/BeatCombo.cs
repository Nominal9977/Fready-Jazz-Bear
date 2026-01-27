using System.Collections;
using System.Linq;
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
    [SerializeField] int curMultIndex;
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
            lastHitTime = Time.time;
            if (CheckBeat())
            {
                shouldScoreDecay = false;
            }

            StartCoroutine(ComboDecayTimer());
        }
    }

    bool CheckBeat()
    {
       
        if(Mathf.Abs(lastBeatTime - lastHitTime) <= hitInterval )
        {
            //Debug.Log(Mathf.Abs(lastBeatTime - lastHitTime));
            //Debug.Log(" HIT!! LBT: " + lastBeatTime + ", " + "LHT: " + lastHitTime);

            IncreaseCombo();
            pScore.UpdateScore((int)(pScore.attackHitScoreIncrease * comboMults[curMultIndex]));
            return true;
        }

        ResetCombo();
        return false;
    }

    void ResetCombo()
    {
        curMultIndex = 0;
    }

    void IncreaseCombo()
    {
        curMultIndex++;
        if (curMultIndex >= comboMults.Count())
            curMultIndex = comboMults.Count();
    }

    IEnumerator ComboDecayTimer()
    {
        yield return new WaitForSeconds(comboDecayTime);
        if (shouldScoreDecay)
            ResetCombo();
    }

    //Change this to use FMOD's system
    IEnumerator BPMTimer()
    {
        yield return new WaitForSeconds(testBPM / 60.0f);
        lastBeatTime = Time.time;
        StartCoroutine(BPMTimer());
    }
}

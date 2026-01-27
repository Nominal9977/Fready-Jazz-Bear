using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using static ChannelNames;
public class PlayerScore : MonoBehaviour
{
    [SerializeField] int score;
    [SerializeField] int totalScore;

    [SerializeField] int attackHitScoreIncrease;
    [SerializeField] int playerHitScoreDecrease;

    [SerializeField] Threshold[] thresholds;
    int curThresholdIndex = 0;

    [SerializeField] TextMeshProUGUI scoreTF; //I know this is bad lol this is for testing
    [SerializeField] TextMeshProUGUI totalTF;
    [SerializeField] TextMeshProUGUI timerTF;
    float time;

    [Serializable]
   public struct Threshold
    {
        public int scoreNeeded;
        public float thresholdTime;
    }


  
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        time = thresholds[0].thresholdTime;
        UpdateTimer();
        //EventManager.Player.OnScoreChanged.Get().AddListener(UpdateScore); 
        StartCoroutine(PlaceHolderTimer(thresholds[0].thresholdTime));
        
    }

    private void Update()
    {
        //EventManager.Player.OnScoreChanged.Get(Default).Invoke(this, score);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            UpdateScore(attackHitScoreIncrease);
        }

        time -= Time.deltaTime;
        UpdateTimer();
    }

    public void UpdateTimer()
    {
        timerTF.text = time.ToString("00:00");
    }

    public void UpdateScore(int theScore)
    {
        score += theScore;
        scoreTF.text = score.ToString();
        
    }

    public void UpdateScore(Component component, int score)
    {
        Debug.Log("Updated score" + score.ToString());
        scoreTF.text = score.ToString();
    }


    public bool CheckScore(int scoreNeeded)
    {
        if (score >= scoreNeeded)
        {
            Debug.Log("progressing phases");
            totalScore += score;
            totalTF.text = totalScore.ToString();

            score = 0;
            scoreTF.text = score.ToString();

            

            curThresholdIndex++;
            if (curThresholdIndex >= thresholds.Count())
            {
                Debug.Log("Ending FIght" + curThresholdIndex);
                curThresholdIndex = 0;
                time = thresholds[0].thresholdTime;
                return false;
            }
            time = thresholds[curThresholdIndex].thresholdTime;
            timerTF.text = thresholds[curThresholdIndex].thresholdTime.ToString("00:00");

            return true;
        }
        Debug.Log("restart");
        score = 0;
        scoreTF.text = score.ToString();

        //Restart fight
        curThresholdIndex = 0;
        time = thresholds[0].thresholdTime;
        return false;
    }

    IEnumerator PlaceHolderTimer(float time)
    {
        yield return new WaitForSeconds(time);
        CheckScore(thresholds[curThresholdIndex].scoreNeeded);
        StartCoroutine(PlaceHolderTimer(thresholds[curThresholdIndex].thresholdTime));
    }


}

using System;
using System.Collections;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static ChannelNames;
public class PlayerScore : MonoBehaviour
{
    [SerializeField] BeatCombo bCombo; //for prototyping

    [SerializeField] int score;
    [SerializeField] int totalScore;

    [SerializeField] public int attackHitScoreIncrease;
    [SerializeField] int playerHitScoreDecrease;

    [SerializeField] Threshold[] thresholds;
    int curThresholdIndex = 0;

    [SerializeField] float scoreMaintainTime;

    bool scoreMaintained = false; //is the score great enough 
    bool couldScoreMaintain = true; //should start score maintain timer 

    [SerializeField] float scoreBleedTime;
    [SerializeField] float scoreBleedAmount;
    bool couldScoreBleed = true;
    bool shouldScoreBleed = true;
    bool scoreBleed = false; 

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
        StartCoroutine(PhaseTimer(thresholds[0].thresholdTime));
        StartCoroutine(ScoreBleedTimer());
    }

    private void Update()
    {
        //EventManager.Player.OnScoreChanged.Get(Default).Invoke(this, score);
        if (Input.GetKeyDown(KeyCode.Space)) //Change this with when player hits boss
        {
            //UpdateScore(attackHitScoreIncrease);

            shouldScoreBleed = false;

            if (scoreBleed)
            {
                scoreBleed = false;

                couldScoreBleed = true;
                shouldScoreBleed = true;
                Debug.Log("score bleed: " + scoreBleed + " couldSB: " + couldScoreBleed + "...Starting Score Bleed");
                StartCoroutine(ScoreBleedTimer());

            }

        }

        if (!CheckScore())
        {
            couldScoreMaintain = true;
        }

        time -= Time.deltaTime;
        UpdateTimer();

        if (scoreBleed)
            ScoreBleed();
    }

    public void ScoreBleed()
    {
        UpdateScore((int)-scoreBleedAmount);
    }

    public void ResetScoreBleed()
    {
        couldScoreBleed = false; // could timer
        shouldScoreBleed = false;
        scoreBleed = false;
    }

    public void UpdateTimer()
    {
        timerTF.text = time.ToString("00:00");
    }

    public void UpdateScore(int theScore)
    {
        //Debug.Log("adding " + theScore + " to score");
        score += theScore;
        scoreTF.text = score.ToString();
        
    }

    public void UpdateScore(Component component, int score)
    {
        Debug.Log("Updated score" + score.ToString());
        scoreTF.text = score.ToString();
    }

    public bool CheckScore()
    {
        if (score >= thresholds[curThresholdIndex].scoreNeeded)
        {
            StartCoroutine(ScoreMaintainTimer());
            return true;
        }
        else
        {
            scoreMaintained = false;
            return false;
        }
    }

    public void CheckScoreMaintain()
    {
        Debug.Log("checking maintained score");
        if (scoreMaintained)
        {
            ProgressThreshold();
            RefreshThreshold();
            StartCoroutine(PhaseTimer(thresholds[curThresholdIndex].thresholdTime));
        }
    }

    public bool CheckScorePhaseEnd()
    {
        if (score >= thresholds[curThresholdIndex].scoreNeeded)
        {
            ProgressThreshold();
            RefreshThreshold();
            StartCoroutine(PhaseTimer(thresholds[curThresholdIndex].thresholdTime));
            return true;
        }
        else
        {
            ResetSystem();
            StartCoroutine(PhaseTimer(thresholds[curThresholdIndex].thresholdTime)); ;
            return false;
        }
    }

    void ProgressThreshold()
    {
        Debug.Log("progressing threshold");
        curThresholdIndex++;
        if(curThresholdIndex >= thresholds.Count())
        {
            curThresholdIndex = thresholds.Count() - 1;
        }
    }

    void RefreshThreshold()
    {
        couldScoreMaintain = true;
        scoreMaintained = false;

        totalScore += score;
        score = 0;
        scoreTF.text = score.ToString();
        totalTF.text = totalScore.ToString();
        time = thresholds[curThresholdIndex].thresholdTime;
        timerTF.text = thresholds[curThresholdIndex].thresholdTime.ToString("00:00");
    }

    void ResetThreshold()
    {
        curThresholdIndex = 0;
    }

    void ResetScore()
    {
        score = 0;;
        curThresholdIndex = 0;
        scoreTF.text = "0";
        totalTF.text = "0";
    }

    void ResetTime()
    {
        time = thresholds[0].thresholdTime;
        timerTF.text = thresholds[0].thresholdTime.ToString("00:00");
    }

    void ResetSystem()
    {
        couldScoreMaintain = true;
        ResetScore();
        ResetThreshold();
        ResetTime();
        
    }

    IEnumerator ScoreBleedTimer()
    {
        if(couldScoreBleed)
        {
            couldScoreBleed = false;
            yield return new WaitForSeconds(scoreBleedTime);
            if (shouldScoreBleed)
            {
                scoreBleed = true;
                shouldScoreBleed = false;
            }
            else
            {
                couldScoreBleed = true;
            }
        }
    }

    IEnumerator ScoreMaintainTimer()
    {
        scoreMaintained = true;
        if (couldScoreMaintain)
        {
            couldScoreMaintain = false;
            yield return new WaitForSeconds(scoreMaintainTime);
            CheckScoreMaintain();
        }
    }

    IEnumerator PhaseTimer(float time)
    {
        yield return new WaitForSeconds(time);
        CheckScorePhaseEnd();
    }


}

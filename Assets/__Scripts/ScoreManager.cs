using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// An enum to handle all the possible scoring events 
public enum eScoreEvent
{
    draw,
    mine,
    mineGold,
    gameWin,
    gameLoss
}
// ScoreManager handles all of the scoring
public class ScoreManager : MonoBehaviour
{
    static private ScoreManager S;

    static public int SCORE_FROM_PREV_ROUND = 0;
    static public int HIGH_SCORE = 0;

    [Header("Set Dynamically")]

    public int chain = 0;
    public int scoreRun = 0;
    public int score = 0;

    void Awake()
    {
        if (S == null)
        {
            S = this;
        }
        else
        {
            Debug.LogError("ERROR: ScoreManager.Awake(): S is already set!");
        }


        if (PlayerPrefs.HasKey("ProspectorHighScore"))
        {
            HIGH_SCORE = PlayerPrefs.GetInt("ProspectorHighScore");
        }

        score += SCORE_FROM_PREV_ROUND;
        SCORE_FROM_PREV_ROUND = 0;
    }

    static public void EVENT(eScoreEvent evt, bool gold)
    {
        try
        {
            S.Event(evt, gold);
        }
        catch (System.NullReferenceException nre)
        {
            Debug.LogError("ScoreManager:EVENT() called while S=null. \n" + nre);
        }
    }

    void Event(eScoreEvent evt, bool gold)
    {
        switch (evt)
        {

            case eScoreEvent.draw: 
            case eScoreEvent.gameWin: 
            case eScoreEvent.gameLoss: 
                chain = 0; 
                score += scoreRun; 
                scoreRun = 0; 
                break;

            case eScoreEvent.mine:
                if (gold)
                {
                    chain += 2;
                    scoreRun += 1;
                }
                else
                {
                    chain++; 
                }
                scoreRun += chain;
                break;
        }


        switch (evt)
        {
            case eScoreEvent.gameWin:
                SCORE_FROM_PREV_ROUND = score;
                print("You won this round! Round score: " + score);
                break;

            case eScoreEvent.gameLoss:
                if (HIGH_SCORE <= score)
                {
                    print(" You got the high score! High Score: " + score);
                    HIGH_SCORE = score;
                    PlayerPrefs.SetInt("ProspectorHighScore", score);
                }
                else
                {
                    print("Your final score for the game was: " + score);
                }
                break;
        }
    }

    static public int CHAIN { get { return S.chain; } }
    static public int SCORE { get { return S.score; } }
    static public int SCORE_RUN { get { return S.scoreRun; } }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class M_Score : MonoBehaviour
{
    int score;
    public Text scoreText;
    

    public void ModifyScore(int amount)
    {
        score += amount;
        scoreText.text = score.ToString();
    }
}

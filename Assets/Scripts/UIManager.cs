using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager>
{
    public TextMeshProUGUI score;
    public GameObject gameOver;

    public void UpdateScore(float score)
    {
        this.score.text = "Score: " + score.ToString("#");
    }

    public void DisplayGameOverButton()
    {
        this.gameOver.SetActive(true);
    }

    public void Replay()
    {
        SceneManager.LoadScene("Menu");
    }

}

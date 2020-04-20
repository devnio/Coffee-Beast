using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    
    public void Play()
    {
        SceneManager.LoadScene("SampleScene");
        Time.timeScale = 1f;
    }
}

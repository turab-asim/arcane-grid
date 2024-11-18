using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MatchResultUnit : MonoBehaviour
{
    public static MatchResultUnit instance;
    public GameObject window;
    public TextMeshProUGUI matchResultText;

    private void Awake()
    {
        instance = this;
    }


    public void ShowVictor(int pid)
    {
        window.SetActive(true);
        if (pid != -1)
        {
            matchResultText.text = "Player " + (pid + 1) + " Victory!";
        }
        else
        {
            matchResultText.text = "Match Draw!";
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
}

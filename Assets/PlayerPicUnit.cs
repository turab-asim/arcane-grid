using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerPicUnit : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI playerName;
    public TextMeshProUGUI manaStore;
    public TextMeshProUGUI timerValue;
    public GameObject timerObj;
    public Image timerFill;

    public void SetTime(float currentTime, float maxTime)
    {
        timerFill.fillAmount = 1 - (currentTime / maxTime);
        timerValue.text = Mathf.RoundToInt(maxTime - currentTime).ToString();
    }

    public void SetTurn(bool state)
    {
        if (state)
        {
            canvasGroup.alpha = 1f;
            timerObj.SetActive(true);
        }
        else
        {
            canvasGroup.alpha = 0.5f;
            timerObj.SetActive(false);
        }
    }
}

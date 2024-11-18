using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameMasterUnit : MonoBehaviour
{
    public static GameMasterUnit instance;

    public RectTransform hudCanvas;

    public TextMeshProUGUI turnText;
    public List<SpellCardUnit> spellCardFabs;
    public List<SpellCardUnit> ultimateCardFabs;
    public List<SpellCardUnit> runeCardFabs;
    public List<GameObject> runeObjs;

    public List<PlayerPicUnit> playerPicUnits;

    public int currentTurn;
    public int turnCount;
    public int maxTurn=60;

    public int initialSpellCount = 4;
    public int initialRuneCount = 1;

    public float playerTime;

    public List<PlayerHUDUnit> playerHUDUnits;

    private void Awake()
    {
        instance = this;
    }

    public void InitializePlayerHUD(int pid,CardElementType ultimateId)
    {
        playerHUDUnits[pid].Initialize((ultimateId));

        if (playerHUDUnits[0].initialized && playerHUDUnits[1].initialized)
        {
            BothPlayersReady();
        }
    }

    public void BothPlayersReady()
    {
        StartCoroutine("PlayerTimer");
        turnCount = 1;
        playerHUDUnits[currentTurn].NewTurn();
        UpdateTurnCount();
    }

    public void SwitchTurn()
    {
        currentTurn = GetOtherPlayer(currentTurn);

        StopCoroutine("PlayerTimer");
        StartCoroutine("PlayerTimer");

        playerHUDUnits[currentTurn].NewTurn();

        turnCount++;

        UpdateTurnCount();


        BoardMasterUnit.instance.CheckForVictory();
    }

    void UpdateTurnCount()
    {
        turnText.text = turnCount + "/" + maxTurn;

        if (turnCount == maxTurn)
        {
            int tilesOwned0 = BoardMasterUnit.instance.GetTilesOwnedByPlayer(0);
            int tilesOwned1 = BoardMasterUnit.instance.GetTilesOwnedByPlayer(1);

            if (tilesOwned0 > tilesOwned1)
            {
                MatchResultUnit.instance.ShowVictor(0);
            }
            else if(tilesOwned1 > tilesOwned0)
            {
                MatchResultUnit.instance.ShowVictor(1);
            }
            else
            {
                MatchResultUnit.instance.ShowVictor(-1);
            }
        }
    }


    public IEnumerator PlayerTimer()
    {
        playerHUDUnits[0].gameObject.SetActive(false);
        playerHUDUnits[1].gameObject.SetActive(false);

        playerPicUnits[0].SetTime(0, playerTime);
        playerPicUnits[1].SetTime(0, playerTime);

        playerPicUnits[0].SetTurn(false);
        playerPicUnits[1].SetTurn(false);

        playerPicUnits[currentTurn].SetTurn(true);
        playerHUDUnits[currentTurn].gameObject.SetActive(true);
        
        float temp = 0;

        while (temp < playerTime)
        {
            temp += Time.deltaTime;

            playerPicUnits[currentTurn].SetTime(temp, playerTime);
            yield return null;
        }

        MatchResultUnit.instance.ShowVictor(GetOtherPlayer(currentTurn));
    }

    public int GetOtherPlayer(int pid)
    {
        if(pid == 0)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
}

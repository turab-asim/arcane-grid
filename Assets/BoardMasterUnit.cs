using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardMasterUnit : MonoBehaviour
{
    public static BoardMasterUnit instance;
    public List<BoardCellUnit> bcus;
    public List<BoardRowUnit> brus;
    public int mountainCount;

    public SpellCardUnit currentlySelectedCard;

    private void Awake()
    {
        instance = this;
    }

    public void SelectCard(SpellCardUnit scu)
    {
        currentlySelectedCard = scu;
    }

    public void DeselectCard()
    {
        currentlySelectedCard = null;

        for (int i = 0; i < bcus.Count; i++)
        {
            bcus[i].SetSelect(false);
        }

    }

    public void SelectAccordingSCU(BoardCellUnit targetCell)
    {

        DeselectCells();

        for(int i=0; i< currentlySelectedCard.effectCells.Count; i++)
        {
            Vector2Int target = targetCell.coordinate + currentlySelectedCard.effectCells[i];
            BoardCellUnit bcu = GetCellUnitByCoordinate(target);
            if (bcu != null)
            {
                bcu.SetSelect(true);
            }
        }
    }

    public void DeselectCells()
    {
        for (int i = 0; i < bcus.Count; i++)
        {
            bcus[i].SetSelect(false);
        }
    }

    public void SetOwnership(Vector2Int cellId,int id)
    {

        BoardCellUnit bcu = GetCell(cellId);
        if (bcu != null)
        {
            bcu.SetOwnership(id);
        }
    }

    public BoardCellUnit GetCell(Vector2Int cellId)
    {
        if (cellId.x >= 0 && cellId.x < brus.Count && cellId.y >= 0 && cellId.y < brus.Count)
        {
            return brus[cellId.y].bcus[cellId.x];
        }
        else
        {
            return null;
        }
    }

    public void CastCard(SpellCardUnit scu,Vector2Int origin)
    {
        if (GameMasterUnit.instance.playerHUDUnits[scu.ownerId].manaPool >= scu.manaCost)
        {
            GameMasterUnit.instance.playerHUDUnits[scu.ownerId].SpendMana(scu.manaCost);
        }
        else
        {
            return;
        }

        for (int i=0; i<scu.effectCells.Count; i++)
        {
            Vector2Int targetCell = origin + scu.effectCells[i];

            if(scu.elementType != CardElementType.Chaos)
            {
                SetOwnership(targetCell, scu.ownerId);
            }
            else
            {
                BoardCellUnit bcu = GetCell(targetCell);
                if (bcu != null)
                {
                    bcu.RandomizeStatus();
                }
            }

            if (scu.elementType == CardElementType.Earth)
            {
                BoardCellUnit bcu = GetCell(targetCell);
                if (bcu != null)
                {
                    bcu.SetFortification(true);
                }
            }
        }

        if(scu.elementType == CardElementType.Water)
        {
            GameMasterUnit.instance.playerHUDUnits[scu.ownerId].DrawRandomCard();
        }else if(scu.elementType == CardElementType.Fire)
        {
            GameMasterUnit.instance.playerHUDUnits[GameMasterUnit.instance.GetOtherPlayer(scu.ownerId)].DiscardRandomCard();
        }

        if(scu.elementType != CardElementType.Wind)
        {
            GameMasterUnit.instance.SwitchTurn();
        }
        CheckForVictory();
        GameObject.Destroy(scu.gameObject);
    }

    public void CheckForVictory()
    {
        GameMasterUnit.instance.playerHUDUnits[0].tilesOwned = GetTilesOwnedByPlayer(0);
        GameMasterUnit.instance.playerHUDUnits[1].tilesOwned = GetTilesOwnedByPlayer(1);

        GameMasterUnit.instance.playerHUDUnits[0].UpdateTilesOwned();
        GameMasterUnit.instance.playerHUDUnits[1].UpdateTilesOwned();

        if (GameMasterUnit.instance.playerHUDUnits[0].tilesOwned == 33)
        {
            MatchResultUnit.instance.ShowVictor(0);
        }else if (GameMasterUnit.instance.playerHUDUnits[1].tilesOwned == 33)
        {
            MatchResultUnit.instance.ShowVictor(1);
        }
    }

    public int GetTilesOwnedByPlayer(int pid)
    {
        int tileOwned = 0;
        for (int i = 0; i < bcus.Count; i++)
        {
            if (bcus[i].idCache == pid)
            {
                tileOwned++;
            }
        }

        return tileOwned;
    }

    public void CastUltimate(SpellCardUnit scu,Vector2Int origin)
    {
        if (GameMasterUnit.instance.playerHUDUnits[scu.ownerId].manaPool >= scu.manaCost)
        {
            GameMasterUnit.instance.playerHUDUnits[scu.ownerId].SpendMana(scu.manaCost);
        }
        else
        {
            return;
        }

        scu.PutOnCooldown();

        for (int i = 0; i < scu.effectCells.Count; i++)
        {
            Vector2Int targetCell = origin + scu.effectCells[i];
            if (scu.elementType != CardElementType.Chaos)
            {
                SetOwnership(targetCell, scu.ownerId);
            }
            else
            {
                BoardCellUnit bcu = GetCell(targetCell);
                if (bcu != null)
                {
                    bcu.RandomizeStatus();
                }
            }

            if (scu.elementType == CardElementType.Earth)
            {
                BoardCellUnit bcu = GetCell(targetCell);
                if (bcu != null)
                {
                    bcu.SetFortification(true);
                }
            }
        }

        if (scu.elementType != CardElementType.Wind)
        {
            GameMasterUnit.instance.SwitchTurn();
        }
        CheckForVictory();
    }

    public void CastRune(BoardCellUnit bcu, SpellCardUnit scu)
    {
        if(GameMasterUnit.instance.playerHUDUnits[scu.ownerId].manaPool>= scu.manaCost)
        {
            GameMasterUnit.instance.playerHUDUnits[scu.ownerId].SpendMana(scu.manaCost);
        }
        else
        {
            return;
        }

        GameObject g = Instantiate(GameMasterUnit.instance.runeObjs[(int)scu.elementType]);
        g.transform.position = bcu.transform.position;
        g.GetComponent<RunePieceUnit>().Initialize(scu.ownerId, bcu);

        GameMasterUnit.instance.playerHUDUnits[scu.ownerId].AddActiveRune(g.GetComponent<RunePieceUnit>());
        g.GetComponent<RunePieceUnit>().Initialize(scu.ownerId, bcu);

        GameObject.Destroy(scu.gameObject);

        if (scu.elementType != CardElementType.Wind)
        {
            GameMasterUnit.instance.SwitchTurn();
        }

        CheckForVictory();
    }

    public void RuneEffect(RunePieceUnit scu)
    {
        for (int i = 0; i < scu.effectCells.Count; i++)
        {
            Vector2Int targetCell = scu.origin.coordinate + scu.effectCells[i];
            if (scu.elementType != CardElementType.Chaos)
            {
                SetOwnership(targetCell, scu.ownerId);
            }
            else
            {
                BoardCellUnit bcu = GetCell(targetCell);
                if (bcu != null)
                {
                    bcu.RandomizeStatus();
                }
            }


            if (scu.elementType == CardElementType.Earth)
            {
                BoardCellUnit bcu = GetCell(targetCell);
                if (bcu != null)
                {
                    bcu.SetFortification(true);
                }
            }

            if (scu.elementType == CardElementType.Water)
            {
                GameMasterUnit.instance.playerHUDUnits[scu.ownerId].DrawRandomCard();
            }
            else if (scu.elementType == CardElementType.Fire)
            {
                GameMasterUnit.instance.playerHUDUnits[GameMasterUnit.instance.GetOtherPlayer(scu.ownerId)].DiscardRandomCard();
            }

        }
        CheckForVictory();
    }


    public List<int> GetRandomIntegers(int N, int M)
    {
        List<int> randomIntegers = new List<int>();

        if (M > N + 1)
        {
            Debug.LogWarning("M is greater than N + 1. Limiting M to N + 1.");
            M = N + 1;
        }

        List<int> availableNumbers = new List<int>();
        for (int i = 0; i <= N; i++)
        {
            availableNumbers.Add(i);
        }

        for (int i = 0; i < M; i++)
        {
            int randomIndex = Random.Range(0, availableNumbers.Count);
            randomIntegers.Add(availableNumbers[randomIndex]);
            availableNumbers.RemoveAt(randomIndex); 
        }

        return randomIntegers;
    }


    public BoardCellUnit GetCellUnitByCoordinate(Vector2Int coordinate)
    {
        if(coordinate.x<0 || coordinate.x >5 || coordinate.y < 0 || coordinate.y > 5)
        {
            return null;
        }
        return brus[coordinate.y].bcus[coordinate.x];
    }



    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        List<int> rids = GetRandomIntegers(bcus.Count-1, mountainCount);

        for(int i=0; i<rids.Count; i++)
        {
            bcus[rids[i]].SetMountain(true);
        }
    }
}



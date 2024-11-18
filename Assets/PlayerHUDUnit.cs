using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHUDUnit : MonoBehaviour
{
    public int pid;
    public Transform selectUltimateRoot;
    public Transform spellCardRoot;
    public Transform runeCardRoot;
    public GameObject ultimatePlaceholder;
    public SpellCardUnit ultimateSCU;
    public List<GameObject> placeholderSpells;
    public List<GameObject> placeholderRunes;
    public List<RunePieceUnit> activeRunes;
    public TextMeshProUGUI manaText;
    public TextMeshProUGUI tilesOwnedCount;
    public int cardinHandLimit=10;
    public int runeInHandLimit = 6;
    public int tilesOwned;
    public int manaPool = 6;

    public int runeCounter;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void AddActiveRune(RunePieceUnit rpu)
    {
        activeRunes.Add(rpu);
    }

    public void CheckActiveRunes()
    {
        for(int i=0; i<activeRunes.Count; i++)
        {
            if (activeRunes[i] != null)
            {
                activeRunes[i].NewTurn();
            }
        }
    }

    public void SpendMana(int amount)
    {
        manaPool -= amount;
        UpdateManaPool();
    }

    public void UpdateTilesOwned()
    {
        tilesOwnedCount.text = tilesOwned.ToString();
    }

    public bool initialized;
    public void Initialize(CardElementType ultimateType)
    {
        SelectUltimate(ultimateType);
        GrantRandomCards();
        GrantRandomRunes();
        initialized = true;
    }

    public void SelectUltimate(CardElementType cet)
    {
        ultimatePlaceholder.SetActive(false);
        GameObject g = Instantiate(GameMasterUnit.instance.ultimateCardFabs[(int)cet].gameObject, selectUltimateRoot);
        g.GetComponent<CardDragCopy>().Initialize(GameMasterUnit.instance.hudCanvas);
        g.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
        ultimateSCU = g.GetComponent<SpellCardUnit>();
        ultimateSCU.ownerId = pid;
        ultimateSCU.PutOnCooldown();
    }

    public void DiscardRandomCard()
    {
        if (spellCardRoot.childCount > 0)
        {
            int randomCard = Random.Range(0, spellCardRoot.childCount);
            GameObject.Destroy(spellCardRoot.GetChild(randomCard).gameObject);
        }
    }

    public void GrantRandomCards()
    {
        for (int i = 0; i < placeholderSpells.Count; i++)
        {
            GameObject.Destroy(placeholderSpells[i]);
        }

        List<int> randomCards = SelectRandomNumbers(GameMasterUnit.instance.initialSpellCount, GameMasterUnit.instance.spellCardFabs.Count - 1);

        for (int i = 0; i < randomCards.Count; i++)
        {
            GameObject g = Instantiate(GameMasterUnit.instance.spellCardFabs[randomCards[i]].gameObject, spellCardRoot);
            CardDragCopy cdc = g.GetComponent<CardDragCopy>();
            g.GetComponent<SpellCardUnit>().ownerId = pid;
            cdc.Initialize(GameMasterUnit.instance.hudCanvas);
        }
    }

    public void DrawCardSwitchTurn()
    {
        DrawRandomCard();
        GameMasterUnit.instance.SwitchTurn();
    }

    public void NewTurn()
    {
        DrawRandomCard();   

        runeCounter++;

        manaPool += 2;
        UpdateManaPool();

        ultimateSCU.DecCooldown();

        CheckActiveRunes();
        if (runeCounter == 3)
        {
            runeCounter = 0;
            DrawRandomRune();
        }
    }

    public void DrawRandomCard()
    {
        if(spellCardRoot.childCount>= cardinHandLimit)
        {
            return;
        }
        Debug.Log("DrawRandomCard");
        int randomCard = Random.Range(0, GameMasterUnit.instance.spellCardFabs.Count);
        GameObject g = Instantiate(GameMasterUnit.instance.spellCardFabs[randomCard].gameObject, spellCardRoot);
        CardDragCopy cdc = g.GetComponent<CardDragCopy>();
        g.GetComponent<SpellCardUnit>().ownerId = pid;
        cdc.Initialize(GameMasterUnit.instance.hudCanvas);
    }

    public void UpdateManaPool()
    {
        manaText.text = manaPool.ToString();
    }

    public void DrawRandomRune()
    {
        if(runeCardRoot.childCount>= runeInHandLimit)
        {
            return;
        }
        int randomRune = Random.Range(0, GameMasterUnit.instance.runeCardFabs.Count);
        GameObject g = Instantiate(GameMasterUnit.instance.runeCardFabs[randomRune].gameObject, runeCardRoot);
        CardDragCopy cdc = g.GetComponent<CardDragCopy>();
        g.GetComponent<SpellCardUnit>().ownerId = pid;
        cdc.Initialize(GameMasterUnit.instance.hudCanvas);
    }

    public void GrantRandomRunes()
    {
        for (int i = 0; i < placeholderRunes.Count; i++)
        {
            GameObject.Destroy(placeholderRunes[i]);
        }

        List<int> randomCards = SelectRandomNumbers(GameMasterUnit.instance.initialRuneCount, GameMasterUnit.instance.runeCardFabs.Count - 1);

        for (int i = 0; i < randomCards.Count; i++)
        {
            GameObject g = Instantiate(GameMasterUnit.instance.runeCardFabs[randomCards[i]].gameObject, runeCardRoot);
            CardDragCopy cdc = g.GetComponent<CardDragCopy>();
            g.GetComponent<SpellCardUnit>().ownerId = pid;
            cdc.Initialize(GameMasterUnit.instance.hudCanvas);
        }
    }

    public List<int> SelectRandomNumbers(int N, int M)
    {
        List<int> selectedNumbers = new List<int>();

        List<int> numberPool = new List<int>();
        for (int i = 0; i <= M; i++)
        {
            numberPool.Add(i);
        }

        N = Mathf.Min(N, M + 1);

        for (int i = 0; i < N; i++)
        {
            int randomIndex = Random.Range(0, numberPool.Count);
            selectedNumbers.Add(numberPool[randomIndex]);
            numberPool.RemoveAt(randomIndex);
        }

        return selectedNumbers;
    }
}

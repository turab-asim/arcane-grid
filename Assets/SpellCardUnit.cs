using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpellCardUnit : MonoBehaviour
{
    public int cardId;
    public CardType cardType;
    public CardElementType elementType;
    public List<Vector2Int> effectCells;
    public int manaCost;
    public int ownerId;
    public GameObject cooldownObject;
    public int castCooldown;
    public int cooldownTime;
    public TextMeshProUGUI tmp;

    void Start()
    {

    }

    public void DecCooldown()
    {
        if (isOnCooldown())
        {
            cooldownTime--;
        }

        if (cooldownTime == 0)
        {
            PutOffCooldown();
        }

        tmp.text = cooldownTime.ToString();
    }

    public void PutOnCooldown()
    {
        cooldownObject.SetActive(true);
        cooldownTime = castCooldown;
        tmp.text = cooldownTime.ToString();
    }

    public void PutOffCooldown()
    {
        cooldownObject.SetActive(false);
    }

    public bool isOnCooldown()
    {
        if (cooldownTime > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

public enum CardType
{
    spell,
    rune,
    ultimate
}

public enum CardElementType
{
    Fire,
    Water,
    Wind,
    Earth,
    Chaos
}

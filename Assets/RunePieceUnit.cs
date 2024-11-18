using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunePieceUnit : MonoBehaviour
{
    public CardElementType elementType;
    public int ownerId;
    public List<Vector2Int> effectCells;
    public List<GameObject> ownershipVisuals;
    public int maxLife = 2;
    public int countLeft;
    public BoardCellUnit origin;

    public void Initialize(int ownerId, BoardCellUnit castOrigin)
    {
        this.ownerId = ownerId;
        SetOwnership(ownerId);
        origin = castOrigin;
        countLeft = maxLife;
        ApplyEffect();
    }

    public void NewTurn()
    {
        countLeft--;
        ApplyEffect();

        if (countLeft == 0)
        {
            GameObject.Destroy(this.gameObject);
        }
    }

    public void ApplyEffect()
    {
        BoardMasterUnit.instance.RuneEffect(this);
    }

    public void SetOwnership(int id)
    {
        for(int i=0; i<ownershipVisuals.Count; i++)
        {
            ownershipVisuals[i].SetActive(false);
        }

        ownershipVisuals[id].SetActive(true);
    }
}

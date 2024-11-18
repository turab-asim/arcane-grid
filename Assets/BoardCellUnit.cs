using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardCellUnit : MonoBehaviour
{
    public List<GameObject> ownershipStatusVisuals;
    public GameObject fortifiactionVisual;
    public GameObject mountainVisual;
    public GameObject selectVisual;
    public Vector2Int coordinate;

    public bool isMountain;

    public int idCache = -1;
    public void SetOwnership(int id,bool force=false)
    {
        if (isMountain)
        {
            return;
        }

        if (isFortified&& idCache != id&&!force)
        {
            SetFortification(false);
            return;
        }

        for (int i = 0; i < ownershipStatusVisuals.Count; i++)
        {
            ownershipStatusVisuals[i].SetActive(false);
        }
        if (id == 0)
        {
            ownershipStatusVisuals[1].SetActive(true);
        }
        else if (id == 1)
        {
            ownershipStatusVisuals[2].SetActive(true);
        }
        else if (id == -1)
        {
            ownershipStatusVisuals[0].SetActive(true);
        }

        idCache = id;
    }

    public bool isFortified;

    public void SetFortification(bool state)
    {
        if (isMountain)
        {
            return;
        }
        fortifiactionVisual.SetActive(state);
        isFortified = state;
    }

    public void RandomizeStatus()
    {
        if (isMountain)
        {
            return;
        }
        int randomOwnership = Random.Range(-1, 2);
        SetOwnership(randomOwnership,true);

        int randomFortification = Random.Range(0, 2);

        if(randomFortification == 0)
        {
            SetFortification(false);
        }
        else
        {
            SetFortification(true);
        }
    }

    public void SetMountain(bool state)
    {
        mountainVisual.SetActive(state);

        isMountain = state;
    }

    public void SetSelect(bool state)
    {
        selectVisual.SetActive(state);
    }

    public void SelectCell()
    {
        BoardMasterUnit.instance.SelectAccordingSCU(this);
    }
}

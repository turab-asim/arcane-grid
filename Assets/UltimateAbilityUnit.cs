using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltimateAbilityUnit : MonoBehaviour
{
    public List<GameObject> ultimateVisuals;

   public void DevLeft()
    {
        int currentlyActive = -1;

        for(int i=0; i<ultimateVisuals.Count; i++)
        {
            if (ultimateVisuals[i].activeSelf)
            {
                currentlyActive = i;
                break;
            }
        }

        ultimateVisuals[currentlyActive].SetActive(false);

        currentlyActive--;
        currentlyActive = currentlyActive % ultimateVisuals.Count;


        ultimateVisuals[currentlyActive].SetActive(true);
    }

    public void DevRight()
    {
        int currentlyActive = -1;

        for (int i = 0; i < ultimateVisuals.Count; i++)
        {
            if (ultimateVisuals[i].activeSelf)
            {
                currentlyActive = i;
                break;
            }
        }

        ultimateVisuals[currentlyActive].SetActive(false);

        currentlyActive++;
        currentlyActive = currentlyActive % ultimateVisuals.Count;


        ultimateVisuals[currentlyActive].SetActive(true);
    }
}

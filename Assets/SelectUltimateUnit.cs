using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectUltimateUnit : MonoBehaviour
{
    public GameObject windowRoot;
    public SpellCardUnit scu;

    public void SelectUltimate()
    {
        GameMasterUnit.instance.InitializePlayerHUD(scu.ownerId, scu.elementType);
        windowRoot.SetActive(false);
    }
}

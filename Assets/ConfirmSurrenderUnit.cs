using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmSurrenderUnit : MonoBehaviour
{
    public int pid;
    public GameObject window;

    public void ConfirmSurrender()
    {
        MatchResultUnit.instance.ShowVictor(GameMasterUnit.instance.GetOtherPlayer(pid));
        window.SetActive(false);
    }


}

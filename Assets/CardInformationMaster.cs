using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardInformationMaster : MonoBehaviour
{
    public static CardInformationMaster instance;
    public Transform cardVisualRoot;

    public Vector3 cardScale;
    public GameObject window;
    public TextMeshProUGUI cardDescription;
    public TextMeshProUGUI cardTip;

    private void Awake()
    {
        instance = this;
    }

    public void ShowCardInfo(CardDragCopy cdc) {
        window.SetActive(true);
        cardDescription.text = cdc.description;
        cardTip.text = cdc.tip;

        GameObject g = Instantiate(cdc.gameObject, cardVisualRoot);
        g.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;

        g.GetComponent<CardDragCopy>().enabled = false;

        SpellCardUnit scu = g.GetComponent<SpellCardUnit>();

        if(scu.cardType == CardType.ultimate)
        {
            scu.cooldownObject.SetActive(false);
        }
        g.transform.localScale = cardScale;
    }
}

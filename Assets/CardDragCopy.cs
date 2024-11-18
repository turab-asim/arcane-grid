using UnityEngine;
using UnityEngine.EventSystems;

public class CardDragCopy : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IPointerClickHandler
{
    public float scaleMultiplier = 1f;
    public RectTransform rectTransform;
    public RectTransform canvasRect;
    public SpellCardUnit scu;
    public GameObject overlay;
    GameObject cardCopy;
    GameObject overlayCopy;
    RectTransform cardCopyRectTransform;
    RectTransform overlayRectTransform;
    public Vector2 offset;

    public string description;
    public string tip;

    public void Initialize(RectTransform cr)
    {
        this.canvasRect = cr;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (scu.isOnCooldown())
        {
            return;
        }

        BoardMasterUnit.instance.SelectCard(scu);
        CreateCardCopy(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (scu.isOnCooldown())
        {
            return;
        }

        if (cardCopy != null)
        {
            Destroy(cardCopy);
        }

        if (scu.cardType == CardType.rune && cellUnitCache!=null)
        {
            Debug.Log("Cast Rune");
            BoardMasterUnit.instance.CastRune(cellUnitCache,scu);
        }else if(scu.cardType == CardType.spell && cellUnitCache != null)
        {
            Debug.Log("Cast Card");
            BoardMasterUnit.instance.CastCard(scu, cellUnitCache.coordinate);
        }else if(scu.cardType == CardType.ultimate && cellUnitCache != null)
        {
            Debug.Log("Cast Ultimate");
            BoardMasterUnit.instance.CastUltimate(scu, cellUnitCache.coordinate);
        }

        cellUnitCache = null;
        BoardMasterUnit.instance.DeselectCard();

        
    }

    public BoardCellUnit cellUnitCache;

    public void OnDrag(PointerEventData eventData)
    {
        if (scu.isOnCooldown())
        {
            return;
        }

        if (cardCopy != null)
        {
            cardCopyRectTransform.anchoredPosition = eventData.position;
            Ray ray = Camera.main.ScreenPointToRay(eventData.position+offset);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("cell"))
                {
                    cellUnitCache = hit.collider.GetComponent<BoardCellUnit>();
                    if (cellUnitCache != null)
                    {
                        cellUnitCache.SelectCell();
                    }
                }
            }
            else
            {
                cellUnitCache = null;
                BoardMasterUnit.instance.DeselectCells();
            }
        }
    }

    private void CreateCardCopy(PointerEventData eventData)
    {
        cardCopy = Instantiate(gameObject, canvasRect.transform);
        overlayCopy = Instantiate(overlay, cardCopy.transform);
        overlayRectTransform = overlayCopy.GetComponent<RectTransform>();

        overlayRectTransform.anchoredPosition = Vector2.zero;
        cardCopyRectTransform = cardCopy.GetComponent<RectTransform>();
        cardCopyRectTransform.anchorMin = Vector2.zero;
        cardCopyRectTransform.anchorMax = Vector2.zero;
        cardCopyRectTransform.pivot = Vector2.one * 0.5f;

        cardCopyRectTransform.localScale = Vector3.one * scaleMultiplier;
        cardCopyRectTransform.anchoredPosition = eventData.position;

        CanvasGroup cardCopyCanvasGroup = cardCopy.GetComponent<CanvasGroup>();
        cardCopyCanvasGroup.alpha = 0.5f;
        cardCopyCanvasGroup.blocksRaycasts = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        CardInformationMaster.instance.ShowCardInfo(this);
    }
}

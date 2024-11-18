using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class ScaledHorizontalLayout : MonoBehaviour
{
    public float childWidth = 100f; // Width of the child element at scale 1
    public float childHeight = 100f; // Height of the child element at scale 1
    public float initialSpacing = 10f; // Initial space between elements (before scaling)
    public RectOffset padding = new RectOffset(); // Padding around the layout

    public RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        UpdateLayout();
    }


    void Update()
    {
        UpdateLayout();
    }

    public void UpdateLayout()
    {
        // Get the available space within the parent, considering padding
        float totalAvailableWidth = rectTransform.rect.width - padding.left - padding.right;
        float totalAvailableHeight = rectTransform.rect.height - padding.top - padding.bottom;

        // Count only the active children
        int childCount = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.activeInHierarchy)
            {
                childCount++;
            }
        }

        if (childCount == 0) return;

        // Calculate the total width of active children including initial spacing
        float totalChildrenWidth = (childWidth * childCount) + (initialSpacing * (childCount - 1));

        // Determine the scale factor based on the available space and desired child size
        float scaleX = Mathf.Min(1f, totalAvailableWidth / totalChildrenWidth);  // Scale for width
        float scaleY = Mathf.Min(1f, totalAvailableHeight / childHeight);        // Scale for height

        // Use the smaller of the two scales to maintain proportionality
        float scale = Mathf.Min(scaleX, scaleY);

        // Scale the spacing proportionally based on the child scale
        float scaledSpacing = initialSpacing * scale;
        float scaledChildWidth = childWidth * scale;
        float scaledChildHeight = childHeight * scale;

        // Calculate the total width occupied by the scaled active children and spacing
        float totalScaledChildrenWidth = (scaledChildWidth * childCount) + (scaledSpacing * (childCount - 1));

        // Calculate the starting X position to fully center the group of active elements
        float startingXPos = (totalAvailableWidth - totalScaledChildrenWidth) / 2 + padding.left;

        // Shift the position of all cards to the right by half a card width
        startingXPos += scaledChildWidth / 2;

        // Set each active child's size, scale, and position
        int activeChildIndex = 0; // To only consider active children
        for (int i = 0; i < transform.childCount; i++)
        {
            RectTransform child = transform.GetChild(i) as RectTransform;

            if (child == null || !child.gameObject.activeInHierarchy) continue;

            // Apply the calculated scale to the child's localScale
            child.localScale = new Vector3(scale, scale, 1f);

            // Calculate the position along the horizontal axis
            float xPos = startingXPos + (scaledChildWidth + scaledSpacing) * activeChildIndex;
            float yPos = (totalAvailableHeight - scaledChildHeight) / 2 + padding.top; // Center vertically

            // Set the child's anchored position
            child.anchoredPosition = new Vector2(xPos, -yPos);

            activeChildIndex++;
        }
    }
}

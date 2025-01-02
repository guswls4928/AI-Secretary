using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableCommands : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        if (canvas == null)
        {
            Debug.LogError("Canvas not found in parent hierarchy!");
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log($"Begin dragging {gameObject.name}");
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (rectTransform == null || canvas == null) return;

        // Move object based on drag event
        Vector2 moveDelta = eventData.delta / canvas.scaleFactor;
        rectTransform.anchoredPosition += moveDelta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log($"End dragging {gameObject.name}");
    }
}

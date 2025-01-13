using TMPro;
using UnityEngine.EventSystems;
using UnityEngine;

public class DraggableCommands : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private GameObject draggedObject;
    public GameObject currentSlot;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    // 드래그 시작 시
    public void OnBeginDrag(PointerEventData eventData)
    {
        draggedObject = gameObject; // 드래그된 명령어 객체 저장
    }

    // 드래그 중
    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor; // 드래그 따라 움직임
    }

    // 드래그 종료 시
    public void OnEndDrag(PointerEventData eventData)
    {
        // 드래그가 끝나면, 단축키 슬롯에 명령어를 설정
        if (currentSlot != null)
        {
            var slot = currentSlot.GetComponent<ShortcutCmdButton>();
            if (slot != null)
            {
                slot.SetCommand(draggedObject.GetComponent<TextMeshProUGUI>().text);
                Debug.Log($"명령어 \"{draggedObject.GetComponent<TextMeshProUGUI>().text}\"가 단축키 {slot.name}에 지정되었습니다.");
            }
        }
    }
}

using TMPro;
using UnityEngine.EventSystems;
using UnityEngine;

public class ShortcutCmdButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private string commandText = string.Empty;
    private TextMeshProUGUI buttonText;
    

    private void Awake()
    {
        // 버튼에 있는 TextMeshProUGUI 컴포넌트 캐싱
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
    }

    // 명령어를 단축키 슬롯에 드래그하여 놓았을 때
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            var draggedCmd = eventData.pointerDrag.GetComponent<DraggableCommands>();
            if (draggedCmd != null)
            {
                draggedCmd.currentSlot = gameObject; // 명령어가 놓일 슬롯 설정
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            var draggedCmd = eventData.pointerDrag.GetComponent<DraggableCommands>();
            if (draggedCmd != null)
            {
                draggedCmd.currentSlot = null; // 드래그가 슬롯을 벗어나면 슬롯을 null로 설정
            }
        }
    }

    // 명령어를 단축키 버튼에 설정
    public void SetCommand(string command)
    {
        commandText = command;
        buttonText.text = command;
        gameObject.SetActive(true);
    }

    // 버튼 클릭 시 명령어 실행
    public void OnPointerClick(PointerEventData eventData)
    {
        if (string.IsNullOrEmpty(commandText))
            return;

        Debug.Log($"단축키 실행: {commandText}");

        FloatingCommands.Instance.EnterCommand(commandText);
    }

}

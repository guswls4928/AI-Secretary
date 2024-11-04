using UnityEngine;

public class CurrentCommand : MonoBehaviour
{
    public GameObject CurrentCommandPrefab;
    private GameObject curCommandInstance;

    public void CreateOrUpdateCurCommand(string commandText)
    {
        if (curCommandInstance == null)
        {
            curCommandInstance = Instantiate(CurrentCommandPrefab, transform);
            RectTransform rectTransform = curCommandInstance.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = Vector3.zero;
        }

        UpdateCurCommandText(commandText);
    }

    private void UpdateCurCommandText(string commandText)
    {
        TextMesh textMesh = curCommandInstance.GetComponent<TextMesh>();
        textMesh.text = commandText;
    }
}

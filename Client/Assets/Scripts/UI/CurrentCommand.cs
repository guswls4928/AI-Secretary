using UnityEngine;

public class CurrentCommand : MonoBehaviour
{
    public GameObject CurrentCommandPrefab;
    private TextMesh textMesh;

    void Start()
    {
        textMesh = transform.GetComponent<TextMesh>();
    }

    public void UpdateCurCommandText(string commandText)
    {
        textMesh.text = commandText;
    }
}

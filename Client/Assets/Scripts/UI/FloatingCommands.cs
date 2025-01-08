using Python.Runtime;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class FloatingCommands : MonoBehaviour
{
    public GameObject FloatingCommandsPrefab;
    public Vector2 boundarySize;

    private Vector2 screenCenter;  // 화면 중심 좌표
    Dictionary<string, string> DllList = new();
    List<string> commandList = new();

    public GameObject curName;
    private CurrentCommand curNameManager;

#nullable enable
    string? moduleName;
    dynamic? func;

    private static FloatingCommands instance = null;

    void Awake()
    {
        if (null == instance)
        {
            instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static FloatingCommands Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    public void ServerResponse(string commandText)
    {
        dynamic req = func.Execute(commandText);

        SetCommand();
        UpdateCommandUI();
    }

    void Start()
    {
        Runtime.PythonDLL = Application.dataPath + "/StreamingAssets/embedded-python/python312.dll";
        PythonEngine.Initialize();

        moduleName = null;

        foreach (var file in System.IO.Directory.GetFiles(Application.dataPath + "/Dlls"))
        {
            if (!file.EndsWith(".pyd"))
                continue;

            dynamic sys = Py.Import("sys");
            sys.path.append(Application.dataPath + "/Dlls");
            sys.path.append(Application.dataPath + "/StreamingAssets/embedded-python/Lib/site-packages");

            string name = System.IO.Path.GetFileNameWithoutExtension(System.IO.Path.GetFileNameWithoutExtension(file));

            dynamic temp = Py.Import(name); 

            DllList.Add((string)temp.GetName(), name);
        }

        curNameManager = curName.GetComponent<CurrentCommand>();

        boundarySize = new Vector2(920, 880);
        Vector2 localPoint;

        Canvas canvas = GetComponentInParent<Canvas>();
        Camera uiCamera = canvas.renderMode == RenderMode.ScreenSpaceCamera ? canvas.worldCamera : null;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            transform as RectTransform,
            new Vector2(Screen.width / 2, Screen.height / 2),
            uiCamera,
            out localPoint
        );
        screenCenter = localPoint;


        SetCommand();
        UpdateCommandUI();
    }

    void Update()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            MoveFloatingCommand(transform.GetChild(i).gameObject);
        }
    }

    void MoveFloatingCommand(GameObject command)
    {
        CommandMover mover = command.GetComponent<CommandMover>();
        if (mover != null)
        {
            mover.Move();
        }
    }

    public void SetCommand()
    {
        commandList.Clear();

        if (moduleName == null)
        {
            commandList = DllList.Keys.ToList();
        }
        else
        {
            commandList.Add("홈");

            foreach (var command in func!.GetCommand())
            {
                commandList.Add(command.ToString());
            }
        }
    }

    public void SetModule()
    {
        dynamic sys = Py.Import("sys");
        sys.path.append(Application.dataPath + "/StreamingAssets/embedded-python/Lib/site-packages");
        sys.path.append(Application.dataPath + "/Dlls");

        dynamic os = Py.Import("os");
        os.environ.__setitem__("DLLS_PATH", Application.dataPath + "/Dlls");

        dynamic module = Py.Import(DllList[moduleName]);
        func = module.Create();
    }

    public void UpdateCommandUI()
    {
        try
        {
            curNameManager.UpdateCurCommandText((string)func.state.title);
        }
        catch (System.Exception e)
        {
            curNameManager.UpdateCurCommandText("Home");
        }

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var command in commandList)
        {
            CreateFloatingCommand(command);
        }
    }

    public void EnterCommand(string commandText)
    {
        var selectedCommand = commandList.Find(cmd => cmd.Replace(" ", string.Empty).ToLower() == commandText.Replace(" ", string.Empty).ToLower());
        Debug.Log(selectedCommand);
        if (selectedCommand == null)
        {
            Debug.LogWarning($"'{commandText}' 명령어를 찾을 수 없습니다.");
            return;
        }
        else if (moduleName == null)
        {
            moduleName = selectedCommand;
            SetModule();
        }
        else if (selectedCommand == "홈")
        {
            func!.Stop();
            moduleName = null;
            func = null;
        }
        else
        {
            dynamic req = func!.Execute(selectedCommand);

            try
            {
                if((bool)req == false)
                {
                    MyPlayer.Instance.SendCommand(moduleName, selectedCommand);
                }
            }
            catch (System.Exception e) { }
            ExpManager.ret.Value = "5";
        }

        SetCommand();
        UpdateCommandUI();
    }

    public void EnterCommandGlobal(string commandText)
    {
        // 현재 계층에서 명령어를 찾는 시도
        if (commandList.Contains(commandText))
        {
            EnterCommand(commandText);
            return;
        }

        // 최상위 계층으로 이동하여 명령어 탐색 및 실행
        string? previousModule = moduleName; // 현재 계층을 저장
        moduleName = null;                   // 최상위 계층으로 이동
        SetCommand();                        // 최상위 계층의 명령어 목록 설정

        if (commandList.Contains(commandText))
        {
            EnterCommand(commandText);       // 최상위 계층에서 명령어 실행
        }

        // 원래 계층으로 복귀
        moduleName = previousModule;
        SetCommand();
    }


    private void OnDestroy()
    {
        func!.Stop();
        func = null;
    }

    #region UI
    void CreateFloatingCommand(string commandText)
    {
        Vector3 startPosition = new Vector3(
            Random.Range(-boundarySize.x / 2, boundarySize.x / 2),
            Random.Range(-boundarySize.y / 2, boundarySize.y / 2),
            0
        );

        var newCommand = Instantiate(FloatingCommandsPrefab, transform);
        RectTransform rectTransform = newCommand.GetComponent<RectTransform>();
        TextMeshProUGUI textMeshPro = newCommand.GetComponentInChildren<TextMeshProUGUI>();

        if (textMeshPro == null)
        {
            Debug.LogError("TextMeshProUGUI component not found in prefab.");
            return;
        }

        rectTransform.anchoredPosition = startPosition;
        textMeshPro.text = commandText;
        textMeshPro.fontSize = Random.Range(28, 64);

        var mover = newCommand.AddComponent<CommandMover>();
        mover.Initialize(boundarySize, screenCenter);
    }
}

public class CommandMover : MonoBehaviour
{
    private Vector2 boundary;
    private RectTransform rectTransform;
    private bool isDragging = false;
    private bool isReturningToCircle = false;

    private Vector3 screenCenter;
    private float radius = 20f;
    private float angle = 0f;
    private float rotationSpeed = 10f;

    public void Initialize(Vector2 boundarySize, Vector3 center)
    {
        boundary = boundarySize;
        rectTransform = GetComponent<RectTransform>();
        screenCenter = center;
    }

    public void SetDragging(bool dragging)
    {
        isDragging = dragging;
        if (!isDragging)
        {
            isReturningToCircle = true;
        }
    }

    public void Move()
    {
        if (isDragging) return;

        if (isReturningToCircle)
        {
            Vector2 targetPosition = CalculateCirclePosition(angle);
            rectTransform.anchoredPosition = Vector2.MoveTowards(
                rectTransform.anchoredPosition,
                targetPosition,
                Time.deltaTime * 10f
            );

            if (Vector2.Distance(rectTransform.anchoredPosition, targetPosition) < 1f)
            {
                isReturningToCircle = false;
            }
        }
        else
        {
            angle += rotationSpeed * Time.deltaTime;
            if (angle >= 360f) angle -= 360f;

            Vector2 targetPosition = CalculateCirclePosition(angle);
            rectTransform.anchoredPosition = targetPosition;
        }
    }


    private Vector2 CalculateCirclePosition(float currentAngle)
    {
        float x = screenCenter.x + Mathf.Cos(Mathf.Deg2Rad * currentAngle) * radius;
        float y = screenCenter.y + Mathf.Sin(Mathf.Deg2Rad * currentAngle) * radius;

        return new Vector2(x, y);
    }


}



#endregion
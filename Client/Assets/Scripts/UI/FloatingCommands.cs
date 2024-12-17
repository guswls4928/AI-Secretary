using Python.Runtime;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class FloatingCommands : MonoBehaviour
{
    public GameObject FloatingCommandsPrefab;
    public Vector2 boundarySize;

    public GameObject curName;
    private CurrentCommand curNameManager;

    Dictionary<string, string> DllList = new();

    List<string> commandList = new();
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

    private void OnDestroy()
    {
        func!.Stop();
        func = null;
    }

    #region UI
    void CreateFloatingCommand(string commandText)
    {
        boundarySize = new Vector2(1720, 880);

        Vector3 startPosition = new Vector3(
            Random.Range(-boundarySize.x / 2, boundarySize.x / 2),
            Random.Range(-boundarySize.y / 2, boundarySize.y / 2),
            0
        );

        var newCommand = Instantiate(FloatingCommandsPrefab, transform);

        RectTransform rectTransform = newCommand.GetComponent<RectTransform>();
        TextMesh textMesh = newCommand.GetComponent<TextMesh>();

        rectTransform.anchoredPosition = startPosition;
        newCommand.GetComponent<TextMesh>().text = commandText; 
        textMesh.characterSize = Random.Range(20, 50);

        newCommand.AddComponent<CommandMover>().Initialize(boundarySize);
    }

    // 명령어 이동 처리 함수
    void MoveFloatingCommand(GameObject command)
    {
        CommandMover mover = command.GetComponent<CommandMover>();
        if (mover != null)
        {
            mover.Move();
        }
    }
}

public class CommandMover : MonoBehaviour
{
    private Vector3 direction;
    private float speed = 20f;
    private Vector2 boundary;
    private RectTransform rectTransform;

    public void Initialize(Vector2 boundarySize)
    {
        boundary = boundarySize;
        rectTransform = GetComponent<RectTransform>();
        direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0).normalized;
    }

    // 이동 처리
    public void Move()
    {
        rectTransform.anchoredPosition += (Vector2)(direction * speed * Time.deltaTime);
        CheckBoundaryCollision();
    }

    // 경계 충돌 반응
    private void CheckBoundaryCollision()
    {
        Vector3 pos = rectTransform.anchoredPosition;

        // ui 화면 경계에 충돌할 경우 xy축 반전.
        if (pos.x < -boundary.x / 2 || pos.x > boundary.x / 2)
        {
            direction.x = -direction.x;
            pos.x = Mathf.Clamp(pos.x, -boundary.x / 2, boundary.x / 2);
        }

        if (pos.y < -boundary.y / 2 || pos.y > boundary.y / 2)
        {
            direction.y = -direction.y;
            pos.y = Mathf.Clamp(pos.y, -boundary.y / 2, boundary.y / 2);
        }

        rectTransform.anchoredPosition = pos;
    }
}
#endregion
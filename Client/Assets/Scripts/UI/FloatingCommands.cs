using Python.Runtime;
using UnityEngine;
using System.Collections.Generic;

public class FloatingCommands : MonoBehaviour
{
    public GameObject FloatingCommandsPrefab;
    public Vector2 boundarySize;

    public List<dynamic> commandList = new();
#nullable enable
    public string? moduleName;
    dynamic func;

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
        Debug.Log(commandText);

        dynamic req = func.Execute(commandText);
        Debug.Log(req);

        SetCommand();
        UpdateCommandUI();
    }

    void Start()
    {
        Runtime.PythonDLL = Application.dataPath + "/StreamingAssets/embedded-python/python312.dll";
        PythonEngine.Initialize();

        moduleName = null;

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
            foreach (var file in System.IO.Directory.GetFiles(Application.dataPath + "/Dlls"))
            {
                if (!file.EndsWith(".pyd"))
                    continue;

                string name = System.IO.Path.GetFileNameWithoutExtension(System.IO.Path.GetFileNameWithoutExtension(file));

                commandList.Add(name);
            }
        }
        else
        {
            foreach (var command in func.GetCommand())
            {
                commandList.Add(command.ToString());
            }
        }
    }

    public void SetModule(string name)
    {
        dynamic sys = Py.Import("sys");
        sys.path.append(Application.dataPath + "/StreamingAssets/embedded-python/Lib/site-packages");
        sys.path.append(Application.dataPath + "/Dlls");

        dynamic os = Py.Import("os");
        os.environ.__setitem__("DLLS_PATH", Application.dataPath + "/Dlls");

        dynamic module = Py.Import(name);
        func = module.Create();
    }

    public void UpdateCommandUI()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        if (moduleName != null)
        {
            CreateFloatingCommand("상위");
        }

        foreach (var command in commandList)
        {
            CreateFloatingCommand(command);
        }
    }

    public void EnterCommand(string commandText)
    {
        var selectedCommand = commandList.Find(cmd => cmd == commandText);
        if (selectedCommand == null)
        {
            Debug.LogWarning($"'{commandText}' 명령어를 찾을 수 없습니다.");
            return;
        }
        else if (moduleName == null)
        {
            moduleName = selectedCommand;
            SetModule(moduleName);
            MyPlayer.Instance.SendCommand(commandText);
        }
        else if (commandText == "상위")
        {
            moduleName = null;
            MyPlayer.Instance.SendCommand(commandText);
        }
        else
        {
            dynamic req = func.Execute(commandText);

            Debug.Log(req);

            Debug.Log(func.Execute("**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"));

            try
            {
                if ((bool)req == false)
                {
                    MyPlayer.Instance.SendCommand(commandText);
                    return;
                }
            }
            catch (System.Exception e) { }
            

            ExpManager.ret.Value = "5";
        }

        SetCommand();
        UpdateCommandUI();
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
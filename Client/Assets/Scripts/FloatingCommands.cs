using Python.Runtime;
using UniRx;
using UnityEngine;
using Python.Runtime;

public class FloatingCommands : MonoBehaviour
{
    public GameObject FloatingCommandsPrefab;
    public Vector2 boundarySize;

    public GameObject CurrentCommandPrefab;
    private GameObject curCommandInstance;

    public Command rootCommand;
    public Command currentCommand;  // 현재 위치한 명령어

    void Start()
    {
        rootCommand = SetCommand();
        currentCommand = rootCommand;
        UpdateCommandUI();
    }

    void Update()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            MoveFloatingCommand(transform.GetChild(i).gameObject);
        }
    }

    public Command SetCommand()
    {
        Runtime.PythonDLL = Application.dataPath + "/StreamingAssets/embedded-python/python312.dll";
        PythonEngine.Initialize();

        Command rootCommand = new Command("Home");

        foreach (var file in System.IO.Directory.GetFiles(Application.dataPath + "/Dlls"))
        {
            if (file.EndsWith(".meta"))
                continue;

            string moduleName = System.IO.Path.GetFileNameWithoutExtension(System.IO.Path.GetFileNameWithoutExtension(file));

            dynamic os = Py.Import("os");
            dynamic sys = Py.Import("sys");
            sys.path.append(Application.dataPath + "/Dlls");

            dynamic module = Py.Import(moduleName);

            string name = module.GetName();
            Command newCommand = new Command(name);

            foreach (dynamic command in module.GetCommands())
            {
                newCommand.AddSubCommand(new Command(command.ToString()));
            }
            rootCommand.AddSubCommand(newCommand);
        }

        //PythonEngine.Shutdown();

        return rootCommand;
    }

    public void UpdateCommandUI()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        if (currentCommand.ParentCommand != null)
        {
            CreateFloatingCommand("상위");
        }

        foreach (var command in currentCommand.SubCommands)
        {
            CreateFloatingCommand(command.CommandText);
        }

        if (CurrentCommandPrefab != null)
        {
            if (curCommandInstance != null)
            {
                Destroy(curCommandInstance);
            }

            curCommandInstance = Instantiate(CurrentCommandPrefab, transform);
            RectTransform rectTransform = curCommandInstance.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = Vector3.zero;

            TextMesh textMesh = curCommandInstance.GetComponent<TextMesh>();
            if (textMesh != null)
            {
                textMesh.text = currentCommand.CommandText;
            }
            else
            {
                Debug.LogWarning("CurCmd 인스턴스에 TextMesh가 없습니다.");
            }
        }
        else
        {
            Debug.LogWarning("CurrentCommandPrefab 할당되지 않았습니다.");
        }
    }

    public void EnterCommand(string commandText)
    {
        var selectedCommand = currentCommand.SubCommands.Find(cmd => cmd.CommandText == commandText);
        if (selectedCommand != null)
        {
            currentCommand = selectedCommand;
        }
        else if (commandText == "상위" && currentCommand.ParentCommand != null)
        {
            currentCommand = currentCommand.ParentCommand;
        }
        else
        {
            Debug.LogWarning($"'{commandText}' 명령어를 찾을 수 없습니다.");
        }

        Debug.Log($"현재 명령어: {currentCommand.CommandText}");
        foreach (var subCommand in currentCommand.SubCommands)
        {
            Debug.Log($"하위 명령어: {subCommand.CommandText}");
        }

        UpdateCommandUI();
    }



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

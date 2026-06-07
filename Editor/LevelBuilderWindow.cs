using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class LevelBuilderWindow : EditorWindow
{
    // =========================
    // DATA
    // =========================
    [SerializeField] private ModularPrefabDatabase database;
    public ModularPrefabDatabase Database
    {
        get => database;
        set => database = value;
    }

    private int selectedCategoryIndex = 0;
    public int SelectedCategoryIndex
    {
        get => selectedCategoryIndex;
        set => selectedCategoryIndex = value;
    }

    private Dictionary<int, Vector2> tabScrollPositions = new();
    public Dictionary<int, Vector2> TabScrollPositions
    {
        get => tabScrollPositions;
    }

    // =========================
    // MODULES
    // =========================
    private LevelBuilderGrid grid;
    public LevelBuilderGrid Grid => grid;

    private LevelBuilderPlacement placement;
    public LevelBuilderPlacement Placement => placement;

    private LevelBuilderWalls walls;
    public LevelBuilderWalls Walls => walls;

    private LevelBuilderStairs stairs;
    public LevelBuilderStairs Stairs => stairs;

    private LevelBuilderGhost ghost;
    public LevelBuilderGhost Ghost => ghost;

    private LevelBuilderInput input;
    public LevelBuilderInput Input => input;

    private LevelBuilderUI ui;
    public LevelBuilderUI UI => ui;

    // =========================
    // STATE
    // =========================
    private BuildMode currentMode = BuildMode.Floor;
    public BuildMode CurrentMode
    {
        get => currentMode;
        set => currentMode = value;
    } 

    private BuildMode lastMode = BuildMode.Floor;
    public BuildMode LastMode
    {
        get => lastMode;
        set => lastMode = value;
    }

    private GameObject selectedPrefab;
    public GameObject SelectedPrefab => selectedPrefab;

    private ModularPiece currentPiece;
    public ModularPiece CurrentPiece => currentPiece;

    private float gridSize = 1f;
    public float GridSize
    {
        get => gridSize;
        set => gridSize = value;
    }

    private float floorHeight = 3f;
    public float FloorHeight
    {
        get => floorHeight;
        set => floorHeight = value;
    }

    private int currentLevel = 0;
    public int CurrentLevel
    {
        get => currentLevel;
        set => currentLevel = value;
    }

    private float rotation;
    public float Rotation
    {
        get => rotation;
        set => rotation = value;
    }

    /*private WallDirection currentWallDirection;
    public WallDirection CurrentWallDirection => currentWallDirection;*/

    // =========================
    // WORLD DATA
    // =========================
    private Dictionary<Vector3Int, GameObject> placedModules = new();
    public Dictionary<Vector3Int, GameObject> PlacedModules => placedModules;

    private Dictionary<WallKey, GameObject> placedWalls = new();
    public Dictionary<WallKey, GameObject> PlacedWalls => placedWalls;

    private List<StairConnection> stairsConnection = new();
    public List<StairConnection> StairsConnection => stairsConnection;

    // =========================
    // MENU
    // =========================
    [MenuItem("Tools/Level Builder")]
    public static void ShowWindow()
    {
        GetWindow<LevelBuilderWindow>("Level Builder");
    }

    private void OnEnable()
    {
        grid = new LevelBuilderGrid(this);
        placement = new LevelBuilderPlacement(this);
        walls = new LevelBuilderWalls(this);
        ghost = new LevelBuilderGhost(this);
        input = new LevelBuilderInput(this);
        ui = new LevelBuilderUI(this);
        stairs = new LevelBuilderStairs(this);

        CleanOrphanGhosts();

        RebuildWorldData();

        SceneView.duringSceneGui += DuringSceneGUI;

        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= DuringSceneGUI;

        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;

        ghost?.Destroy();
    }

    private void DuringSceneGUI(SceneView sceneView)
    {
        Event e = Event.current;

        // Bloquear la selección por defecto de Unity en la escena mientras usamos la herramienta
        int controlID = GUIUtility.GetControlID(FocusType.Passive);
        if (e.type == EventType.Layout)
        {
            HandleUtility.AddDefaultControl(controlID);
        }

        input.Handle(e);

        Vector3 world = grid.GetMouseWorld();
        Vector3Int cell = grid.WorldToCell(world);

        //currentWallDirection = LevelBuilderMath.GetClosestWallDirection(world, cell);
        WallDirection dir =
            (WallDirection)(Mathf.RoundToInt(Rotation / 90f) % 4);

        ghost.Update(cell);

        if (e.type == EventType.MouseDown && e.button == 0)
        {
            //placement.HandleClick(cell, currentWallDirection);
            placement.HandleClick(cell, dir);
            e.Use();
        }

        placement.DrawDebug();

        // Forzar actualización visual fluida del Ghost al mover el ratón
        if (e.type == EventType.MouseMove)
        {
            sceneView.Repaint();
        }
    }

    private void OnGUI()
    {
        ui.Draw();
    }

    // =========================
    // PREFAB SELECT
    // =========================
    public void SetSelectedPrefab(GameObject prefab)
    {
        selectedPrefab = prefab;
        currentPiece = prefab != null ? prefab.GetComponent<ModularPiece>() : null;

        if (currentPiece != null)
            currentMode = GetModeFromPiece(currentPiece.moduleType);

        ghost.Create();
    }

    public void SetMode(BuildMode mode)
    {
        currentMode = mode;
    }

    public void SetLastMode(BuildMode mode)
    {
        lastMode = mode;
    }

    public void ToggleErase()
    {
        if (currentMode == BuildMode.Erase)
            currentMode = lastMode;
        else
        {
            lastMode = currentMode;
            currentMode = BuildMode.Erase;
        }
    }

    private BuildMode GetModeFromPiece(ModuleType type)
    {
        return type switch
        {
            ModuleType.Floor => BuildMode.Floor,
            ModuleType.Wall => BuildMode.Wall,
            ModuleType.Stair => BuildMode.Stair,
            ModuleType.Prop => BuildMode.Prop,
            _ => BuildMode.Floor
        };
    }

    private void RebuildWorldData()
    {
        placedModules.Clear();
        placedWalls.Clear();
        stairsConnection.Clear();

        // Buscar todas las piezas colocadas en la escena
        ModularPiece[] pieces = FindObjectsByType<ModularPiece>(FindObjectsSortMode.None);

        foreach (var piece in pieces)
        {
            if (piece == null) continue;

            switch (piece.moduleType)
            {
                case ModuleType.Floor:
                case ModuleType.Stair:
                    var cells = LevelBuilderMath.GetCellsCovered(piece.gridOrigin, piece.size, piece.gridRotationY);
                    foreach (var c in cells)
                    {
                        placedModules[c] = piece.gameObject;
                    }
                    
                    if (piece.moduleType == ModuleType.Stair)
                    {
                        // Reconstruir la conexión de la escalera si es necesario para tus sistemas lógicos
                        Vector3Int exit = stairs.GetExitCell(piece.gridOrigin, piece.size, piece.gridRotationY, piece.levelDelta);
                        stairsConnection.Add(new StairConnection {
                            stairObject = piece.gameObject,
                            startCell = piece.gridOrigin,
                            endCell = exit,
                            // Nota: requeriría persistencia de niveles o inferencia matemática
                        });
                    }
                    break;

                case ModuleType.Wall:
                    var keys = walls.GetWallKeysCovered(piece.gridOrigin, piece.wallDirection, piece.size);
                    foreach (var k in keys)
                    {
                        placedWalls[k] = piece.gameObject;
                    }
                    break;
            }
        }
    }

    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        // Destruir el ghost justo ANTES de que empiece el modo juego o cambie el estado
        ghost?.Destroy();
    }

    private void CleanOrphanGhosts()
    {
        // Buscamos cualquier objeto en la escena que tenga activadas las flags de ocultación de nuestra herramienta
        // O si tus previsualizaciones fantasmas tienen algún tag/nombre característico (por ejemplo, si termina en (Clone))
        var allGameObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (var go in allGameObjects)
        {
            // Como tu sistema de ghost instancia un prefab, suele mantener el nombre del objeto o el del prefab.
            // Una forma muy segura es buscar si tiene un material clonado sin persistencia o simplemente por nombre si sabes que es un ghost:
            if (go != null && go.name.Contains("Ghost") || (SelectedPrefab != null && go.name == SelectedPrefab.name + "(Clone)" && go.GetComponent<Collider>()?.enabled == false))
            {
                // Si encontramos un objeto que parece un ghost abandonado y no tiene colliders (como los seteas tú), lo borramos
                if (go.transform.parent == null) // Los ghosts suelen estar en la raíz de la escena
                {
                    Object.DestroyImmediate(go);
                }
            }
        }
    }
}
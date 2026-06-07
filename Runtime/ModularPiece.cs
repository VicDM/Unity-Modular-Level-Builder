using UnityEngine;

public enum ModuleType
{
    Floor,
    Wall,
    Stair,
    Door,
    Window,
    Prop
}

public class ModularPiece : MonoBehaviour
{
    /*[Header("Info")]
    public string moduleId;*/

    public ModuleType moduleType;

    [Header("Grid")]
    public Vector3Int size = Vector3Int.one;

    /*[Header("Placement")]
    public bool blocksPlacement = true;*/

    [Header("Wall Physics")]
    public Vector3 worldSize = new Vector3(1, 1, 1);

    [Header("Stairs")]
    public int levelDelta = 1;

    [HideInInspector] public Vector3Int gridOrigin;
    [HideInInspector] public float gridRotationY;
    [HideInInspector] public WallDirection wallDirection;
}
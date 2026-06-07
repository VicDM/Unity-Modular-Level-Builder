using UnityEngine;

[System.Serializable]
public class StairConnection
{
    public GameObject stairObject;
    
    public Vector3Int startCell;
    public Vector3Int endCell;

    public int startLevel;
    public int endLevel;
}
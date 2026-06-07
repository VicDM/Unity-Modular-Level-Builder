using UnityEngine;
using UnityEditor;

public class LevelBuilderGrid
{
    private LevelBuilderWindow w;

    public LevelBuilderGrid(LevelBuilderWindow window)
    {
        w = window;
    }

    public Vector3 GetMouseWorld()
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        Plane p = new Plane(Vector3.up, Vector3.zero);

        return p.Raycast(ray, out float d)
            ? ray.GetPoint(d)
            : Vector3.zero;
    }

    public Ray GetMouseRay()
    {
        return HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
    }

    public Vector3Int WorldToCell(Vector3 world)
    {
        return new Vector3Int(
            Mathf.RoundToInt(world.x / w.GridSize),
            w.CurrentLevel,
            Mathf.RoundToInt(world.z / w.GridSize)
        );
    }

    public Vector3 CellToWorld(Vector3Int cell)
    {
        return new Vector3(
            cell.x * w.GridSize,
            cell.y * w.FloorHeight,
            cell.z * w.GridSize
        );
    }
}
using UnityEngine;
using System.Collections.Generic;

public static class LevelBuilderMath
{
    public static List<Vector3Int> GetCellsCovered(Vector3Int origin, Vector3Int size, float rotationY)
    {
        var cells = new List<Vector3Int>();

        int rot = Mathf.RoundToInt(rotationY / 90f) % 4;

        for (int x = 0; x < size.x; x++)
        {
            for (int z = 0; z < size.z; z++)
            {
                Vector3Int offset = new(x, 0, z);
                Vector3Int rotated = RotateOffset(offset, rot);

                cells.Add(origin + rotated);
            }
        }

        return cells;
    }

    public static Vector3Int RotateOffset(Vector3Int offset, int rot)
    {
        return rot switch
        {
            0 => offset,
            1 => new Vector3Int(offset.z, 0, -offset.x),
            2 => new Vector3Int(-offset.x, 0, -offset.z),
            3 => new Vector3Int(-offset.z, 0, offset.x),
            _ => offset,
        };
    }

    public static Vector3 GetObjectWorldPosition(Vector3Int origin, Vector3Int size, float rotationY, float gridSize, float floorHeight, Vector3 gridOrigin)
    {
        int rot = Mathf.RoundToInt(rotationY / 90f) % 4;

        Vector3 half = new(
            (size.x - 1) * 0.5f,
            0,
            (size.z - 1) * 0.5f
        );

        Vector3 offset = rot switch
        {
            0 => new Vector3(half.x, 0, half.z),
            1 => new Vector3(half.z, 0, -half.x),
            2 => new Vector3(-half.x, 0, -half.z),
            3 => new Vector3(-half.z, 0, half.x),
            _ => half
        };

        return new Vector3(
            origin.x * gridSize,
            origin.y * floorHeight,
            origin.z * gridSize
        ) + gridOrigin + offset;
    }

    /*public static Vector3 GetObjectPivotWorld(Vector3Int origin, float gridSize, float floorHeight)
    {
        return new Vector3(
            origin.x * gridSize,
            origin.y * floorHeight,
            origin.z * gridSize
        );
    }*/

    /*public static WallDirection GetClosestWallDirection(Vector3 worldPos, Vector3 cell)
    {
        float dx = worldPos.x - cell.x;
        float dz = worldPos.z - cell.z;

        return Mathf.Abs(dx) > Mathf.Abs(dz)
            ? (dx > 0 ? WallDirection.East : WallDirection.West)
            : (dz > 0 ? WallDirection.North : WallDirection.South);
    }*/

    /*public static WallDirection RotateDirection(WallDirection dir, float rotation)
    {
        int steps = Mathf.RoundToInt(rotation / 90f) % 4;

        int value = ((int)dir + steps) % 4;

        return (WallDirection)value;
    }*/
}
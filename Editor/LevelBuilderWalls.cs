using UnityEngine;
using System.Collections.Generic;

public class LevelBuilderWalls
{
    private readonly LevelBuilderWindow w;

    public LevelBuilderWalls(LevelBuilderWindow window) => w = window;

    public Vector3 GetWallPosition(Vector3Int cell, WallDirection dir, Vector3Int size)
    {
        Vector3 center = w.Grid.CellToWorld(cell);

        float half = w.GridSize * 0.5f;
        float lengthOffset = (size.x - 1) * w.GridSize * 0.5f;
        float thickness = w.CurrentPiece.worldSize.z * 0.5f;

        return dir switch
        {
            WallDirection.North => center + new Vector3(lengthOffset, 0, half + thickness),
            WallDirection.South => center + new Vector3(lengthOffset, 0, -(half + thickness)),
            WallDirection.East => center + new Vector3(half + thickness, 0, lengthOffset),
            WallDirection.West => center + new Vector3(-(half + thickness), 0, lengthOffset),
            _ => center,
        };
    }


    /*public Quaternion GetWallRotation(WallDirection dir)
    {
        return (dir == WallDirection.East || dir == WallDirection.West)
            ? Quaternion.Euler(0, 90, 0)
            : Quaternion.identity;
    }*/

    /*public Quaternion GetWallRotation(WallDirection dir, float rotation)
    {
        float baseRotation = dir switch
        {
            WallDirection.North => 0f,
            WallDirection.East  => 90f,
            WallDirection.South => 180f,
            WallDirection.West  => 270f,
            _ => 0f
        };

        return Quaternion.Euler(
            0,
            baseRotation + rotation,
            0
        );
    }*/

    public Quaternion GetWallRotation(WallDirection dir)
    {
        return dir switch
        {
            WallDirection.North => Quaternion.Euler(0, 0, 0),
            WallDirection.East  => Quaternion.Euler(0, 90, 0),
            WallDirection.South => Quaternion.Euler(0, 180, 0),
            WallDirection.West  => Quaternion.Euler(0, 270, 0),
            _ => Quaternion.identity
        };
    }

    public List<WallKey> GetWallKeysCovered(Vector3Int cell, WallDirection direction, Vector3Int size)
    {
        List<WallKey> keys = new();

        switch (direction)
        {
            case WallDirection.North:
            case WallDirection.South:

                // pared horizontal (eje X)
                for (int x = 0; x < size.x; x++)
                {
                    Vector3Int c = new Vector3Int(cell.x + x, cell.y, cell.z);
                    keys.Add(new WallKey(c, direction));
                }
                break;

            case WallDirection.East:
            case WallDirection.West:

                // pared vertical (eje Z)
                for (int x = 0; x < size.x; x++)
                {
                    Vector3Int c = new Vector3Int(cell.x, cell.y, cell.z + x);
                    keys.Add(new WallKey(c, direction));
                }
                break;
        }

        return keys;
    }
}
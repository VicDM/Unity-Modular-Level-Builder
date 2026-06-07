using UnityEngine;

public class LevelBuilderStairs
{
    private readonly LevelBuilderWindow w;

    public LevelBuilderStairs(LevelBuilderWindow window)
    {
        w = window;
    }

    public Vector3Int GetExitCell(Vector3Int start, Vector3Int size, float rotation, int levelDelta)
    {
        Vector3Int exit = start;

        int rot = Mathf.RoundToInt(rotation / 90f) % 4;

        switch (rot)
        {
            case 0:
                exit += new Vector3Int(0, levelDelta, size.z);
                break;

            case 1:
                exit += new Vector3Int(size.x, levelDelta, 0);
                break;

            case 2:
                exit += new Vector3Int(0, levelDelta, -size.z);
                break;

            case 3:
                exit += new Vector3Int(-size.x, levelDelta, 0);
                break;
        }

        return exit;
    }
}
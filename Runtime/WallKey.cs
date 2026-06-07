using UnityEngine;

[System.Serializable]
public struct WallKey
{
    public Vector3Int cell;
    public WallDirection direction;

    public WallKey(Vector3Int cell, WallDirection direction)
    {
        this.cell = cell;
        this.direction = direction;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is WallKey))
            return false;

        WallKey other = (WallKey)obj;

        return cell == other.cell &&
               direction == other.direction;
    }

    public override int GetHashCode()
    {
        return cell.GetHashCode() ^
               direction.GetHashCode();
    }
}
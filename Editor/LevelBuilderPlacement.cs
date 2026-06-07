using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class LevelBuilderPlacement
{
    private readonly LevelBuilderWindow w;

    public LevelBuilderPlacement(LevelBuilderWindow window)
    {
        w = window;
    }

    // =========================
    // ENTRY POINT DESDE WINDOW
    // =========================
    public void HandleClick(Vector3Int cell, WallDirection wallDir)
    {
        if (w.SelectedPrefab == null || w.CurrentPiece == null)
            return;

        switch (w.CurrentMode)
        {
            case BuildMode.Floor:
                PlaceFloor(cell);
                break;

            case BuildMode.Wall:
                PlaceWall(cell);
                break;

            case BuildMode.Stair:
                PlaceStair(cell);
                break;

            case BuildMode.Prop:
                PlaceProp();
                break;

            case BuildMode.Erase:
                TryDelete(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition));
                break;
        }
    }

    // =========================
    // FLOOR
    // =========================
    private void PlaceFloor(Vector3Int cell)
    {
        var cells = LevelBuilderMath.GetCellsCovered(cell, w.CurrentPiece.size, w.Rotation);

        foreach (var c in cells)
            if (w.PlacedModules.ContainsKey(c))
                return;

        GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(w.SelectedPrefab);

        obj.transform.SetPositionAndRotation(
            LevelBuilderMath.GetObjectWorldPosition(
                cell,
                w.CurrentPiece.size,
                w.Rotation,
                w.GridSize,
                w.FloorHeight,
                Vector3.zero
            ),
            Quaternion.Euler(0, w.Rotation, 0)
        );

        ModularPiece pieceScript = obj.GetComponent<ModularPiece>();
        if (pieceScript != null)
        {
            pieceScript.gridOrigin = cell;
            pieceScript.gridRotationY = w.Rotation;
            // Nota: wallDirection no es necesario aquí porque es un suelo, 
            // pero puedes dejarlo por defecto si tu script lo requiere.
        }

        Undo.RegisterCreatedObjectUndo(obj, "Place Floor");

        foreach (var c in cells)
            w.PlacedModules[c] = obj;
    }

    // =========================
    // WALL
    // =========================
    /*private void PlaceWall(Vector3Int cell, WallDirection dir)
    {
        Vector3Int size = w.CurrentPiece.size;

        List<WallKey> keys = w.Walls.GetWallKeysCovered(cell, dir, size);

        foreach (var k in keys)
            if (w.PlacedWalls.ContainsKey(k))
                return;

        GameObject wall = (GameObject)PrefabUtility.InstantiatePrefab(w.SelectedPrefab);

        Undo.RegisterCreatedObjectUndo(wall, "Wall");

        wall.transform.SetPositionAndRotation(
            w.Walls.GetWallPosition(cell, dir, size),
            w.Walls.GetWallRotation(dir)
        );

        foreach (var k in keys)
            w.PlacedWalls[k] = wall;
    }*/
    private void PlaceWall(Vector3Int cell)
    {
        Vector3Int size = w.CurrentPiece.size;

        WallDirection dir = (WallDirection)(Mathf.RoundToInt(w.Rotation / 90f) % 4);

        var keys =
            w.Walls.GetWallKeysCovered(
                cell,
                dir,
                w.CurrentPiece.size
            );

        //List<WallKey> keys = w.Walls.GetWallKeysCovered(cell, dir, size);

        foreach (var k in keys)
            if (w.PlacedWalls.ContainsKey(k))
                return;

        GameObject wall = (GameObject)PrefabUtility.InstantiatePrefab(w.SelectedPrefab);

        wall.transform.SetPositionAndRotation(
            w.Walls.GetWallPosition(
                cell,
                dir,
                w.CurrentPiece.size
            ),
            w.Walls.GetWallRotation(
                dir
            )
        );

        ModularPiece pieceScript = wall.GetComponent<ModularPiece>();
        if (pieceScript != null)
        {
            pieceScript.gridOrigin = cell;
            pieceScript.gridRotationY = w.Rotation;
            pieceScript.wallDirection = dir; // Guardamos la dirección cardinal de la pared
        }

        Undo.RegisterCreatedObjectUndo(wall, "Wall");

        foreach (var k in keys)
            w.PlacedWalls[k] = wall;
    }

    // =========================
    // STAIR
    // =========================
    private void PlaceStair(Vector3Int cell)
    {
        var cells = LevelBuilderMath.GetCellsCovered(cell, w.CurrentPiece.size, w.Rotation);

        foreach (var c in cells)
            if (w.PlacedModules.ContainsKey(c))
                return;

        GameObject stair = (GameObject)PrefabUtility.InstantiatePrefab(w.SelectedPrefab);        

        stair.transform.SetPositionAndRotation(
            LevelBuilderMath.GetObjectWorldPosition(
                cell,
                w.CurrentPiece.size,
                w.Rotation,
                w.GridSize,
                w.FloorHeight,
                Vector3.zero
            ),
            Quaternion.Euler(0, w.Rotation, 0)
        );

        ModularPiece pieceScript = stair.GetComponent<ModularPiece>();
        if (pieceScript != null)
        {
            pieceScript.gridOrigin = cell;
            pieceScript.gridRotationY = w.Rotation;
        }

        Undo.RegisterCreatedObjectUndo(stair, "Place Stair");

        foreach (var c in cells)
            w.PlacedModules[c] = stair;

        Vector3Int exit =
            w.Stairs.GetExitCell(
                cell,
                w.CurrentPiece.size,
                w.Rotation,
                w.CurrentPiece.levelDelta
            );

        w.StairsConnection.Add(new StairConnection
        {
            startCell = cell,
            endCell = exit,
            startLevel = w.CurrentLevel,
            endLevel = w.CurrentLevel + w.CurrentPiece.levelDelta
        });
    }

    private void PlaceProp()
    {
        Ray ray = w.Grid.GetMouseRay();

        if (!Physics.Raycast(ray, out RaycastHit hit))
            return;

        ModularPiece floor = hit.collider.GetComponentInParent<ModularPiece>();

        if (floor == null)
            return;

        if (floor.moduleType != ModuleType.Floor)
            return;

        Vector3 pos = hit.point;

        Renderer renderer = w.SelectedPrefab.GetComponentInChildren<Renderer>();

        if (renderer != null)
        {
            float halfHeight =
                renderer.bounds.size.y * 0.5f;

            pos += Vector3.up * halfHeight;
        }

        GameObject obj =
            (GameObject)PrefabUtility.InstantiatePrefab(w.SelectedPrefab);

        Undo.RegisterCreatedObjectUndo(
            obj,
            "Place Prop");

        obj.transform.SetPositionAndRotation(
            pos,
            Quaternion.Euler(0, w.Rotation, 0)
        );
    }

    // =========================
    // DELETE
    // =========================
    /*public void TryDelete(Ray ray)
    {
        if (!Physics.Raycast(ray, out RaycastHit hit))
            return;

        GameObject target = hit.collider.gameObject;

        // MODULES
        List<Vector3Int> toRemove = new();

        foreach (var p in w.PlacedModules)
            if (p.Value == target)
                toRemove.Add(p.Key);

        foreach (var k in toRemove)
            w.PlacedModules.Remove(k);

        // WALLS
        List<WallKey> wallRemove = new();
        
        foreach (var p in w.PlacedWalls)
            if (p.Value == target)
                wallRemove.Add(p.Key);

        foreach (var k in wallRemove)
            w.PlacedWalls.Remove(k);

        Undo.DestroyObjectImmediate(target);
    }*/

    public void TryDelete(Ray ray)
    {
        if (!Physics.Raycast(ray, out RaycastHit hit))
            return;

        ModularPiece targetPiece = hit.collider.GetComponentInParent<ModularPiece>();
        if (targetPiece == null)
            return;

        GameObject targetGo = targetPiece.gameObject;

        if (targetPiece.moduleType == ModuleType.Floor || targetPiece.moduleType == ModuleType.Stair)
        {
            var cells = LevelBuilderMath.GetCellsCovered(targetPiece.gridOrigin, targetPiece.size, targetPiece.gridRotationY);
            foreach (var c in cells)
            {
                w.PlacedModules.Remove(c);
            }
        }
        else if (targetPiece.moduleType == ModuleType.Wall)
        {
            var keys = w.Walls.GetWallKeysCovered(targetPiece.gridOrigin, targetPiece.wallDirection, targetPiece.size);
            foreach (var k in keys)
            {
                w.PlacedWalls.Remove(k);
            }
        }

        Undo.DestroyObjectImmediate(targetGo);
    }

    // =========================
    // DEBUG
    // =========================
    public void DrawDebug()
    {
        Handles.color = Color.cyan;

        foreach (var m in w.PlacedModules)
        {
            Handles.DrawWireCube(w.Grid.CellToWorld(m.Key), Vector3.one * w.GridSize);
        }

        Handles.color = Color.red;

        foreach (var wall in w.PlacedWalls)
        {
            Vector3 basePos = w.Grid.CellToWorld(wall.Key.cell);

            Vector3 offset = wall.Key.direction switch
            {
                WallDirection.North => new Vector3(0, 0, w.GridSize * 0.5f),
                WallDirection.South => new Vector3(0, 0, -w.GridSize * 0.5f),
                WallDirection.East  => new Vector3(w.GridSize * 0.5f, 0, 0),
                WallDirection.West  => new Vector3(-w.GridSize * 0.5f, 0, 0),
                _ => Vector3.zero
            };

            Handles.DrawWireCube(basePos + offset, new Vector3(w.GridSize, 1, w.GridSize));
        }
    }
}
using UnityEngine;
using UnityEditor;

public class LevelBuilderGhost
{
    private LevelBuilderWindow w;
    private GameObject ghost;

    public LevelBuilderGhost(LevelBuilderWindow window) => w = window;

    public void Create()
    {
        if (ghost) 
            Object.DestroyImmediate(ghost);
        if (!w.SelectedPrefab) 
            return;

        ghost = (GameObject)PrefabUtility.InstantiatePrefab(w.SelectedPrefab);

        ghost.hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy;

        foreach (var r in ghost.GetComponentsInChildren<Collider>())
            r.enabled = false;

        foreach (var r in ghost.GetComponentsInChildren<Renderer>())
            r.sharedMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"))
            {
                color = new Color(0,1,0,0.25f)
            };
    }

    public void Update(Vector3Int cell)
    {
        if (!ghost || !w.CurrentPiece)
            return;

        /*if (w.CurrentPiece.moduleType == ModuleType.Wall)
        {
            ghost.transform.SetPositionAndRotation(
                w.Walls.GetWallPosition(
                    cell,
                    w.CurrentWallDirection,
                    w.CurrentPiece.size),
                w.Walls.GetWallRotation(
                    w.CurrentWallDirection)
            );
        }*/
        if (w.CurrentPiece.moduleType == ModuleType.Wall)
        {
            WallDirection dir = (WallDirection)(Mathf.RoundToInt(w.Rotation / 90f) % 4);

            ghost.transform.SetPositionAndRotation(
                w.Walls.GetWallPosition(
                    cell,
                    dir,
                    w.CurrentPiece.size),
                w.Walls.GetWallRotation(dir)
            );
        }
        else if(w.CurrentPiece.moduleType == ModuleType.Prop)
        {
            Ray ray = w.Grid.GetMouseRay();

            if (!Physics.Raycast(ray, out RaycastHit hit))
            return;

            Vector3 pos = hit.point;

            Renderer renderer = w.SelectedPrefab.GetComponentInChildren<Renderer>();

            if (renderer != null)
            {
                float halfHeight =
                    renderer.bounds.size.y * 0.5f;

                pos += Vector3.up * halfHeight;
            }
            
            ghost.transform.SetPositionAndRotation(
                pos,
                Quaternion.Euler(0, w.Rotation, 0)
            );
        }
        else
        {
            ghost.transform.SetPositionAndRotation(
                LevelBuilderMath.GetObjectWorldPosition(
                    cell,
                    w.CurrentPiece.size,
                    w.Rotation,
                    w.GridSize,
                    w.FloorHeight,
                    Vector3.zero),
                Quaternion.Euler(
                    0,
                    w.Rotation,
                    0)
            );
        }
    }

    /*public void Destroy()
    {
        if (ghost) Object.DestroyImmediate(ghost);
        ghost = null;
    }*/

    public void Destroy()
    {
        if (ghost)
        {
            foreach (var r in ghost.GetComponentsInChildren<Renderer>())
            {
                if (r.sharedMaterial != null)
                {
                    Object.DestroyImmediate(r.sharedMaterial);
                }
            }
            Object.DestroyImmediate(ghost);
        }
        ghost = null;
    }
}
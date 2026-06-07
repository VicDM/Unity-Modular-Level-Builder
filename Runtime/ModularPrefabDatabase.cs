using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Level Builder/Prefab Database")]
public class ModularPrefabDatabase : ScriptableObject
{
    public List<ModularPrefabCategory> categories =
        new List<ModularPrefabCategory>();
}
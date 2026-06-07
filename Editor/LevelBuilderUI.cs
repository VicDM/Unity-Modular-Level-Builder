using UnityEngine;
using UnityEditor;

public class LevelBuilderUI
{
    private readonly LevelBuilderWindow w;

    public LevelBuilderUI(LevelBuilderWindow window)
    {
        w = window;
    }

    public void Draw()
    {
        GUILayout.Label("Level Builder", EditorStyles.boldLabel);

        // =========================
        // DATABASE
        // =========================

        w.Database =
            (ModularPrefabDatabase)EditorGUILayout.ObjectField(
                "Prefab Database",
                w.Database,
                typeof(ModularPrefabDatabase),
                false);

        EditorGUILayout.Space();

        // =========================
        // CATEGORY TABS
        // =========================

        if (w.Database != null && w.Database.categories != null && w.Database.categories.Count > 0)
        {
            string[] tabNames = new string[w.Database.categories.Count];

            for (int i = 0; i < w.Database.categories.Count; i++)
            {
                tabNames[i] =
                    w.Database.categories[i] != null
                    ? w.Database.categories[i].categoryName
                    : "Null";
            }

            w.SelectedCategoryIndex = GUILayout.Toolbar(w.SelectedCategoryIndex, tabNames);

            w.SelectedCategoryIndex = Mathf.Clamp(w.SelectedCategoryIndex, 0, w.Database.categories.Count - 1);

            var category = w.Database.categories[w.SelectedCategoryIndex];

            /*if (category != null)
            {
                if (!w.TabScrollPositions.ContainsKey(w.SelectedCategoryIndex))
                {
                    w.TabScrollPositions.Add(w.SelectedCategoryIndex, Vector2.zero);
                }

                w.TabScrollPositions[w.SelectedCategoryIndex] = EditorGUILayout.BeginScrollView(w.TabScrollPositions[w.SelectedCategoryIndex], GUILayout.Height(350));

                const int columns = 4;

                for (int i = 0; i < category.prefabs.Count; i += columns)
                {
                    EditorGUILayout.BeginHorizontal();

                    for (int j = 0; j < columns; j++)
                    {
                        int index = i + j;

                        if (index >= category.prefabs.Count)
                            break;

                        GameObject prefab = category.prefabs[index];

                        if (prefab == null)
                            continue;

                        Texture2D preview = AssetPreview.GetAssetPreview(prefab);

                        if (preview == null)
                        {
                            preview = AssetPreview.GetMiniThumbnail(prefab);
                        }

                        bool selected = prefab == w.SelectedPrefab;

                        EditorGUILayout.BeginVertical(GUILayout.Width(90));

                        Rect rect = GUILayoutUtility.GetRect(80, 80, GUILayout.Width(80), GUILayout.Height(80));

                        GUI.Box(rect, GUIContent.none);

                        if (preview != null)
                        {
                            GUI.DrawTexture(rect, preview, ScaleMode.ScaleToFit);
                        }

                        // =====================
                        // BORDE VERDE
                        // =====================

                        if (selected)
                        {
                            Handles.BeginGUI();

                            Color oldColor = Handles.color;

                            Handles.color = Color.green;

                            Vector3 p1 = new(rect.xMin, rect.yMin);

                            Vector3 p2 = new(rect.xMax, rect.yMin);

                            Vector3 p3 = new(rect.xMax, rect.yMax);

                            Vector3 p4 = new(rect.xMin, rect.yMax);

                            Handles.DrawAAPolyLine(3f, p1, p2);
                            Handles.DrawAAPolyLine(3f, p2, p3);
                            Handles.DrawAAPolyLine(3f, p3, p4);
                            Handles.DrawAAPolyLine(3f, p4, p1);

                            Handles.color = oldColor;

                            Handles.EndGUI();
                        }

                        // =====================
                        // CLICK PREFAB
                        // =====================

                        if (GUI.Button(rect, GUIContent.none, GUIStyle.none))
                        {
                            w.SetSelectedPrefab(prefab);
                        }

                        GUILayout.Label(prefab.name, EditorStyles.miniLabel, GUILayout.Width(80));

                        EditorGUILayout.EndVertical();
                    }

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndScrollView();
            }*/

            if (category == null)
                return;

            if (!w.TabScrollPositions.ContainsKey(w.SelectedCategoryIndex))
            {
                w.TabScrollPositions.Add(w.SelectedCategoryIndex, Vector2.zero);
            }

            w.TabScrollPositions[w.SelectedCategoryIndex] = EditorGUILayout.BeginScrollView(w.TabScrollPositions[w.SelectedCategoryIndex], GUILayout.Height(350));

            const int columns = 4;

            for (int i = 0; i < category.prefabs.Count; i += columns)
            {
                EditorGUILayout.BeginHorizontal();

                for (int j = 0; j < columns; j++)
                {
                    int index = i + j;

                    if (index >= category.prefabs.Count)
                        break;

                    GameObject prefab = category.prefabs[index];

                    if (prefab == null)
                        continue;

                    Texture2D preview = AssetPreview.GetAssetPreview(prefab);

                    if (preview == null)
                    {
                        preview = AssetPreview.GetMiniThumbnail(prefab);
                    }

                    bool selected = prefab == w.SelectedPrefab;

                    EditorGUILayout.BeginVertical(GUILayout.Width(90));

                    Rect rect = GUILayoutUtility.GetRect(80, 80, GUILayout.Width(80), GUILayout.Height(80));

                    GUI.Box(rect, GUIContent.none);

                    if (preview != null)
                    {
                        GUI.DrawTexture(rect, preview, ScaleMode.ScaleToFit);
                    }

                    // =====================
                    // BORDE VERDE
                    // =====================

                    if (selected)
                    {
                        Handles.BeginGUI();

                        Color oldColor = Handles.color;

                        Handles.color = Color.green;

                        Vector3 p1 = new(rect.xMin, rect.yMin);

                        Vector3 p2 = new(rect.xMax, rect.yMin);

                        Vector3 p3 = new(rect.xMax, rect.yMax);

                        Vector3 p4 = new(rect.xMin, rect.yMax);

                        Handles.DrawAAPolyLine(3f, p1, p2);
                        Handles.DrawAAPolyLine(3f, p2, p3);
                        Handles.DrawAAPolyLine(3f, p3, p4);
                        Handles.DrawAAPolyLine(3f, p4, p1);

                        Handles.color = oldColor;

                        Handles.EndGUI();
                    }

                    // =====================
                    // CLICK PREFAB
                    // =====================

                    if (GUI.Button(rect, GUIContent.none, GUIStyle.none))
                    {
                        w.SetSelectedPrefab(prefab);
                    }

                    GUILayout.Label(prefab.name, EditorStyles.miniLabel, GUILayout.Width(80));

                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }

        EditorGUILayout.Space();

        // =========================
        // INFO
        // =========================

        EditorGUILayout.LabelField(
            "Selected Prefab",
            w.SelectedPrefab != null
                ? w.SelectedPrefab.name
                : "None");

        EditorGUILayout.LabelField("Build Mode", w.CurrentMode.ToString());

        EditorGUILayout.Space();

        // =========================
        // SETTINGS
        // =========================

        w.CurrentLevel = EditorGUILayout.IntField("Current Level", w.CurrentLevel);

        w.FloorHeight = EditorGUILayout.FloatField("Floor Height", w.FloorHeight);

        w.GridSize = EditorGUILayout.FloatField("Grid Size", w.GridSize);

        w.Rotation = EditorGUILayout.Slider("Rotation", w.Rotation, 0f, 360f);

        bool eraseMode = w.CurrentMode == BuildMode.Erase;

        bool newEraseMode = EditorGUILayout.Toggle("Erase Mode", eraseMode);

        if (newEraseMode != eraseMode)
        {
            if (newEraseMode)
            {
                w.LastMode = w.CurrentMode;

                w.CurrentMode = BuildMode.Erase;
            }
            else
            {
                w.CurrentMode = w.LastMode;
            }
        }

        if (GUILayout.Button("Rotate 90°"))
        {
            w.Rotation += 90f;

            if (w.Rotation >= 360f)
                w.Rotation = 0f;
        }

        EditorGUILayout.Space();

        EditorGUILayout.HelpBox(
            "R = Rotate 90°\n" +
            "E = Toggle Erase Mode\n" +
            "Click Izquierdo = Place/Delete",
            MessageType.Info);
    }
}
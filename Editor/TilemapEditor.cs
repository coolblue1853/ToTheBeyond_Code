using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TilemapEditor : EditorWindow
{
    private string mapInput = "1 2 3\n4 0 5\n0 6 0";

    private TileBase groundLeft;
    private TileBase groundMiddle;
    private TileBase groundRight;
    private TileBase wallLeft;
    private TileBase wallMiddle;
    private TileBase wallLeftBottom;
    private TileBase wallRight;
    private TileBase wallRightBottom;
    private TileBase wallBottom;

    private Vector3Int origin = Vector3Int.zero;

    [MenuItem("Window/Tilemap Generator")]
    public static void ShowWindow() => GetWindow<TilemapEditor>("Tilemap Generator");

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Ground Tiles", EditorStyles.boldLabel);
        groundLeft   = (TileBase)EditorGUILayout.ObjectField("Ground Left (1)", groundLeft, typeof(TileBase), false);
        groundMiddle = (TileBase)EditorGUILayout.ObjectField("Ground Middle (2)", groundMiddle, typeof(TileBase), false);
        groundRight  = (TileBase)EditorGUILayout.ObjectField("Ground Right (3)", groundRight, typeof(TileBase), false);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Wall Tiles", EditorStyles.boldLabel);
        wallLeft         = (TileBase)EditorGUILayout.ObjectField("Wall Left (4)", wallLeft, typeof(TileBase), false);
        wallMiddle       = (TileBase)EditorGUILayout.ObjectField("Wall Middle (5)", wallMiddle, typeof(TileBase), false);
        wallRight        = (TileBase)EditorGUILayout.ObjectField("Wall Right (6)", wallRight, typeof(TileBase), false);
        wallLeftBottom   = (TileBase)EditorGUILayout.ObjectField("Wall Left Bottom (7)", wallLeftBottom, typeof(TileBase), false);
        wallBottom       = (TileBase)EditorGUILayout.ObjectField("Wall Bottom (8)", wallBottom, typeof(TileBase), false);
        wallRightBottom  = (TileBase)EditorGUILayout.ObjectField("Wall Right Bottom (9)", wallRightBottom, typeof(TileBase), false);

        EditorGUILayout.Space();
        origin = EditorGUILayout.Vector3IntField("Start Cell (Grid)", origin);

        EditorGUILayout.LabelField("Map data (space/comma, line = row)", EditorStyles.boldLabel);
        mapInput = EditorGUILayout.TextArea(mapInput, GUILayout.Height(100));

        if (GUILayout.Button("Generate Tilemap"))
        {
            int[,] map = ParseMapData(mapInput);
            if (map != null)
            {
                GenerateTilemap(map);
            }
        }

        if (GUILayout.Button("Clear Tilemap"))
        {
            ClearTilemap();
        }
    }

    private int[,] ParseMapData(string input)
    {
        var lines = input.Split('\n');
        var rows = new List<int[]>();

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            var parts = line.Trim().Split(new[] { ' ', ',' }, System.StringSplitOptions.RemoveEmptyEntries);
            var row = new int[parts.Length];

            for (int i = 0; i < parts.Length; i++)
            {
                if (int.TryParse(parts[i], out int val)) row[i] = val;
                else
                {
                    return null;
                }
            }

            rows.Add(row);
        }

        int height = rows.Count;
        int width = rows[0].Length;
        int[,] map = new int[height, width];

        for (int y = 0; y < height; y++)
        {
            if (rows[y].Length != width)
            {
                Debug.LogError("Row lengths are inconsistent.");
                return null;
            }

            for (int x = 0; x < width; x++)
            {
                map[y, x] = rows[y][x];
            }
        }

        return map;
    }

    private void GenerateTilemap(int[,] map)
    {
        var tilemap = GetSelectedTilemap();
        if (tilemap == null) return;

        Undo.RecordObject(tilemap, "Generate Tilemap");

        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                TileBase tile = null;
                switch (map[y, x])
                {
                    case 1: tile = groundLeft; break;
                    case 2: tile = groundMiddle; break;
                    case 3: tile = groundRight; break;
                    case 4: tile = wallLeft; break;
                    case 5: tile = wallMiddle; break;
                    case 6: tile = wallRight; break;
                    case 7: tile = wallLeftBottom; break;
                    case 8: tile = wallBottom; break;
                    case 9: tile = wallRightBottom; break;
                }

                if (tile != null)
                {
                    Vector3Int pos = origin + new Vector3Int(x, -y, 0);
                    tilemap.SetTile(pos, tile);
                }
            }
        }
    }

    private void ClearTilemap()
    {
        var tilemap = GetSelectedTilemap();
        if (tilemap == null) return;

        Undo.RecordObject(tilemap, "Clear Tilemap");
        tilemap.ClearAllTiles();
    }

    private void ClearAssignedTiles()
    {
        var tilemap = GetSelectedTilemap();
        if (tilemap == null) return;

        Undo.RecordObject(tilemap, "Clear Assigned Tiles");

        BoundsInt bounds = tilemap.cellBounds;
        foreach (var pos in bounds.allPositionsWithin)
        {
            if (tilemap.GetTile(pos) != null)
            {
                tilemap.SetTile(pos, null);
            }
        }
    }

    private Tilemap GetSelectedTilemap()
    {
        var selected = Selection.activeGameObject;

        var tilemap = selected.GetComponent<Tilemap>();
        return tilemap;
    }
}

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGeneration))]
public class MapGenerationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGeneration mapGen = (MapGeneration)target;

        if (DrawDefaultInspector())
        {
            // Здесь можно добавить дополнительный код, который будет выполняться после обновления инспектора
        }

        if (GUILayout.Button("Load Tiles"))
        {
            mapGen.LoadTiles();
        }
    }
}
#endif
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AbstractDungeonGeneration), true)]
public class RandomDungeonGenerationEditor : Editor
{
    AbstractDungeonGeneration dungeonGeneration;
    private void Awake()
    {
        dungeonGeneration = (AbstractDungeonGeneration)target;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Generate Dungeon"))
        {
            dungeonGeneration.GenerateDungeon();
        }
    }
}

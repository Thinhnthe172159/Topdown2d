using System.Collections;
using UnityEngine;

public abstract class AbstractDungeonGeneration : MonoBehaviour
{
    [SerializeField]
    protected TitlemapVisualizer titlemapVisualizer = null;
    [SerializeField]
    protected Vector2Int startPosition = Vector2Int.zero;

    public void GenerateDungeon()
    {
        titlemapVisualizer.Clear();
        RunProceduralGeneration();
    }

    protected abstract void RunProceduralGeneration();
}

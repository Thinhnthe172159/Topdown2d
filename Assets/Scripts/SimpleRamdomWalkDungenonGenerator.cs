using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimpleRamdomWalkDungenonGenerator : AbstractDungeonGeneration
{
    [SerializeField]
    protected SimpleRandomWalkSO randomWalkParameters;
    protected override void RunProceduralGeneration()
    {
        HashSet<Vector2Int> floorPositions = RunRandomWalk(randomWalkParameters, startPosition);
        titlemapVisualizer.Clear();
        titlemapVisualizer.PainFloorTiles(floorPositions);
        WallGenerator.CreateWalls(floorPositions, titlemapVisualizer);
    }

    protected HashSet<Vector2Int> RunRandomWalk(SimpleRandomWalkSO parameters, Vector2Int position)
    {
        var currentPosition = position;
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        for (int i = 0; i < parameters.iterations; i++)
        {
            var path = ProceduralRenerationAlgorithms.SimpleRandomWalk(currentPosition, parameters.walkLength);
            floorPositions.UnionWith(path);
            if(parameters.startRandomlyEachIteration) 
                currentPosition = floorPositions.ElementAt(UnityEngine.Random.Range(0, floorPositions.Count));
        }
        return floorPositions;
    }
}

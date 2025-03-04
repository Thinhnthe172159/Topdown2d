using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CorridorFirstDungeGenerator : SimpleRamdomWalkDungenonGenerator
{
    [SerializeField]
    private int corridorLenght = 14, corridorCount = 5;
    [SerializeField]
    [Range(0.1f, 1)]
    private float roomPercent = 0.8f;
    protected override void RunProceduralGeneration()
    {
        CorridorFirstGenerator();
    }

    private void CorridorFirstGenerator()
    {
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        HashSet<Vector2Int> pottentialRoomPositions = new HashSet<Vector2Int>();

        List<List<Vector2Int>> corridors = CreateCorridors(floorPositions, pottentialRoomPositions);

        CreateCorridors(floorPositions, pottentialRoomPositions);

        HashSet<Vector2Int> roomPositions = CreateRooms(pottentialRoomPositions);

        List<Vector2Int> deadEnds = FindAllDeadEnds(floorPositions);

        CreateRoomsAtDeadEnd(deadEnds, roomPositions);

        floorPositions.UnionWith(roomPositions);

        for (int i = 0; i < corridors.Count; i++)
        {
            corridors[i] = IncreaseCorridorSizeByOne(corridors[i]);
            floorPositions.UnionWith(corridors[i]);
        }

        titlemapVisualizer.PainFloorTiles(floorPositions);
        WallGenerator.CreateWalls(floorPositions, titlemapVisualizer);
    }

    private List<Vector2Int> InCreaseCorridorBrush3by3(List<Vector2Int> corridor)
    {
        List<Vector2Int> newCorridor = new List<Vector2Int>();
        for (int i = 1; i < corridor.Count; i++)
        {
            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    newCorridor.Add(corridor[i] + new Vector2Int(x, y));
                }
            }
        }
        return newCorridor;
    }

    private List<Vector2Int> IncreaseCorridorSizeByOne(List<Vector2Int> corriders)
    {
        List<Vector2Int> newCorridor = new List<Vector2Int>();
        Vector2Int previousDirecition = Vector2Int.zero;
        for (int i = 1; i < corriders.Count; i++)
        {
            Vector2Int directionFromCell = corriders[i] - corriders[i - 1];
            if(previousDirecition != Vector2Int.zero &&
                directionFromCell != previousDirecition)
            {
                for(int x = -1; x < 2; x++)
                {
                    for (int y = -1; y < 2; y++)
                    {
                        newCorridor.Add(corriders[i - 1] + new Vector2Int(x, y));
                    }
                }
                previousDirecition = directionFromCell;
            }
            else
            {
                Vector2Int newCorridorTileOffSet = GetDirection90From(directionFromCell);
                newCorridor.Add(corriders[i - 1]);
                newCorridor.Add(corriders[i - 1] + newCorridorTileOffSet);
            }
        }
        return newCorridor;
    }

    private Vector2Int GetDirection90From(Vector2Int direction)
    {
        if(direction == Vector2Int.up) return Vector2Int.right;
        if (direction == Vector2Int.right) return Vector2Int.down;
        if (direction == Vector2Int.down) return Vector2Int.left;
        if (direction == Vector2Int.left) return Vector2Int.up;
        return Vector2Int.zero;
    }

    private void CreateRoomsAtDeadEnd(List<Vector2Int> deadEnds, HashSet<Vector2Int> roomPositions)
    {
        foreach (var deadEnd in deadEnds)
        {
            var roomFloor = RunRandomWalk(randomWalkParameters, deadEnd);
            roomPositions.UnionWith(roomFloor);
        }
    }

    private List<Vector2Int> FindAllDeadEnds(HashSet<Vector2Int> floorPositions)
    {
        List<Vector2Int> deadEnds = new List<Vector2Int>();

        foreach (var position in floorPositions)
        {
            int neighbourCount = 0;
            foreach (var direction in Direction2D.cardinalDirectionsList)
            {
                if (floorPositions.Contains(position + direction))
                    neighbourCount++;
            }
            if(neighbourCount == 1)
                deadEnds.Add(position);
        }

        return deadEnds;
    }

    private HashSet<Vector2Int> CreateRooms(HashSet<Vector2Int> pottentialRoomPositions)
    {
        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();
        int roomToCreateCount = Mathf.RoundToInt(pottentialRoomPositions.Count * roomPercent);
        List<Vector2Int> roomsToCreate = pottentialRoomPositions.OrderBy(x => Guid.NewGuid()).Take(roomToCreateCount).ToList();

        foreach (var room in roomsToCreate)
        {
            var roomFloor = RunRandomWalk(randomWalkParameters, room);
            roomPositions.UnionWith(roomFloor);
        }

        return roomPositions;
    }

    private List<List<Vector2Int>> CreateCorridors(HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> pottentialRoomPositions)
    {
        var curentPosition = startPosition;
        pottentialRoomPositions.Add(curentPosition);
        List<List<Vector2Int>> corridors = new List<List<Vector2Int>>();
        for (int i = 0; i < corridorCount; i++)
        {
            var corridor = ProceduralRenerationAlgorithms.RandomWalkCorridor(curentPosition, corridorLenght);
            corridors.Add(corridor);
            curentPosition = corridor[corridor.Count - 1];
            pottentialRoomPositions.Add(curentPosition);
            floorPositions.UnionWith(corridor);
        }
        return corridors;
    }
}

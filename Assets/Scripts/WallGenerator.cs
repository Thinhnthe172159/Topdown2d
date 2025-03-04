using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WallGenerator
{
    public static void CreateWalls(HashSet<Vector2Int> floorPositions, TitlemapVisualizer titlemapVisualizer)
    {
        var basicWallPositons = FindWallsInDirection(floorPositions, Direction2D.cardinalDirectionsList);
        var cornerWallPositions = FindWallsInDirection(floorPositions, Direction2D.diagonalDirectionsList);
        CreateBasicWall(titlemapVisualizer, basicWallPositons, floorPositions);
        CreateCornerWalls(titlemapVisualizer, cornerWallPositions, floorPositions);
    }

    private static void CreateCornerWalls(TitlemapVisualizer titlemapVisualizer, HashSet<Vector2Int> cornerWallPositions, HashSet<Vector2Int> floorPositions)
    {
        foreach (var position in cornerWallPositions)
        {
            string neighboursBinaryType = "";
            foreach (var direction in Direction2D.eightDirectionsList)
            {
                var neighbourPosition = position + direction;
                if (floorPositions.Contains(neighbourPosition))
                    neighboursBinaryType += "1";
                else
                    neighboursBinaryType += "0";
            }
            titlemapVisualizer.PainSingleCornerWall(position, neighboursBinaryType);
        }
    }

    private static void CreateBasicWall(TitlemapVisualizer titlemapVisualizer, HashSet<Vector2Int> basicWallPositons, HashSet<Vector2Int> floorPositions)
    {
        foreach (var wallPosition in basicWallPositons)
        {
            string neighboursBinaryType = "";
            foreach (var direction in Direction2D.cardinalDirectionsList)
            {
                var neighbourPosition = wallPosition + direction;
                if (floorPositions.Contains(neighbourPosition))
                    neighboursBinaryType += "1";
                else
                    neighboursBinaryType += "0";
            }
            titlemapVisualizer.PainSingleBasicWall(wallPosition, neighboursBinaryType);
        }
    }

    private static HashSet<Vector2Int> FindWallsInDirection(HashSet<Vector2Int> floorPositions, List<Vector2Int> directionsList)
    {
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
        foreach (var positions in floorPositions)
        {
            foreach (var direction in directionsList)
            {
                var neighbourPosition = positions + direction;
                if (!floorPositions.Contains(neighbourPosition))
                    wallPositions.Add(neighbourPosition);
            }
        }
        return wallPositions;
    }
}

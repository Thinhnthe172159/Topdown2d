using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomFirstDungeGenerator : SimpleRamdomWalkDungenonGenerator
{
    [SerializeField]
    private int minRoomWidth = 3, minRoomHeight = 3;
    [SerializeField]
    private int dungeonWidth = 20, dungeonHeight = 20;
    [SerializeField]
    [Range(0, 10)]
    private int offset = 1;
    [SerializeField]
    private bool randomWalkRooms = false;
    [SerializeField]
    private GameObject playerPrefab;
    private Vector2Int playerStartPosition;
    private Transform spawnedObjectsParent;
    [SerializeField] private GameObject shopPrefab;
    protected override void RunProceduralGeneration()
    {
        CreateRooms();
    }

    private void CreateRooms()
    {
        ClearSpawnedObjects();
        var roomList = ProceduralRenerationAlgorithms.BinarySpacePartitioning(new BoundsInt((Vector3Int)startPosition,
            new Vector3Int(dungeonWidth, dungeonHeight, 0)), minRoomWidth, minRoomHeight);

        BoundsInt bossRoom = SelectBossRoom(roomList);
        roomList.Remove(bossRoom);
        BoundsInt spawnRoom = SelectFarthestRoom(roomList, bossRoom);
        playerStartPosition = (Vector2Int)Vector3Int.RoundToInt(spawnRoom.center);
        BoundsInt shopRoom = SelectShopRoom(roomList, spawnRoom, bossRoom);
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();

        if (randomWalkRooms)
        {
            floor = CreateRoomsRandomly(roomList);
        }
        else
        {
            floor = CreateSimpleRooms(roomList);
        }

        HashSet<Vector2Int> bossRoomFloor = CreateSimpleRooms(new List<BoundsInt> { bossRoom });
        floor.UnionWith(bossRoomFloor);

        List<Vector2Int> roomCenter = new List<Vector2Int>();
        foreach (var room in roomList)
        {
            roomCenter.Add((Vector2Int)Vector3Int.RoundToInt(room.center));
        }
        roomCenter.Add((Vector2Int)Vector3Int.RoundToInt(bossRoom.center));

        HashSet<Vector2Int> corridors = ConnectRooms(roomCenter, bossRoom);
        floor.UnionWith(corridors);
        titlemapVisualizer.PainFloorTiles(floor);
        WallGenerator.CreateWalls(floor, titlemapVisualizer);
        SpawnShopNPC(shopRoom);
        SpawnObjectsInRooms(roomList, spawnRoom, shopRoom, floor, corridors);
        SpawnBossRoomObjects(bossRoom);
        SpawnPlayer();
    }
    private BoundsInt SelectShopRoom(List<BoundsInt> roomList, BoundsInt spawnRoom, BoundsInt bossRoom)
    {
        if (roomList == null || roomList.Count == 0)
        {
            return spawnRoom;
        }

        BoundsInt shopRoom = spawnRoom;
        float maxDistance = 0;

        Vector2Int spawnCenter = (Vector2Int)Vector3Int.RoundToInt(spawnRoom.center);

        foreach (var room in roomList)
        {
            // Loại bỏ phòng boss và phòng quá nhỏ
            if (room == bossRoom || room.size.x < 4 || room.size.y < 4)
                continue;

            Vector2Int roomCenter = (Vector2Int)Vector3Int.RoundToInt(room.center);
            float distance = Vector2Int.Distance(roomCenter, spawnCenter);

            if (distance > maxDistance)
            {
                maxDistance = distance;
                shopRoom = room;
            }
        }

        return shopRoom;
    }

    private void SpawnShopNPC(BoundsInt shopRoom)
    {
        if (shopPrefab == null)
        {
            return;
        }

        // Chọn vị trí chính giữa phòng shop
        Vector3 shopPosition = new Vector3(
            shopRoom.center.x + 0.5f,
            shopRoom.center.y + 0.5f,
            0
        );

        shopPrefab.transform.position = shopPosition;
        Debug.Log($"Moved existing Shop NPC to: {shopPosition}");
    }


    private void SpawnBossRoomObjects(BoundsInt bossRoom)
    {
        if (titlemapVisualizer.bossRoomObjects == null || titlemapVisualizer.bossRoomObjects.Count == 0) return;

        int numberOfObjects = Random.Range(3, 6); // Spawn từ 3-6 vật chắn đạn
        HashSet<Vector2Int> usedPositions = new HashSet<Vector2Int>();

        for (int i = 0; i < numberOfObjects; i++)
        {
            Vector2Int randomPosition;
            int attempts = 10;
            do
            {
                randomPosition = new Vector2Int(
                    Random.Range(bossRoom.xMin + 1, bossRoom.xMax - 1),
                    Random.Range(bossRoom.yMin + 1, bossRoom.yMax - 1)
                );
                attempts--;
            } while (usedPositions.Contains(randomPosition) && attempts > 0);

            if (attempts == 0) continue;

            usedPositions.Add(randomPosition);

            GameObject randomObject = titlemapVisualizer.bossRoomObjects[Random.Range(0, titlemapVisualizer.bossRoomObjects.Count)];
            GameObject spawnedObj = Instantiate(randomObject,
                new Vector3(randomPosition.x + 0.5f, randomPosition.y + 0.5f, 0),
                Quaternion.identity);

            spawnedObj.transform.SetParent(spawnedObjectsParent);
        }
    }

    private void SpawnObjectsInRooms(List<BoundsInt> roomList, BoundsInt spawnRoom, BoundsInt shopRoom, HashSet<Vector2Int> floor, HashSet<Vector2Int> corridors)
    {
        if (titlemapVisualizer.roomObjects == null || titlemapVisualizer.roomObjects.Count == 0) return;

        if (spawnedObjectsParent == null)
        {
            spawnedObjectsParent = new GameObject("SpawnedObjects").transform;
        }

        foreach (var room in roomList)
        {
            if (room == spawnRoom || room == shopRoom) continue; // Không spawn trong phòng spawn và shop

            int numberOfObjects = Random.Range(2, 6);
            HashSet<Vector2Int> usedPositions = new HashSet<Vector2Int>();

            for (int i = 0; i < numberOfObjects; i++)
            {
                Vector2Int randomPosition;
                int attempts = 10;
                do
                {
                    randomPosition = new Vector2Int(
                        Random.Range(room.xMin + 1, room.xMax - 1),
                        Random.Range(room.yMin + 1, room.yMax - 1)
                    );
                    attempts--;
                } while (
                    (!floor.Contains(randomPosition) || usedPositions.Contains(randomPosition) || corridors.Contains(randomPosition))
                    && attempts > 0
                );

                if (attempts == 0) continue;

                usedPositions.Add(randomPosition);

                GameObject randomObject = titlemapVisualizer.roomObjects[Random.Range(0, titlemapVisualizer.roomObjects.Count)];
                GameObject spawnedObj = Instantiate(randomObject,
                    new Vector3(randomPosition.x + 0.5f, randomPosition.y + 0.5f, 0),
                    Quaternion.identity);

                spawnedObj.transform.SetParent(spawnedObjectsParent);
            }
        }
    }



    private void ClearSpawnedObjects()
    {
        if (spawnedObjectsParent != null)
        {
            if (Application.isPlaying)
            {
                Destroy(spawnedObjectsParent.gameObject);
            }
            else
            {
                DestroyImmediate(spawnedObjectsParent.gameObject);
            }
            spawnedObjectsParent = null;
        }
    }



    private BoundsInt SelectBossRoom(List<BoundsInt> roomList)
    {
        BoundsInt largestRoom = roomList[0];
        foreach (var room in roomList)
        {
            if (room.size.x * room.size.y > largestRoom.size.x * largestRoom.size.y)
            {
                largestRoom = room;
            }
        }
        return largestRoom;
    }

    private BoundsInt SelectFarthestRoom(List<BoundsInt> roomList, BoundsInt bossRoom)
    {
        BoundsInt farthestRoom = roomList[0];
        float maxDistance = 0;
        Vector2Int bossRoomCenter = (Vector2Int)Vector3Int.RoundToInt(bossRoom.center);

        foreach (var room in roomList)
        {
            Vector2Int roomCenter = (Vector2Int)Vector3Int.RoundToInt(room.center);
            float distance = Vector2Int.Distance(roomCenter, bossRoomCenter);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                farthestRoom = room;
            }
        }
        return farthestRoom;
    }

    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenter, BoundsInt bossRoom)
    {
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
        var currentRoomCenter = roomCenter[Random.Range(0, roomCenter.Count)];
        roomCenter.Remove(currentRoomCenter);
        while (roomCenter.Count > 0)
        {
            Vector2Int closest = FindClosestPoinTo(currentRoomCenter, roomCenter);
            roomCenter.Remove(closest);
            HashSet<Vector2Int> newCorridor = CreateCorridor(currentRoomCenter, closest);
            currentRoomCenter = closest;
            corridors.UnionWith(newCorridor);
        }

        Vector2Int bossRoomCenter = (Vector2Int)Vector3Int.RoundToInt(bossRoom.center);
        if (!corridors.Contains(bossRoomCenter))
        {
            Vector2Int closestToBoss = FindClosestPoinTo(bossRoomCenter, roomCenter);
            HashSet<Vector2Int> bossCorridor = CreateCorridor(bossRoomCenter, closestToBoss);
            corridors.UnionWith(bossCorridor);
        }
        return corridors;
    }

    private void SpawnPlayer()
    {
        if (playerPrefab != null)
        {
            Vector3 spawnPosition = new Vector3(playerStartPosition.x + 0.5f, playerStartPosition.y + 0.5f, 0);
            playerPrefab.transform.position = spawnPosition;
        }
        else
        {
            Debug.LogWarning("Player Prefab is not assigned!");
        }
    }

    private HashSet<Vector2Int> CreateRoomsRandomly(List<BoundsInt> roomList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        for (int i = 0; i < roomList.Count; i++)
        {
            var roomBounds = roomList[i];
            var roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));
            var roomFloor = RunRandomWalk(randomWalkParameters, roomCenter);
            foreach (var position in roomFloor)
            {
                if (position.x >= (roomBounds.xMin + offset) && position.x < (roomBounds.xMax - offset) &&
                    position.y >= (roomBounds.yMin + offset) && position.y < (roomBounds.yMax - offset))
                {
                    floor.Add(position);
                }
            }
        }
        return floor;
    }

    private HashSet<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int closest)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        var position = currentRoomCenter;
        corridor.Add(position);

        Vector2Int lastDirection = Vector2Int.zero;
        Vector2Int perpendicularDirection = Vector2Int.zero;

        while (position.y != closest.y)
        {
            if (closest.y > position.y)
            {
                position += Vector2Int.up;
                lastDirection = Vector2Int.up;
            }
            else if (closest.y < position.y)
            {
                position += Vector2Int.down;
                lastDirection = Vector2Int.down;
            }

            perpendicularDirection = GetPerpendicularDirection(lastDirection);
            corridor.Add(position);
            corridor.Add(position + perpendicularDirection);
        }

        while (position.x != closest.x)
        {
            if (closest.x > position.x)
            {
                position += Vector2Int.right;
                lastDirection = Vector2Int.right;
            }
            else if (closest.x < position.x)
            {
                position += Vector2Int.left;
                lastDirection = Vector2Int.left;
            }

            perpendicularDirection = GetPerpendicularDirection(lastDirection);
            corridor.Add(position);

            if (lastDirection == Vector2Int.right || lastDirection == Vector2Int.left)
            {
                corridor.Add(position + Vector2Int.up);
            }
            else
            {
                corridor.Add(position + Vector2Int.right);
            }
        }

        return corridor;
    }

    private Vector2Int GetPerpendicularDirection(Vector2Int direction)
    {
        if (direction == Vector2Int.up || direction == Vector2Int.down)
            return Vector2Int.right;
        else
            return Vector2Int.up;
    }


    private Vector2Int FindClosestPoinTo(Vector2Int currentRoomCenter, List<Vector2Int> roomCenter)
    {
        Vector2Int closest = Vector2Int.zero;
        float distance = float.MaxValue;
        foreach (var position in roomCenter)
        {
            float currentDistance = Vector2Int.Distance(currentRoomCenter, position);
            if (currentDistance < distance)
            {
                distance = currentDistance;
                closest = position;
            }
        }
        return closest;
    }

    private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        foreach (var room in roomList)
        {
            for (int col = offset; col < room.size.x - offset; col++)
            {
                for (int row = offset; row < room.size.y - offset; row++)
                {
                    Vector2Int position = (Vector2Int)room.min + new Vector2Int(col, row);
                    floor.Add(position);
                }
            }
        }
        return floor;
    }
}

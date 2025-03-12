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
    [SerializeField] private GameObject miniBoss;
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private int minEnemiesPerRoom = 1;
    [SerializeField] private int maxEnemiesPerRoom = 4;
    [SerializeField] private GameObject bossPrefab;
    [SerializeField]
    private GameObject horizontalDoorPrefab;
    [SerializeField]
    private GameObject verticalDoorPrefab;
    protected override void RunProceduralGeneration()
    {
        CreateRooms();
    }

    private void CreateRooms()
    {
        ClearSpawnedObjects();
        var roomList = ProceduralRenerationAlgorithms.BinarySpacePartitioning(
            new BoundsInt((Vector3Int)startPosition, new Vector3Int(dungeonWidth, dungeonHeight, 0)),
            minRoomWidth, minRoomHeight);

        BoundsInt bossRoom = SelectBossRoom(roomList);
        roomList.Remove(bossRoom);
        BoundsInt spawnRoom = SelectFarthestRoom(roomList, bossRoom);
        playerStartPosition = (Vector2Int)Vector3Int.RoundToInt(spawnRoom.center);
        BoundsInt shopRoom = SelectShopRoom(roomList, spawnRoom, bossRoom);

        HashSet<Vector2Int> floor = randomWalkRooms ?
            CreateRoomsRandomly(roomList) :
            CreateSimpleRooms(roomList);

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
        SpawnEnemiesInRooms(roomList, bossRoom, spawnRoom, floor, corridors);
        SpawnBoss(bossRoom);
    }

    private void SpawnBoss(BoundsInt bossRoom)
    {
        if (bossPrefab == null) return;

        // Chọn vị trí chính giữa phòng boss
        Vector3 bossPosition = new Vector3(
            bossRoom.center.x + 0.5f,
            bossRoom.center.y + 0.5f,
            0
        );

        Instantiate(bossPrefab, bossPosition, Quaternion.identity, spawnedObjectsParent);
    }

    private void SpawnEnemiesInRooms(List<BoundsInt> roomList, BoundsInt bossRoom, BoundsInt spawnRoom, HashSet<Vector2Int> floor, HashSet<Vector2Int> corridors)
    {
        foreach (var room in roomList)
        {
            if (room == bossRoom || room == spawnRoom) continue; // Bỏ qua phòng boss và phòng spawn

            int enemyCount = Random.Range(minEnemiesPerRoom, maxEnemiesPerRoom + 1);
            HashSet<Vector2Int> spawnedPositions = new HashSet<Vector2Int>();

            for (int i = 0; i < enemyCount; i++)
            {
                Vector2Int randomPosition;
                int attempts = 10;
                do
                {
                    int x = Random.Range(room.xMin + 1, room.xMax - 1);
                    int y = Random.Range(room.yMin + 1, room.yMax - 1);
                    randomPosition = new Vector2Int(x, y);
                    attempts--;
                } while (
                    spawnedPositions.Contains(randomPosition) || // Tránh trùng lặp
                    !floor.Contains(randomPosition) ||          // Đảm bảo vị trí nằm trên sàn
                    corridors.Contains(randomPosition)          // Không spawn trong hành lang
                    && attempts > 0
                );

                if (attempts == 0)
                {
                    Debug.LogWarning($"Không tìm được vị trí hợp lệ để spawn quái vật trong phòng [{room.xMin}, {room.yMin}]!");
                    continue;
                }

                spawnedPositions.Add(randomPosition);
                SpawnEnemy(randomPosition);
            }
        }
    }


    private void SpawnEnemy(Vector2Int position)
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;

        GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        Vector3 spawnPosition = new Vector3(position.x + 0.5f, position.y + 0.5f, 0);
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, spawnedObjectsParent);
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
        if (miniBoss == null)
        {
            return;
        }

        // Chọn vị trí chính giữa phòng shop
        Vector3 shopPosition = new Vector3(
            shopRoom.center.x + 0.5f,
            shopRoom.center.y + 0.5f,
            0
        );

        miniBoss.transform.position = shopPosition;
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

    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters, BoundsInt bossRoom)
    {
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();

        if (roomCenters.Count == 0) return corridors;

        var currentRoomCenter = roomCenters[Random.Range(0, roomCenters.Count)];
        if (roomCenters.Count > 1)
        {
            roomCenters.Remove(currentRoomCenter);
        }

        while (roomCenters.Count > 0)
        {
            Vector2Int closest = FindClosestPoinTo(currentRoomCenter, roomCenters);
            roomCenters.Remove(closest);

            // Tạo hành lang giữa currentRoomCenter và closest
            HashSet<Vector2Int> corridor = CreateCorridor(currentRoomCenter, closest);
            corridors.UnionWith(corridor);

            // Lấy điểm đầu và cuối của hành lang
            var (start, end) = GetCorridorEndpoints(corridor);

            // Chỉ đặt cửa ở điểm bắt đầu của hành lang
            PlaceDoor(start, currentRoomCenter, closest);

            Debug.Log($"✅ Corridor from {currentRoomCenter} to {closest}, total corridors: {corridors.Count}");

            currentRoomCenter = closest;
        }

        // Nối bossRoom với hành lang gần nhất
        Vector2Int bossRoomCenter = (Vector2Int)Vector3Int.RoundToInt(bossRoom.center);
        if (!corridors.Contains(bossRoomCenter))
        {
            Vector2Int closestToBoss = FindClosestPoinTo(bossRoomCenter, new List<Vector2Int>(corridors));
            HashSet<Vector2Int> bossCorridor = CreateCorridor(bossRoomCenter, closestToBoss);
            corridors.UnionWith(bossCorridor);

            var (startBoss, endBoss) = GetCorridorEndpoints(bossCorridor);
            // Chỉ đặt cửa ở điểm bắt đầu của hành lang boss
            PlaceDoor(startBoss, bossRoomCenter, closestToBoss);

            Debug.Log($"🔥 Boss room corridor from {bossRoomCenter} to {closestToBoss}");
        }

        Debug.Log($"🚪 Total corridors: {corridors.Count}");
        return corridors;
    }
    private void PlaceDoor(Vector2Int position, Vector2Int roomA, Vector2Int roomB)
    {
        // Kiểm tra xem roomA và roomB có thẳng hàng không
        if (roomA.x != roomB.x && roomA.y != roomB.y)
        {
            Debug.LogError("Rooms are not aligned! Cannot place door.");
            return;
        }

        // Xác định hướng của hành lang
        bool isVertical = roomA.x == roomB.x;

        // Xác định vị trí đặt cửa
        Vector2Int doorPosition = position;

        // Nếu hành lang dọc, cửa sẽ được đặt ở vị trí x của hành lang và y của phòng
        if (isVertical)
        {
            doorPosition.y = Mathf.RoundToInt((roomA.y + roomB.y) / 2f);
        }
        // Nếu hành lang ngang, cửa sẽ được đặt ở vị trí y của hành lang và x của phòng
        else
        {
            doorPosition.x = Mathf.RoundToInt((roomA.x + roomB.x) / 2f);
        }

        // Kiểm tra xem đã có cửa ở vị trí này chưa
        if (IsDoorAtPosition(doorPosition))
        {
            Debug.LogWarning($"🚪 Door already exists at {doorPosition}. Skipping.");
            return;
        }

        // Chọn prefab cửa dựa trên hướng
        GameObject doorPrefab = isVertical ? verticalDoorPrefab : horizontalDoorPrefab;

        // Đặt cửa vào vị trí đã tính toán
        Instantiate(doorPrefab, new Vector3(doorPosition.x + 0.5f, doorPosition.y + 0.5f, 0), Quaternion.identity);
        Debug.Log($"🚪 Door placed at {doorPosition} {(isVertical ? "Vertical" : "Horizontal")}");
    }

    private bool IsDoorAtPosition(Vector2Int position)
    {
        // Kiểm tra xem có cửa ở vị trí này không (ví dụ: dùng Physics2D.OverlapCircle)
        Collider2D[] colliders = Physics2D.OverlapCircleAll(new Vector2(position.x + 0.5f, position.y + 0.5f), 0.1f);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Door"))
            {
                return true;
            }
        }
        return false;
    }
    private (Vector2Int start, Vector2Int end) GetCorridorEndpoints(HashSet<Vector2Int> corridor)
    {
        List<Vector2Int> corridorList = new List<Vector2Int>(corridor);
        Vector2Int start = corridorList[0];
        Vector2Int end = corridorList[0];
        float minDist = float.MaxValue;
        float maxDist = float.MinValue;

        foreach (var pos in corridorList)
        {
            float dist = Vector2Int.Distance(start, pos);
            if (dist < minDist)
            {
                minDist = dist;
                start = pos;
            }
            if (dist > maxDist)
            {
                maxDist = dist;
                end = pos;
            }
        }

        return (start, end);
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
    private HashSet<Vector2Int> CreateCorridor(Vector2Int start, Vector2Int end, int corridorWidth = 3)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        int x = start.x;
        int y = start.y;

        int dx = Mathf.Abs(end.x - start.x);
        int dy = Mathf.Abs(end.y - start.y);
        int sx = start.x < end.x ? 1 : -1;
        int sy = start.y < end.y ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            // Thêm ô chính
            corridor.Add(new Vector2Int(x, y));

            // Mở rộng hành lang theo chiều rộng (corridorWidth)
            for (int i = 1; i <= corridorWidth / 2; i++)
            {
                corridor.Add(new Vector2Int(x + i, y)); // Mở rộng sang phải
                corridor.Add(new Vector2Int(x - i, y)); // Mở rộng sang trái
                corridor.Add(new Vector2Int(x, y + i)); // Mở rộng lên trên
                corridor.Add(new Vector2Int(x, y - i)); // Mở rộng xuống dưới
            }

            if (x == end.x && y == end.y) break;

            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y += sy;
            }
        }

        return corridor;
    }
    //private HashSet<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int closest)
    //{
    //    HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
    //    var position = currentRoomCenter;
    //    corridor.Add(position);

    //    Vector2Int lastDirection = Vector2Int.zero;
    //    Vector2Int perpendicularDirection = Vector2Int.zero;

    //    while (position.y != closest.y)
    //    {
    //        position += (closest.y > position.y) ? Vector2Int.up : Vector2Int.down;
    //        lastDirection = (closest.y > position.y) ? Vector2Int.up : Vector2Int.down;

    //        perpendicularDirection = GetPerpendicularDirection(lastDirection);
    //        corridor.Add(position);
    //        corridor.Add(position + perpendicularDirection);
    //        corridor.Add(position - perpendicularDirection); // Mở rộng đường đi
    //    }

    //    while (position.x != closest.x)
    //    {
    //        position += (closest.x > position.x) ? Vector2Int.right : Vector2Int.left;
    //        lastDirection = (closest.x > position.x) ? Vector2Int.right : Vector2Int.left;

    //        perpendicularDirection = GetPerpendicularDirection(lastDirection);
    //        corridor.Add(position);
    //        corridor.Add(position + perpendicularDirection);
    //        corridor.Add(position - perpendicularDirection); // Mở rộng đường đi
    //    }

    //    return corridor;
    //}



    private Vector2Int GetPerpendicularDirection(Vector2Int direction)
    {
        if (direction == Vector2Int.up || direction == Vector2Int.down)
            return Vector2Int.right;
        else
            return Vector2Int.up;
    }


    private Vector2Int FindClosestPoinTo(Vector2Int point, List<Vector2Int> points)
    {
        Vector2Int closest = points[0];
        float minDist = Vector2Int.Distance(point, closest);

        foreach (var p in points)
        {
            float dist = Vector2Int.Distance(point, p);
            if (dist < minDist)
            {
                minDist = dist;
                closest = p;
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

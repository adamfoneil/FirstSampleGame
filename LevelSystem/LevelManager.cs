using System.Collections.Generic;
using System.Numerics;
using FirstSampleGame;

public class LevelManager
{

	private List<LevelRecord> Levels;
	public int CurrentLevel;
    private dynamic navPaths;
    private GameCore GameCoreRef;


	public LevelManager(GameCore gameCoreRef)
	{
        GameCoreRef = gameCoreRef;
		Levels = new List<LevelRecord>();
		CurrentLevel = 1;
        LoadShapePaths();
	}



    public void InitLevel()
    {
        LevelRecord currentLevel = GetCurrentLevel();
        foreach (var spawnEnemyRecord in currentLevel.SpawnEnemyRecords)
        {
            GameCoreRef.Spawner.SpawnEnemyWithRecord(spawnEnemyRecord);

        }


        foreach (var levelItemRecord in currentLevel.SpawnLevelItemRecords)
        {
            GameCoreRef.Spawner.SpawnLevelItemWithRecord(levelItemRecord);

        }


        foreach (var spawnHostageRecord in currentLevel.SpawnHostageRecords)
        {
            GameCoreRef.Spawner.SpawnHostageWithRecord(spawnHostageRecord);

        }
        
    }

    public void LoadShapePaths()
    {
        dynamic jsonObject = GameCore.LoadJsonFile("paths.json");
        navPaths = jsonObject.paths;
        //Console.WriteLine($"navPaths {navPaths[0]}");
    }

	public void CreateLevel(string textureName, Vector2 playerSpawnPosition, List<SpawnEnemyRecord> spawnEnemyRecords, List<SpawnLevelItemRecord> spawnLevelItemRecords, List<SpawnHostageRecord> spawnHostageRecords)
	{
		int level = Levels.Count + 1;
		LevelRecord levelRecord = new LevelRecord(level, textureName, playerSpawnPosition, spawnEnemyRecords, spawnLevelItemRecords, spawnHostageRecords);
		Levels.Add(levelRecord);
	}

	public LevelRecord GetCurrentLevel()
	{
		return Levels[CurrentLevel - 1];
	}


	public void CreateLevels()
	{
		List<SpawnEnemyRecord> spawnEnemyRecords =
		[
			new SpawnEnemyRecord(0, 150f , 1f, 100, GetShapePath(2), new Vector2(700f, 540f)),
			new SpawnEnemyRecord(1, 150f , 1f, 100, GetShapePath(3), new Vector2(700f, 540f)),
		];

		List<SpawnLevelItemRecord> spawnLevelItemRecords =
		[
			new SpawnLevelItemRecord(0, 1f, new Vector2(100f, 200f) ),
			new SpawnLevelItemRecord(0, 1f, new Vector2(200f, 200f) ),
			new SpawnLevelItemRecord(0, 1f, new Vector2(300f, 200f) ),
		];

		List<SpawnHostageRecord> spawnHostageRecords =
		[
			new SpawnHostageRecord(0, 1f, new Vector2(100f, 500f) ),
			new SpawnHostageRecord(0, 1f, new Vector2(200f, 500f) ),
			new SpawnHostageRecord(0, 1f, new Vector2(300f, 500f) ),
		];

		CreateLevel("test-level.png", new Vector2(200, 300), spawnEnemyRecords, spawnLevelItemRecords, spawnHostageRecords);
	}

    public dynamic GetShapePath(int pathIndex)
    {
        dynamic path = navPaths[pathIndex];
        List<Vector2> fPath = new List<Vector2>();
        foreach (var waypoint in path)
        {
            Vector2 floatWaypoint = new Vector2((float)waypoint.X, (float)waypoint.Y);
            fPath.Add(floatWaypoint);
            //Console.WriteLine($"path: {floatWaypoint}");
        }
        return fPath;
    }

}

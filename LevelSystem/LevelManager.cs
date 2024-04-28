using System.Collections.Generic;
using System.Numerics;
using FirstSampleGame;
using System;

public class LevelManager
{

	private List<LevelRecord> Levels;
	private int CurrentLevel;
    private dynamic navPaths;

	public LevelManager()
	{
		Levels = new List<LevelRecord>();
		CurrentLevel = 1;
        LoadShapePaths();
	}

    public void LoadShapePaths()
    {
        dynamic jsonObject = GameCore.LoadJsonFile("paths.json");
        navPaths = jsonObject.paths;
        //Console.WriteLine($"navPaths {navPaths[0]}");
    }

	public void CreateLevel(string textureName, Vector2 playerSpawnPosition, List<SpawnRecord> spawnRecords)
	{
		int level = Levels.Count + 1;
		LevelRecord levelRecord = new LevelRecord(level, textureName, playerSpawnPosition, spawnRecords);
		Levels.Add(levelRecord);
	}

	public LevelRecord GetCurrentLevel()
	{
		return Levels[CurrentLevel - 1];
	}


	public void CreateLevels()
	{
		List<SpawnRecord> spawnRecords =
		[
			new SpawnRecord(0, 150f , 1f, 100, GetShapePath(2), new Vector2(700f, 540f)),
			new SpawnRecord(1, 150f , 1f, 100, GetShapePath(3), new Vector2(700f, 540f)),
		];
		CreateLevel("test-level.png", new Vector2(200, 300), spawnRecords);
	}

    public dynamic GetShapePath(int pathIndex)
    {
        dynamic path = navPaths[pathIndex];
        List<Vector2> fPath = new List<Vector2>();
        foreach (var waypoint in path)
        {
            Vector2 floatWaypoint = new Vector2((float)waypoint.X, (float)waypoint.Y);
            fPath.Add(floatWaypoint);
            Console.WriteLine($"path: {floatWaypoint}");
        }
        return fPath;
    }

}

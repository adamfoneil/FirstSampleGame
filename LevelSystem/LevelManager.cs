using System.Collections.Generic;
using System.Numerics;

public class LevelManager
{
	private List<LevelRecord> Levels;
	private int CurrentLevel;

	public LevelManager()
	{
		Levels = new List<LevelRecord>();
		CurrentLevel = 1;
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
			new SpawnRecord(0, 150f , 1f, 100, PathNav.GetShapePath(2), new Vector2(700f, 540f)),
			new SpawnRecord(1, 150f , 1f, 100, PathNav.GetShapePath(3), new Vector2(700f, 540f)),
		];
		CreateLevel("test-level.png", new Vector2(200, 300), spawnRecords);
	}




}
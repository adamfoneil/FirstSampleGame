using System.Collections.Generic;
using System.Numerics;

public class LevelRecord
{
	public int Level;
	public string TextureName;
	public Vector2 PlayerSpawnPosition;
	public List<SpawnEnemyRecord> SpawnEnemyRecords;
    public List<SpawnLevelItemRecord> SpawnLevelItemRecords;
    public List<SpawnHostageRecord> SpawnHostageRecords;

	public LevelRecord(int level, string textureName, Vector2 playerSpawnPosition, List<SpawnEnemyRecord> spawnEnemyRecords, List<SpawnLevelItemRecord> spawnLevelItemRecords, List<SpawnHostageRecord> spawnHostageRecords)
	{
		Level = level;
		TextureName = textureName;
		PlayerSpawnPosition = playerSpawnPosition;
		SpawnEnemyRecords = spawnEnemyRecords;
        SpawnLevelItemRecords = spawnLevelItemRecords;
        SpawnHostageRecords = spawnHostageRecords;
	}
}
using System.Collections.Generic;
using System.Numerics;

public class LevelRecord
{
	public int Level;
	public string TextureName;
	public Vector2 PlayerSpawnPosition;
	public List<SpawnRecord> SpawnRecords;

	public LevelRecord(int level, string textureName, Vector2 playerSpawnPosition, List<SpawnRecord> spawnRecords)
	{
		Level = level;
		TextureName = textureName;
		PlayerSpawnPosition = playerSpawnPosition;
		SpawnRecords = spawnRecords;
	}
}
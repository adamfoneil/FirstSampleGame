using System.Collections.Generic;
using System.Numerics;

public class SpawnLevelItemRecord
{
	public int ItemType;
	public float Scale;
	public Vector2 SpawnPosition;


	public SpawnLevelItemRecord(int itemType, float scale, Vector2 spawnPosition = default)
	{
        ItemType = itemType;
        Scale = scale;
        SpawnPosition = Vector2.Zero;

		if (spawnPosition != default) SpawnPosition = spawnPosition;
	}
}
using System.Collections.Generic;
using System.Numerics;

public class SpawnHostageRecord
{
	public int HostageType;
	public float Scale;
	public Vector2 SpawnPosition;


	public SpawnHostageRecord(int hostageType, float scale, Vector2 spawnPosition = default)
	{
        HostageType = hostageType;
        Scale = scale;
        SpawnPosition = Vector2.Zero;

		if (spawnPosition != default) SpawnPosition = spawnPosition;
	}
}
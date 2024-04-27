using System.Collections.Generic;
using System.Numerics;

public class SpawnRecord
{

	public int EnemyType;
	public float EnemySpeed;
	public float EnemyScale;
	public int EnemyHitPoints;
	public Vector2 SpawnPosition;
	public List<Vector2> EnemyPath;


	public SpawnRecord(int enemyType, float enemySpeed, float enemyScale, int enemyHitPoints, List<Vector2> enemyPath, Vector2 spawnPosition = default)
	{
		EnemyType = enemyType;
		EnemySpeed = enemySpeed;
		EnemyScale = enemyScale;
		EnemyHitPoints = enemyHitPoints;
		EnemyPath = enemyPath;
		SpawnPosition = enemyPath[0];
		if (spawnPosition != default) SpawnPosition = spawnPosition;
	}
}
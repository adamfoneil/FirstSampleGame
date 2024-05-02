using FirstSampleGame;
using System.Collections.Generic;
using System.Numerics;
using FirstSampleGame.Entities;
using FirstSampleGame.Abstract;
using System;

public class SpawnManager
{
    private GameCore GameCoreRef;
    private Pooler PoolerRef;
    public List<GameEntity> Enemies;
    public List<GameEntity> LevelItems;
    public List<GameEntity> Hostages;
    public List<GameEntity> HealthMeters;
    public bool SpawnerActive;
    public float SpawnTimer;
    public float SpawnDelay;

    public SpawnManager(GameCore gameCoreRef)
    {
        GameCoreRef = gameCoreRef;
        PoolerRef = gameCoreRef.Pooler;
        Enemies = new List<GameEntity>();
        LevelItems = new List<GameEntity>();
        Hostages = new List<GameEntity>();
        HealthMeters = GameCoreRef.HealthMeters;
        SpawnerActive = false;
        SpawnDelay = 2f;
        SpawnTimer = SpawnDelay;
    }



    public void SpawnEnemyWithRecord(SpawnEnemyRecord spawnEnemyRecord)
    {
        SpawnEnemy(spawnEnemyRecord.SpawnPosition, spawnEnemyRecord.EnemySpeed, spawnEnemyRecord.EnemyScale, spawnEnemyRecord.EnemyPath, 1f, true, false);
    }

    public void SpawnLevelItemWithRecord(SpawnLevelItemRecord spawnLevelItemRecord)
    {
        LevelItem levelItem = (LevelItem) PoolerRef.GetEntityFromPool(Pooler.PoolType.LEVEL_ITEM);
        levelItem.Position = spawnLevelItemRecord.SpawnPosition;
        levelItem.Active = true;
        levelItem.Visible = true;
        levelItem.Speed = 0f;
        levelItem.Scale = new(spawnLevelItemRecord.Scale);
        LevelItems.Add((GameEntity) levelItem);
    }

    public void SpawnHostageWithRecord(SpawnHostageRecord spawnHostageRecord)
    {
        Hostage hostage = (Hostage) PoolerRef.GetEntityFromPool(Pooler.PoolType.HOSTAGE);
        hostage.Position = spawnHostageRecord.SpawnPosition;
        hostage.Active = true;
        hostage.Visible = true;
        hostage.Speed = 0f;
        hostage.Scale = new(spawnHostageRecord.Scale);
        Hostages.Add((GameEntity) hostage);
    }



    // spawns and enemy and health meter
    public void SpawnEnemy(Vector2 position, float speed, float enemyScale,  List<Vector2> path = default, float pathScale = 1f, bool setActive = true, bool offsetPath = true)
    {
        Enemy enemy = (Enemy) PoolerRef.GetEntityFromPool(Pooler.PoolType.ENEMY);
        enemy.Position = position;
        enemy.Active = true;
        enemy.Visible = true;
        enemy.Speed = speed;
        enemy.Scale = new(enemyScale);
        Enemies.Add((GameEntity)enemy);

        if (path != default) enemy.pathNav.SetPath(enemy, position, path, pathScale, setActive, offsetPath);

        HealthMeter meter = (HealthMeter) PoolerRef.GetEntityFromPool(Pooler.PoolType.HEALTH_METER);
        meter.Position = position;
        meter.Active = true;
        meter.Visible = true;
        meter.parent = enemy;
        meter.Scale = new(1f);
        enemy.meter = meter;
        HealthMeters.Add((GameEntity)meter);
    }



    public void SpawnCheck(float delta)
    {
        Random random = GameCoreRef.RandomNumberGenerator;

        SpawnTimer -= delta;
        if (SpawnerActive && SpawnTimer < 0 && Enemies.Count < 2) 
        {
            // get a random spawn location at a random angle on a radius; 
            float radius = 300f;
            float randomAngle = GameCore.RandomRange(random, 0f, 360f) * MathF.PI / 180;
            Vector2 originPosition = new Vector2(960f, 540f);
            Vector2 offsetPosition = new Vector2(MathF.Cos(randomAngle) * radius, MathF.Sin(randomAngle) * radius);
            Vector2 spawnPosition =  originPosition +  offsetPosition;

            // get a randomPath
            int pathIndex =GameCore.RandomRange(random, 2, 3);
            List<Vector2> randomPath = GameCoreRef.LevelManager.GetShapePath(pathIndex);

            // generate random scale, speed, scale of Enemies
            float randomPathScale = GameCore.RandomRange(random, 50f, 400f);
            float randomSpeed = GameCore.RandomRange(random, 30f, 150f);
            float randomEnemyScale = GameCore.RandomRange(random, 1f, 2f);

            bool offsetPath = false;
            if (pathIndex == 2 || pathIndex == 3) 
            {
                randomPathScale = 1;
                offsetPath = false;
                spawnPosition = Vector2.Zero;
            }

            // spawn the enemy with the random data
            SpawnEnemy(spawnPosition, randomSpeed, randomEnemyScale,  randomPath, randomPathScale, true, offsetPath);

            // reset the timer for next spawn
            SpawnTimer = SpawnDelay;

            Console.WriteLine($"spawnPosition: {spawnPosition} pathIndex:  {pathIndex} randomSpeed: {randomSpeed} randomEnemyScale: {randomEnemyScale}");
        }


    }


}
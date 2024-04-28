
using System;
using FirstSampleGame.Abstract;
using System.Collections.Generic;
using FirstSampleGame.Entities;
using Chroma.Graphics;



public class Pooler
{
    public Dictionary<PoolType, List<GameEntity>> pools;

    public enum PoolType {
        BULLET,
        ENEMY,
        HEALTH_METER
    }


    public Pooler()
    {
        // dictionary of pools
        pools = new Dictionary<PoolType, List<GameEntity>>();

        // create a pool for each pool type
        foreach (PoolType type in Enum.GetValues(typeof(PoolType)))
        {
            pools[type] = new List<GameEntity>();
        }


    }


    // return entity of specified type
    public GameEntity GetEntityFromPool(PoolType poolType)
    {

        var pool = pools[poolType];
        if (pool.Count > 0)
        {
            var entity = pool[0];
            pool.RemoveAt(0);
            return entity;
        }
            else
            {
                //TODO need to create a new entity if one doesn't exist
                return null;
            }
    }

    // reteurn an entity the the specified pool and remove from another list if needed
    public void ReturnEntityToPool<T>(PoolType poolType, GameEntity entity, List<T> removalList = null)
    {
        var pool = pools[poolType];
        pool.Add(entity);

        entity.Visible = false;
        entity.Active = false;

        if (removalList != null)
        {
            T itemToRemove = removalList.Find(item => item.Equals(entity));
            if (itemToRemove != null)
            {
                removalList.Remove(itemToRemove);
            }
        }
    }

    /* //TODO evaluate this method later
    public void ReturnEntityToPool<T>(PoolType poolType, Entity entity, List<T> removalList = null) where T : Entity
    {
        var pool = pools[poolType];
        pool.Add(entity);

        if (removalList != null && removalList.Contains(entity))
        {
            removalList.Remove((T)entity);
        }
    }*/



    // create a pool of health meters
    public void CreateHealthMetersPool(int amount, Texture texture)
    {

        var pool = pools[PoolType.HEALTH_METER];
        for (int index = 0;index < amount;index++)
        {
            HealthMeter meter = new HealthMeter();
            //meter.setTexture(texture);
            pool.Add(meter);
            meter.Visible = false;
            meter.Active = false;
        }

        Console.WriteLine("created health meters pool " + pool.Count);

    }

    // create a pool of bullets
    public void CreateBulletPool(int amount, Texture texture)
    {

        var pool = pools[PoolType.BULLET];
        for (int index = 0;index < amount;index++)
        {
            Bullet bullet = new Bullet();
            bullet.SetTexture(texture);
            pool.Add(bullet);
            bullet.Visible = false;
            bullet.Active = false;
        }

        Console.WriteLine("created bulletsPool " + pool.Count);

    }

    // create a pool of enemies
    public void CreateEnemyPool(int amount, Texture texture)
    {

        var pool = pools[PoolType.ENEMY];
        for (int index = 0;index < amount;index++)
        {
            Enemy enemy = new Enemy();
            enemy.SetTexture(texture);
            pool.Add(enemy);
            enemy.Visible = false;
            enemy.Active = false;
        }

        Console.WriteLine("created enemiesPool " + pool.Count);

    }


}
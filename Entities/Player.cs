using Chroma.Input;
using FirstSampleGame.Abstract;
using System.Numerics;

namespace FirstSampleGame.Entities;

public class Player : GameEntity
{
    public float Speed;
    public Vector2 shotVector;
    public float fireRate;
    public float shotTimer;
    public float shotSpeed;

    public Player(string textureName = "player.png")
    {
        TextureName = textureName;
        Speed = 300;
        shotSpeed = 1500f;
        fireRate = .1f;
        shotVector = Vector2.Zero;
        Scale = new(1f);
    }

    public void Update(float delta, Vector2 direction)
    {
        Vector2 velocity = direction * Speed * delta;
        Position += velocity;
        shotTimer -= delta;
    }


}

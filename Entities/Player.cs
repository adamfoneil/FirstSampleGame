using Chroma.Graphics;
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

    public void Update(float delta, Vector2 direction, Texture collisionTexture)
    {
        shotTimer -= delta;
        Vector2 velocity = direction * Speed * delta;
        Vector2 newPosition = Position + velocity;

        // Check collision with level map in both horizontal and vertical directions
        bool collidesHorizontally = hitTestWithTexture(collisionTexture, new Vector2(newPosition.X, Position.Y));
        bool collidesVertically = hitTestWithTexture(collisionTexture, new Vector2(Position.X, newPosition.Y));

        // If player collides horizontally, reset horizontal position
        if (collidesHorizontally)
        {
            newPosition.X = Position.X;
        }

        // If player collides vertically, reset vertical position
        if (collidesVertically)
        {
            newPosition.Y = Position.Y;
        }

        Position = newPosition;

    }


}

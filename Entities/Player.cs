using Chroma.Graphics;
using FirstSampleGame.Abstract;
using System.Numerics;

namespace FirstSampleGame.Entities;

public class Player : GameEntity
{
	public float Speed;
    public float MinSpeed;
    public float MaxSpeed;
	public Vector2 ShotVector;
	public float FireRate;
	public float ShotTimer;
	public float ShotSpeed;

	public Player(string textureName = "player.png")
	{
		TextureName = textureName;
        MinSpeed = 40f;
        MaxSpeed = 150f;
		Speed = 70f;
		ShotSpeed = 1500f;
		FireRate = .1f;
		ShotVector = Vector2.Zero;
		Scale = new(1f);
	}

    public void Update(float delta, Vector2 direction, Texture collisionTexture)
    {
        ShotTimer -= delta;
        Vector2 velocity = direction * Speed * delta;
        Vector2 newPosition = Position + velocity;

        // Check collision with level map in both horizontal and vertical directions
        bool collidesHorizontally = HitTestWithTexture(collisionTexture, new Vector2(newPosition.X, Position.Y));
        bool collidesVertically = HitTestWithTexture(collisionTexture, new Vector2(Position.X, newPosition.Y));

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

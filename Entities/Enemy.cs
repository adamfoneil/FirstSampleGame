using FirstSampleGame.Abstract;
using System.Numerics;
namespace FirstSampleGame.Entities;

public class Enemy : GameEntity
{

	public float Speed;

	public Enemy(string textureName = "player.png")
	{
		TextureName = textureName;
		Scale = new(1f);
		Speed = 1f;

	}

	public void Update(float delta, Vector2 direction = default)
	{
		if (pathNav.IsActive)
		{
			pathNav.Update(delta, this, Speed);
		}
		else
		{
			//Vector2 velocity = direction * Speed * delta;
			//Position += velocity;
		}

	}


}
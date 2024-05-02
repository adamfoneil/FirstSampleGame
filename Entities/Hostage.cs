using FirstSampleGame.Abstract;
using System.Numerics;
namespace FirstSampleGame.Entities;

public class Hostage : GameEntity
{

	public float Speed;

	public Hostage(string textureName = "player.png")
	{
		TextureName = textureName;
		Scale = new(1f);
		Speed = 1f;

	}

	public void Update(float delta, Vector2 direction = default)
	{
	}


}
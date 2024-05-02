using FirstSampleGame.Abstract;
using System.Numerics;
namespace FirstSampleGame.Entities;

public class LevelItem : GameEntity
{

	public float Speed;

	public LevelItem(string textureName = "player.png")
	{
		TextureName = textureName;
		Scale = new(1f);
		Speed = 1f;

	}

	public void Update(float delta, Vector2 direction = default)
	{
	}


}
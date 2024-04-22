using FirstSampleGame.Abstract;
using System.Numerics;
namespace FirstSampleGame.Entities;
using Chroma.Input;

public class Reticle : GameEntity
{

    public float Speed;

    public Reticle(string textureName = "reticle.png")
    {
        TextureName = textureName;
        Scale = new(.04f);
    }

    public void Update(float delta, Vector2 direction)
    {


        Vector2 velocity = direction * Speed * delta;
        Position += velocity;
    }


}
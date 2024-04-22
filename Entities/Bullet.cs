using FirstSampleGame.Abstract;
using System.Numerics;
namespace FirstSampleGame.Entities;

public class Bullet : GameEntity
{
    public int BulletDamage;
    public float Speed;
    public Vector2 Direction;
    


    public Bullet(string textureName = "bullet.png")
    {
        Direction = Vector2.Zero;
        BulletDamage = 15;
        TextureName = textureName;
        Scale = new(.3f);
    }

    public void Update(float delta)
    {
        Vector2 velocity = Direction * Speed * delta;
        Position += velocity;
    }


}
using Chroma.Input;
using FirstSampleGame.Abstract;
using System.Numerics;

namespace FirstSampleGame.Entities;

internal class Player(Vector2 position, Vector2 speed) : Entity(position, speed)
{
    protected override string TextureName => "player.png";

    protected override void OnUpdate(float verticalDelta, float horizontalDelta)
    {
        if (Keyboard.IsKeyDown(KeyCode.Up))
        {
            Position.Y -= verticalDelta;
        }            
        else if (Keyboard.IsKeyDown(KeyCode.Down))
        {
            Position.Y += verticalDelta;
        }
            
        if (Keyboard.IsKeyDown(KeyCode.Left))
        {
            Position.X -= horizontalDelta;
        }
        else if (Keyboard.IsKeyDown(KeyCode.Right))
        {
            Position.X += horizontalDelta;
        }
    }

}

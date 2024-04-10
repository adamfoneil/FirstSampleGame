using Chroma.ContentManagement;
using Chroma.Graphics;
using System.Numerics;

namespace FirstSampleGame.Abstract;

/// <summary>
/// anything in the game that can be move or interact
/// </summary>
internal abstract class Entity(Vector2 position, Vector2 speed)
{
    public Vector2 Position = position;
    public Vector2 Speed = speed;
    protected abstract string TextureName { get; }
    public Texture Texture { get; private set; }

    internal void LoadTexture(IContentProvider content)
    {
        Texture = content.Load<Texture>(TextureName);
    }

    internal void Update(float delta)
    {
        var verticalDelta = Speed.Y * delta;
        var horizontalDelta = Speed.X * delta;
        OnUpdate(verticalDelta, horizontalDelta);
    }

    protected abstract void OnUpdate(float verticalDelta, float horizontalDelta);
}

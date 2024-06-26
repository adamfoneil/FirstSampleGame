﻿using Chroma.ContentManagement;
using Chroma.Graphics;
using FirstSampleGame.Entities;
using System.Numerics;

namespace FirstSampleGame.Abstract;

/// <summary>
/// anything in the game that can move or interact
/// </summary>
public class GameEntity
{
    public Vector2 Position = Vector2.Zero;
    public Vector2 Scale;
    public string TextureName;
    public Texture Texture;
    public Vector2 TextureCenter;
    public Vector2 VisualCenter;
    public bool Visible;
    public bool Active;
    public int HitPoints;
    public HealthMeter meter;
    public int Damage;
    public PathNav pathNav;


    public GameEntity()
    {
        Scale = new Vector2(1,1);
        Visible = true;
        Active = true;
        HitPoints = 100;
        Damage = 0;
        pathNav = new PathNav();
    
    }

    public Vector2 DrawPosition()
    {
        Vector2 drawAt = new Vector2(Position.X - (Texture.Width * Scale.X) * .5f, Position.Y - (Texture.Height * Scale.Y) *.5f);
        return drawAt;
    }

    public bool HitTestWith(GameEntity entity)
    {
        
        float distance = Vector2.Distance(Position, entity.Position);
        float threshold = (Texture.Width * Scale.X + entity.Texture.Width * entity.Scale.X) * .5f;

        if (distance < threshold) return true;
        return false;

    }

    public bool HitTestWithTexture(Texture texture, Vector2 testPosition = default)
    {

        if (testPosition == default) testPosition = Position;
        Color pixelColor = texture.GetPixel((int)testPosition.X, (int)testPosition.Y);
        if (pixelColor.A != 0) return true;

        return false;

    }

    public void SetTexture(Texture texture)
    {
        Texture = texture;
        TextureCenter = Texture.AbsoluteCenter;
        VisualCenter = Position + TextureCenter;
    }
    

    internal void LoadTexture(IContentProvider content)
    {
        Texture = content.Load<Texture>(TextureName);
        TextureCenter = Texture.AbsoluteCenter;
        VisualCenter = Position + TextureCenter;
    }


}

using Chroma;
using Chroma.ContentManagement;
using Chroma.ContentManagement.FileSystem;
using Chroma.Diagnostics.Logging;
using Chroma.Graphics;
using Chroma.Input;
using FirstSampleGame.Abstract;
using FirstSampleGame.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace FirstSampleGame;

internal class GameCore : Game
{
    private Log Log { get; } = LogManager.GetForCurrentAssembly();    

    private readonly List<Entity> Entities =
    [
        new Player(new(300), new(200)),
    ];

    private Entity Player => Entities[0];

    internal GameCore()
    {        
    }

    protected override void LoadContent()
    {
        foreach (var e in Entities) e.LoadTexture(Content);
    }

    protected override IContentProvider InitializeContentPipeline() =>
        new FileSystemContentProvider(Path.Combine(AppContext.BaseDirectory, "Resources"));       

    protected override void Draw(RenderContext context)
    {                
        var mousePosition = Mouse.GetPosition();
        var delta = Player.VisualCenter - mousePosition;
        var radians = Math.Atan2(delta.Y, delta.X);
        var degrees = radians * (180 / Math.PI);

        context.DrawString($"Mouse position: {mousePosition}, {degrees:00} deg", new(20), Color.White);
        context.Line(Player.VisualCenter, mousePosition, Color.White);

        foreach (var e in Entities) context.DrawTexture(e.Texture, e.Position, new(1));
    }

    protected override void Update(float delta)
    {
        foreach (var e in Entities) e.Update(delta);        
        base.Update(delta);
    }
}

using Chroma;
using Chroma.ContentManagement;
using Chroma.ContentManagement.FileSystem;
using Chroma.Diagnostics.Logging;
using Chroma.Graphics;
using FirstSampleGame.Abstract;
using FirstSampleGame.Entities;
using System;
using System.Collections.Generic;
using System.IO;

namespace FirstSampleGame;

internal class GameCore : Game
{
    private Log Log { get; } = LogManager.GetForCurrentAssembly();

    private IEnumerable<Entity> Entities =
    [
        new Player(new(300), new(200)),
    ];

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
        foreach (var e in Entities) context.DrawTexture(e.Texture, e.Position, new(1));
    }

    protected override void Update(float delta)
    {
        foreach (var e in Entities) e.Update(delta);
        

        base.Update(delta);
    }
}

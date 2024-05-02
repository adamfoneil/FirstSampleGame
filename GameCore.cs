using Chroma;
using Chroma.ContentManagement;
using Chroma.ContentManagement.FileSystem;
using Chroma.Diagnostics.Logging;
using Chroma.Diagnostics;
using Chroma.Graphics;
using Chroma.Input;
using FirstSampleGame.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using FirstSampleGame.Abstract;
using Chroma.Audio.Sources;
using Chroma.Input.GameControllers;
using GameController.Views;
using Chroma.Graphics.Particles;
using Newtonsoft.Json;

//pending
// different shapes for enemies, player, projectiles
// damage indicator text float up
// pools create new object if run out of objects



namespace FirstSampleGame;

public class GameCore : Game
{
    private Log Log { get; } = LogManager.GetForCurrentAssembly();

    // views utilized by the game controller
    private List<GenericControllerView> _views;

    // GameEntity lists for enemies, bullets, and health meters
    public List<GameEntity> Bullets = new List<GameEntity>();
    public List<GameEntity> HealthMeters = new List<GameEntity>();


    // player and reticle instances
    public Player Player;
    public Reticle Reticle;

    // level map
    private GameEntity LevelMap;

    // texture variables
    private Texture BulletTexture;
    private Texture EnemyTexture;
    private Texture LevelItemTexture;
    private Texture HostageTexture;
    private Texture ReticleTexture;

    // toggles for joystick and mouse control
    private bool ActiveJoystick;
    public bool MouseReticle;

    
    // analog sticks
    private Vector2 LeftAnalogStick;
    private Vector2 RightAnalogStick;

    // screen dimensions
    private float screenWidth = 1920f;
    private float screenHeight = 1080f;

    // sound effects
    private Sound sfxShot;
    private Sound sfxHit;
    private Sound sfxExplosion;

    // particle system variables
    private RenderTarget particleSystemRenderTarget;
    private Texture particleTexture;
    private ParticleEmitter particleEmitter;

    public Pooler Pooler = new Pooler();
    public Random RandomNumberGenerator = new Random();

    public LevelManager LevelManager;

    public SpawnManager Spawner;

    public DevMenu DevMenu;

    

    internal GameCore() : base(new(false, false))
    {     
        Window.Mode.SetWindowed((ushort)screenWidth, (ushort)screenHeight, true);

        // init dev menu
        DevMenu = new DevMenu(this);
        Spawner = new SpawnManager(this);


        // set visibility of mouse pointer
        Chroma.Input.Cursor.IsVisible = DevMenu.ShowDevMenu;

        // init contol variables
        ActiveJoystick = false;
        MouseReticle = false;
        Spawner.SpawnerActive = false;


        // init LevelManager
        LevelManager = new LevelManager(this);
        LevelManager.CreateLevels();


        LevelManager.CurrentLevel = 1;

        // create views for controller system
        _views = new List<GenericControllerView>
        {
            new GenericControllerView(Window),
            new DualShockControllerView(Window),
            new DualSenseControllerView(Window),
            new NintendoControllerView(Window)
        };
    }


    protected override IContentProvider InitializeContentPipeline()
    {
        return new FileSystemContentProvider(
            Path.Combine(AppContext.BaseDirectory, "Resources")
        );
    }

    protected override void LoadContent()
    {

        // load textures to be used for GameEntity pools
        BulletTexture = Content.Load<Texture>("bullet.png");
        EnemyTexture = Content.Load<Texture>("pentagram.png");
        LevelItemTexture = Content.Load<Texture>("pentagram.png");
        HostageTexture = Content.Load<Texture>("pentagram.png");
        ReticleTexture = Content.Load<Texture>("reticle.png");

        // create player, reticle, level map
        Player = new Player();
        Reticle = new Reticle();
        LevelMap = new GameEntity();
        LevelMap.TextureName = "test-level.png";


        // load textures for player, reticle, and LevelMap
        Player.LoadTexture(Content);
        Reticle.LoadTexture(Content);
        LevelMap.LoadTexture(Content);

        // set player's initial position in center
        Player.Position = new Vector2(760f, 540f);
        

        // particle system 
        particleSystemRenderTarget = new RenderTarget(Window.Width, Window.Height);
        particleTexture = Content.Load<Texture>("pentagram.png");
        particleTexture.FilteringMode = TextureFilteringMode.NearestNeighbor;

        particleEmitter = new ParticleEmitter(particleTexture)
        {
            Density = 300
        };

        //particleEmitter.RegisterIntegrator(BuiltInParticleStateIntegrators.ScaleDown);
        particleEmitter.RegisterIntegrator(BuiltInParticleStateIntegrators.FadeOut);
        particleEmitter.RegisterIntegrator(CustomStateIntegrator);
        
        // load sound effects
        sfxShot = Content.Load<Sound>("shot.wav");
        sfxHit = Content.Load<Sound>("pop.wav");
        sfxExplosion = Content.Load<Sound>("doomsg.wav");

        // create GameEntity pools
        Pooler.CreateBulletPool(50, BulletTexture);
        Pooler.CreateEnemyPool(50, EnemyTexture);
        Pooler.CreateHealthMetersPool(50, EnemyTexture);
        Pooler.CreateLevelItemPool(50, LevelItemTexture);
        Pooler.CreateHostagePool(50, HostageTexture);

        
        // create meter for player
        HealthMeter meter = (HealthMeter) Pooler.GetEntityFromPool(Pooler.PoolType.HEALTH_METER);
        Player.meter = meter;
        meter.parent = Player;
    
        LevelManager.InitLevel();

    }

    protected override void Draw(RenderContext context)
    {                

        context.DrawTexture(LevelMap.Texture, LevelMap.Position, LevelMap.Scale);
        // render blank background for particle emitter to texture
        context.RenderTo(particleSystemRenderTarget, (ctx, tgt) =>
        {
            ctx.Clear(new Color (0f, 0f, 0f, 0f));
            particleEmitter.Draw(context);
        });

        // draw the particle system render texture
        context.DrawTexture(particleSystemRenderTarget, Vector2.Zero, Vector2.One, Vector2.Zero, 0);

        // draw debug menu text
        context.DrawString($" FPS {PerformanceCounter.FPS}\n <1> DevMenu.ShowDevMenu: {DevMenu.ShowDevMenu} \n <2> Player.Speed: {Player.Speed} \n <3> ShotSpeed: {Player.ShotSpeed} \n <4> FireRate:{Player.FireRate:0.##} \n <5> SpawnerActive: {Spawner.SpawnerActive} \n <6> MouseReticle: {MouseReticle} \n <7> PathLogging: {DevMenu.PathLogging} \n Bullets: {Bullets.Count} \n Enemies: {Spawner.Enemies.Count} \n mousePosition: {Reticle.Position} \n LeftAnalogStick: {LeftAnalogStick} \n RightAnalogStick: {RightAnalogStick}", new(20), Chroma.Graphics.Color.White);


        // draw player , reticle and health meter
        context.DrawTexture(Player.Texture, Player.DrawPosition(), Player.Scale);
        context.DrawTexture(Reticle.Texture, Reticle.DrawPosition(), Reticle.Scale);
        Player.meter.drawHealthMeter(context);

        // draw bullets
        foreach (var bullet in Bullets)
        {
            if (bullet.Visible) context.DrawTexture(bullet.Texture, bullet.DrawPosition(), bullet.Scale);
        }

        // draw enemies
        foreach (var enemy in Spawner.Enemies)
        {
            if (enemy.Visible) context.DrawTexture(enemy.Texture, enemy.DrawPosition(), enemy.Scale);
            enemy.meter.drawHealthMeter(context);

            // only draw paths and reticle ray if DevMenu.ShowDevMenu is true
            if (DevMenu.ShowDevMenu)
            {
                PathNav.DrawPath(enemy.pathNav.path, context);
                context.Line(Player.Position, Reticle.Position, Color.White);
                if (DevMenu.PathLogging)
                {
                    PathNav.DrawPath(DevMenu.PathLoggingPath, context);
                }
            } 
        }

            // only draw paths and reticle ray if DevMenu.ShowDevMenu is true
            if (DevMenu.ShowDevMenu && DevMenu.PathLogging)
            {
                PathNav.DrawPath(DevMenu.PathLoggingPath, context);
            } 
    }

    protected override void Update(float delta)
    {

        // update controllers data
        for (var i = 0; i < _views.Count; i++)
            _views[i].Update(delta);


        // handle RightAnalogStick or mouse control of reticle
        if (ActiveJoystick && !MouseReticle)
        {
            Reticle.Position = Player.Position + RightAnalogStick * 50f;
            float reticleDistance = Vector2.Distance(Reticle.Position, Player.Position);
            if (50f - reticleDistance <  5f) FireBullet();
        }
            else
            {
                Reticle.Position = Mouse.GetPosition();
            }
        // calculate player's ShotVector based on reticle position
        Player.ShotVector = Reticle.Position - Player.Position;

        // update bullet positions
        foreach (var bullet in Bullets)
        {
            if (bullet.Active) ((Bullet)bullet).Update(delta);
        }

        // update enemy positions
        foreach (var enemy in Spawner.Enemies)
        {
            if (enemy.Active) ((Enemy)enemy).Update(delta);
        }


        // handle LeftAnalogStick and keyboard for getting direction of player
        Vector2 playerDirection = ProcessKeyboardInput();
        if (playerDirection.X == 0 && playerDirection.Y == 0)
        {
            playerDirection = ProcessAnalogStick(0);
        }
        // update player location (collision detection with map is done in Udpate method)
        Player.Update(delta, playerDirection, LevelMap.Texture);

        // execute collision checks
        CheckBulletCollision();
        CheckPlayerCollision();

        


        // remove bullets that have left the screen
        var bulletsToKill = new List<GameEntity>();
        foreach (var bullet in Bullets)
        {
            if (bullet.Position.X > screenWidth || bullet.Position.X < 0 || bullet.Position.Y > screenHeight || bullet.Position.Y < 0)
            {
                bulletsToKill.Add(bullet);
            }
        }
        RemoveBullets(bulletsToKill);


        // handle LeftAnalogStick and keyboard for getting direction of player
        //Vector2 playerDirection = ProcessKeyboardInput();
        if (playerDirection.X == 0 && playerDirection.Y == 0)
        {
            playerDirection = ProcessAnalogStick(0);
        }

        // update player's position based on direction
        Player.Update(delta, playerDirection, LevelMap.Texture);
        
        // update particle emitter
        particleEmitter.Update(delta);

        // handle debug menu hotkeys
        DevMenu.ProcessDevMenuInput(delta);

        // handle mouse input for firing a bullet
        if (Mouse.IsButtonDown(MouseButton.Left))
        {
            if (DevMenu.PathLogging && DevMenu.KeypressTimer < 0)
            {
                Console.WriteLine($"new Vector2({Mouse.GetPosition().X}, {Mouse.GetPosition().Y}),");
                DevMenu.KeypressTimer = DevMenu.KeypressDelay;
                DevMenu.PathLoggingPath.Add(Mouse.GetPosition());
            }
                else
                {
                    if (!DevMenu.PathLogging) FireBullet();
                }

            FireBullet();
        }

        // check if enemy should be spawned
        Spawner.SpawnCheck(delta);


        base.Update(delta);
    }

    // returns a 
    Vector2 ProcessAnalogStick(int index)
    {
        Vector2 stick = LeftAnalogStick;
        if (index == 0)
        {
            stick = LeftAnalogStick;
        } 
            else
            {
                stick = RightAnalogStick;
            }

        Vector2 direction = Vector2.Zero;

        if (stick.X > .5f) direction.X = 1;
        if (stick.X < -.5f) direction.X = -1;
        if (stick.Y > .5f) direction.Y = 1;
        if (stick.Y < -.5f) direction.Y = -1;

        return stick;
    }

    // returns a direction based on keyboard movement
    // 8 possible directions
    Vector2 ProcessKeyboardInput()
    {
        Vector2 direction = Vector2.Zero;
        if (Keyboard.IsKeyDown(KeyCode.Up) || Keyboard.IsKeyDown(KeyCode.W))
        {
            direction.Y = -1;
        }            
        if (Keyboard.IsKeyDown(KeyCode.Down) || Keyboard.IsKeyDown(KeyCode.S))
        {
            direction.Y = 1;
        }
            
        if (Keyboard.IsKeyDown(KeyCode.Left) || Keyboard.IsKeyDown(KeyCode.A))
        {
            direction.X = -1;
        }
        else if (Keyboard.IsKeyDown(KeyCode.Right) || Keyboard.IsKeyDown(KeyCode.D))
        {
            direction.X = 1;
        }
        return direction;
    }

    
    // fires a bullet in direction of the reticle
    void FireBullet()
    {
        if (Player.ShotTimer > 0) return;
        Player.ShotTimer = Player.FireRate;
        sfxShot.Play();
        Bullet bullet = (Bullet)Pooler.GetEntityFromPool(Pooler.PoolType.BULLET);
        Bullets.Add(bullet);
        bullet.Speed = Player.ShotSpeed;
        bullet.Visible = true;
        bullet.Active = true;
        bullet.Position = Player.Position;

        bullet.Direction = Vector2.Normalize(Player.ShotVector);
    }


    // functions for random numbers in a range

    public static float RandomRange(Random rand, float min, float max)
    {
        return (float) (rand.NextDouble() * (max - min) + min);
    }
    public static int RandomRange(Random rand, int min, int max)
    {
        return rand.Next(min, max + 1);
    }


    // checks for player collision with enemies and applies damage, plays sound effect
    void CheckPlayerCollision()
    {
        for (int enemyIndex = 0; enemyIndex < Spawner.Enemies.Count; enemyIndex++)
        {
            var enemy = Spawner.Enemies[enemyIndex];
            if (Player.HitTestWith(enemy))
            {
                Player.Damage += 1;
                if (Player.Damage > Player.HitPoints) Player.Damage = 0; // reset health because he doesn't die yet
                sfxHit.Play();
            }
        }
    }


    // checks for collision betweeen entities in bullets and enemies lists
    void CheckBulletCollision()
    {
        var enemiesToKill = new List<GameEntity>();
        var bulletsToKill = new List<GameEntity>();
        var metersToKill = new List<GameEntity>();

        foreach (GameEntity bullet in Bullets)  
        {
            foreach (GameEntity enemy in Spawner.Enemies)
            {
                if (enemy.HitTestWith(bullet) && enemy.Active) 
                {
                    enemy.Damage += ((Bullet)bullet).BulletDamage;
                    if (enemy.Damage >= enemy.HitPoints)
                    {
                        sfxExplosion.Play();
                        particleEmitter.Emit(enemy.Position, 10);
                        enemy.Active = false;
                        enemiesToKill.Add(enemy);
                        metersToKill.Add(enemy.meter);

                    }
                    else 
                    {
                        sfxHit.Play();
                    }
                    bulletsToKill.Add(bullet);
                    break;
                }
            }
        } 

        // remove entities that need to be recycled
        RemoveEnemies(enemiesToKill);
        RemoveBullets(bulletsToKill);
        RemoveMeters(metersToKill);

    }


    // batch removal of various entities

    private void RemoveBullets(List<GameEntity> bulletsToKill)
    {
        while (bulletsToKill.Count > 0)
        {
            var bullet = bulletsToKill[0];
            bulletsToKill.RemoveAt(0);
            Pooler.ReturnEntityToPool(Pooler.PoolType.BULLET, bullet, Bullets);
        }
    }

    private void RemoveEnemies(List<GameEntity> enemiesToKill)
    {
        while (enemiesToKill.Count > 0)
        {
            var enemy = enemiesToKill[0];
            enemiesToKill.RemoveAt(0);
            Pooler.ReturnEntityToPool(Pooler.PoolType.ENEMY, enemy, Spawner.Enemies);
        }
    }

    private void RemoveMeters(List<GameEntity> metersToKill)
    {
        while (metersToKill.Count > 0)
        {
            var meter = metersToKill[0];
            metersToKill.RemoveAt(0);
            GameEntity parent = ((HealthMeter)meter).parent;
            parent.meter = null;
            ((HealthMeter)meter).parent = null;
            Pooler.ReturnEntityToPool(Pooler.PoolType.HEALTH_METER, meter, HealthMeters);
        }
    }

    // game controller event handlers

    protected override void ControllerDisconnected(ControllerEventArgs e)
    {
        Console.WriteLine("controller disconnected");
        ActiveJoystick = false;
        for (var i = 0; i < _views.Count; i++)
        {
            if (_views[i].AcceptedControllers.Contains(e.Controller.Info.Type))
                _views[i].OnDisconnected(e);
        }
    }

    protected override void ControllerConnected(ControllerEventArgs e)
    {
        Console.WriteLine("controller connected");
        ActiveJoystick = true;
        for (var i = 0; i < _views.Count; i++)
        {
            if (_views[i].AcceptedControllers.Contains(e.Controller.Info.Type))
                _views[i].OnConnected(e);
        }
    }

    protected override void ControllerAxisMoved(ControllerAxisEventArgs e)
    {
        //if (e.Axis == ControllerAxis.LeftStickX) Console.WriteLine($"Axis {e.Axis} Value: {e.Value}");
        if (e.Axis == ControllerAxis.LeftStickX) LeftAnalogStick.X = NormalizeValue(e.Value);
        if (e.Axis == ControllerAxis.LeftStickY) LeftAnalogStick.Y = NormalizeValue(e.Value);
        if (e.Axis == ControllerAxis.RightStickX) RightAnalogStick.X = NormalizeValue(e.Value);
        if (e.Axis == ControllerAxis.RightStickY) RightAnalogStick.Y = NormalizeValue(e.Value);


        for (var i = 0; i < _views.Count; i++)
        {
            if (_views[i].AcceptedControllers.Contains(e.Controller.Info.Type))
                _views[i].OnAxisMoved(e);
        }
    }

    protected override void ControllerButtonPressed(ControllerButtonEventArgs e)
    {

        Console.WriteLine($"Button Pressed {e.Button} ");
        if (e.Button == ControllerButton.A) FireBullet();
        
        for (var i = 0; i < _views.Count; i++)
        {
            if (_views[i].AcceptedControllers.Contains(e.Controller.Info.Type))
                _views[i].OnButtonPressed(e);
        }
    }

    protected override void ControllerButtonReleased(ControllerButtonEventArgs e)
    {
        for (var i = 0; i < _views.Count; i++)
        {
            if (_views[i].AcceptedControllers.Contains(e.Controller.Info.Type))
                _views[i].OnButtonReleased(e);
        }
    }

    public float NormalizeValue(int value)
    {
        return (float)value / 32768.0f;
    }


    // custom particle state integrator
    private void CustomStateIntegrator(Particle part, float delta)
    {
        part.Origin = part.Owner.Texture.Center;

        part.Position.X += part.Velocity.X * 5 * delta;
        part.Position.Y += part.Velocity.Y * delta;

        part.Rotation += part.Velocity.X - part.Velocity.Y * delta;

        part.Velocity.X *= (float)part.TTL / part.InitialTTL;
        if (part.Velocity.Y < 0)
            part.Velocity.Y *= -1;
        part.Scale = part.InitialScale * (((float)part.TTL / part.InitialTTL) * .5f);
    }

    public static dynamic LoadJsonFile(string filename)
    {
        string filePath = Path.Combine(AppContext.BaseDirectory, "Resources/", filename);

        Console.WriteLine(filePath);
        
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);

            dynamic jsonObject = JsonConvert.DeserializeObject<dynamic>(json);


            return jsonObject;

        }
            else
            {
                Console.WriteLine("file does not exist");
            }

        return null;
    }



    public void SaveObjectToJsonFile(LevelRecordObj obj, string filePath)
    {
        string json = JsonConvert.SerializeObject(obj, Formatting.Indented);

        File.WriteAllText(filePath, json);
    }

    public void SaveLevelRecordToJson()
    {
        string filename = "level.json";
        string filePath = Path.Combine(AppContext.BaseDirectory, "Resources/", filename);
        
        LevelRecordObj obj = new LevelRecordObj
        {
            level = 1,
            textureName = "test-level.png",
            playerSpawnPosition = new Vector2(5,5),
            spawnEnemyRecords = new List<SpawnEnemyRecord>()

        };

        SaveObjectToJsonFile(obj, filePath);

        Console.WriteLine("JSON file saved successfully.");
    }

}



public class LevelRecordObj
{
    public int level;
	public string textureName;
	public Vector2 playerSpawnPosition;
	public List<SpawnEnemyRecord> spawnEnemyRecords;

}
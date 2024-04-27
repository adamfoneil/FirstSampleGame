using Chroma;
using Chroma.Audio.Sources;
using Chroma.ContentManagement;
using Chroma.ContentManagement.FileSystem;
using Chroma.Diagnostics;
using Chroma.Diagnostics.Logging;
using Chroma.Graphics;
using Chroma.Graphics.Particles;
using Chroma.Input;
using Chroma.Input.GameControllers;
using FirstSampleGame.Abstract;
using FirstSampleGame.Entities;
using GameController.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

//pending

// explosion effects
// more shape paths
// different shapes for enemies, player, projectiles
// cycling through shot damage
// damage indicator text float up
// pools create new object if run out of objects
// ability to center shapes on a position
// enemies that fire at player
// different projectiles for enemies
// weapon types - shotgun, rapid fire, normal
// animated sprites


namespace FirstSampleGame;

internal class GameCore : Game
{
	private Log Log { get; } = LogManager.GetForCurrentAssembly();

	// views utilized by the game controller
	private List<GenericControllerView> _views;

	// GameEntity lists for enemies, bullets, and health meters
	private List<GameEntity> enemies = new List<GameEntity>();
	private List<GameEntity> bullets = new List<GameEntity>();
	private List<GameEntity> healthMeters = new List<GameEntity>();


	// player and reticle instances
	private Player Player;
	private Reticle Reticle;

	// level map
	private GameEntity LevelMap;

	// texture variables
	private Texture BulletTexture;
	private Texture EnemyTexture;
	private Texture ReticleTexture;

	// level texture names
	private List<string> LeveTexturelNames = new List<string>();


	// spawner variables
	private float spawnTimer;
	private float spawnDelay;
	private bool showDebug;
	private bool spawnerActive;

	// keypress delay for debug menu
	private float keypressTimer;
	private float keypressDelay;

	// toggles for joystick and mouse control
	private bool activeJoystick;
	private bool mouseReticle;

	// toggle Path logging
	private bool pathLogging = false;

	private List<Vector2> pathLoggingPath;

	// analog sticks
	private Vector2 leftStick;
	private Vector2 rightStick;

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


	public Pooler pooler = new Pooler();
	private Random random = new Random();
	private LevelManager levelManager;
	private int CurrentLevel;


	internal GameCore() : base(new(false, false))
	{
		Window.Mode.SetWindowed((ushort)screenWidth, (ushort)screenHeight, true);

		// init debug menu keypress delay
		keypressDelay = .25f;
		keypressTimer = keypressDelay;

		// set visibility of mouse pointer
		Chroma.Input.Cursor.IsVisible = showDebug;

		// init contol variables
		activeJoystick = false;
		mouseReticle = false;
		showDebug = false;
		spawnerActive = false;

		// init LevelManager
		levelManager = new LevelManager();
		levelManager.CreateLevels();

		// create views for controller system
		_views =
		[
			new GenericControllerView(Window),
			new DualShockControllerView(Window),
			new DualSenseControllerView(Window),
			new NintendoControllerView(Window)
		];
	}


	protected override IContentProvider InitializeContentPipeline()
	{
		return new FileSystemContentProvider(
			//Path.Combine(AppContext.BaseDirectory, "../../../Resources")
			Path.Combine(AppContext.BaseDirectory, "Resources")
		);
	}





	protected override void LoadContent()
	{

		// load textures to be used for GameEntity pools
		BulletTexture = Content.Load<Texture>("bullet.png");
		EnemyTexture = Content.Load<Texture>("pentagram.png");
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
		pooler.CreateBulletPool(50, BulletTexture);
		pooler.CreateEnemyPool(50, EnemyTexture);
		pooler.CreateHealthMetersPool(50, EnemyTexture);


		// create meter for player
		HealthMeter meter = (HealthMeter)pooler.GetEntityFromPool(Pooler.PoolType.HEALTH_METER);
		Player.meter = meter;
		meter.parent = Player;

		// init spawner delay
		spawnDelay = 2f;
		spawnTimer = spawnDelay;

		InitLevel(1);

	}

	public void InitLevel(int level)
	{
		LevelRecord currentLevel = levelManager.GetCurrentLevel();
		foreach (var spawnRecord in currentLevel.SpawnRecords)
		{
			SpawnEnemyWithRecord(spawnRecord);
		}
	}

	protected override void Draw(RenderContext context)
	{

		context.DrawTexture(LevelMap.Texture, LevelMap.Position, LevelMap.Scale);
		// render blank background for particle emitter to texture
		context.RenderTo(particleSystemRenderTarget, (ctx, tgt) =>
		{
			ctx.Clear(new Color(0f, 0f, 0f, 0f));
			particleEmitter.Draw(context);
		});

		// draw the particle system render texture
		context.DrawTexture(particleSystemRenderTarget, Vector2.Zero, Vector2.One, Vector2.Zero, 0);

		// draw debug menu text
		context.DrawString($" FPS {PerformanceCounter.FPS}\n <1> showDebug: {showDebug} \n <2> Player.Speed: {Player.Speed} \n <3> shotSpeed: {Player.shotSpeed} \n <4> fireRate:{Player.fireRate:0.##} \n <5> spawnerActive: {spawnerActive} \n <6> mouseReticle: {mouseReticle} \n <7> pathLogging: {pathLogging} \n Bullets: {bullets.Count} \n Enemies: {enemies.Count} \n mousePosition: {Reticle.Position} \n leftStick: {leftStick} \n rightStick: {rightStick}", new(20), Chroma.Graphics.Color.White);


		// draw player , reticle and health meter
		context.DrawTexture(Player.Texture, Player.DrawPosition(), Player.Scale);
		context.DrawTexture(Reticle.Texture, Reticle.DrawPosition(), Reticle.Scale);
		Player.meter.drawHealthMeter(context);

		// draw bullets
		foreach (var bullet in bullets)
		{
			if (bullet.Visible) context.DrawTexture(bullet.Texture, bullet.DrawPosition(), bullet.Scale);
		}

		// draw enemies
		foreach (var enemy in enemies)
		{
			if (enemy.Visible) context.DrawTexture(enemy.Texture, enemy.DrawPosition(), enemy.Scale);
			enemy.meter.drawHealthMeter(context);

			// only draw paths and reticle ray if showDebug is true
			if (showDebug)
			{
				PathNav.DrawPath(enemy.pathNav.path, context);
				context.Line(Player.Position, Reticle.Position, Color.White);
				if (pathLogging)
				{
					Console.WriteLine("draw it");
					PathNav.DrawPath(pathLoggingPath, context);
				}
			}
		}

		// only draw paths and reticle ray if showDebug is true
		if (showDebug && pathLogging)
		{
			PathNav.DrawPath(pathLoggingPath, context);
		}
	}

	protected override void Update(float delta)
	{

		// update controllers data
		for (var i = 0; i < _views.Count; i++)
			_views[i].Update(delta);


		// handle rightStick or mouse control of reticle
		if (activeJoystick && !mouseReticle)
		{
			Reticle.Position = Player.Position + rightStick * 50f;
			float reticleDistance = Vector2.Distance(Reticle.Position, Player.Position);
			if (50f - reticleDistance < 5f) FireBullet();
		}
		else
		{
			Reticle.Position = Mouse.GetPosition();
		}
		// calculate player's shotVector based on reticle position
		Player.shotVector = Reticle.Position - Player.Position;

		// update bullet positions
		foreach (var bullet in bullets)
		{
			if (bullet.Active) ((Bullet)bullet).Update(delta);
		}

		// update enemy positions
		foreach (var enemy in enemies)
		{
			if (enemy.Active) ((Enemy)enemy).Update(delta);
		}


		// handle leftStick and keyboard for getting direction of player
		Vector2 playerDirection = ProcessKeyboardInput();
		if (playerDirection.X == 0 && playerDirection.Y == 0)
		{
			playerDirection = ProcessAnalogStick(0);
		}

		Vector2 lastPosition = Player.Position;

		// update player's position based on direction
		Player.Update(delta, playerDirection);

		// check player collision with level map
		if (Player.HitTestWithTexture(LevelMap.Texture)) Player.Position = lastPosition;

		// execute collision checks
		CheckBulletCollision();
		CheckPlayerCollision();




		// remove bullets that have left the screen
		var bulletsToKill = new List<GameEntity>();
		foreach (var bullet in bullets)
		{
			if (bullet.Position.X > screenWidth || bullet.Position.X < 0 || bullet.Position.Y > screenHeight || bullet.Position.Y < 0)
			{
				bulletsToKill.Add(bullet);
			}
		}
		RemoveBullets(bulletsToKill);


		// handle leftStick and keyboard for getting direction of player
		//Vector2 playerDirection = ProcessKeyboardInput();
		if (playerDirection.X == 0 && playerDirection.Y == 0)
		{
			playerDirection = ProcessAnalogStick(0);
		}

		// update player's position based on direction
		Player.Update(delta, playerDirection);

		// update particle emitter
		particleEmitter.Update(delta);

		// handle debug menu hotkeys
		keypressTimer -= delta;
		if (keypressTimer < 0)
		{
			if (Keyboard.IsKeyDown(KeyCode.Alpha1))
			{
				showDebug = !showDebug;
				Chroma.Input.Cursor.IsVisible = showDebug;
				keypressTimer = keypressDelay;
			}

			if (Keyboard.IsKeyDown(KeyCode.Alpha2))
			{
				Player.Speed += 25f;
				if (Player.Speed > 500f) Player.Speed = 200f;
				keypressTimer = keypressDelay;
			}

			if (Keyboard.IsKeyDown(KeyCode.Alpha3))
			{
				Player.shotSpeed += 250f;
				if (Player.shotSpeed > 2500f) Player.shotSpeed = 1000f;
				keypressTimer = keypressDelay;
			}

			if (Keyboard.IsKeyDown(KeyCode.Alpha4))
			{
				Player.fireRate -= .1f;
				if (Player.fireRate < .1f) Player.fireRate = .5f;
				keypressTimer = keypressDelay;
			}

			if (Keyboard.IsKeyDown(KeyCode.Alpha5))
			{
				spawnerActive = !spawnerActive;
				keypressTimer = keypressDelay;
			}

			if (Keyboard.IsKeyDown(KeyCode.Alpha6))
			{
				mouseReticle = !mouseReticle;
				keypressTimer = keypressDelay;
			}

			if (Keyboard.IsKeyDown(KeyCode.Alpha7))
			{
				pathLogging = !pathLogging;
				if (pathLogging)
				{
					pathLoggingPath = new List<Vector2>();
					showDebug = true;
					mouseReticle = true;
				}
				keypressTimer = keypressDelay;
			}
		}

		// handle mouse input for firing a bullet
		if (Mouse.IsButtonDown(MouseButton.Left))
		{

			if (pathLogging && keypressTimer < 0)
			{
				Console.WriteLine($"new Vector2({Mouse.GetPosition().X}, {Mouse.GetPosition().Y}),");
				keypressTimer = keypressDelay;
				pathLoggingPath.Add(Mouse.GetPosition());
			}
			else
			{
				if (!pathLogging) FireBullet();
			}

			FireBullet();
		}

		// check if enemy should be spawned
		SpawnCheck(delta);


		base.Update(delta);
	}

	// returns a 
	Vector2 ProcessAnalogStick(int index)
	{
		Vector2 stick = leftStick;
		if (index == 0)
		{
			stick = leftStick;
		}
		else
		{
			stick = rightStick;
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

	void SpawnCheck(float delta)
	{

		spawnTimer -= delta;
		if (spawnerActive && spawnTimer < 0 && enemies.Count < 2)
		{
			// get a random spawn location at a random angle on a radius; 
			float radius = 300f;
			float randomAngle = RandomRange(random, 0f, 360f) * MathF.PI / 180;
			Vector2 originPosition = new Vector2(960f, 540f);
			Vector2 offsetPosition = new Vector2(MathF.Cos(randomAngle) * radius, MathF.Sin(randomAngle) * radius);
			Vector2 spawnPosition = originPosition + offsetPosition;

			// get a randomPath
			int pathIndex = RandomRange(random, 2, 3);
			List<Vector2> randomPath = PathNav.GetShapePath(pathIndex);

			// generate random scale, speed, scale of enemies
			float randomPathScale = RandomRange(random, 50f, 400f);
			float randomSpeed = RandomRange(random, 30f, 150f);
			float randomEnemyScale = RandomRange(random, 1f, 2f);

			bool offsetPath = false;
			if (pathIndex == 2 || pathIndex == 3)
			{
				randomPathScale = 1;
				offsetPath = false;
				spawnPosition = Vector2.Zero;
			}

			// spawn the enemy with the random data
			SpawnEnemy(spawnPosition, randomSpeed, randomEnemyScale, randomPath, randomPathScale, true, offsetPath);

			// reset the timer for next spawn
			spawnTimer = spawnDelay;

			Console.WriteLine($"spawnPosition: {spawnPosition} pathIndex:  {pathIndex} randomSpeed: {randomSpeed} randomEnemyScale: {randomEnemyScale}");
		}


	}


	// fires a bullet in direction of the reticle
	void FireBullet()
	{
		if (Player.shotTimer > 0) return;
		Player.shotTimer = Player.fireRate;
		sfxShot.Play();
		Bullet bullet = (Bullet)pooler.GetEntityFromPool(Pooler.PoolType.BULLET);
		bullets.Add(bullet);
		bullet.Speed = Player.shotSpeed;
		bullet.Visible = true;
		bullet.Active = true;
		bullet.Position = Player.Position;

		bullet.Direction = Vector2.Normalize(Player.shotVector);
	}


	void SpawnEnemyWithRecord(SpawnRecord spawnRecord)
	{
		SpawnEnemy(spawnRecord.SpawnPosition, spawnRecord.EnemySpeed, spawnRecord.EnemyScale, spawnRecord.EnemyPath, 1f, true, false);
	}

	// spawns and enemy and health meter
	void SpawnEnemy(Vector2 position, float speed, float enemyScale, List<Vector2> path = default, float pathScale = 1f, bool setActive = true, bool offsetPath = true)
	{
		Enemy enemy = (Enemy)pooler.GetEntityFromPool(Pooler.PoolType.ENEMY);
		enemy.Position = position;
		enemy.Active = true;
		enemy.Visible = true;
		enemy.Speed = speed;
		enemy.Scale = new(enemyScale);
		enemies.Add((GameEntity)enemy);

		if (path != default) enemy.pathNav.SetPath(enemy, position, path, pathScale, setActive, offsetPath);

		HealthMeter meter = (HealthMeter)pooler.GetEntityFromPool(Pooler.PoolType.HEALTH_METER);
		meter.Position = position;
		meter.Active = true;
		meter.Visible = true;
		meter.parent = enemy;
		meter.Scale = new(1f);
		enemy.meter = meter;
		healthMeters.Add((GameEntity)meter);
	}

	// functions for random numbers in a range

	static float RandomRange(Random rand, float min, float max)
	{
		return (float)(rand.NextDouble() * (max - min) + min);
	}
	static int RandomRange(Random rand, int min, int max)
	{
		return rand.Next(min, max + 1);
	}


	// checks for player collision with enemies and applies damage, plays sound effect
	void CheckPlayerCollision()
	{
		for (int enemyIndex = 0; enemyIndex < enemies.Count; enemyIndex++)
		{
			var enemy = enemies[enemyIndex];
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

		foreach (GameEntity bullet in bullets)
		{
			foreach (GameEntity enemy in enemies)
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
			pooler.ReturnEntityToPool(Pooler.PoolType.BULLET, bullet, bullets);
		}
	}

	private void RemoveEnemies(List<GameEntity> enemiesToKill)
	{
		while (enemiesToKill.Count > 0)
		{
			var enemy = enemiesToKill[0];
			enemiesToKill.RemoveAt(0);
			pooler.ReturnEntityToPool(Pooler.PoolType.ENEMY, enemy, enemies);
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
			pooler.ReturnEntityToPool(Pooler.PoolType.HEALTH_METER, meter, healthMeters);
		}
	}

	// game controller event handlers

	protected override void ControllerDisconnected(ControllerEventArgs e)
	{
		Console.WriteLine("controller disconnected");
		activeJoystick = false;
		for (var i = 0; i < _views.Count; i++)
		{
			if (_views[i].AcceptedControllers.Contains(e.Controller.Info.Type))
				_views[i].OnDisconnected(e);
		}
	}

	protected override void ControllerConnected(ControllerEventArgs e)
	{
		Console.WriteLine("controller connected");
		activeJoystick = true;
		for (var i = 0; i < _views.Count; i++)
		{
			if (_views[i].AcceptedControllers.Contains(e.Controller.Info.Type))
				_views[i].OnConnected(e);
		}
	}

	protected override void ControllerAxisMoved(ControllerAxisEventArgs e)
	{
		//if (e.Axis == ControllerAxis.LeftStickX) Console.WriteLine($"Axis {e.Axis} Value: {e.Value}");
		if (e.Axis == ControllerAxis.LeftStickX) leftStick.X = NormalizeValue(e.Value);
		if (e.Axis == ControllerAxis.LeftStickY) leftStick.Y = NormalizeValue(e.Value);
		if (e.Axis == ControllerAxis.RightStickX) rightStick.X = NormalizeValue(e.Value);
		if (e.Axis == ControllerAxis.RightStickY) rightStick.Y = NormalizeValue(e.Value);


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

}

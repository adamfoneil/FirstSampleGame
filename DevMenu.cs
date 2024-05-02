using FirstSampleGame;
using Chroma.Input;
using System.Collections.Generic;
using System.Numerics;
using FirstSampleGame.Entities;

public class DevMenu
{
    public float KeypressTimer = 0.0f;
    public float KeypressDelay = 0.2f;
    public bool ShowDevMenu;
    public bool PathLogging;
    public List<Vector2> PathLoggingPath;
    private GameCore GameCoreRef;

    public DevMenu(GameCore gameCore)
    {
        GameCoreRef = gameCore;
        PathLogging = false;
        PathLoggingPath = new List<Vector2>();

    }

    public void ProcessDevMenuInput(float delta)
    {
        Player player = GameCoreRef.Player;
        KeypressTimer -= delta;
        if (KeypressTimer < 0)
        {
            if (Keyboard.IsKeyDown(KeyCode.Alpha1))
            {
                ShowDevMenu = !ShowDevMenu;
                Chroma.Input.Cursor.IsVisible = ShowDevMenu;
                KeypressTimer = KeypressDelay;
                GameCoreRef.SaveLevelRecordToJson();
            }

            if (Keyboard.IsKeyDown(KeyCode.Alpha2))
            {
                player.Speed += 5f;
                if (player.Speed > player.MaxSpeed) player.Speed = player.MinSpeed;
                KeypressTimer = KeypressDelay;
            }

            if (Keyboard.IsKeyDown(KeyCode.Alpha3))
            {
                player.ShotSpeed += 250f;
                if (player.ShotSpeed > 2500f) player.ShotSpeed = 1000f;
                KeypressTimer = KeypressDelay;
            }

            if (Keyboard.IsKeyDown(KeyCode.Alpha4))
            {
                player.FireRate -= .1f;
                if (player.FireRate < .1f) player.FireRate = .5f;
                KeypressTimer = KeypressDelay;
            }

            if (Keyboard.IsKeyDown(KeyCode.Alpha5))
            {
                GameCoreRef.Spawner.SpawnerActive = !GameCoreRef.Spawner.SpawnerActive;
                KeypressTimer = KeypressDelay;
            }

            if (Keyboard.IsKeyDown(KeyCode.Alpha6))
            {
                GameCoreRef.MouseReticle = !GameCoreRef.MouseReticle;
                KeypressTimer = KeypressDelay;
            }

            if (Keyboard.IsKeyDown(KeyCode.Alpha7))
            {
                PathLogging = !PathLogging;
                if (PathLogging) 
                {
                    PathLoggingPath = new List<Vector2>();
                    ShowDevMenu = true;
                    GameCoreRef.MouseReticle = true;
                }
                KeypressTimer = KeypressDelay;
            }
        }
    }
}
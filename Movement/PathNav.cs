using Chroma.Graphics;
using FirstSampleGame;
using FirstSampleGame.Abstract;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;

public class PathNav
{
	public List<Vector2> path;
	public Vector2 wayPoint;
	private int pathIndex = 0;
	public bool IsActive;
	public bool isLooping;

	public PathNav()
	{
		IsActive = false;
		path = new List<Vector2>();
		wayPoint = Vector2.Zero;
		isLooping = true;

	}



	// set path for entity
	public void SetPath(GameEntity entity, Vector2 origin, List<Vector2> newPath, float scale = 1f, bool setActive = true, bool offsetPath = true)
	{
		if (scale != 1f)
		{
			path = ScalePath(newPath, scale);
		}
		else
		{
			path = newPath;
		}

		// offset path data based on entity's origin
		if (offsetPath) path = PathNav.OffSetPath(path, origin);

		IsActive = setActive;

		pathIndex = 0;

		// set entity's position to first path position
		entity.Position = path[pathIndex];
		Console.WriteLine(entity.Position);

		// get next waypoint (where the entity will travel to)
		NextWayPoint();
	}

	// return offset path data based on an origin
	public static List<Vector2> OffSetPath(List<Vector2> newPath, Vector2 origin)
	{
		List<Vector2> adjustedPath = new List<Vector2>();

		foreach (Vector2 point in newPath)
		{
			Vector2 adjustedPoint = point + origin;
			adjustedPath.Add(adjustedPoint);
		}

		return adjustedPath;
	}

	// return scaled path
	public static List<Vector2> ScalePath(List<Vector2> path, float scale)
	{
		List<Vector2> scaledPath = new List<Vector2>();

		foreach (Vector2 point in path)
		{
			float scaledX = point.X * scale;
			float scaledY = point.Y * scale;

			scaledPath.Add(new Vector2(scaledX, scaledY));
		}

		return scaledPath;
	}

	// set next wapoint as active
	private void NextWayPoint()
	{
		if (pathIndex < path.Count - 1)
		{
			pathIndex++;
			wayPoint = path[pathIndex];
		}
		else
		{
			if (isLooping)
			{
				pathIndex = 0;
				NextWayPoint();
			}
			else
			{
				IsActive = false;
			}
		}
	}

	// draw the path
	public static void DrawPath(List<Vector2> navPath, RenderContext context)
	{
		if (navPath.Count > 0)
		{
			for (int index = 0; index < navPath.Count; index++)
			{
				if (index > 0) context.Line(navPath[index - 1], navPath[index], Color.White);
			}
		}

	}

	// update the entity movement towards waypoint
	public void Update(float delta, GameEntity entity, float speed)
	{
		float distanceToWaypoint = Vector2.Distance(entity.Position, wayPoint);
		Vector2 moveVector = wayPoint - entity.Position;
		moveVector = Vector2.Normalize(moveVector);
		float moveDistance = moveVector.Length();
		if (moveDistance > distanceToWaypoint)
		{
			float remainingDistance = moveDistance - distanceToWaypoint;
			entity.Position = wayPoint;
			NextWayPoint();
			if (IsActive)
			{
				moveVector = wayPoint - entity.Position;
				moveVector = Vector2.Normalize(moveVector);
				entity.Position += moveVector * remainingDistance * delta;
			}

		}
		else
		{
			entity.Position += moveVector * speed * delta;
		}
	}

}
using System.Numerics;
using System.Collections.Generic;
using FirstSampleGame.Abstract;
using Chroma.Graphics;
using System;

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
    public void SetPath(GameEntity entity, Vector2 origin, List<Vector2> newPath, float scale = 1f,  bool setActive = true, bool offsetPath = true)
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
        nextWayPoint();
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
    private void nextWayPoint()
    {
        if (pathIndex < path.Count-1)
        {
            pathIndex++;
            wayPoint = path[pathIndex];
        }
            else
            {
                if (isLooping)
                {
                    pathIndex = 0;
                    nextWayPoint();
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
            for (int index = 0;index < navPath.Count;index++)
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
            nextWayPoint();
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

    // return a shape path
    public static List<Vector2> getShapePath(int index)
    {
        List<Vector2> path;
        switch(index)
        {
            case 0 : // square
                path = new List<Vector2>
                {
                    new Vector2(0, 0),     // Top-left corner
                    new Vector2(1, 0),     // Top-right corner
                    new Vector2(1, 1),     // Bottom-right corner
                    new Vector2(0, 1),     // Bottom-left corner
                    new Vector2(0, 0)      // Closing the loop (back to start)
                    
                };
                break;
                
            case 1 : // triangle
                path = new List<Vector2>
                {
                    new Vector2(0, 0),
                    new Vector2(1, 1),
                    new Vector2(-1, 1),
                    new Vector2(0, 0)
                };
                break;

            case 2 :
                path = new List<Vector2>
                {
                    new Vector2(869, 91),
                    new Vector2(399, 184),
                    new Vector2(575, 587),
                    new Vector2(551, 636),
                    new Vector2(120, 518),
                    new Vector2(116, 563),
                    new Vector2(542, 689),
                    new Vector2(592, 717),
                    new Vector2(730, 1031),
                    new Vector2(919, 1005),
                    new Vector2(823, 782),
                    new Vector2(890, 780),
                    new Vector2(1334, 912),
                    new Vector2(1378, 833),
                    new Vector2(1385, 534),
                    new Vector2(1838, 526),
                    new Vector2(1831, 284),
                    new Vector2(1280, 290),
                    new Vector2(644, 316),
                    new Vector2(596, 177),
                    new Vector2(901, 117),
                    new Vector2(869, 93) 
                };

                //path = PathNav.OffSetPath(path, new Vector2(0,0));
                break;

            case 3 :
                path = new List<Vector2>
                {
                    new Vector2(553, 153),
                    new Vector2(392, 182),
                    new Vector2(515, 475),
                    new Vector2(601, 699),
                    new Vector2(742, 1044),
                    new Vector2(931, 1038),
                    new Vector2(552, 152)
                };
                //path = PathNav.OffSetPath(path, new Vector2(0,0));
                break;

            default :
                path = null;
                break;
              
        }

        return path;

    }


 
}
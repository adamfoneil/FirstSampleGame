using System.Collections.Generic;
using System.Numerics;

public class PathRecord
{
    public int NodeType;
    public Vector2 NodePosition;
    public float Duration; 


    public enum PathNodeType {
        MOVE,
        PAUSE,
        ROTATE
    }

    public PathRecord(int nodeType, Vector2 nodePosition, float duration)
    {
        NodeType = nodeType;
        NodePosition = nodePosition;
        Duration = duration;
    } 

}
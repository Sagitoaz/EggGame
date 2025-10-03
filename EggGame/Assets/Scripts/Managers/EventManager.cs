using System;
using UnityEngine;

public class EventManager
{
    public static Action<Vector3> onMouseClick;
    public static Action onReorganizeBoard;
    public static Action<Node> onNodeClicked;
    public static Action onNodeReleased;
    public static Action onClick;
    public static Action<Node, BfsResult> onValidMergeFound;
    public static Action<Node> onMergeStarted;
    public static Action<Node> onMergeCompleted;
}

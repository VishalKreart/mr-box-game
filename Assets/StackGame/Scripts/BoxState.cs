using UnityEngine;

public class BoxState : MonoBehaviour
{
    public enum State { Spawned, Falling, Sleep }
    public State state = State.Spawned;
} 
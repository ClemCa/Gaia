using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAnimation : MonoBehaviour
{
    public enum AnimationState
    {
        Idle,
        Moving,
        Fleeing,
        Eating,
        Drinking,
        Sleeping,
        Mating,
        Hunting,
        Attacking,
        Hurt,
        Dying,
        Dead
    }

    public void SetAnimationState(AnimationState state)
    {
    }
}

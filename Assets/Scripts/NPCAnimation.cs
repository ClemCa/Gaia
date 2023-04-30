using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPCAnimation : MonoBehaviour
{
	private AnimationState currentstate;
	[SerializeField] private Animator animator = null;
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
		if (currentstate == state || IsOneWay(currentstate, state))
		{
			return; // Don't change the animation if it's already playing
		}
        animator.SetBool("is"+currentstate.ToString(), false);
		currentstate = state;
        animator.SetBool("is"+state.ToString(), true);
	}

    private bool IsOneWay(AnimationState fromState, AnimationState toState)
    {
        return fromState switch
        {
            AnimationState.Fleeing => toState == AnimationState.Moving,
            AnimationState.Hunting => toState == AnimationState.Moving,
            _ => false,
        };
    }
}
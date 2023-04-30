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
		if (currentstate == state)
		{
			return; // Don't change the animation if it's already playing
		}

		switch (currentstate)
		{
			case AnimationState.Idle:
				animator.SetBool("isIdle", false);
				break;
			case AnimationState.Moving:
				animator.SetBool("isMoving", false);
				break;
			case AnimationState.Fleeing:
				animator.SetBool("isFleeing", false);
				break;
			case AnimationState.Eating:
				animator.SetBool("isEating", false);
				break;
			case AnimationState.Drinking:
				animator.SetBool("isDrinking", false);
				break;
			case AnimationState.Sleeping:
				animator.SetBool("isSleeping", false);
				break;
			case AnimationState.Mating:
				animator.SetBool("isMating", false);
				break;
			case AnimationState.Hunting:
				animator.SetBool("isHunting", false);
				break;
			case AnimationState.Attacking:
				animator.SetBool("isAttacking", false);
				break;
			case AnimationState.Hurt:
				animator.SetBool("isHurt", false);
				break;
			case AnimationState.Dying:
				animator.SetBool("isDying", false);
				break;
			case AnimationState.Dead:
				animator.SetBool("isDead", false);
				break;
			default:
				break;
		}

		// Set the current state to the new state
		currentstate = state;

		// Set the animator parameter for the new state
		switch (state)
		{
			case AnimationState.Idle:
				animator.SetBool("isIdle", true);
				break;
			case AnimationState.Moving:
				animator.SetBool("isMoving", true);
				break;
			case AnimationState.Fleeing:
				animator.SetBool("isFleeing", true);
				break;
			case AnimationState.Eating:
				animator.SetBool("isEating", true);
				break;
			case AnimationState.Drinking:
				animator.SetBool("isDrinking", true);
				break;
			case AnimationState.Sleeping:
				animator.SetBool("isSleeping", true);
				break;
			case AnimationState.Mating:
				animator.SetBool("isMating", true);
				break;
			case AnimationState.Hunting:
				animator.SetBool("isHunting", true);
				break;
			case AnimationState.Attacking:
				animator.SetBool("isAttacking", true);
				break;
			case AnimationState.Hurt:
				animator.SetBool("isHurt", true);
				break;
			case AnimationState.Dying:
				animator.SetBool("isDying", true);
				break;
			case AnimationState.Dead:
				animator.SetBool("isDead", true);
				break;
			default:
				break;
		}

	}
}
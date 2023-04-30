using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPCSounds : MonoBehaviour
{
	[SerializeField] private AudioSource audioSource;
	[SerializeField] private AudioData[] audioData;
	[System.Serializable]
	public struct AudioData
	{
		public string name;
		public AudioClip clip;
		public float volume;
	}
	public enum Sounds
	{
		BearAttack, // V
		BearEat, // V
		BearIdle, // V
		BearSleep, // V
		GoatBabyIdle, // V
		GoatBabyDead,
		GoatDead,
		GoatEat, // V
		GoatMate, // V
		GoatIdle, // V
		BearMate, // V
		GoatTakeDamage, // V
		GoatSleep, // V
	}

	public void PlaySound(Sounds sound)
	{
		
	}

	private void PlayByName(string name)
	{
		foreach (var data in audioData)
		{
			if (data.name == name)
			{
				audioSource.clip = data.clip;
				audioSource.volume = data.volume;
				audioSource.Play();
				return;
			}
		}
		Debug.LogError("No sound with name " + name + " found!");
	}
}
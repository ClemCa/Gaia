using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillUser : MonoBehaviour
{
    [SerializeField] private Button[] buttons = new Button[6];
    [SerializeField] private float[] radiuses = new float[6];
    [SerializeField] private float[] cooldowns = new float[6];
    private float[] cooldownTimers = new float[6];
    void Start()
    {
        cooldownTimers = new float[cooldowns.Length];
    }
    void Update()
    {
        for(int i = 0; i < cooldownTimers.Length; i++)
        {
            cooldownTimers[i] -= Time.deltaTime;
            buttons[i].interactable = cooldownTimers[i] <= 0f;
        }
    }
    public void UseSkill(int id)
    {
        if(cooldownTimers[id] > 0f)
            return;
        StartCoroutine(WaitThenPlaceSkill(id));
    }
    private IEnumerator WaitThenPlaceSkill(int id)
    {
        yield return null; // wait for a frame to not be on the frame the button is pressed
        SkillPlacer.PlaceSkill((position) => {
            UseSkill(position, id);
        }, radiuses[id]);
    }
    public void UseSkill(Vector3 position, int id)
    {
        cooldownTimers[id] = cooldowns[id];
        Debug.Log("Used skill " + id + " at " + position);
    }
}

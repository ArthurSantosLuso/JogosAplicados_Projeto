using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyCombatController : MonoBehaviour
{
    private enum DesiredAction { Break = 0, Attack = 1}


    [SerializeField]
    private float timeToBreakPosture = 2.5f;
    [SerializeField]
    private float timeToAttack = 4f;

    [SerializeField]
    private AudioClip breakPostureAudio;
    [SerializeField]
    private AudioClip attackAudio;

    private float timer;
    private DesiredAction desiredAction;
    private float currentActionTime;

    private void Start()
    {
        ChooseNextAction();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= currentActionTime)
        {
            DoAction();
            ChooseNextAction();
            timer = 0f;
        }
    }

    private void DoAction()
    {
        switch (desiredAction)
        {
            case DesiredAction.Attack:
                AudioManager.Instance.PlaySound(attackAudio);
                Debug.Log("Enemy is attacking! defend!!!!");
                GameManager.Instance.Player.GetComponent<PlayerCombatController>()._DefenseWindow?.OpenWindow();
                break;

            case DesiredAction.Break:
                AudioManager.Instance.PlaySound(breakPostureAudio);
                Debug.Log("Enemy broke posture! Attack it!!!!");
                GameManager.Instance.Player.GetComponent<PlayerCombatController>()._AttackWindow?.OpenWindow();
                break;
        }

    }

    private void ChooseNextAction()
    {
        int actionValue = UnityEngine.Random.Range(0, Enum.GetNames(typeof(DesiredAction)).Length);
        desiredAction = (DesiredAction)actionValue;

        switch (desiredAction)
        {
            case DesiredAction.Attack:
                currentActionTime = timeToAttack;
                break;

            case DesiredAction.Break:
                currentActionTime = timeToBreakPosture;
                break;
        }
    }
}

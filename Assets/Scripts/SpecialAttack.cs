using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialAttack : MonoBehaviour
{
    [SerializeField] public float armorModifier = 0f;
    [SerializeField] public float armorDuration = 0f;

    [SerializeField] public float speedModifier = 0f;
    [SerializeField] public float speedDuration = 0f;

    [SerializeField] public float poisonPerSecond = 0f;
    [SerializeField] public float poisonDuration = 0f;

    [SerializeField] public float attackArea = 0f;
    [SerializeField] public float cleaveDamageRate = 0f;
}

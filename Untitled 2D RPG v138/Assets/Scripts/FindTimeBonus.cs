using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindTimeBonus : MonoBehaviour
{
    [SerializeField] GameObject timeBonus;

    // Function that gets the timeBonus text GameObject
    public GameObject GetTimeBonusGameObject()
    {
        return timeBonus;
    }
}

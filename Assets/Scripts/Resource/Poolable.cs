using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poolable : MonoBehaviour
{
    [SerializeField] private int amount;
    public int Amount { get { return amount; } }
}

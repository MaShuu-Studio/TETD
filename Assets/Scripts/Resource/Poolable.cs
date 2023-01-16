using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Poolable : MonoBehaviour
{
    public int Amount { get { return amount; } }
    protected int amount;
    public int Id { get { return id; } }
    protected int id;

    public abstract bool MakePrefab(int id);
}

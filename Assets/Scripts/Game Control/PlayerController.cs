using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get { return instance; } }
    private static PlayerController instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private List<Tower> towers;
    public List<Tower> Towers { get { return towers; } }

    public void Init()
    {
        towers = new List<Tower>();
    }

    public bool AddTower(Tower tower)
    {
        if (towers.Count == 10) return false;

        towers.Add(tower);

        return true;
    }
}

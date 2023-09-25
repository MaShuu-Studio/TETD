using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;

public class TowerController : MonoBehaviour
{
    public static TowerController Instance { get { return instance; } }
    private static TowerController instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    [SerializeField] private Pool splashPool;
    [SerializeField] private SplashPoint splashPoint;

    private Dictionary<Vector3, TowerObject> towers = new Dictionary<Vector3, TowerObject>();
    private TowerObject selectedTower = null;
    public TowerObject SelectedTower { get { return selectedTower; } }

    public void Init()
    {
        splashPool.Init(splashPoint);
    }

    public SplashPoint PopSplashPoint()
    {
        GameObject point = splashPool.Pop();

        return point.GetComponent<SplashPoint>();
    }

    public void PushSplashPoint(GameObject obj)
    {
        splashPool.Push(obj);
    }

    public bool ContainsTower(Vector3 pos)
    {
        return towers.ContainsKey(pos);
    }

    public TowerObject FindTower(GameObject go)
    {
        foreach(TowerObject tower in towers.Values)
        {
            if (tower.gameObject == go)
            {
                return tower;
            }
        }
        return null;
    }

    public void RemoveTowerObject(TowerObject obj)
    {
        foreach (var tower in towers.Values)
        {
            tower.RemoveTower(obj);
        }
    }

    public bool BuildTower(int id, Vector3 pos)
    {
        if (ContainsTower(pos)) return false;
        UIController.Instance.ShowTutorial(4);

        Tower tower = TowerManager.GetTower(id);
        if (tower == null) return false;
        if (PlayerController.Instance.Buy(tower.cost) == false) return false;

        GameObject obj = PoolController.Pop(tower.id);
        obj.transform.position = pos;
        obj.transform.SetParent(transform);

        TowerObject towerObj = obj.GetComponent<TowerObject>();

        TileProperty tp = null;
        MapProperty mp = null;
        Vector3Int tilePos = MapController.Instance.GetTilePos(pos);
        if (MapController.Instance.MapData.tilemap.tileProperties.ContainsKey(tilePos))
        {
            tp = MapController.Instance.MapData.tilemap.tileProperties[tilePos];
        }
        if (MapController.Instance.MapData.tilemap.mapProperties != null)
            foreach (var prop in MapController.Instance.MapData.tilemap.mapProperties)
            {
                if (prop.element == tower.element)
                {
                    mp = prop;
                    break;
                }
            }
        towerObj.Build(pos, tp, mp);

        towers.Add(pos, towerObj);
        SelectTower(pos);

        return true;
    }

    public bool SelectTower(Vector3 pos)
    {
        if (selectedTower != null)
        {
            selectedTower.SelectTower(false);
            selectedTower = null;
        }

        Tower tower = null;
        bool select = false;

        if (ContainsTower(pos))
        {
            TowerObject towerObj = towers[pos];
            towerObj.SelectTower(true);
            selectedTower = towerObj;

            tower = towerObj.Data;
            select = true;
        }
        UIController.Instance.SelectTower(select, tower);
        return select;
    }

    public void SellTower()
    {
        if (selectedTower == null) return;

        int value = selectedTower.Data.Value();
        GameObject obj = selectedTower.gameObject;
        PoolController.Push(selectedTower.Id, obj);
        towers.Remove(selectedTower.Pos);

        PlayerController.Instance.Reward(0, value);
        selectedTower.Reset();
        selectedTower = null;

        UIController.Instance.SelectTower(false);
    }

    public void RemoveEnemyObject(EnemyObject enemy)
    {
        foreach (var tower in towers.Values)
        {
            tower.RemoveEnemy(enemy);
        }
    }

    public void UpdateLanguage(LanguageType lang)
    {
        foreach (var tower in towers.Values)
        {
            tower.UpdateLanguage(lang);
        }
    }
}

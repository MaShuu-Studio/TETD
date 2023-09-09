using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CustomEditor : MonoBehaviour
{
    [SerializeField] private RectTransform buttonsParent;
    [SerializeField] private SelectCustomDataButton selectDataButtonPrefab;
    [SerializeField] private TextMeshProUGUI dataNameText;
    [SerializeField] private GameObject[] infoObjects;
    [SerializeField] private TextMeshProUGUI[] infoTexts;
    [SerializeField] private GameObject editButton;
    private bool[] loadData;
    private int selectedData;

    public void Init()
    {
        selectDataButtonPrefab.gameObject.SetActive(false);
        editButton.SetActive(false);
        loadData = new bool[CustomDataManager.Datas.Count];

        for (int i = 0; i < CustomDataManager.Datas.Count; i++)
        {
            CustomData data = CustomDataManager.Datas[i];

            SelectCustomDataButton button = Instantiate(selectDataButtonPrefab);
            button.Init(data.name, this, i);
            button.transform.SetParent(buttonsParent);
            button.transform.localScale = Vector3.one;
            button.gameObject.SetActive(true);
        }
        buttonsParent.sizeDelta = new Vector2(600, 90 * CustomDataManager.Datas.Count);

        dataNameText.gameObject.SetActive(false);
        for (int i = 0; i < infoObjects.Length; i++)
        {
            infoObjects[i].SetActive(false);
        }
    }

    public void SelectData(int index)
    {
        editButton.SetActive(true);
        selectedData = index;

        CustomData data = CustomDataManager.Datas[index];
        dataNameText.gameObject.SetActive(true);
        dataNameText.text = data.name;
        for (int i = 0; i < data.dataAmount.Length; i++)
        {
            if (data.dataAmount[i] == 0) infoObjects[i].SetActive(false);
            else
            {
                infoObjects[i].SetActive(true);
                infoTexts[i].text = data.dataAmount[i].ToString();
            }
        }
    }

    public void AddData(int index, bool b)
    {
        loadData[index] = b;
    }

    public void Edit()
    {
        GameController.Instance.EditCustomData(CustomDataManager.Datas[selectedData].pathes);
    }

    public void Apply()
    {
        List<string>[] pathes = new List<string>[3];
        for (int i = 0; i < 3; i++)
        {
            pathes[i] = new List<string>();
        }

        bool b = false;
        for (int i = 0; i < loadData.Length; i++)
        {
            if (loadData[i] == false) continue;
            b = true;

            CustomData data = CustomDataManager.Datas[i];

            // path 안에 데이터를 전부 때려박고 진행.
            for (int j = 0; j < 3; j++)
            {
                pathes[j].AddRange(data.pathes[j]);
            }
        }

        // 이 후 해당 path를 넘겨서 로드 진행.
        // 후에 데이터를 굳이 로드할 필요가 없을 경우에 대한 예외처리 필요.
        if (b)
        {
            TowerManager.ResetCustomData();
            EnemyManager.ResetCustomData();
            MapManager.ResetCustomData();
            SceneController.Instance.LoadCustomData(pathes);
        }
    }
}

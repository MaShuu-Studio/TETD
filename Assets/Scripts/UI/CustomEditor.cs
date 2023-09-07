using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CustomEditor : MonoBehaviour
{
    [SerializeField] private Transform buttonsParent;
    [SerializeField] private SelectCustomDataButton selectDataButtonPrefab;
    [SerializeField] private TextMeshProUGUI dataNameText;
    [SerializeField] private GameObject[] infoObjects;
    [SerializeField] private TextMeshProUGUI[] infoTexts;

    public void Init()
    {
        selectDataButtonPrefab.gameObject.SetActive(false);

        for (int i = 0; i < CustomDataManager.Datas.Count; i++)
        {
            CustomData data = CustomDataManager.Datas[i];

            SelectCustomDataButton button = Instantiate(selectDataButtonPrefab);
            button.Init(data.name, this, i);
            button.transform.SetParent(buttonsParent);
            button.transform.localScale = Vector3.one;
            button.gameObject.SetActive(true);
        }

        dataNameText.gameObject.SetActive(false);
        for (int i = 0; i < infoObjects.Length; i++)
        {
            infoObjects[i].SetActive(false);
        }
    }

    public void SelectData(int index)
    {
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
}

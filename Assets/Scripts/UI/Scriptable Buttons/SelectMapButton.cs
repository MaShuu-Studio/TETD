using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

[RequireComponent(typeof(Button))]
public class SelectMapButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    private Button button;

    private GameSettingController gameSetting = null;
    private int index;


    public void Init(string name, GameSettingController gameSetting, int index)
    {
        button = GetComponent<Button>();
        text.text = name;
        this.gameSetting = gameSetting;
        this.index = index;
        button.onClick.AddListener(() => gameSetting.SelectMap(index));
    }

    private void Update()
    {
        if (gameSetting != null && UIController.PointOverUI(gameObject))
        {
            gameSetting.ShowMap(index);
        }
    }
}

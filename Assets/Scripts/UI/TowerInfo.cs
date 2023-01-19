using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerInfo : MonoBehaviour
{
    [SerializeField] protected Image elementImage;
    [SerializeField] protected List<Sprite> elementSprites;
    [SerializeField] protected Image iconImage;
    [SerializeField] protected TextMeshProUGUI nameText;
    [SerializeField] protected TextMeshProUGUI damageText;
    [SerializeField] protected TextMeshProUGUI attackSpeedText;
    [SerializeField] protected TextMeshProUGUI rangeText;

    protected Tower data;
    public Tower Data { get { return data; } }

    public virtual void SetData(Tower data)
    {
        this.data = data;

        nameText.text = data.name.Substring(6);
        iconImage.sprite = SpriteManager.GetSprite(data.id);

        // UI Icon�鵵 id�� ���̰� SpriteManager���� ���� ����
        elementImage.sprite = elementSprites[(int)data.element];

        UpdateInfo();
    }

    public virtual void UpdateInfo()
    {
        damageText.text = string.Format("{0:0.#}", data.stat[EnumData.TowerMainStatType.DAMAGE]);
        attackSpeedText.text = string.Format("{0:0.#}", data.stat[EnumData.TowerMainStatType.ATTACKSPEED]);
        rangeText.text = string.Format("{0:0.#}", data.stat[EnumData.TowerMainStatType.DISTANCE]);
    }

    protected void AddSkill()
    {

    }
}

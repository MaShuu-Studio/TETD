using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class DamageUI : Poolable
{
    private TextMeshProUGUI damageText;

    public override bool MakePrefab(int id)
    {
        amount = 50;
        damageText = GetComponent<TextMeshProUGUI>();
        return true;
    }

    public void ShowDamage()
    {
        StartCoroutine(Damage());
    }

    IEnumerator Damage()
    {
        float time = 0;

        while (time < 0.3f)
        {
            time += Time.deltaTime;
            transform.position += Vector3.up * 1f;
            yield return null;
        }

        UIController.Instance.PushDamageUI(gameObject);
    }

    private void OnEnable()
    {
        string[] xyf = gameObject.name.Split(",");
        float x, y, f;

        float.TryParse(xyf[0], out x);
        float.TryParse(xyf[1], out y);
        float.TryParse(xyf[2], out f);

        transform.position = new Vector2(x, y);
        damageText.text = string.Format("{0:0.#}", f);

        ShowDamage();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

[RequireComponent(typeof(Toggle))]
public class CustomDropdown : MonoBehaviour
{
    [SerializeField] private Image targetImage;
    [SerializeField] private TextMeshProUGUI targetText;

    [SerializeField] private CustomDropdownItem item;

    [SerializeField] private List<CustomDropdownOption> options;

    public CustomDropdownEvent onValueChanged { get { return m_OnValueChanged; } set { m_OnValueChanged = value; } }
    private CustomDropdownEvent m_OnValueChanged;

    public RectTransform template;
    private GameObject dropDownList;

    private Toggle toggle;
    public int value
    {
        get
        {
            return m_value;
        }
        set
        {
            m_value = value;
            ChangeValue(value);
        }
    }
    private int m_value;

    private void Awake()
    {
        if (options == null)
            options = new List<CustomDropdownOption>();

        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(isOn => OpenDropdown(isOn));
        m_OnValueChanged = new CustomDropdownEvent();

        item.gameObject.SetActive(false);
        template.gameObject.SetActive(false);
    }

    public void AddOption(CustomDropdownOption option)
    {
        options.Add(option);
    }

    public void AddOptions(List<CustomDropdownOption> options)
    {
        this.options.AddRange(options);
    }

    public void Clear()
    {
        options.Clear();
    }

    public void Select(int index)
    {
        value = index;
    }

    public void ChangeValue(int index, bool sendCallBack = true)
    {
        if (targetImage != null) targetImage.sprite = options[index].sprite;
        if (targetText != null) targetText.text = options[index].str;

        if (sendCallBack)
        {
            m_OnValueChanged.Invoke(value);
        }
        toggle.isOn = false;
    }

    private void OpenDropdown(bool b)
    {
        if (b)
        {
            dropDownList = Instantiate(template.gameObject);

            dropDownList.transform.SetParent(transform);
            RectTransform dropDownRect = dropDownList.transform as RectTransform;
            dropDownRect.anchorMin = template.anchorMin;
            dropDownRect.anchorMax = template.anchorMax;
            dropDownRect.anchoredPosition = template.anchoredPosition;

            for (int i = 0; i < options.Count; i++)
            {
                CustomDropdownItem item = Instantiate(this.item);
                item.SetOption(this, i, options[i]);
                item.transform.SetParent(dropDownList.transform);
                item.gameObject.SetActive(true);
            }
            dropDownList.SetActive(true);
        }
        else
        {
            Destroy(dropDownList);
        }
    }

    public class CustomDropdownEvent : UnityEvent<int> { };
}
[System.Serializable]
public class CustomDropdownOption
{
    public string str;
    public Sprite sprite;

    public CustomDropdownOption(string str, Sprite sprite)
    {
        this.str = str;
        this.sprite = sprite;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EnumData;
using Data;

public class UnitEditor : MonoBehaviour
{

    [Header("Alert")]
    [SerializeField] private GameObject alertObject;
    [SerializeField] private TextMeshProUGUI alertText;
    private IEnumerator alertCoroutine;

    [Space]
    [SerializeField] private UnitEditorUnitIcon unitIconPrefab;
    [Space]
    [Header("Tower")]
    [SerializeField] private GameObject towerPanel;
    [SerializeField] private RectTransform towerViewPort;
    private List<UnitEditorUnitIcon> towerIcons;

    [Header("Enemy")]
    [SerializeField] private GameObject enemyPanel;
    [SerializeField] private RectTransform enemyViewPort;
    private List<UnitEditorUnitIcon> enemyIcons;

    [Space]
    [Header("Info")]
    [SerializeField] private UnitEditorPoster poster;
    [SerializeField] private TextMeshProUGUI saveButtonText;

    [Space]
    [Header("Adjust Data")]
    [SerializeField] private TMP_InputField idInput;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private UnitEditorSetImageIcon[] imageIcons;
    [SerializeField] private TextMeshProUGUI[] animationTexts;
    [SerializeField] private TMP_InputField[] pivotInputs;
    [SerializeField] private TMP_InputField spfInput;

    [Space]
    [SerializeField] private TMP_Dropdown elementDropdown;
    [SerializeField] private GameObject gradeObject;
    [SerializeField] private TMP_Dropdown gradeDropdown;
    [SerializeField] private GameObject enemyGradeObject;
    [SerializeField] private TMP_Dropdown enemyGradeDropdown;

    [Space]
    [SerializeField] private Image[] statImages;
    [SerializeField] private TMP_InputField[] statInputs;
    [SerializeField] private GameObject[] units;
    [SerializeField] private TMP_InputField attackAmountInput;

    [Space]
    [SerializeField] private GameObject towerAbilityParent;
    [SerializeField] private GameObject[] towerAbilities;
    [SerializeField] private CustomDropdown[] towerAbilityDropdowns;
    [SerializeField] private TMP_InputField[] towerAbilityInputs;
    /*
    [Space]
    [SerializeField] private GameObject enemyAbilityParent;
    [SerializeField] private GameObject[] enemyAbilities;
    [SerializeField] private TMP_Dropdown[] enemyAbilityDropdowns;
    [SerializeField] private TMP_InputField[] enemyAbilityInputs;
    */
    [Space]
    [Header("Advanced Option")]
    [SerializeField] private GameObject towerAdvancedOption;
    [SerializeField] private GameObject towerAdvancedPopup;

    [Space]
    [SerializeField] private TMP_InputField[] attackTimeInputs;
    [Space]
    [Header("Effect")]
    [SerializeField] private UnitEditorSetImageIcon effectImageIcon;
    [SerializeField] private TMP_InputField effectColorInput;
    [SerializeField] private Image effectColorIcon;
    [SerializeField] private TMP_InputField effectSpfInput;

    [Space]
    [Header("Type")]
    [SerializeField] private Toggle[] typeToggles;
    [Space]
    [Header("Projectile")]
    [SerializeField] private GameObject projOptionObject;
    [SerializeField] private UnitEditorSetImageIcon projImageIcon;
    [SerializeField] private TextMeshProUGUI projRemainTimeText;
    [SerializeField] private TMP_InputField projRemainTimeInput;
    [SerializeField] private TMP_InputField projSpfInput;
    [SerializeField] private TMP_InputField projAttackTimeInput;

    private bool isTower;
    private bool isNew;
    private int typeInfo;

    #region Init
    public void Init()
    {
        towerIcons = new List<UnitEditorUnitIcon>();
        enemyIcons = new List<UnitEditorUnitIcon>();

        #region Adjust Data
        idInput.onValueChanged.AddListener(s => UpdateDataToPosterBasicData());
        nameInput.onValueChanged.AddListener(s => UpdateDataToPosterBasicData());

        for (int i = 0; i < pivotInputs.Length; i++)
            pivotInputs[i].onValueChanged.AddListener(s => PivotDataChanged());

        elementDropdown.onValueChanged.AddListener(i => UpdateDataToPosterBasicData());
        gradeDropdown.onValueChanged.AddListener(i => UpdateDataToPosterBasicData());
        enemyGradeDropdown.onValueChanged.AddListener(i => UpdateDataToPosterBasicData());

        for (int i = 0; i < statInputs.Length; i++)
            statInputs[i].onValueChanged.AddListener(s => UpdateDataToPosterStat());

        attackAmountInput.onValueChanged.AddListener(s => UpdateDataToPosterStat());
        attackAmountInput.onValueChanged.AddListener(s => ChangeAttackAmount(s));

        for (int i = 0; i < towerAbilityDropdowns.Length; i++)
        {
            towerAbilityDropdowns[i].onValueChanged.AddListener(s => AblityChanged());
            towerAbilityDropdowns[i].onValueChanged.AddListener(s => UpdateDataToPosterAbility());
            towerAbilityInputs[i].onValueChanged.AddListener(s => UpdateDataToPosterAbility());
        }

        spfInput.onValueChanged.AddListener(s => ChangeSpf(s, spfInput, imageIcons[0]));
        spfInput.onValueChanged.AddListener(s => ChangeSpf(s, spfInput, imageIcons[1]));

        effectColorInput.onValueChanged.AddListener(s => Hexadecimal());
        effectSpfInput.onValueChanged.AddListener(s => ChangeSpf(s, effectSpfInput, effectImageIcon));

        for (int i = 0; i < typeToggles.Length; i++)
            typeToggles[i].onValueChanged.AddListener(b => ChangeTowerType());
        /*
        for (int i = 0; i < enemyAbilityDropdowns.Length; i++)
        {
            enemyAbilityDropdowns[i].onValueChanged.AddListener(s => UpdateDataToPoster());
        }*/

        // Element
        {
            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
            for (int i = 0; i < EnumArray.Elements.Length; i++)
            {
                Element e = EnumArray.Elements[i];
                string oName = EnumArray.ElementStrings[e].ToUpper();
                Sprite oSprite = SpriteManager.GetSpriteWithNumber(SpriteManager.ETCDataNumber.ELEMENT, i);
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(oName, oSprite);
                options.Add(option);
            }
            elementDropdown.AddOptions(options);
            elementDropdown.template.sizeDelta = new Vector2(elementDropdown.template.sizeDelta.x, 75 * (1 + options.Count));
        }

        // Grade
        {
            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
            for (int i = 0; i < EnumArray.Grades.Length; i++)
            {
                Grade g = EnumArray.Grades[i];
                string oName = EnumArray.GradeStrings[g].ToUpper();
                Sprite oSprite = SpriteManager.GetSpriteWithNumber(SpriteManager.ETCDataNumber.GRADE, i);
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(oName, oSprite);
                options.Add(option);
            }
            gradeDropdown.AddOptions(options);
            gradeDropdown.template.sizeDelta = new Vector2(gradeDropdown.template.sizeDelta.x, 75 * (1 + options.Count));
        }

        // EnemyGrade
        {
            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
            for (int i = 0; i < EnumArray.EnemyGrades.Length; i++)
            {
                EnemyGrade g = EnumArray.EnemyGrades[i];
                string oName = EnumArray.EnemyGradeStrings[g].ToUpper();
                Sprite oSprite = SpriteManager.GetSpriteWithNumber(SpriteManager.ETCDataNumber.ENEMYGRADE, i);
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(oName, oSprite);
                options.Add(option);
            }
            enemyGradeDropdown.AddOptions(options);
            enemyGradeDropdown.template.sizeDelta = new Vector2(enemyGradeDropdown.template.sizeDelta.x, 75 * (1 + options.Count));
        }

        // abilities
        {
            List<CustomDropdownOption> options = new List<CustomDropdownOption>();
            // 0 ~ 2까지는 메인스탯
            for (int i = 3; i < EnumArray.TowerStatTypes.Length; i++)
            {
                TowerStatType type = EnumArray.TowerStatTypes[i];
                string oName = EnumArray.TowerStatTypeStrings[type];
                Sprite oSprite = SpriteManager.GetSpriteWithNumber(SpriteManager.ETCDataNumber.TOWERSTAT, i);
                CustomDropdownOption option = new CustomDropdownOption(oName, oSprite);
                options.Add(option);
            }

            for (int i = 0; i < EnumArray.BuffTypes.Length; i++)
            {
                BuffType type = EnumArray.BuffTypes[i];
                string oName = EnumArray.BuffTypeStrings[type];
                Sprite oSprite = SpriteManager.GetSpriteWithNumber(SpriteManager.ETCDataNumber.BUFF, i);
                CustomDropdownOption option = new CustomDropdownOption(oName, oSprite);
                options.Add(option);
            }

            for (int i = 0; i < EnumArray.DebuffTypes.Length; i++)
            {
                DebuffType type = EnumArray.DebuffTypes[i];
                string oName = EnumArray.DebuffTypeStrings[type];
                Sprite oSprite = SpriteManager.GetSpriteWithNumber(SpriteManager.ETCDataNumber.DEBUFF, i);
                CustomDropdownOption option = new CustomDropdownOption(oName, oSprite);
                options.Add(option);
            }

            float width = ((options.Count >= 5) ? 5 : options.Count) * 87.5f;
            float height = (100f / 3f * 2f) * (1 + (int)(options.Count / 5));

            for (int i = 0; i < towerAbilityDropdowns.Length; i++)
            {
                towerAbilityDropdowns[i].template.sizeDelta = new Vector2(width, height);
                towerAbilityDropdowns[i].AddOptions(options);
            }
        }

        towerAdvancedPopup.SetActive(false);
        #endregion
    }

    // 로딩방식 채용
    public void EditUnit()
    {
        alertObject.SetActive(false);
        gameObject.SetActive(true);

        // TowerPanel 초기화
        foreach (var icon in towerIcons)
        {
            Destroy(icon.gameObject);
        }
        towerIcons.Clear();

        for (int i = 0; i < TowerManager.CustomDataKeys.Count; i++)
        {
            int id = TowerManager.Keys[i];

            UnitEditorUnitIcon icon = Instantiate(unitIconPrefab);
            icon.Init(TowerManager.Keys[i]);
            icon.transform.SetParent(towerViewPort);
            towerIcons.Add(icon);
        }
        towerViewPort.sizeDelta = new Vector2(475, (int)((towerIcons.Count + 1) / 2) * 275 + 25);

        // EnemyPanel 초기화
        foreach (var icon in enemyIcons)
        {
            Destroy(icon.gameObject);
        }
        enemyIcons.Clear();

        for (int i = 0; i < EnemyManager.CustomDataKeys.Count; i++)
        {
            int id = EnemyManager.Keys[i];

            UnitEditorUnitIcon icon = Instantiate(unitIconPrefab);
            icon.Init(EnemyManager.Keys[i]);
            icon.transform.SetParent(enemyViewPort);
            enemyIcons.Add(icon);
        }
        enemyViewPort.sizeDelta = new Vector2(475, (int)((enemyIcons.Count + 1) / 2) * 275 + 25);

        UpdatePoster(TowerManager.GetTower(TowerManager.Keys[0]));
        SelectType(true);
    }
    #endregion

    private void Update()
    {
        if (isTower)
        {
            float value;
            float.TryParse(statInputs[1].text, out value);

            if (value < 0 || value >= 10)
            {
                value = Mathf.Clamp(value, 0, 9.9f);
                statInputs[1].text = string.Format("{0:0.#}", value);
            }
        }
    }

    public void NewUnit()
    {
        isNew = true;
        isTower = towerPanel.activeSelf;
        if (isTower) typeInfo = 4;
        else typeInfo = 5;

        ChangeEditor();

        poster.ChangePosterType(isTower);
        NewPoster();
    }

    public void NewPoster()
    {
        idInput.text = "";
        nameInput.text = "";
        for (int i = 0; i < imageIcons.Length; i++)
            imageIcons[i].Clear();

        for (int i = 0; i < pivotInputs.Length; i++)
            pivotInputs[i].text = "";

        elementDropdown.value = 0;
        gradeDropdown.value = 0;
        enemyGradeDropdown.value = 0;

        for (int i = 0; i < statInputs.Length; i++)
            statInputs[i].text = "";
        attackAmountInput.text = "";

        for (int i = 0; i < towerAbilities.Length; i++)
        {
            towerAbilityDropdowns[i].value = 0;
            towerAbilityInputs[i].text = "";
        }

        for (int i = 0; i < attackTimeInputs.Length; i++)
        {
            attackTimeInputs[i].text = "";
        }

        effectImageIcon.Clear();
        effectColorInput.text = "";
        effectSpfInput.text = "";

        projImageIcon.Clear();
        projRemainTimeInput.text = "";
        projSpfInput.text = "";
        projAttackTimeInput.text = "";

        typeToggles[0].isOn = true;
        UpdateDataToPosterBasicData();
    }

    public void ChangeEditor()
    {
        if (isTower)
        {
            gradeObject.gameObject.SetActive(true);
            enemyGradeObject.gameObject.SetActive(false);

            animationTexts[0].text = "IDLE";
            animationTexts[1].text = "ATTACK";

            towerAbilityParent.SetActive(true);
            towerAdvancedOption.SetActive(true);
            //enemyAbilityParent.SetActive(false);

            statInputs[1].characterValidation = TMP_InputField.CharacterValidation.Decimal;
            ((TextMeshProUGUI)(statInputs[1].placeholder)).text = "1.0";
            statInputs[1].characterLimit = 3;

            UpdateStatImage();
            for (int i = 0; i < units.Length; i++)
                units[i].SetActive(true);
        }
        else
        {
            gradeObject.gameObject.SetActive(false);
            enemyGradeObject.gameObject.SetActive(true);

            animationTexts[0].text = "IDLE";
            animationTexts[1].text = "MOVE";

            towerAbilityParent.SetActive(false);
            towerAdvancedOption.SetActive(false);
            towerAdvancedPopup.SetActive(false);

            statInputs[1].characterValidation = TMP_InputField.CharacterValidation.Decimal;
            ((TextMeshProUGUI)(statInputs[1].placeholder)).text = "0";
            statInputs[1].characterLimit = 1;

            UpdateStatImage();
            for (int i = 0; i < units.Length; i++)
                units[i].SetActive(false);
        }
    }

    public void SaveUnit()
    {
    }

    #region Update Data
    private void UpdateStatImage()
    {
        SpriteManager.ETCDataNumber number =
            (isTower) ? SpriteManager.ETCDataNumber.TOWERSTAT : SpriteManager.ETCDataNumber.ENEMYSTAT;

        for (int i = 0; i < statImages.Length; i++)
        {
            Sprite sprite = SpriteManager.GetSpriteWithNumber(number, i);
            statImages[i].sprite = sprite;
        }
    }

    public void SelectType(bool isTower)
    {
        towerPanel.SetActive(isTower);
        enemyPanel.SetActive(!isTower);
    }

    public void UpdatePoster(Tower data)
    {
        string id = data.id.ToString();
        typeInfo = id[0] - '0';

        poster.SetData(data);
        saveButtonText.text = $"SAVE {data.id}";

        isTower = true;
        ChangeEditor();

        UpdatePosterToData();
    }

    public void UpdatePoster(Enemy data)
    {
        string id = data.id.ToString();
        typeInfo = id[0] - '0';

        poster.SetData(data);
        saveButtonText.text = $"SAVE {data.id}";

        isTower = false;
        ChangeEditor();

        UpdatePosterToData();
    }

    public void UpdatePosterToData()
    {
        if (isTower)
        {
            Tower data = poster.Data;
            string id = data.id.ToString().Substring(4);
            string name = data.name;
            Element element = data.element;
            Grade grade = data.grade;
            string[] stat = new string[3];
            stat[0] = string.Format("{0:0.#}", data.Stat(TowerStatType.DAMAGE));
            stat[1] = string.Format("{0:0.#}", data.Stat(TowerStatType.ATTACKSPEED));
            stat[2] = string.Format("{0:0.#}", data.Stat(TowerStatType.DISTANCE));
            int attackAmount = data.AttackAmount;

            pivotInputs[0].text = data.pivot.x.ToString();
            pivotInputs[1].text = data.pivot.y.ToString();

            float spf = data.spf;
            spfInput.text = spf.ToString();

            for (int i = 0; i < imageIcons.Length; i++)
            {
                Sprite[] sprites;
                if (data.animation.TryGetValue((AnimationType)i, out sprites) == false)
                {
                    if (i == 0)
                    {
                        sprites = new Sprite[1];
                        sprites[0] = SpriteManager.GetSprite(data.id);
                    }
                }
                imageIcons[i].SetSprites(sprites);
                imageIcons[i].SetSpf(spf);
            }

            for (int i = 0; i < attackAmount; i++)
            {
                attackTimeInputs[i].text = data.attackTime[i].ToString();
            }

            bool b = towerAdvancedPopup.activeSelf;
            towerAdvancedPopup.SetActive(true);
            {
                Sprite[] sprites = null;
                if (TowerManager.Effects.ContainsKey(data.id))
                    sprites = TowerManager.Effects[data.id];

                float effectSpf = data.effectspf;
                effectImageIcon.SetSprites(sprites);
                effectImageIcon.SetSpf(effectSpf);
                effectSpfInput.text = effectSpf.ToString();
            }

            typeToggles[(int)data.attackType].isOn = true;
            if (data.attackType != AttackType.PROMPT)
            {
                Sprite[] sprites = null;
                if (TowerManager.Projs.ContainsKey(data.id))
                    sprites = TowerManager.Projs[data.id];

                float projSpf = data.projspf;
                projImageIcon.SetSprites(sprites);
                projImageIcon.SetSpf(projSpf);
                projSpfInput.text = projSpf.ToString();
                projRemainTimeInput.text = data.projTime.ToString();
                projAttackTimeInput.text = data.projAttackTime.ToString();
            }
            towerAdvancedPopup.SetActive(b);

            #region Poster Update

            idInput.text = id;
            nameInput.text = name;
            elementDropdown.value = (int)element;
            gradeDropdown.value = (int)grade;
            for (int i = 0; i < statInputs.Length; i++)
            {
                statInputs[i].text = stat[i];
            }
            attackAmountInput.text = attackAmount.ToString();

            #region ability
            int index = 0;
            towerAbilities[0].SetActive(true);
            for (int i = 3; i < data.StatTypes.Length; i++)
            {
                if (index < towerAbilityDropdowns.Length)
                {
                    towerAbilityDropdowns[index].value = ChangeStatToValue(0, (int)data.StatTypes[i]);
                    towerAbilityInputs[index].text = data.Stat(data.StatTypes[i]).ToString();
                    towerAbilities[index++].SetActive(true);
                }
            }

            if (data.BuffTypes != null)
                for (int i = 0; i < data.BuffTypes.Length; i++)
                {
                    if (index < towerAbilityDropdowns.Length)
                    {
                        towerAbilityDropdowns[index].value = ChangeStatToValue(1, (int)data.BuffTypes[i]);
                        towerAbilityInputs[index].text = data.Buff(data.BuffTypes[i]).ToString();
                        towerAbilities[index++].SetActive(true);
                    }
                }

            if (data.DebuffTypes != null)
                for (int i = 0; i < data.DebuffTypes.Length; i++)
                {
                    if (index < towerAbilityDropdowns.Length)
                    {
                        towerAbilityDropdowns[index].value = ChangeStatToValue(2, (int)data.DebuffTypes[i]);
                        towerAbilityInputs[index].text = data.Debuff(data.DebuffTypes[i]).ToString();
                        towerAbilities[index++].SetActive(true);
                    }
                }

            for (int i = index; i < towerAbilityDropdowns.Length; i++)
            {
                towerAbilityDropdowns[i].value = 0;
                towerAbilities[i].SetActive(i == index + 1);
            }
            #endregion
            #endregion
        }
        else
        {
            Enemy data = poster.EnemyData;
            string id = data.id.ToString().Substring(4);
            string name = data.name;
            Element element = data.element;
            EnemyGrade grade = data.grade;
            string[] stat = new string[3];
            stat[0] = string.Format("{0:0.#}", data.hp);
            stat[1] = string.Format("{0:0.#}", data.speed);
            stat[2] = string.Format("{0:0.#}", data.exp);

            float spf = data.spf;
            spfInput.text = spf.ToString();
            for (int i = 0; i < imageIcons.Length; i++)
            {
                Sprite[] sprites;
                AnimationType type = (i == 0) ? AnimationType.IDLE : AnimationType.MOVE;
                if (data.animation.TryGetValue(type, out sprites) == false)
                {
                    if (i == 0)
                    {
                        sprites = new Sprite[1];
                        sprites[0] = SpriteManager.GetSprite(data.id);
                    }
                }
                imageIcons[i].SetSprites(sprites);
                imageIcons[i].SetSpf(spf);
            }

            idInput.text = id;
            nameInput.text = name;
            elementDropdown.value = (int)element;
            enemyGradeDropdown.value = (int)grade;
            for (int i = 0; i < statInputs.Length; i++)
            {
                statInputs[i].text = stat[i];
            }
        }
    }

    private void UpdateDataToPosterBasicData()
    {
        string unitId = idInput.text;
        string name = nameInput.text;
        int element = elementDropdown.value;
        int grade = (isTower) ? gradeDropdown.value : enemyGradeDropdown.value;

        poster.UpdatePoster(isTower, name, element, grade);

        int id = typeInfo;
        id = id * 100 + element;
        id = id * 10 + grade;

        string idStr = id.ToString() + unitId;
        saveButtonText.text = "SAVE " + idStr;
    }

    private void UpdateDataToPosterStat()
    {
        float[] stats = new float[3];
        for (int i = 0; i < stats.Length; i++)
        {
            float.TryParse(statInputs[i].text, out stats[i]);
        }

        poster.UpdateStat(stats);
    }

    private void UpdateDataToPosterAbility()
    {
        Dictionary<TowerStatType, float> statAbils = new Dictionary<TowerStatType, float>();
        Dictionary<BuffType, float> buffs = new Dictionary<BuffType, float>();
        Dictionary<DebuffType, float> debuffs = new Dictionary<DebuffType, float>();

        int statAmount = EnumArray.TowerStatTypes.Length - 3;
        int buffAmount = EnumArray.BuffTypes.Length;
        int debuffAmount = EnumArray.DebuffTypes.Length;

        for (int i = 0; i < towerAbilities.Length; i++)
        {
            int index = towerAbilityDropdowns[i].value;
            float value;
            float.TryParse(towerAbilityInputs[i].text, out value);

            if (index == 0) continue;
            else if (index < 1 + statAmount)
            {
                TowerStatType type = (TowerStatType)(index + statAmount);
                if (statAbils.ContainsKey(type)) towerAbilityDropdowns[i].value = 0;
                else statAbils.Add(type, value);
            }
            else if (index < 1 + statAmount + buffAmount)
            {
                BuffType type = (BuffType)(index - statAmount - 1);
                if (buffs.ContainsKey(type)) towerAbilityDropdowns[i].value = 0;
                else buffs.Add(type, value);
            }
            else
            {
                DebuffType type = (DebuffType)(index - buffAmount - statAmount - 1);
                if (debuffs.ContainsKey(type)) towerAbilityDropdowns[i].value = 0;
                else debuffs.Add(type, value);
            }
        }

        poster.UpdateAbility(statAbils, buffs, debuffs);
    }
    #endregion

    // 0: Stats, 1: Buff, 2: Debuff
    private int ChangeStatToValue(int type, int index)
    {
        int value = 0;

        int statAmount = EnumArray.TowerStatTypes.Length - 3;
        int buffAmount = EnumArray.BuffTypes.Length;
        int debuffAmount = EnumArray.DebuffTypes.Length;

        switch (type)
        {
            case 0:
                value = index - statAmount;
                break;
            case 1:
                value = 1 + statAmount + index;
                break;
            case 2:
                value = 1 + statAmount + buffAmount + index;
                break;
        }
        return value;
    }

    #region valueChanged
    private void ChangeAttackAmount(string s)
    {
        int value;
        if (int.TryParse(s, out value))
        {
            if (value == 0)
            {
                value = 1;
                attackAmountInput.text = "1";
            }
            int i = 0;
            for (; i < value; i++)
                attackTimeInputs[i].gameObject.SetActive(true);

            for (; i < attackTimeInputs.Length; i++)
            {
                attackTimeInputs[i].text = "";
                attackTimeInputs[i].gameObject.SetActive(false);
            }
        }
    }

    private void ArrangeAttackTime()
    {
        int value;
        if (int.TryParse(attackAmountInput.text, out value))
        {
            float[] spf = new float[value];
            for (int i = 0; i < value; i++)
            {
                float.TryParse(attackTimeInputs[i].text, out spf[i]);
            }

            Array.Sort(spf);
            for (int i = 0; i < value; i++)
                attackTimeInputs[i].text = spf[i].ToString();
        }
    }

    private void AblityChanged()
    {
        for (int i = 0; i < towerAbilities.Length; i++)
        {
            if (towerAbilityDropdowns[i].value == 0)
            {
                // 뒷 부분이 있을 경우 스왑함. (다음 부분으로 넘어갔을 때 연이어 스왑하도록
                // 대신 뒷 부분도 0이라면 나머지도 전부 0임.더이상 체크할 이유가 없음.
                if (i + 1 < towerAbilities.Length)
                {
                    if (towerAbilityDropdowns[i + 1].value != 0)
                    {
                        towerAbilityDropdowns[i].value = towerAbilityDropdowns[i + 1].value;
                        towerAbilityInputs[i].text = towerAbilityInputs[i + 1].text;
                        towerAbilityDropdowns[i + 1].value = 0;
                        towerAbilityInputs[i + 1].text = "0";
                    }
                    else
                    {
                        for (i = i + 1; i < towerAbilities.Length; i++)
                        {
                            towerAbilities[i].SetActive(false);
                        }
                    }
                }
            }
            // 0이 아니라면 다음 부분 미리 열어둠.
            else if (i + 1 < towerAbilities.Length)
            {
                towerAbilities[i + 1].SetActive(true);
            }
        }
    }

    private void PivotDataChanged()
    {
        for (int i = 0; i < pivotInputs.Length; i++)
        {
            float value;
            float.TryParse(pivotInputs[i].text, out value);

            if (value < 0 || value > 1)
            {
                value = Mathf.Clamp(value, 0, 1);
                pivotInputs[i].text = string.Format("{0:0.#}", value);
            }
        }
    }

    private void Hexadecimal()
    {
        string str = effectColorInput.text;
        int index = 0;
        string[] hex = new string[3];
        int[] rgb = new int[3];
        hex[0] = hex[1] = hex[2] = "";

        foreach (var c in str)
        {
            char ch = 'F';
            if (c <= 'a' && c >= 'z')
                ch = (char)(c - ('a' - 'A'));

            if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z'))
                ch = c;

            hex[(index++) / 2] += ch;
        }

        effectColorInput.text = hex[0] + hex[1] + hex[2];

        for (int i = 0; i < hex.Length; i++)
        {
            while (hex[i].Length < 2) hex[i] += "F";
        }

        for (int i = 0; i < hex.Length; i++)
        {
            rgb[i] = Convert.ToInt32(hex[i], 16);
        }

        Color color = new Color(rgb[0] / 255f, rgb[1] / 255f, rgb[2] / 255f);
        effectColorIcon.color = color;
    }

    private void ChangeSpf(string s, TMP_InputField input, UnitEditorSetImageIcon imageIcon)
    {
        float value;

        if (float.TryParse(s, out value))
        {
            if (value < 0.001f) value = 0.001f;
            input.text = value.ToString();

            imageIcon.SetSpf(value);
        }
    }

    private void ChangeTowerType()
    {
        for (int i = 0; i < typeToggles.Length; i++)
        {
            if (typeToggles[i].isOn)
            {
                switch (i)
                {
                    // 즉발 공격
                    case 0:
                        projOptionObject.SetActive(false);
                        break;
                    // 투사체 공격
                    case 1:
                        projOptionObject.SetActive(true);
                        projRemainTimeText.text = "Flying Time";
                        break;
                    // 지점 공격
                    case 2:
                        projOptionObject.SetActive(true);
                        projRemainTimeText.text = "Remain Time";
                        break;
                }
                break;
            }
        }
    }

    public void UpdatePortrait(Sprite sprite)
    {
        poster.UpdatePortrait(sprite);
    }
    #endregion

    public void UpdateLanguage()
    {
        poster.UpdateLanguage();
    }

    public void Alert(string str)
    {
        alertText.text = str;

        if (alertCoroutine != null)
            StopCoroutine(alertCoroutine);

        alertCoroutine = Alert();
        StartCoroutine(alertCoroutine);
    }

    public void StopAlert()
    {
        if (alertCoroutine != null)
            StopCoroutine(alertCoroutine);

        alertObject.SetActive(false);
    }

    public IEnumerator Alert()
    {
        alertObject.SetActive(true);

        float time = 2f;

        while (time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }

        yield return null;

        alertObject.SetActive(false);
        alertCoroutine = null;
    }
}

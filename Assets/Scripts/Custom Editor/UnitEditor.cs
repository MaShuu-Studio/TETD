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
    [SerializeField] private TextMeshProUGUI idInfoText;
    [SerializeField] private TMP_InputField idInput;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_Dropdown nameDropdown;
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
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TMP_InputField costInput;

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
    private int typeInfo;

    #region Init
    public void Init()
    {
        towerIcons = new List<UnitEditorUnitIcon>();
        enemyIcons = new List<UnitEditorUnitIcon>();

        #region Adjust Data
        idInput.onEndEdit.AddListener(s => UpdateDataToPosterBasicData());
        nameInput.onEndEdit.AddListener(s => UpdateDataToPosterBasicData());

        nameInput.onEndEdit.AddListener(s => nameDropdown.options[nameDropdown.value].text = s);
        nameDropdown.onValueChanged.AddListener(i => nameInput.text = nameDropdown.options[nameDropdown.value].text);

        for (int i = 0; i < pivotInputs.Length; i++)
            pivotInputs[i].onEndEdit.AddListener(s => PivotDataChanged());

        elementDropdown.onValueChanged.AddListener(i => UpdateDataToPosterBasicData());
        gradeDropdown.onValueChanged.AddListener(i => UpdateDataToPosterBasicData());
        enemyGradeDropdown.onValueChanged.AddListener(i => UpdateDataToPosterBasicData());
        costInput.onEndEdit.AddListener(i => UpdateDataToPosterBasicData());

        statInputs[0].onEndEdit.AddListener(s => StatChanged(0));
        statInputs[1].onEndEdit.AddListener(s => StatChanged(1));
        statInputs[2].onEndEdit.AddListener(s => StatChanged(2));

        for (int i = 0; i < statInputs.Length; i++)
            statInputs[i].onEndEdit.AddListener(s => UpdateDataToPosterStat());

        attackAmountInput.onValueChanged.AddListener(s => UpdateDataToPosterStat());
        attackAmountInput.onValueChanged.AddListener(s => ChangeAttackAmount(s));

        for (int i = 0; i < towerAbilityDropdowns.Length; i++)
        {
            towerAbilityDropdowns[i].onValueChanged.AddListener(s => AblityChanged());
            towerAbilityDropdowns[i].onValueChanged.AddListener(s => UpdateDataToPosterAbility());
            towerAbilityInputs[i].onValueChanged.AddListener(s => UpdateDataToPosterAbility());
        }

        spfInput.onEndEdit.AddListener(s => ChangeSpf(s, spfInput, imageIcons[0]));
        spfInput.onEndEdit.AddListener(s => ChangeSpf(s, spfInput, imageIcons[1]));

        for (int i = 0; i < attackTimeInputs.Length; i++)
            attackTimeInputs[i].onEndEdit.AddListener(s => ArrangeAttackTime());

        effectColorInput.onValueChanged.AddListener(s => Hexadecimal());
        effectSpfInput.onEndEdit.AddListener(s => ChangeSpf(s, effectSpfInput, effectImageIcon));

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
            for (int i = 0; i < EnumArray.AbilityTypes.Length; i++)
            {
                AbilityType type = EnumArray.AbilityTypes[i];
                string oName = EnumArray.AbilityTypeStrings[type];
                Sprite oSprite = SpriteManager.GetSpriteWithNumber(SpriteManager.ETCDataNumber.TOWERABILITY, (int)type);
                CustomDropdownOption option = new CustomDropdownOption(oName, oSprite);
                options.Add(option);
            }

            float width = ((options.Count >= 5) ? 5 : options.Count) * 87.5f;
            float height = (100f / 3f * 2f) * (1 + (int)(options.Count / 5)) + 25;

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

        LoadUnitData();

        UpdatePoster(CustomDataManager.EditingTowerData[CustomDataManager.TowerKeys[0]]);
        SelectType(true);
    }

    public void LoadUnitData()
    {
        // TowerPanel 초기화
        foreach (var icon in towerIcons)
        {
            Destroy(icon.gameObject);
        }
        towerIcons.Clear();

        for (int i = 0; i < CustomDataManager.EditingTowerData.Count; i++)
        {
            int id = CustomDataManager.TowerKeys[i];
            UnitEditorUnitIcon icon = Instantiate(unitIconPrefab);

            icon.Init(CustomDataManager.EditingTowerData[id], CustomDataManager.EditingTowerSprites[id]);
            icon.transform.SetParent(towerViewPort);
            icon.transform.localScale = Vector3.one;
            towerIcons.Add(icon);
        }
        towerViewPort.sizeDelta = new Vector2(342, (int)((towerIcons.Count + 1) / 2) * 198 + 18);

        // EnemyPanel 초기화
        foreach (var icon in enemyIcons)
        {
            Destroy(icon.gameObject);
        }
        enemyIcons.Clear();

        for (int i = 0; i < CustomDataManager.EditingEnemyData.Count; i++)
        {
            int id = CustomDataManager.EnemyKeys[i];
            UnitEditorUnitIcon icon = Instantiate(unitIconPrefab);
            icon.Init(CustomDataManager.EditingEnemyData[id], CustomDataManager.EditingEnemySprites[id]);
            icon.transform.SetParent(enemyViewPort);
            icon.transform.localScale = Vector3.one;
            enemyIcons.Add(icon);
        }
        enemyViewPort.sizeDelta = new Vector2(342, (int)((enemyIcons.Count + 1) / 2) * 198 + 18);
    }
    #endregion

    public void NewUnit()
    {
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

    public void ChangeEditor(int id = -1)
    {
        if (Translator.Keys.Contains(id))
        {
            Language[] langs = Translator.GetLanguages(id);
            for (int i = 0; i < nameDropdown.options.Count; i++)
            {
                nameDropdown.options[i].text = langs[i].name;
            }
            nameDropdown.value = (int)Translator.CurrentLanguage;
        }

        if (isTower)
        {
            gradeObject.gameObject.SetActive(true);
            enemyGradeObject.gameObject.SetActive(false);

            costText.text = "COST:";

            animationTexts[0].text = "IDLE";
            animationTexts[1].text = "ATTACK";

            towerAbilityParent.SetActive(true);
            towerAdvancedOption.SetActive(true);
            //enemyAbilityParent.SetActive(false);

            statInputs[2].characterValidation = TMP_InputField.CharacterValidation.Decimal;
            ((TextMeshProUGUI)(statInputs[2].placeholder)).text = "1.0";

            UpdateStatImage();
            for (int i = 0; i < units.Length; i++)
                units[i].SetActive(true);
        }
        else
        {
            gradeObject.gameObject.SetActive(false);
            enemyGradeObject.gameObject.SetActive(true);

            costText.text = "REWARD:";

            animationTexts[0].text = "IDLE";
            animationTexts[1].text = "MOVE";

            towerAbilityParent.SetActive(false);
            towerAdvancedOption.SetActive(false);
            towerAdvancedPopup.SetActive(false);

            statInputs[2].characterValidation = TMP_InputField.CharacterValidation.Digit;
            ((TextMeshProUGUI)(statInputs[2].placeholder)).text = "000";

            UpdateStatImage();
            for (int i = 0; i < units.Length; i++)
                units[i].SetActive(false);
        }
    }

    public void SaveUnit()
    {
        string unitId = idInput.text;

        int element = elementDropdown.value;
        int grade = (isTower) ? gradeDropdown.value : enemyGradeDropdown.value;

        int id = typeInfo;
        id = id * 100 + element;
        id = id * 10 + grade;

        string idStr = id.ToString() + unitId;

        if (int.TryParse(idStr, out id))
        {
            string dataPath = Application.streamingAssetsPath + ((isTower) ? CustomDataManager.editingPath + "Tower/" : CustomDataManager.editingPath + "Enemy/") + $"{EnumArray.Elements[element]}.json";
            string imgPath = Application.streamingAssetsPath + ((isTower) ? CustomDataManager.editingTowerSpritePath  : CustomDataManager.editingEnemySpritePath) + $"{selectedId}/";
            if (id != selectedId) RemoveUnit();

            Vector2 pivot;
            float.TryParse(pivotInputs[0].text, out pivot.x);
            float.TryParse(pivotInputs[1].text, out pivot.y);

            int cost;
            int.TryParse(costInput.text, out cost);

            float[] stats = new float[3];
            for (int i = 0; i < 3; i++)
                float.TryParse(statInputs[i].text, out stats[i]);

            float spf;
            float.TryParse(spfInput.text, out spf);

            Dictionary<string, List<Sprite>> sprites = new Dictionary<string, List<Sprite>>();
            for (int i = 0; i < animationTexts.Length; i++)
            {
                sprites.Add(animationTexts[i].text, imageIcons[i].Sprites);
            }

            JsonData data;
            if (isTower)
            {
                int attackAmount;
                int.TryParse(attackAmountInput.text, out attackAmount);
                List<float> attacktimes = new List<float>();

                for (int i = 0; i < attackAmount; i++)
                {
                    float time;
                    float.TryParse(attackTimeInputs[i].text, out time);
                    attacktimes.Add(time);
                }

                // 어빌리티
                List<TowerAbility> abils = new List<TowerAbility>();

                for (int i = 0; i < towerAbilities.Length; i++)
                {
                    TowerAbility ability = new TowerAbility();
                    int index = towerAbilityDropdowns[i].value;

                    if (index == 0) continue;

                    ability.type = (int)EnumArray.AbilityTypes[index - 1];
                    float.TryParse(towerAbilityInputs[i].text, out ability.value);
                    abils.Add(ability);
                }

                AttackType attackType = AttackType.PROMPT;
                for (int i = 0; i < typeToggles.Length; i++)
                {
                    if (typeToggles[i].isOn)
                    {
                        attackType = (AttackType)i;
                        break;
                    }
                }

                float projspf = 0;
                float projattackTime = 0;
                float projTime = 0;

                float effectspf;
                Color effectColor = effectColorIcon.color;

                Dictionary<string, List<Sprite>> efprojs = new Dictionary<string, List<Sprite>>();

                float.TryParse(effectSpfInput.text, out effectspf);
                if (effectImageIcon.isEmpty == false)
                {
                    efprojs.Add("EFFECT", effectImageIcon.Sprites);
                    sprites.Add("EFFECT", effectImageIcon.Sprites);
                }


                // 투사체 공격이나 지점 공격 시 투사체 세팅.
                if (attackType != AttackType.PROMPT)
                {
                    float.TryParse(projSpfInput.text, out projspf);
                    float.TryParse(projAttackTimeInput.text, out projattackTime);
                    float.TryParse(projRemainTimeInput.text, out projTime);

                    if (projImageIcon.isEmpty == false)
                    {
                        efprojs.Add("WEAPON", projImageIcon.Sprites);
                        sprites.Add("WEAPON", projImageIcon.Sprites);
                    }
                }

                data = new TowerData()
                {
                    id = id,

                    imgsrc = imgPath,
                    pivot = pivot,
                    spf = spf,

                    cost = cost,

                    dmg = stats[0],
                    attackspeed = 1 / stats[1],
                    range = stats[2],

                    attacktime = attacktimes,

                    ability = abils,

                    type = attackType,

                    effectspf = effectspf,
                    effectcolor = effectColor,

                    projspf = projspf,
                    projattacktime = projattackTime,
                    projtime = projTime
                };

                Dictionary<AnimationType, List<Sprite>> dataAnims = new Dictionary<AnimationType, List<Sprite>>();
                dataAnims.Add(AnimationType.IDLE, imageIcons[0].Sprites);
                dataAnims.Add(AnimationType.ATTACK, imageIcons[1].Sprites);

                CustomDataManager.AddData((TowerData)data);
            }
            else
            {
                data = new EnemyData()
                {
                    id = id,

                    imgsrc = imgPath,
                    pivot = pivot,
                    spf = spf,

                    hp = stats[0],
                    speed = stats[1],
                    exp = (int)stats[2],
                    money = cost,
                };

                Dictionary<AnimationType, List<Sprite>> dataAnims = new Dictionary<AnimationType, List<Sprite>>();
                dataAnims.Add(AnimationType.IDLE, imageIcons[0].Sprites);
                dataAnims.Add(AnimationType.MOVE, imageIcons[1].Sprites);

                CustomDataManager.AddData((EnemyData)data);
            }

            Language[] langs = new Language[Translator.Langs.Count];
            for (int i = 0; i < nameDropdown.options.Count; i++)
            {
                langs[i] = new Language(id, nameDropdown.options[i].text);
            }

            DataManager.SaveCustomData(data, dataPath, sprites, langs);
            LoadUnitData();
        }
    }
    private int selectedId;

    public void RemoveUnit(bool onlyRemove = false)
    {
        string id = selectedId.ToString().Substring(1, 3);
        int element;
        int.TryParse(id.Substring(0, 2), out element);

        int grade;
        int.TryParse(id.Substring(2, 1), out grade);

        string dataPath = Application.streamingAssetsPath + ((isTower) ? CustomDataManager.editingPath + "Tower/" : CustomDataManager.editingPath + "Enemy/") + $"{EnumArray.Elements[element]}.json";
        string imgPath = Application.streamingAssetsPath + ((isTower) ? CustomDataManager.editingTowerSpritePath : CustomDataManager.editingEnemySpritePath) + $"{selectedId}/";

        if (isTower)
            CustomDataManager.RemoveData(selectedId);
        else
            CustomDataManager.RemoveData(selectedId);

        DataManager.RemoveCustomData(selectedId, dataPath, imgPath);

        if (onlyRemove)
        {
            NewPoster();
            LoadUnitData();
        }
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
        selectedId = data.id;

        string id = data.id.ToString();
        typeInfo = id[0] - '0';

        poster.SetData(data);
        saveButtonText.text = $"SAVE {data.id}";

        isTower = true;
        ChangeEditor(data.id);

        UpdatePosterToData();
    }

    public void UpdatePoster(Enemy data)
    {
        selectedId = data.id;

        string id = data.id.ToString();
        typeInfo = id[0] - '0';

        poster.SetData(data);
        saveButtonText.text = $"SAVE {data.id}";

        isTower = false;
        ChangeEditor(data.id);

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
            int cost = data.cost;

            int idInfo = typeInfo;
            idInfo = idInfo * 100 + (int)element;
            idInfo = idInfo * 10 + (int)grade;
            idInfoText.text = idInfo.ToString();

            costInput.text = cost.ToString();

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
                        sprites[0] = CustomDataManager.EditingTowerSprites[data.id];
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
                if (CustomDataManager.EditingTowerEffects.ContainsKey(data.id))
                    sprites = CustomDataManager.EditingTowerEffects[data.id];

                float effectSpf = data.effectspf;
                effectImageIcon.SetSprites(sprites);
                effectImageIcon.SetSpf(effectSpf);
                effectSpfInput.text = effectSpf.ToString();
            }

            typeToggles[(int)data.attackType].isOn = true;
            if (data.attackType != AttackType.PROMPT)
            {
                Sprite[] sprites = null;
                if (CustomDataManager.EditingTowerProjs.ContainsKey(data.id))
                    sprites = CustomDataManager.EditingTowerProjs[data.id];

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
            if (data.AbilityTypes != null)
                for (int i = 0; i < data.AbilityTypes.Length; i++)
                {
                    if (index < towerAbilityDropdowns.Length)
                    {
                        towerAbilityDropdowns[index].value = ChangeAbilityToValue(data.AbilityTypes[i]);
                        towerAbilityInputs[index].text = data.Ability(data.AbilityTypes[i]).ToString();
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
            int money = data.money;

            int idInfo = typeInfo;
            idInfo = idInfo * 100 + (int)element;
            idInfo = idInfo * 10 + (int)grade;
            idInfoText.text = idInfo.ToString();

            costInput.text = money.ToString();
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
                        sprites[0] = CustomDataManager.EditingEnemySprites[data.id];
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
        string cost = costInput.text;

        poster.UpdatePoster(isTower, name, element, grade, cost);

        int id = typeInfo;
        id = id * 100 + element;
        id = id * 10 + grade;

        idInfoText.text = id.ToString();

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
        Dictionary<AbilityType, float> abils = new Dictionary<AbilityType, float>();

        for (int i = 0; i < towerAbilities.Length; i++)
        {
            int index = towerAbilityDropdowns[i].value;

            if (index == 0) continue;
            AbilityType type = EnumArray.AbilityTypes[index - 1];
            float value;
            float.TryParse(towerAbilityInputs[i].text, out value);

            abils.Add(type, value);
        }

        poster.UpdateAbility(abils);
    }
    #endregion

    #region valueChanged

    private void StatChanged(int index)
    {
        float value;
        float.TryParse(statInputs[index].text, out value);

        // 1번 스탯(공속, 이속) 타워의 2번스탯(사거리)에 적용되는 방식
        if (index == 1 || (index == 2 && isTower))
        {
            if (value < 0 || value >= 10)
            {
                value = Mathf.Clamp(value, 0, 9.9f);
                statInputs[index].text = string.Format("{0:0.#}", value);
            }
        }
        // 나머지는 음수만 아니면 해결
        else if (value < 0)
        {
            value = value * -1;
            statInputs[index].text = value.ToString();
        }
    }

    private int ChangeAbilityToValue(AbilityType type)
    {
        for (int i = 0; i < EnumArray.AbilityTypes.Length; i++)
        {
            if (type == EnumArray.AbilityTypes[i]) return i + 1;
        }
        return 0;
    }

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
            if (c >= 'a' && c <= 'f')
                ch = (char)(c - ('a' - 'A'));

            if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'F'))
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
        nameDropdown.value = (int)Translator.CurrentLanguage;
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

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EnumData;

public class Library : MonoBehaviour
{
    [SerializeField] private LibraryCard[] items;

    [SerializeField] private Button prevButton;
    [SerializeField] private Button nextButton;

    [SerializeField] private LibraryFilterToggle[] typeFilterToggles;
    [SerializeField] private LibraryFilterToggle[] elementFilterToggles;
    [SerializeField] private LibraryFilterToggle[] gradeFilterToggles;
    [SerializeField] private LibraryFilterToggle[] enemyGradeFilterToggles;
    [SerializeField] private LibraryFilterToggle[] abilFilterToggles;

    [SerializeField] private TextMeshProUGUI pageText;

    private int cardUnitinPage;
    public int CurrentPage { get { return currentPage; } }
    private int currentPage;

    private int lastPage;

    private List<int> ids;

    public async Task Init()
    {
        cardUnitinPage = items.Length;

        ids = new List<int>();

        // ������ �̵� ��ư
        prevButton.onClick.AddListener(() => MovePage(false));
        nextButton.onClick.AddListener(() => MovePage(true));

        // Ÿ���� ���� �ε�� ������ ��� ���
        while (TowerManager.Keys == null || EnemyManager.Keys == null) await Task.Yield();

        // �� ���� ��� �ʱ�ȭ
        // �Ŀ� �������� Ȱ���� ���, �Ӽ��� ������ ���� �ڵ����� �����Ǹ� ���� ��.

        for (int i = 0; i < typeFilterToggles.Length; i++)
        {
            typeFilterToggles[i].Init((int)SpriteManager.ETCDataNumber.TYPE, i);
        }
        for (int i = 0; i < elementFilterToggles.Length; i++)
        {
            elementFilterToggles[i].Init((int)SpriteManager.ETCDataNumber.ELEMENT, i);
        }
        for (int i = 0; i < gradeFilterToggles.Length; i++)
        {
            gradeFilterToggles[i].Init((int)SpriteManager.ETCDataNumber.GRADE, i);
        }
        for (int i = 0; i < enemyGradeFilterToggles.Length; i++)
        {
            enemyGradeFilterToggles[i].Init((int)SpriteManager.ETCDataNumber.ENEMYGRADE, i);
        }

        // �⺻������ �ɷ� ���ʹ� �� ���·� ����.
        for (int i = 0; i < EnumArray.AbilityTypes.Length; i++)
        {
            int type = (int)EnumArray.AbilityTypes[i];
            abilFilterToggles[i].Init((int)SpriteManager.ETCDataNumber.TOWERABILITY, type, false);
        }

        UpdateLibrary();
    }

    public void Open(bool b)
    {
        if (b)
        {
            currentPage = 1;
            UpdatePage();
            gameObject.SetActive(true);
        }
        else
            gameObject.SetActive(false);
    }

    // ID: TEEGNNN
    public void UpdateLibrary()
    {
        // ��ü ���� �� ��°�� �߰�.
        ids.Clear();

        if (typeFilterToggles[0].isOn)
        {
            // ����� �¿����� üũ�Ͽ� ������� �߰�
            for (int e = 0; e < elementFilterToggles.Length; e++)
            {
                if (elementFilterToggles[e].isOn == false) continue;

                for (int g = 0; g < gradeFilterToggles.Length; g++)
                {
                    if (gradeFilterToggles[g].isOn == false) continue;
                    for (int i = 0; i < TowerManager.EgTowerIds[e, g].Count; i++)
                    {
                        int id = TowerManager.EgTowerIds[e, g][i];
                        bool add = true;

                        // ���Ͱ� �����ִ� ���� ����� ����.
                        // �׷��� ���Ͱ� �����ִٸ� �ش���� �ʴ� id�� ���� �ʾƾ���.
                        // ���� �⺻������ �߰��� �����Ͽ� �����ϸ�
                        // ���࿡ ���Ͱ� �����־��µ� �ش� id�� �ش� ability�� ���ٸ� ��ŵ.
                        for (int s = 0; add && s < abilFilterToggles.Length; s++)
                        {
                            if (abilFilterToggles[s].isOn == false) continue;
                            int type = (int)EnumArray.AbilityTypes[s];
                            if (TowerManager.AbilityIds[type].Contains(id) == false)
                            {
                                add = false;
                                break;
                            }
                        }

                        if (add) ids.Add(id);
                    }
                }
            }
        }

        if (typeFilterToggles[1].isOn)
        {
            // ����� �¿����� üũ�Ͽ� ������� �߰� 
            // enemy�� �ڷΰ����� ��ġ
            for (int e = 0; e < elementFilterToggles.Length; e++)
            {
                if (elementFilterToggles[e].isOn == false) continue;

                for (int g = 0; g < enemyGradeFilterToggles.Length; g++)
                {
                    if (enemyGradeFilterToggles[g].isOn == false) continue;

                    for (int i = 0; i < EnemyManager.EgEnemyIds[e, g].Count; i++)
                    {
                        ids.Add(EnemyManager.EgEnemyIds[e, g][i]);
                    }
                }
            }
        }

        // ���͸� �Ŀ��� 1������ �̵�
        currentPage = 1;
        lastPage = ids.Count / cardUnitinPage + ((ids.Count % cardUnitinPage != 0) ? 1 : 0);

        if (lastPage == 0) lastPage = 1;
        pageText.text = $"{currentPage} / {lastPage}";

        // 1�������� �̵��ϱ� ������ ������ �̵� ��ư ǥ�� ����
        prevButton.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(lastPage > 1);

        UpdatePage();
    }

    public void UpdatePage()
    {
        int i;

        for (i = 0; i < cardUnitinPage; i++)
            items[i].gameObject.SetActive(true);

        for (i = 0; i < cardUnitinPage; i++)
        {
            // �������� ���� index ����
            int index = i + (currentPage - 1) * cardUnitinPage;
            if (index >= ids.Count) break;
            int id = ids[index];

            Tower tower = TowerManager.GetTower(id);
            if (tower != null) items[i].SetData(tower);
            else
            {
                Enemy enemy = EnemyManager.GetEnemy(id);
                if (enemy != null) items[i].SetData(enemy);
                else items[i].gameObject.SetActive(false);
            }
        }

        for (; i < cardUnitinPage; i++)
            items[i].gameObject.SetActive(false);
    }

    public void MovePage(bool next)
    {
        if (next) currentPage++;
        else currentPage--;

        if (currentPage <= 1)
        {
            currentPage = 1;
            prevButton.gameObject.SetActive(false);
        }
        else if (currentPage >= lastPage)
        {
            currentPage = lastPage;
            nextButton.gameObject.SetActive(false);
        }
        else
        {
            prevButton.gameObject.SetActive(true);
            nextButton.gameObject.SetActive(true);
        }

        pageText.text = $"{currentPage} / {lastPage}";
        UpdatePage();
    }
}

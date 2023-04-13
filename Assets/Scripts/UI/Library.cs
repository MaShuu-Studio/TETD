using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Library : MonoBehaviour
{
    public enum LibraryFilterType { TYPE = 0, GRADE = 1, ELEMENT = 2 }

    [SerializeField] private TowerInfoCard[] items;

    [SerializeField] private Button prevButton;
    [SerializeField] private Button nextButton;

    [SerializeField] private LibraryFilterToggle[] typeFilterToggles;
    [SerializeField] private LibraryFilterToggle[] gradeFilterToggles;
    [SerializeField] private LibraryFilterToggle[] elementFilterToggles;

    [SerializeField] private TextMeshProUGUI pageText;

    private const int cardUnitinPage = 8;
    public int CurrentPage { get { return currentPage; } }
    private int currentPage;

    private int lastPage;

    private List<int> ids;

    public async Task Init()
    {
        ids = new List<int>();

        // ������ �̵� ��ư
        prevButton.onClick.AddListener(() => MovePage(false));
        nextButton.onClick.AddListener(() => MovePage(true));

        // Ÿ���� ���� �ε�� ������ ��� ���
        while (TowerManager.Keys == null) await Task.Yield();

        // �� ���� ��� �ʱ�ȭ
        // �Ŀ� �������� Ȱ���� ���, �Ӽ��� ������ ���� �ڵ����� �����Ǹ� ���� ��.

        for (int i = 0; i < typeFilterToggles.Length; i++)
        {
            typeFilterToggles[i].Init((int)LibraryFilterType.TYPE, i + 1);
        }
        for (int i = 0; i < gradeFilterToggles.Length; i++)
        {
            gradeFilterToggles[i].Init((int)LibraryFilterType.GRADE, i + 1);
        }
        for (int i = 0; i < elementFilterToggles.Length; i++)
        {
            elementFilterToggles[i].Init((int)LibraryFilterType.ELEMENT, i + 1);
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

        // ����� �¿����� üũ�Ͽ� ������� �߰�
        for (int e = 0; e < elementFilterToggles.Length; e++)
        {
            if (elementFilterToggles[e].isOn == false) continue;

            for (int g = 0; g < gradeFilterToggles.Length; g++)
            {
                if (gradeFilterToggles[g].isOn == false) continue;

                for (int i = 0; i < TowerManager.EgTowerIds[e, g].Count; i++)
                {
                    ids.Add(TowerManager.EgTowerIds[e, g][i]);
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

    private void UpdatePage()
    {
        int i;

        for (i = 0; i < cardUnitinPage; i++)
            items[i].gameObject.SetActive(true);

        for (i = 0; i < cardUnitinPage; i++)
        {
            // �������� ���� index ����
            int index = i + (currentPage - 1) * cardUnitinPage;
            if (index >= ids.Count) break;

            Tower tower = TowerManager.GetTower(ids[index]);
            items[i].SetData(tower);
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

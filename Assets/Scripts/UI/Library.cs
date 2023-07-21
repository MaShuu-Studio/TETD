using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Library : MonoBehaviour
{
    [SerializeField] private LibraryCard[] items;

    [SerializeField] private Button prevButton;
    [SerializeField] private Button nextButton;

    [SerializeField] private LibraryFilterToggle[] typeFilterToggles;
    [SerializeField] private LibraryFilterToggle[] elementFilterToggles;
    [SerializeField] private LibraryFilterToggle[] gradeFilterToggles;
    [SerializeField] private LibraryFilterToggle[] enemyGradeFilterToggles;

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

        // 페이지 이동 버튼
        prevButton.onClick.AddListener(() => MovePage(false));
        nextButton.onClick.AddListener(() => MovePage(true));

        // 타워가 전부 로드될 때까지 잠시 대기
        while (TowerManager.Keys == null || EnemyManager.Keys == null) await Task.Yield();

        // 각 필터 토글 초기화
        // 후에 프리팹을 활용해 등급, 속성의 갯수에 따라 자동으로 생성되면 좋을 듯.

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
        // 전체 삭제 후 통째로 추가.
        ids.Clear();

        if (typeFilterToggles[0].isOn)
        {
            // 토글의 온오프를 체크하여 순서대로 추가
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
        }

        if (typeFilterToggles[1].isOn)
        {
            // 토글의 온오프를 체크하여 순서대로 추가 
            // enemy가 뒤로가도록 배치
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

        // 필터링 후에는 1페이지 이동
        currentPage = 1;
        lastPage = ids.Count / cardUnitinPage + ((ids.Count % cardUnitinPage != 0) ? 1 : 0);

        if (lastPage == 0) lastPage = 1;
        pageText.text = $"{currentPage} / {lastPage}";

        // 1페이지로 이동하기 때문에 페이지 이동 버튼 표기 조정
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
            // 페이지에 따라 index 조정
            int index = i + (currentPage - 1) * cardUnitinPage;
            if (index >= ids.Count) break;
            int id = ids[index];

            Tower tower = TowerManager.GetTower(id);
            if (tower != null)                items[i].SetData(tower);
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

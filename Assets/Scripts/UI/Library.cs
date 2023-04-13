using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EnumData;

public class Library : MonoBehaviour
{
    [SerializeField] private TowerInfoCard[] items;
    [SerializeField] private Button prevButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private TextMeshProUGUI pageText;
    private const int cardUnitinPage = 8;
    public int CurrentPage { get { return currentPage; } }
    private int currentPage;

    private int lastPage;

    private List<int> ids;

    public async Task Init()
    {
        ids = new List<int>();

        while (TowerManager.Keys == null) await Task.Yield();
        ids.AddRange(TowerManager.Keys);

        currentPage = 1;
        lastPage = ids.Count / cardUnitinPage + ((ids.Count % cardUnitinPage != 0) ? 1 : 0);

        prevButton.onClick.AddListener(() => MovePage(false));
        nextButton.onClick.AddListener(() => MovePage(true));

        prevButton.gameObject.SetActive(false);
        prevButton.gameObject.SetActive(true);
        UpdateLibrary();

    }

    public void Open(bool b)
    {
        if (b)
        {
            currentPage = 1;
            UpdateLibrary();
            gameObject.SetActive(true);
        }
        else
            gameObject.SetActive(false);
    }

    // ID: TEEGNNN

    public void UpdateLibrary()
    {
        int i;

        for (i = 0; i < cardUnitinPage; i++)
            items[i].gameObject.SetActive(true);

        for (i = 0; i < cardUnitinPage; i++)
        {
            // 페이지에 따라 index 조정
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

        pageText.text = currentPage.ToString();
        UpdateLibrary();
    }
}

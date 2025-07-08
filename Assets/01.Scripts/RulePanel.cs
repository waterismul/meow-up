using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RulePanel : MonoBehaviour
{
    [SerializeField] private List<GameObject> pages; // 페이지 오브젝트들
    [SerializeField] private Button nextButton;       // 다음 버튼

    private int currentPageIndex = 0;

    void Start()
    {
        UpdatePage(); // 초기 상태 설정

        nextButton.onClick.AddListener(OnNextButtonClicked);
    }

    private void OnNextButtonClicked()
    {
        // 현재 페이지 끄기
        pages[currentPageIndex].SetActive(false);

        // 다음 페이지 인덱스 계산
        currentPageIndex = (currentPageIndex + 1) % pages.Count;

        // 새 페이지 켜기
        pages[currentPageIndex].SetActive(true);
    }

    private void UpdatePage()
    {
        for (int i = 0; i < pages.Count; i++)
        {
            pages[i].SetActive(i == currentPageIndex);
        }
    }
}

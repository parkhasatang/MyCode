using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public class UIEscapeStack : MonoBehaviour
{
    public bool isEscapeLock;
    [ShowInInspector]
    private Stack<UIStack> viewStack = new();

    public void Awake()
    {
        isEscapeLock = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isEscapeLock)
            {
                OnBackPressed();
                Debug.Log("키보드 뒤로가기");
            }
            else
            {
                Debug.Log("뒤로가기 막힘");
                return;
            }
        }
    }

    // UIView를 스택에 추가
    public void PushView(UIStack view)
    {
        viewStack.Push(view);
    }

    // 뒤로 가기 버튼을 눌렀을 때 호출
    private void OnBackPressed()
    {
        if (viewStack.Count > 0)
        {
            UIStack topView = viewStack.Pop();
            string viewName = topView.gameObject.name.Replace("(Clone)", ""); // "(Clone)" 제거
            UIManager.Instance.HideUI(viewName);
        }
        else
        {
            // 게임 종료 물어보기
        }
    }


    // 모든 뷰를 숨기고 스택을 초기화
    public void ClearStack()
    {
        while (viewStack.Count > 0)
        {
            UIStack topView = viewStack.Pop();
            topView.gameObject.SetActive(false);
        }
    }
}
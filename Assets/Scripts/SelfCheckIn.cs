using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SelfCheckIn : MonoBehaviour
{
    // 기기 상태
    public enum ECheckInState
    {
        // 대기
        READY,
        // 탑승권 조회
        BOARDINGPASS_SCAN,
        // 여권 조회
        PASSPORT_SCAN,
        // 좌석 선택
        SEAT_SELECTION,
        // 출력
        BOARDINGPASS_PRINTING,

        END
    }

    // 현재 기기 상태
    public ECheckInState currState;

    // 화면 대기 시간
    float delayTime;

    // 진행되고 있는 코루틴
    Coroutine currCo;

    // 각 상태별 배경 색상
    public Color[] bgColor;

    // 배경 Image
    public Image bg;

    // 나만의 카메라
    public Camera myCam;

    // 화면 크기 조절에 필요한 변수
    Rect startRect;
    Rect endRect;
    Rect originRect;
    float currTime;
    bool isMax;
    bool isChangeRect;

    void Start()
    {
        // 나의 카메라의 초기 Rect 저장
        originRect = myCam.rect;

        delayTime = Random.Range(2.0f, 5.0f);
        currCo = StartCoroutine(Process());
    }

    void Update()
    {
        UpdateRect();
    }

    IEnumerator Process()
    {
        while(true)
        {
            yield return new WaitForSeconds(delayTime);
            ChangeState();
        }
    }

    void ChangeState()
    {
        // 현재 상태를 다음 상태로
        currState = currState + 1;
        // 상태가 END 이면 READY 로 설정
        if(currState == ECheckInState.END)
        {
            currState = ECheckInState.READY;
        }

        // 화면 대기 시간을 랜덤하게 설정
        delayTime = Random.Range(2.0f, 5.0f);

        // 배경 색상 바꾸자
        bg.color = bgColor[(int)currState];
    }

    public void OnClickMaxScreen()
    {
        isMax = !isMax;

        isChangeRect = isMax;

        // 나의 카메라로 보이는 화면을 맨위에 보이게 하자
        myCam.depth = isMax ? 1 : 0;

        // Rect 시작 값
        startRect = isMax ? originRect : new Rect(0, 0, 1, 1);
        // Rect 변화 되야 하는 값
        endRect = isMax ? new Rect(0, 0, 1, 1) : originRect;

        // 시간 초기화
        currTime = 0;

        // 만약에 줄어드는 연출일 경우
        if(isMax == false)
        {
            // mainCam 의 Focus 실행
            Camera.main.GetComponent<MainCam>().Focus(() =>
            {
                isChangeRect = true;
            });
        }
    }
    

    void UpdateRect()
    {
        if (isChangeRect == false) return;

        // 시간을 흐르게 하고
        currTime += Time.deltaTime * 2;
        // 만약에 시간이 1보다 크거나 같으면
        if (currTime >= 1)
        {
            currTime = 1;
            // 화면크기가 변경 완료 되었다.
            isChangeRect = false;

            if(isMax)
            {
                // 메인 카메라의 부모를 나로 하자.
                Camera.main.transform.SetParent(transform);
                // 메인 카메라의 MainCam 컴포넌트 찾아오자.
                MainCam mainCam = Camera.main.GetComponent<MainCam>();
                // 찾아온 컴포넌트에서 Fouce  함수 실행
                mainCam.Focus();
            }
        }

        // 점점 화면이 endRect 로 변화하게
        float ratio = Easing.Linear(currTime);
        
        Rect rect = new Rect();
        rect.x = Mathf.Lerp(startRect.x, endRect.x, ratio);
        rect.y = Mathf.Lerp(startRect.y, endRect.y, ratio);
        rect.width = Mathf.Lerp(startRect.width, endRect.width, ratio);
        rect.height = Mathf.Lerp(startRect.height, endRect.height, ratio);

        // 계산된 rect 값을 나의 카메라에 설정
        myCam.rect = rect;
    }
}

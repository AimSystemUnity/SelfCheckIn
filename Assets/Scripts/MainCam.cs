using UnityEngine;

public class MainCam : MonoBehaviour
{
    // 시작, 도착 위치
    Vector3 startPos = new Vector3(5.52f, 5.17f, 5.82f);
    Vector3 endPos = new Vector3(0, 2.055f, 2.101f);
    // 시작, 도착 회전
    Quaternion startRot = new Quaternion(0.0912143812f, -0.895071208f, 0.214887559f, 0.379935265f);
    Quaternion endRot = new Quaternion(0, 1, 0, 0);

    Vector3 sPos, ePos;
    Quaternion sRot, eRot;

    // 셀프체크인 기기를 바라봐야 하는지
    bool isFocus;

    // 현재 시간
    float currTime = 1;

    void Start()
    {
        
    }

    void Update()
    {
        // 만약에 시간이 1보다 크거나 같으면 함수나가자.
        if (currTime >= 1) return;

        // 시간 증가시키자.
        currTime += Time.deltaTime;
        if (currTime > 1) currTime = 1;

        float ratio = Easing.Linear(currTime);

        // 나의 위치 ePos 값으로 변환
        transform.localPosition = Vector3.Lerp(sPos, ePos, ratio);
        // 나의 회전 eRot 값으로 변환
        transform.localRotation = Quaternion.Lerp(sRot, eRot, ratio);
    }

    public void Focus()
    {
        isFocus = !isFocus;

        // 카메라의 우선순위를 2로
        Camera camera = GetComponent<Camera>();
        camera.depth = 2;

        // 처음 로컬 위치 설정
        transform.localPosition = sPos = isFocus ? startPos : endPos;
        ePos = isFocus ? endPos : startPos;

        // 처음 로컬 회전 설정
        transform.localRotation = sRot = isFocus ? startRot : endRot;
        eRot = isFocus ? endRot : startRot;

        // 시간 초기화
        currTime = 0;
    }
}

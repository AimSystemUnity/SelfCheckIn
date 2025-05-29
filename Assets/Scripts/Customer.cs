using System.Collections;
using UnityEngine;

public class Customer : MonoBehaviour
{
    // 고객 상태
    public enum ECustomerState
    {
        DELAY,
        MOVE_TO_MACHINE,
        CHECKING,
        MOVE_TO_ORIGIN
    }

    // 현재 고객 상태
    public ECustomerState currState;

    // 이동 속력
    private float speed = 3;

    // 기계 앞 위치
    public Transform trEnd;
    // 초기 위치
    public Transform trStart;

    // 이동 해야 하는 거리
    float remainDist;

    // 애니메이터
    Animator anim;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();

        ChangeState(ECustomerState.DELAY);
    }

    void Update()
    {
        // 각 상태에서 계속 해야하는 일들
        switch (currState)
        {
            case ECustomerState.MOVE_TO_MACHINE:
                UpdateMoveToMachine();
                break;
            case ECustomerState.MOVE_TO_ORIGIN:
                UpdateMoveToOrigin();
                break;
        }
    }

    void UpdateMoveToMachine()
    {
        // 나의 앞방향으로 이동하자.
        transform.position += transform.forward * speed * Time.deltaTime;
        // 이동거리 만큼 remainDist 를 줄이자.
        remainDist -= speed * Time.deltaTime;
        // 만약에 도착했다면
        if(remainDist <= 0)
        {
            transform.position = trEnd.position;
            // 상태를 CHECKING 으로 전환
            ChangeState(ECustomerState.CHECKING);
        }
    }

    void UpdateMoveToOrigin()
    {
        // 나의 앞방향으로 이동하자.
        transform.position += transform.forward * speed * Time.deltaTime;
        // 이동거리 만큼 remainDist 를 줄이자.
        remainDist -= speed * Time.deltaTime;
        // 만약에 도착했다면
        if (remainDist <= 0)
        {
            transform.position = trStart.position;
            // 상태를 DELAY 으로 전환
            ChangeState(ECustomerState.DELAY);
        }
    }

    public void ChangeState(ECustomerState state)
    {
        // 현재 상태를 state 변경
        currState = state;

        // 각 상태에 맞는 초기화 설정
        switch (currState)
        {
            case ECustomerState.DELAY:
                StartCoroutine(Delay());
                anim.SetTrigger("IDLE");

                break;
            case ECustomerState.MOVE_TO_MACHINE:
                // 나의 앞방향을 trEnd - 나의 위치 (셀프체크 기계를 향하는 방향)
                transform.forward = trEnd.position - transform.position;
                // 내가 이동해야 하는 거리
                remainDist = Vector3.Distance(trEnd.position, transform.position);
                anim.SetTrigger("MOVE");

                break;
            case ECustomerState.CHECKING:
                // 셀프체크인 기계 동작
                GetComponentInParent<SelfCheckIn>().StartCheckInProcess();
                anim.SetTrigger("IDLE");

                break;
            case ECustomerState.MOVE_TO_ORIGIN:
                // 나의 앞방향을 trStart - 나의 위치 (원래 있었던 곳을 향하는 방향)
                transform.forward = trStart.position - transform.position;
                // 내가 이동해야 하는 거리
                remainDist = Vector3.Distance(trStart.position, transform.position);
                anim.SetTrigger("MOVE");

                break;
        }
    }

    IEnumerator Delay()
    {
        float time = Random.Range(2.0f, 5.0f);
        yield return new WaitForSeconds(time);

        ChangeState(ECustomerState.MOVE_TO_MACHINE);
    }
}

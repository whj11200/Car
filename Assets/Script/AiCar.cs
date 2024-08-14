using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//1. 무게중심 2.pathTransfrom 3. 휠콜라이더 컴포넌트 모델링 배치
public class AiCar : MonoBehaviour
{
    [Header("CenterMass")]
    [SerializeField] Rigidbody rb;
    public Vector3 CenterMass = new Vector3(0f, -0.5f, 0f);
    [Header("Path")]
    [SerializeField] Transform path;
    [SerializeField] Transform[] pathTransforms;
    [SerializeField] List<Transform> pathList;
    [Header("콜라이더")]
    [SerializeField]
    WheelCollider FL;
    [SerializeField]
    WheelCollider FR;
    [SerializeField]
    WheelCollider BL;
    [SerializeField]
    WheelCollider BR;
    [SerializeField]
    Transform FLTr;
    [SerializeField]
    Transform FRTr;
    [SerializeField]
    Transform BLTr;
    [SerializeField]
    Transform BRTr;
    // 현재 스피드
    public float currentspeed = 0f; // 현재 스피드 
    private int currentNode = 0; // 현재 노드
    private float maxspeed = 150f; //  150 이상 못달리게 제한
    public float maxMotorTorque = 1000f; // 훨 콜라이더가 회전하는 최대 힘
    public float maxSteer = 30f; // 앞바퀴 회전 각
  
    public
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = CenterMass;
        path = GameObject.Find("PantTransform").transform;
        pathTransforms = path.GetComponentsInChildren<Transform>();
        for(int i = 0; i < pathTransforms.Length; i++)
        {
            if(pathTransforms[i] != path)
            {
                pathList.Add(pathTransforms[i]);
            } 
        }
    }


     void FixedUpdate()
    {
        ApplySteer();
        Drive();
        CheckWayPointDistance();
    }
    void ApplySteer()// 앞바퀴 가 휠 콜라이더 가 path에 따라서 회전 하는 메서드 
    {
         Vector3 relativeVector = transform.InverseTransformPoint(pathList[currentNode].position);
        // 실제적인 방향  = 월드 좌표를 게임상의 자표를 로컬좌표로 변환 한다.
                          // 패스트랜스폼 x값 / 패스트랜폼 개체크기 *30
        float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteer;

        FL.steerAngle = newSteer;
        FR.steerAngle = newSteer;
    }


    void Drive()
    {
        currentspeed = 2 * Mathf.PI * FL.radius * FL.rpm * 60 / 1000;

        // 목표 지점에 가까워질 때 속도를 줄입니다.
        float targetSpeed = maxspeed;
        if (Vector3.Distance(transform.position, pathList[currentNode].position) < 10f) // 목표 지점에서 10m 이내일 때
        {
            targetSpeed = Mathf.Lerp(currentspeed, 0f, Time.deltaTime); // 속도를 점진적으로 줄입니다.
        }

        if (currentspeed < targetSpeed)
        {
            BL.motorTorque = maxMotorTorque;
            BR.motorTorque = maxMotorTorque;
        }
        else // 현재 스피드가 최고 속도보다 높으면
        {
            BL.motorTorque = 0;
            BR.motorTorque = 0;
        }
    }

    void CheckWayPointDistance()
    {  
            // 차량과 도착지점거리를 비교해서 3.5보다 낮아지면
        if (Vector3.Distance(transform.position, pathList[currentNode].position) < 3.5f)
        {
            
            BL.motorTorque = 10;
            BR.motorTorque = 10;
            Debug.Log(BL.motorTorque);
            // 마지막 까지 왔을때 다시 0으로 초기화
            if (currentNode == pathList.Count - 1)
            {
                currentNode = 0;
            }
            else
            {
                currentNode++;
            }
        }
    }


}

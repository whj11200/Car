using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCar : MonoBehaviour
{
    public Light[] P_Lights;
    public Light[] BF_lights;
    public Light[] B_lights;
    [Header("WhellCollider")]
    public WheelCollider frontLeft_col;
    public WheelCollider frontRight_col;
    public WheelCollider backLeft_col;
    public WheelCollider backRight_col;
    [Header("Model")]
    public Transform frontLeft_M;
    public Transform frontRight_M;
    public Transform backLeft_M;
    public Transform backRight_M;
    [Header("Mass Blance")]//무게중심
    private Vector3 CentOffMass = new Vector3 (0, -0.5f, 0);
    // 자동차나 마차등 바퀴가 있는 모델의  무제중심은 y축은 항상 -0.5f로 잡아야한다.
    public Rigidbody rb;
    [Header("앞바퀴 최대 회전각")]
    public float maxSteer = 35f;
    [Header("최대 마찰력")]
    public float maxToque = 3500f; // 최대 토크
    [Header("최대 브레이크")]
    public float maxBrake = 5000f; // 브레이크 
    [Header("현재 스피드")]
    public float currentSpeed = 0f;
    // A,D키값을 받을 변수 방향잡는 용도
    private float Steer = 0f;
    //  W키만 받아서 전진하는 용도
    private float forward = 0f;
    //  S키만 받아서 후진하는 용도
    private float back = 0f; 
    bool isrevers = false;
    private float motor = 0f; //  

    private float brake = 0f;
    private GetintheCar getinthe;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = CentOffMass;
        for(int i = 0; i< P_Lights.Length; i++)
        {
            P_Lights[i].enabled = false;
        }
        for(int j = 0; j<B_lights.Length; j++)
        {
            B_lights[j].enabled = false;
        }
        for(int i = 0; i<BF_lights.Length; i++)
        {
            BF_lights[i].enabled = false;
        }
        getinthe = transform.GetChild(5).GetComponent<GetintheCar>();

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            for (int i = 0; i < P_Lights.Length; i++)
            {
                P_Lights[i].enabled = !P_Lights[i].enabled;
            }

        }
    }

    //고정프레임 정확한 물리량이나 정확한 시간에 따라 움직이는 호직을 구현 하려면 Fixed 업데이트로 바꿔야한다.
    void FixedUpdate()
    {
        if (getinthe.isGetin)
        {
            // 휠 퀄라이드가 회전을 하면 리지드바디도 포함해서 앞으로 가게 설정
            Carmove();
            if (Input.GetKey(KeyCode.Space))
            {
                CarBreak();
               
            }
            else
            {
                Carmove();

            }
            //else if (Input.GetKeyUp(KeyCode.Space))
            //{
            //    for (int j = 0; j < B_lights.Length; j++)
            //    {
            //        B_lights[j].enabled = false;
            //    }
            //}

        }
        
        
        
        
        //후진중이라면
        if (isrevers)
        {

            motor = -1 * back;
            brake = forward;
            
        }
        else
        {
            motor = forward;
            brake = back;
        }
        // 뒷바퀴 토크 회전력
        backLeft_col.motorTorque = motor * maxToque;
        backRight_col.motorTorque = motor * maxToque;
        backLeft_col.brakeTorque = brake * maxBrake;
        backRight_col.brakeTorque = brake * maxBrake;
        // 앞바퀴 토크 회전
        frontLeft_col.steerAngle = maxSteer * Steer;
        frontRight_col.steerAngle = maxSteer * Steer;
        // 모델링 회전 훨콜라이더 회전 토크 값을 받아서 같이 회전한다.
        // A,D 눌렀을때 Y축 회전
        frontLeft_M.localEulerAngles = new Vector3(frontLeft_M.localEulerAngles.x, Steer * maxSteer, frontLeft_M.localEulerAngles.z);
        frontRight_M.localEulerAngles = new Vector3(frontRight_M.localEulerAngles.x, Steer * maxSteer, frontRight_M.localEulerAngles.z);
        // 앞쪽 왼쪽 바퀴모델에 콜라이더 rpm을 받아 프레임단위로 회전시키게 함
        frontLeft_M.Rotate(frontLeft_col.rpm * Time.deltaTime, 0f, 0f);
        frontRight_M.Rotate(frontRight_col.rpm * Time.deltaTime, 0f, 0f);
        backLeft_M.Rotate(backLeft_col.rpm * Time.deltaTime, 0f, 0f);
        backRight_M.Rotate(backRight_col.rpm, Time.deltaTime, 0f, 0f);
        
        
    }

    private void Carmove()
    {
        for (int j = 0; j < B_lights.Length; j++)
        {
            B_lights[j].enabled = false;
        }
        frontLeft_col.brakeTorque = 0f;
        frontRight_col.brakeTorque = 0f;
        currentSpeed = rb.velocity.sqrMagnitude;
        // 회전역활
        Steer = Mathf.Clamp(Input.GetAxis("Horizontal"), -1f, 1f);
        // W키 는 전진만 하도록 설정
        forward = Mathf.Clamp(Input.GetAxis("Vertical"), 0f, 1f);
        // S키 는 후잔만 하도록 설정
        back = -1 * Mathf.Clamp(Input.GetAxis("Vertical"), -1f, 0f);
        if (Input.GetKey(KeyCode.W))
        {
            StartCoroutine(ForWardCar());
           
        }
        if (Input.GetKey(KeyCode.S))
        {
            StartCoroutine(BackWardCar());
            for (int i = 0; i < BF_lights.Length; i++)
            {
                BF_lights[i].enabled = true;
            }
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            for (int i = 0; i < BF_lights.Length; i++)
            {
                BF_lights[i].enabled = false;
            }
        }
       

    }

    IEnumerator ForWardCar()
    {
        yield return new WaitForSeconds(0.1f);
        currentSpeed = 0f;
        if (back > 0f)
            isrevers = true;
        if (forward > 0f)
            isrevers = false;
    }
    IEnumerator BackWardCar()
    {
        yield return new WaitForSeconds(0.1f);
        currentSpeed = 0.1f;
        if (back > 0f)
            isrevers = true;
        if (forward > 0f)
            isrevers = false;
    }
    void CarBreak()
    {
        frontRight_col.motorTorque = 0f;
        frontLeft_col.motorTorque = 0f;
        frontLeft_col.brakeTorque = 50000f;
        frontRight_col.brakeTorque = 50000f;
        for (int j = 0; j < B_lights.Length; j++)
        {
            B_lights[j].enabled = true;
        }
    }
    
}

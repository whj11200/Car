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
    [Header("Mass Blance")]//�����߽�
    private Vector3 CentOffMass = new Vector3 (0, -0.5f, 0);
    // �ڵ����� ������ ������ �ִ� ����  �����߽��� y���� �׻� -0.5f�� ��ƾ��Ѵ�.
    public Rigidbody rb;
    [Header("�չ��� �ִ� ȸ����")]
    public float maxSteer = 35f;
    [Header("�ִ� ������")]
    public float maxToque = 3500f; // �ִ� ��ũ
    [Header("�ִ� �극��ũ")]
    public float maxBrake = 5000f; // �극��ũ 
    [Header("���� ���ǵ�")]
    public float currentSpeed = 0f;
    // A,DŰ���� ���� ���� ������� �뵵
    private float Steer = 0f;
    //  WŰ�� �޾Ƽ� �����ϴ� �뵵
    private float forward = 0f;
    //  SŰ�� �޾Ƽ� �����ϴ� �뵵
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

    //���������� ��Ȯ�� �������̳� ��Ȯ�� �ð��� ���� �����̴� ȣ���� ���� �Ϸ��� Fixed ������Ʈ�� �ٲ���Ѵ�.
    void FixedUpdate()
    {
        if (getinthe.isGetin)
        {
            // �� �����̵尡 ȸ���� �ϸ� ������ٵ� �����ؼ� ������ ���� ����
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
        
        
        
        
        //�������̶��
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
        // �޹��� ��ũ ȸ����
        backLeft_col.motorTorque = motor * maxToque;
        backRight_col.motorTorque = motor * maxToque;
        backLeft_col.brakeTorque = brake * maxBrake;
        backRight_col.brakeTorque = brake * maxBrake;
        // �չ��� ��ũ ȸ��
        frontLeft_col.steerAngle = maxSteer * Steer;
        frontRight_col.steerAngle = maxSteer * Steer;
        // �𵨸� ȸ�� ���ݶ��̴� ȸ�� ��ũ ���� �޾Ƽ� ���� ȸ���Ѵ�.
        // A,D �������� Y�� ȸ��
        frontLeft_M.localEulerAngles = new Vector3(frontLeft_M.localEulerAngles.x, Steer * maxSteer, frontLeft_M.localEulerAngles.z);
        frontRight_M.localEulerAngles = new Vector3(frontRight_M.localEulerAngles.x, Steer * maxSteer, frontRight_M.localEulerAngles.z);
        // ���� ���� �����𵨿� �ݶ��̴� rpm�� �޾� �����Ӵ����� ȸ����Ű�� ��
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
        // ȸ����Ȱ
        Steer = Mathf.Clamp(Input.GetAxis("Horizontal"), -1f, 1f);
        // WŰ �� ������ �ϵ��� ����
        forward = Mathf.Clamp(Input.GetAxis("Vertical"), 0f, 1f);
        // SŰ �� ���ܸ� �ϵ��� ����
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

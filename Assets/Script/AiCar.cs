using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//1. �����߽� 2.pathTransfrom 3. ���ݶ��̴� ������Ʈ �𵨸� ��ġ
public class AiCar : MonoBehaviour
{
    [Header("CenterMass")]
    [SerializeField] Rigidbody rb;
    public Vector3 CenterMass = new Vector3(0f, -0.5f, 0f);
    [Header("Path")]
    [SerializeField] Transform path;
    [SerializeField] Transform[] pathTransforms;
    [SerializeField] List<Transform> pathList;
    [Header("�ݶ��̴�")]
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
    // ���� ���ǵ�
    public float currentspeed = 0f; // ���� ���ǵ� 
    private int currentNode = 0; // ���� ���
    private float maxspeed = 150f; //  150 �̻� ���޸��� ����
    public float maxMotorTorque = 1000f; // �� �ݶ��̴��� ȸ���ϴ� �ִ� ��
    public float maxSteer = 30f; // �չ��� ȸ�� ��
  
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
    void ApplySteer()// �չ��� �� �� �ݶ��̴� �� path�� ���� ȸ�� �ϴ� �޼��� 
    {
         Vector3 relativeVector = transform.InverseTransformPoint(pathList[currentNode].position);
        // �������� ����  = ���� ��ǥ�� ���ӻ��� ��ǥ�� ������ǥ�� ��ȯ �Ѵ�.
                          // �н�Ʈ������ x�� / �н�Ʈ���� ��üũ�� *30
        float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteer;

        FL.steerAngle = newSteer;
        FR.steerAngle = newSteer;
    }


    void Drive()
    {
        currentspeed = 2 * Mathf.PI * FL.radius * FL.rpm * 60 / 1000;

        // ��ǥ ������ ������� �� �ӵ��� ���Դϴ�.
        float targetSpeed = maxspeed;
        if (Vector3.Distance(transform.position, pathList[currentNode].position) < 10f) // ��ǥ �������� 10m �̳��� ��
        {
            targetSpeed = Mathf.Lerp(currentspeed, 0f, Time.deltaTime); // �ӵ��� ���������� ���Դϴ�.
        }

        if (currentspeed < targetSpeed)
        {
            BL.motorTorque = maxMotorTorque;
            BR.motorTorque = maxMotorTorque;
        }
        else // ���� ���ǵ尡 �ְ� �ӵ����� ������
        {
            BL.motorTorque = 0;
            BR.motorTorque = 0;
        }
    }

    void CheckWayPointDistance()
    {  
            // ������ ���������Ÿ��� ���ؼ� 3.5���� ��������
        if (Vector3.Distance(transform.position, pathList[currentNode].position) < 3.5f)
        {
            
            BL.motorTorque = 10;
            BR.motorTorque = 10;
            Debug.Log(BL.motorTorque);
            // ������ ���� ������ �ٽ� 0���� �ʱ�ȭ
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

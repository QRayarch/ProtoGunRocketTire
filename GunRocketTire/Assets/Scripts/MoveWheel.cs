using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class MoveWheel : MonoBehaviour
{

    [Header("Setup")]
    public Camera cam;
    public WheelCollider wheel;
    public Transform wheelGraphic;
    public Transform cannnon;
    public GameObject projectile;

    [Header("Config")]
    public float scale = 3.0f;
    public float torque;
    public float angle;
    public float shootTime = 1.0f;

    private Rigidbody body;
    private Quaternion startWheelRotation;
    private float shootTimer = 0;

    // Use this for initialization
    void Start()
    {
        body = GetComponent<Rigidbody>();
        if(wheelGraphic != null)
        {
            startWheelRotation = wheelGraphic.transform.rotation;
        }
    }

    // Update is called once per frame
    void Update()
    {
        shootTimer += Time.deltaTime;
        Vector3 upVector = transform.forward;
        upVector.y = 0;
        Quaternion rotateUp = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(upVector), scale * Time.deltaTime);
        /*Vector3 angles = rotateUp.eulerAngles;
        angles.x *= scale;
        angles.z *= scale;
        rotateUp = Quaternion.Euler(angles);*/
        RaycastHit hit = new RaycastHit();
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Vector3 forward = ray.direction;
        if (Physics.Raycast(ray, out hit,  400))
        {
            forward = hit.point - cannnon.position;
        }
        cannnon.rotation = Quaternion.LookRotation(forward);
        transform.rotation = rotateUp;

        if(Input.GetMouseButton(0))
        {
            if(shootTimer >= shootTime)
            {
                shootTime = 0;
                Instantiate(projectile, cannnon.position + cannnon.transform.forward * 1.0f, cannnon.rotation);
            }
        }
    }

    public void FixedUpdate()
    {
        Vector2 input = new Vector2(Input.GetAxis("H"), Input.GetAxis("V"));
        Vector3 movePos = transform.forward * torque * -input.y;
        //body.MovePosition(transform.position + movePos);
        //body.AddForce(movePos, ForceMode.Impulse);
        Vector3 turnAngle = transform.up * input.x * angle;
        body.AddTorque(turnAngle, ForceMode.Force);

        if (wheel == null) return;
        float motor = torque * -input.y;
        float steering = angle * input.x;
        //Debug.Log(motor + " " + steering);
        //wheel.steerAngle = steering;
        Debug.Log(Mathf.Abs(input.y));
        if(Mathf.Abs(input.y) < 0.03f)
        {
            wheel.brakeTorque = torque;
            wheel.motorTorque = 0;
        } else
        {
            wheel.brakeTorque = 0;
            wheel.motorTorque = motor;
        }
    }
}

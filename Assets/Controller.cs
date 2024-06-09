using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public float maxRPM = 255f; // Maximum RPM of the wheels
    public Transform leftWheel;
    public Transform rightWheel;
    public float wheelSeparation = 0.131f; // Distance between the left and right wheels
    public float speedMultiplier = 0.1f; // Multiplier to adjust the movement speed

    [Range(0, 255)] public int LeftWheelRPM, RightWheelRPM;
    
    public TMP_Text leftRPMText;
    public TMP_Text rightRPMText;
    public TMP_Text leftLinearVelText;
    public TMP_Text rightLinearVelText;
    public TMP_Text linearVelText;
    public TMP_Text angularVelText;
    
    public GameObject sphere;
    private Vector3 _lastPosLeft;
    public Transform _sphereParent;
    private float _time;
    private Vector3 _oldPosLeft;
    private List<Vector3> _pointListMiddle = new List<Vector3>();
    public LineRenderer ln,ln2,ln3;
    private float _robotAngle;
    private Vector3 _robotPosition;
    private bool _firstMessage = true;
    private bool _pointsHaveChangeed;

    private void Start()
    {
        _lastPosLeft = transform.position - new Vector3(0, 0, 0);
        _pointListMiddle.Add(_lastPosLeft);
    }

    public void ResetPos()
    {
        _pointListMiddle.Clear();
    }

    void Update()
    {
        // // Example: Get RPM data from the real-life robot (replace with your actual method)
        // float leftWheelRPM = LeftWheelRPM / 60;
        // float rightWheelRPM = RightWheelRPM / 60;
        //
        // float WheelDia = 65f;
        //
        // float WheelCircumference = Mathf.PI * (WheelDia*0.001f);
        //
        // float LeftLinearVel = (leftWheelRPM * WheelCircumference);
        // float RightLinearVel = (rightWheelRPM * WheelCircumference);
        //
        // // float leftLinearVel = (2f* Mathf.PI * (26f/2) * (leftWheelRPM / 60))/1000;
        // // float rightLinearVel = (2f* Mathf.PI * (26f/2) * (rightWheelRPM / 60))/1000;
        // //
        // float linearVel = (LeftLinearVel + RightLinearVel) / 2;
        // float angularVel = (LeftLinearVel - RightLinearVel) / wheelSeparation;
        // //
        // // // Move the object forward based on the average linear velocity
        // transform.Translate(Vector3.forward * linearVel * Time.deltaTime * speedMultiplier);
        // //
        // // // Rotate the object based on angular velocity for turning
        // transform.Rotate(Vector3.up, angularVel * Time.deltaTime * Mathf.Rad2Deg);
        // //
        // // // Display the RPM values on the screen
        // leftRPMText.text =  "Left RPM:   " + leftWheelRPM;
        // rightRPMText.text = "Right RPM: " + rightWheelRPM;
        // //
        // // // Display the linear velocity values on the screen
        // leftLinearVelText.text =  "Left Linear Vel:   " + LeftLinearVel + " m/s";
        // rightLinearVelText.text = "Right Linear Vel: " + RightLinearVel + " m/s";
        // //
        // // // Display the linear velocity values on the screen
        // linearVelText.text =  "Linear Vel:   " + linearVel + " m/s";
        // angularVelText.text = "Angular Vel: " + angularVel * Mathf.Rad2Deg + " deg/s";


        // // Convert RPM to angular velocity (degrees per second)
        // float leftAngularVelocity = leftWheelRPM / 60f * 360f;
        // float rightAngularVelocity = rightWheelRPM / 60f * 360f;
        //
        // // Calculate linear velocity of the robot
        // float linearVelocity = (leftAngularVelocity + rightAngularVelocity) * Mathf.PI * wheelSeparation / 360f;
        //
        // // Move the robot based on linear velocity
        // transform.Translate(transform.forward * linearVelocity * speedMultiplier * Time.deltaTime, Space.World);

        // // Rotate the wheels based on RPM
        // RotateWheel(leftWheel, leftAngularVelocity);
        // RotateWheel(rightWheel, rightAngularVelocity);
        
        // Instantiate sphere every 0.25 m travelled
        // if (Vector3.Distance(_lastPos, transform.position) > 0.5f*0.1f)
        // {
        //     var position = transform.position;
        //     // instantiate sphere with position and rotation of the robot
        //     Instantiate(sphere, position, transform.rotation, _sphereParent);
        //     
        //     _lastPos = position;
        // }
        _time += Time.deltaTime;
        if (_pointsHaveChangeed)
        {
            UpdateLine();
            _pointsHaveChangeed = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(_oldPosLeft, 0.1f);
    }

    public void UpdateMovement(int leftRPM, int rightRPM)
    {
        if (_firstMessage)
        {
            _time = 0;
            _firstMessage = false;
            return;
        }
        float leftRPS = leftRPM / 60f;
        float rightRPS = rightRPM / 60f;
        
        float revolutionsLeft = leftRPS * _time;
        float revolutionsRight = rightRPS * _time;
        
        float distanceLeft = revolutionsLeft * 2 * Mathf.PI * 0.065f;
        float distanceRight = revolutionsRight * 2 * Mathf.PI * 0.065f;
        
        float distance = (distanceLeft + distanceRight) / 2;
        
        _robotAngle += (distanceRight - distanceLeft) / wheelSeparation;
        
        _robotPosition += new Vector3(distance * Mathf.Cos(_robotAngle), 0, distance * Mathf.Sin(_robotAngle));
        
        _pointListMiddle.Add(_robotPosition);
        
        // new position of left wheel after moving
        // var movedPos = new Vector3(distance * Mathf.Cos(angle), 0, distance * Mathf.Sin(angle));
        // var newPos = _oldPos + movedPos;
        // _pointList.Add(newPos);
        // _oldPos = newPos;

        _time = 0;
        
        _pointsHaveChangeed = true;
    }

    private void UpdateLine()
    {
        // var newLine = _pointListMiddle.Select(point => point + new Vector3(0, 0, (wheelSeparation / 2))).ToList();
        // ln.positionCount = newLine.Count;
        // ln.SetPositions(newLine.ToArray());
        //
        // var newLine2 = _pointListMiddle.Select(point => point - new Vector3(0, 0, (wheelSeparation / 2))).ToList();
        // ln2.positionCount = newLine2.Count;
        // ln2.SetPositions(newLine2.ToArray());
        
        ln3.positionCount = _pointListMiddle.Count;
        ln3.SetPositions(_pointListMiddle.ToArray());
        
    }
}
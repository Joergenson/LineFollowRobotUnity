using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public float maxRPM = 255f; // Maximum RPM of the wheels
    public Transform leftWheel;
    public Transform rightWheel;
    public float wheelSeparation = 0.131f; // Distance between the left and right wheels
    public float speedMultiplier = 0.1f; // Multiplier to adjust the movement speed

    [Range(100, 255)] public float LeftWheelRPM, RightWheelRPM;
    
    public TMP_Text leftRPMText;
    public TMP_Text rightRPMText;
    public TMP_Text leftLinearVelText;
    public TMP_Text rightLinearVelText;
    public TMP_Text linearVelText;
    public TMP_Text angularVelText;
    
    public GameObject sphere;
    private Vector3 _lastPos;
    public Transform _sphereParent;

    private void Start()
    {
        _lastPos = transform.position;
    }

    void Update()
    {
        // Example: Get RPM data from the real-life robot (replace with your actual method)
        float leftWheelRPM = LeftWheelRPM / 60;
        float rightWheelRPM = RightWheelRPM / 60;
        
        float WheelDia = 65f;
        
        float WheelCircumference = Mathf.PI * (WheelDia*0.001f);

        float LeftLinearVel = (leftWheelRPM * WheelCircumference);
        float RightLinearVel = (rightWheelRPM * WheelCircumference);

        // float leftLinearVel = (2f* Mathf.PI * (26f/2) * (leftWheelRPM / 60))/1000;
        // float rightLinearVel = (2f* Mathf.PI * (26f/2) * (rightWheelRPM / 60))/1000;
        //
        float linearVel = (LeftLinearVel + RightLinearVel) / 2;
        float angularVel = (LeftLinearVel - RightLinearVel) / wheelSeparation;
        //
        // // Move the object forward based on the average linear velocity
        transform.Translate(Vector3.forward * linearVel * Time.deltaTime * speedMultiplier);
        //
        // // Rotate the object based on angular velocity for turning
        transform.Rotate(Vector3.up, angularVel * Time.deltaTime * Mathf.Rad2Deg);
        //
        // // Display the RPM values on the screen
        leftRPMText.text =  "Left RPM:   " + leftWheelRPM;
        rightRPMText.text = "Right RPM: " + rightWheelRPM;
        //
        // // Display the linear velocity values on the screen
        leftLinearVelText.text =  "Left Linear Vel:   " + LeftLinearVel + " m/s";
        rightLinearVelText.text = "Right Linear Vel: " + RightLinearVel + " m/s";
        //
        // // Display the linear velocity values on the screen
        linearVelText.text =  "Linear Vel:   " + linearVel + " m/s";
        angularVelText.text = "Angular Vel: " + angularVel * Mathf.Rad2Deg + " deg/s";


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
        if (Vector3.Distance(_lastPos, transform.position) > 0.5f*0.1f)
        {
            var position = transform.position;
            // instantiate sphere with position and rotation of the robot
            Instantiate(sphere, position, transform.rotation, _sphereParent);
            
            _lastPos = position;
        }
        
    }
}
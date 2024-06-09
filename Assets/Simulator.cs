
// This class represents a simulator and inherits from MonoBehaviour.

using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Simulator : MonoBehaviour
{
    // List of positions and their corresponding speeds.
    List<(Vector3, float)> positions = new();

    // Public variables for speed, current distance, current position, and start time.
    public float speed = 1;
    public float currentDistance = 0;
    public int currentPos = 0;
    public float startTime = 0;

    // Private variables for time, test, line renderer, positions, and rotation.
    private float _time;
    [SerializeField] private float test;
    [SerializeField] private LineRenderer _lineRenderer;
    private List<Vector3> _poisitons = new List<Vector3>();
    private Quaternion _rotattion;

    // Public variables for displaying RPM and linear velocity.
    public TMP_Text leftRPMText;
    public TMP_Text rightRPMText;
    public TMP_Text leftLinearVelText;
    public TMP_Text rightLinearVelText;
    public TMP_Text linearVelText;
    public TMP_Text angularVelText;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the positions list with predefined positions and speeds.
        positions = new List<(Vector3, float)>()
        {
            (new Vector3(10, 0, 2), 1),
            // ... (other positions)
            (new Vector3(8,0, 0), 1),
        };

        // Set the initial values for current position, start time, speed, and transform position.
        currentPos = 0;
        startTime = Time.time;
        speed = positions[currentPos].Item2;
        transform.position = positions[currentPos].Item1;

        // Add the initial position to the positions list for line rendering.
        _poisitons.Add(transform.position);

        // Invoke the CharterLine method repeatedly with a delay of 0.1 seconds.
        InvokeRepeating(nameof(CharterLine), 0.1f, 1);

        // Invoke the SetRPMFromPositions method repeatedly with a delay of 0.1 seconds.
        InvokeRepeating(nameof(SetRPMFromPositions), 0.1f, 1);
    }

    // Add the current position to the positions list for line rendering.
    private void CharterLine()
    {
        _poisitons.Add(transform.position);
        _lineRenderer.positionCount = _poisitons.Count;
        _lineRenderer.SetPositions(_poisitons.ToArray());
    }

    // Update is called once per frame
    void Update()
    {
        // If the current position is the last position, return.
        if (currentPos == 10) return;

        // Calculate the elapsed time since the start.
        _time = Time.time - startTime;

        // Calculate the distance traveled based on time and speed.
        float dist = _time * speed;

        // Calculate the interpolation factor based on the distance traveled and current distance.
        float f = dist > 0 ? dist / currentDistance : 0;

        // If the interpolation factor is greater than or equal to 1, move to the next position.
        if (f >= 1)
        {
            // If the current position is the second-to-last position, set the current position to the last position and return.
            if (currentPos + 1 == positions.Count - 1)
            {
                currentPos = 10;
                return;
            }

            // Move to the next position in a circular manner.
            currentPos = ++currentPos % positions.Count;

            // Update the start time, current distance, speed, and rotation based on the new position.
            startTime = Time.time;
            currentDistance = Vector3.Distance(positions[currentPos].Item1, positions[(currentPos + 1) % positions.Count].Item1);
            speed = currentDistance / positions[currentPos].Item2;
            _rotattion = Quaternion.LookRotation(positions[(currentPos + 1) % positions.Count].Item1 - positions[currentPos].Item1);
        }

        // Interpolate the position between the current position and the next position based on the interpolation factor.
        transform.position = Vector3.Lerp(positions[currentPos].Item1, positions[(currentPos + 1) % positions.Count].Item1, f);

        // Set the rotation to the calculated rotation.
        transform.rotation = _rotattion;
    }

    // Calculate and display RPM and linear velocity based on the current position.
    public void SetRPMFromPositions()
    {
        // If the current position is the last position, return.
        if (currentPos == 10) return;

        // Calculate the total distance.
        float distance = currentDistance;

        // Divide the total distance into left and right distances.
        float distanceLeft = distance / 2;
        float distanceRight = distance / 2;

        // Calculate the number of revolutions for the left and right wheels.
        float revolutionsLeft = distanceLeft / (2 * Mathf.PI * 0.065f);
        float revolutionsRight = distanceRight / (2 * Mathf.PI * 0.065f);

        // Calculate the revolutions per second for the left and right wheels.
        float leftRPS = revolutionsLeft / test;
        float rightRPS = revolutionsRight / test;

        // Calculate the RPM for the left and right wheels.
        float leftRPM = leftRPS * 60;
        float rightRPM = rightRPS * 60;

        // Add random variations to the RPM values.
        leftRPM += Random.Range(-5.0f, 100.3f);
        rightRPM += Random.Range(-5.0f, 50.3f);

        // Set the text values for left and right RPM.
        leftRPMText.text = "Left RPM: " + leftRPM;
        rightRPMText.text = "Right RPM: " + rightRPM;

        // Calculate the linear velocity for the left and right wheels.
        float leftLinearVel = leftRPS * 0.065f * 2 * Mathf.PI;
        float rightLinearVel = rightRPS * 0.065f * 2 * Mathf.PI;

        // Add random variations to the linear velocity values.
        leftLinearVel += Random.Range(-0.5f, 0.5f);
        rightLinearVel += Random.Range(-0.5f, 0.5f);

        // Set the text values for average left and right linear velocity.
        leftLinearVelText.text = "Avg Left Linear Velocity: " + leftLinearVel + " m/s";
        rightLinearVelText.text = "Avg Right Linear Velocity: " + rightLinearVel + " m/s";

        // Calculate and set the text value for linear velocity.
        linearVelText.text = "Linear Velocity: " + (leftLinearVel + rightLinearVel) / 2 + " m/s";

        // Calculate and set the text value for angular velocity.
        angularVelText.text = "Angular Velocity: " + (leftLinearVel - rightLinearVel) / 0.13f + " deg/s";
    }
}
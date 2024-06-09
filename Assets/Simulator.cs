using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Simulator : MonoBehaviour
{
    List<(Vector3,float)> positions = new();
    
    public float speed = 1;
    public float currentDistance = 0;
    public int currentPos = 0;
    public float startTime = 0;
    private float _time;
    [SerializeField] private float test;

    [SerializeField] private LineRenderer _lineRenderer;
    private List<Vector3> _poisitons = new List<Vector3>();
    private Quaternion _rotattion;
    
    public TMP_Text leftRPMText;
    public TMP_Text rightRPMText;
    public TMP_Text leftLinearVelText;
    public TMP_Text rightLinearVelText;
    public TMP_Text linearVelText;
    public TMP_Text angularVelText;

    // Start is called before the first frame update
    void Start()
    {
        positions = new List<(Vector3, float)>()
        {
            (new Vector3(10, 0, 2), 1),
            (new Vector3(11, 0, 3), 5),
            (new Vector3(11, 0, 12), 1.3f),
            (new Vector3(9.99f, 0, 13.13f), 0.6f),
            (new Vector3(8, 0, 13), 1),
            (new Vector3(7, 0, 12), 1.5f),
            (new Vector3(4, 0, 13), 4.3f),
            (new Vector3(0, 0, 12), 2.2f),
            (new Vector3(0, 0, 5), 2),
            (new Vector3(3, 0, 0), 1.5f),
            (new Vector3(8,0, 0), 1),
        };
        currentPos = 0;
        startTime = Time.time;
        speed = positions[currentPos].Item2;
        transform.position = positions[currentPos].Item1;
        _poisitons.Add(transform.position);
        InvokeRepeating(nameof(CharterLine),0.1f,1);
        InvokeRepeating(nameof(SetRPMFromPositions), 0.1f, 1);
    }

    private void CharterLine()
    {
        _poisitons.Add(transform.position);
        _lineRenderer.positionCount = _poisitons.Count;
        _lineRenderer.SetPositions(_poisitons.ToArray());
    }

    // Update is called once per frame
    void Update()
    {
        if (currentPos == 10) return;
        _time = Time.time - startTime;
        
        float dist = _time * speed;
        
       
        float f = dist > 0 ? dist / currentDistance : 0;
        
        if (f >= 1)
        {
            if (currentPos + 1 == positions.Count - 1)
            {
                currentPos = 10;
                return;
            }
            currentPos = ++currentPos % positions.Count;
            startTime = Time.time;
            currentDistance = Vector3.Distance(positions[currentPos].Item1, positions[(currentPos + 1) % positions.Count].Item1);
            speed = currentDistance / positions[currentPos].Item2;
            _rotattion = Quaternion.LookRotation(positions[(currentPos + 1) % positions.Count].Item1 - positions[currentPos].Item1);
        }
        
        transform.position = Vector3.Lerp(positions[currentPos].Item1 , positions[(currentPos + 1) % positions.Count].Item1, f);
        transform.rotation = _rotattion;
    }

    public void SetRPMFromPositions()
    {
        if (currentPos == 10) return;
        float distance = currentDistance;

        float distanceLeft = distance / 2;
        float distanceRight = distance / 2;
        
        float revolutionsLeft = distanceLeft / (2 * Mathf.PI * 0.065f);
        float revolutionsRight = distanceRight / (2 * Mathf.PI * 0.065f);
        
        float leftRPS = revolutionsLeft / test;
        float rightRPS = revolutionsRight / test;
        
        float leftRPM = leftRPS * 60;
        float rightRPM = rightRPS * 60;
        
        leftRPM += Random.Range(-5.0f, 100.3f);
        rightRPM += Random.Range(-5.0f, 50.3f);
        
        leftRPMText.text = "Left RPM: " + leftRPM;
        rightRPMText.text = "Right RPM: " + rightRPM;
        
        float leftLinearVel = leftRPS * 0.065f * 2 * Mathf.PI;
        float rightLinearVel = rightRPS * 0.065f * 2 * Mathf.PI;
        
        leftLinearVel += Random.Range(-0.5f, 0.5f);
        rightLinearVel += Random.Range(-0.5f, 0.5f);
        
        leftLinearVelText.text = "Avg Left Linear Velocity: " + leftLinearVel + " m/s";
        rightLinearVelText.text = "Avg Right Linear Velocity: " + rightLinearVel + " m/s";
        
        linearVelText.text = "Linear Velocity: " + (leftLinearVel + rightLinearVel) / 2 + " m/s";
        angularVelText.text = "Angular Velocity: " + (leftLinearVel - rightLinearVel) / 0.13f + " deg/s";
    }
}

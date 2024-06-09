using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class Controller : MonoBehaviour
{
    public float maxRPM = 255f; // Maximum RPM of the wheels
    public float wheelSeparation = 0.131f; // Distance between the left and right wheels
    public float speedMultiplier = 0.1f; // Multiplier to adjust the movement speed

    [Range(0, 255)] public int LeftWheelRPM, RightWheelRPM;
    
    public TMP_Text leftRPMText;
    public TMP_Text rightRPMText;
    public TMP_Text leftLinearVelText;
    public TMP_Text rightLinearVelText;
    public TMP_Text linearVelText;
    public TMP_Text angularVelText;
    
    private Vector3 _lastPosLeft;
    private float _time;
    private Vector3 _oldPosLeft;
    private List<Vector3> _pointListMiddle = new List<Vector3>();
    public LineRenderer line;
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
        _time += Time.deltaTime;
        if (_pointsHaveChangeed)
        {
            UpdateLine();
            _pointsHaveChangeed = false;
            transform.position = _robotPosition;
            transform.rotation = Quaternion.Euler(0, _robotAngle * Mathf.Rad2Deg, 0);
        }
    }

    // private void OnDrawGizmos()
    // {
    //     Gizmos.DrawSphere(_oldPosLeft, 0.1f);
    // }

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

        _time = 0;
        
        _pointsHaveChangeed = true;
        
        leftRPMText.text = leftRPM.ToString();
        rightRPMText.text = rightRPM.ToString();
        leftLinearVelText.text = (leftRPS * 0.065f * Mathf.PI).ToString();
        rightLinearVelText.text = (rightRPS * 0.065f * Mathf.PI).ToString();
        linearVelText.text = ((leftRPS + rightRPS) * 0.065f * Mathf.PI / 2).ToString();
        angularVelText.text = ((rightRPS - leftRPS) * 0.065f / wheelSeparation).ToString();
    }

    private void UpdateLine()
    {
        line.positionCount = _pointListMiddle.Count;
        line.SetPositions(_pointListMiddle.ToArray());
        
    }
}
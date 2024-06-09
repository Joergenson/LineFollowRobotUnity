using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UDPManager : MonoBehaviour
{
    // Static variable that holds the instance
    public static UDPManager Instance { get; private set; }

    // UDP Settings
    [Header("UDP Settings")] [SerializeField]
    private int UDPPort = 50195;

    [SerializeField] private bool displayUDPMessages = false;
    private UdpClient udpClient;
    private IPEndPoint endPoint;
    private string _ip = "192.168.87.56";
    private float _startTime;
    private int _count;
    private float _speed;
    private int _value;
    private float _endTime;
    private bool _toggleStop;

    [SerializeField] private TMP_InputField _leftSensorThreshold;
    [SerializeField] private TMP_InputField _rightSensorThreshold;
    [SerializeField] private Button _calibrateThresholdLeft;
    [SerializeField] private Button _calibrateThresholdRight;

    private Controller _controller;


    // ESP32 Sensor
    public int potentiometerValue { get; private set; } = 0;

    void Awake()
    {
        _controller = GetComponent<Controller>();
        // Assign the instance to this instance, if it is the first one
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        _leftSensorThreshold.onEndEdit.AddListener(OnLeftThresholdChanged);
        _rightSensorThreshold.onEndEdit.AddListener(OnRightThresholdChanged);
        _calibrateThresholdLeft.onClick.AddListener(OnCalibrateThresholdLeft);
        _calibrateThresholdRight.onClick.AddListener(OnCalibrateThresholdRight);
    }

    private void OnCalibrateThresholdRight()
    {
        SendUDPMessage("Unity|" + 10, _ip, 3002);
    }

    private void OnCalibrateThresholdLeft()
    {
        SendUDPMessage("Unity|" + 11, _ip, 3002);
    }

    private void OnRightThresholdChanged(string arg0)
    {
        SendUDPMessage("TresR|" + _rightSensorThreshold.text, _ip, 3002);
    }

    private void OnLeftThresholdChanged(string arg0)
    {
        SendUDPMessage("TresL|" + _leftSensorThreshold.text, _ip, 3002);
    }

    // Start is called before the first frame update
    void Start()
    {
        //Get IP Address
        DisplayIPAddress();

        // UDP begin
        endPoint = new IPEndPoint(IPAddress.Any, UDPPort);
        udpClient = new UdpClient(endPoint);
        udpClient.BeginReceive(ReceiveCallback, null);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SendUDPMessage("Unity|" + (_toggleStop ? 1 : 0), _ip, 3002);
            _toggleStop = !_toggleStop;
            _controller.ResetPos();
        }
    }

    private void ReceiveCallback(IAsyncResult result)
    {
        byte[] receivedBytes = udpClient.EndReceive(result, ref endPoint);
        string receivedData = Encoding.UTF8.GetString(receivedBytes);

        // Log UDP message
        if (displayUDPMessages)
        {
            Debug.Log("Received data from " + endPoint.Address.ToString() + ": " + receivedData);
        }

        // Splitting the receivedData string by the '|' character


        //
        if (receivedData.Contains("|"))
        {
            string[] data = receivedData.Split('|');

            int.TryParse(data[1], out int value2);

            if (data[0] == "leftS")
            {
                Debug.Log("Left Sensor: " + value2);
            }
            else if (data[0] == "rightS")
            {
                Debug.Log("Right Sensor: " + value2);
            }
            else
            {
                int.TryParse(data[0], out int value);
                if (value > 0 || value2 > 0)
                {
                    Debug.Log("RPM: " + value + " : " + value2);
                }

                try
                {
                    _controller.UpdateMovement(value,value2);
                }
                catch (Exception e)
                {
                    Debug.Log("Error: " + e.Message);
                    throw;
                }
               
            }
        }


        // _value = value;

        udpClient.BeginReceive(ReceiveCallback, null);
    }

    // Function to send UDP message
    public void SendUDPMessage(string message, string ipAddress, int port)
    {
        UdpClient client = new UdpClient();
        try
        {
            // Convert the message string to bytes
            byte[] data = Encoding.UTF8.GetBytes(message);

            // Send the UDP message
            client.Send(data, data.Length, ipAddress, port);
            Debug.Log("UDP message sent: " + message);
        }
        catch (Exception e)
        {
            Debug.LogError("Error sending UDP message: " + e.Message);
        }
        finally
        {
            client.Close();
        }
    }

    void DisplayIPAddress()
    {
        try
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    Debug.Log(ip.ToString());
                    break;
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error fetching local IP address: " + ex.Message);
        }
    }

    private void OnDestroy()
    {
        if (udpClient != null)
        {
            udpClient.Close();
        }
    }
}
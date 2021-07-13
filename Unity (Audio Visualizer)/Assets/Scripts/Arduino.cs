using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;
using System.Threading;

public class Arduino : MonoBehaviour
{
    [SerializeField]
    int baud = 9600;
    public int Baud
    {
        get
        {
            return baud;
        }

        set
        {
            baud = value;
        }
    }

    [SerializeField]
    GameObject failText;

    public SerialPort serialPort;
    Dropdown serialPortDropdown;
    public string[] serialPorts;
    GUIStyle style = new GUIStyle();
    AudioVisualizer soundVisual;

    void Start()
    {
        serialPortDropdown = GetComponent<Dropdown>();
        if (serialPortDropdown == null)
            Debug.LogError("ArduinoController: This object is not a Dropdown!");
        style.normal.textColor = Color.green;
        StartCoroutine(FindUpdate());
    }

    public void Send(string Data)
    {
        if(serialPort != null && serialPort.IsOpen)
        { serialPort.Write(Data); }
    }
    public void Send(float Data)
    {
        if (serialPort != null && serialPort.IsOpen)
        { serialPort.Write(Data.ToString()); }
    }

    public void SendLine(string Data)
    {
        if (serialPort != null && serialPort.IsOpen)
        { serialPort.WriteLine(Data); }
    }
    public void SendLine(float Data)
    {
        if (serialPort != null && serialPort.IsOpen)
        { serialPort.WriteLine(Data.ToString()); }
    }

    public void FindArudino()
    {
        serialPorts = SerialPort.GetPortNames();
        serialPortDropdown.ClearOptions();
        serialPortDropdown.AddOptions(new List<string> {"Not Connected"});
        foreach (string s in serialPorts)
        {
            serialPortDropdown.AddOptions(new List<string>{s});//Lists all the connected serial ports
        }
        if (serialPort != null && serialPort.IsOpen)//if arduino exists
        {
            for (int i = 0; i < serialPorts.Length; i++)
            {
                if (serialPorts[i] == serialPort.PortName) { serialPortDropdown.value = i + 1; }
            }
        }
        else
        { serialPortDropdown.value = 0; }
    }

    public void Connect(int value)
    {
        if (value == 0 && serialPort != null) { CloseArduino(); serialPort = null; }
        else
        {
            for (int i = 0; i < serialPorts.Length; i++)
            {
                if (value == i + 1) { OpenConnection(serialPorts[i]); break; }
            }
        }
    }

    public void OpenConnection(string portName)
    {
        serialPort = new SerialPort(portName, baud);
        if (serialPort != null)
        {
            if (serialPort.IsOpen)
            {
                CloseArduino();
            }
            else
            {
                try
                {
                    serialPort.Open();
                    serialPort.ReadTimeout = 8;
                    serialPort.WriteTimeout = 8;
                    Send("on");
                }
                catch
                {
                    serialPortDropdown.value = 0;
                    Fail();
                }
            }
        }
    }

    void Fail()
    {
        Instantiate(failText, transform.parent);
    }

    public void CloseArduino()
    {
        Send("0");
        serialPort.Close();
    }
    void OnApplicationQuit()
    { if (serialPort != null) { if (serialPort.IsOpen) { CloseArduino(); } } }
    IEnumerator FindUpdate()
    {
        yield return new WaitForSeconds(0.01f);
        FindArudino();
        StartCoroutine(FindUpdate());
    }
    public IEnumerator DelaySend(float Seconds, string message)
    {
        yield return new WaitForSeconds(Seconds);
        Send(message);
    }
}
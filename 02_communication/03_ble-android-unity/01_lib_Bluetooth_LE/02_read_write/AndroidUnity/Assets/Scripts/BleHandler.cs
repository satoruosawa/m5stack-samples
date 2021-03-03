using System;
using UnityEngine.Events;
using UnityEngine;

public class BleHandler : MonoBehaviour
{
  public string deviceName = "M5Stack";
  public string serviceUUID = "2220";
  public string readCharacteristicUUID = "2221";
  public string writeCharacteristicUUID = "2222";
  [Serializable] public class StepEvent : UnityEvent<string> { }
  [SerializeField] private StepEvent readEvent = new StepEvent();

  enum States
  {
    None,
    Scan,
    Connect,
    Connected,
    Disconnect,
  }

  bool _connected = false;
  float _timeout = 0f;
  States _state = States.None;
  string _deviceAddress;
  bool _foundCharacteristicUUID = false;

  string StatusMessage
  {
    set
    {
      BluetoothLEHardwareInterface.Log(value);
    }
  }

  void Reset()
  {
    _connected = false;
    _timeout = 0f;
    _state = States.None;
    _deviceAddress = null;
    _foundCharacteristicUUID = false;
  }

  void SetState(States newState, float timeout)
  {
    _state = newState;
    _timeout = timeout;
  }

  void StartProcess()
  {
    Reset();
    BluetoothLEHardwareInterface.Initialize(true, false, () =>
    {
      SetState(States.Scan, 0.1f);
    }, (error) =>
    {
      StatusMessage = "Error during initialize: " + error;
    });
  }

  void Start()
  {
    StartProcess();
  }

  public void Reconnect()
  {
    StartProcess();
  }

  void DataReceived(byte[] bytes)
  {
    Debug.Log("DataReceived");
    Debug.Log(bytes[0]);
    string str = System.Text.Encoding.ASCII.GetString(bytes);
    readEvent.Invoke(str);
  }

  void Update()
  {
    // TODO: Refactoring
    if (_timeout > 0f)
    {
      _timeout -= Time.deltaTime;
      if (_timeout <= 0f)
      {
        _timeout = 0f;
        switch (_state)
        {
          case States.None:
            break;
          case States.Scan:
            StatusMessage = "Scanning for " + deviceName;
            BluetoothLEHardwareInterface.ScanForPeripheralsWithServices(
            null, (address, name) =>
            {
              if (name.Contains(deviceName))
              {
                StatusMessage = "Found " + name;
                BluetoothLEHardwareInterface.StopScan();
                _deviceAddress = address;
                SetState(States.Connect, 0.5f);
              }
            });
            break;
          case States.Connect:
            StatusMessage = "Connecting...";
            _foundCharacteristicUUID = false;
            BluetoothLEHardwareInterface.ConnectToPeripheral(
            _deviceAddress, null, null,
            (address, serviceUUID, characteristicUUID) =>
            {
              StatusMessage = "Connected...";
              if (IsEqual(serviceUUID, serviceUUID))
              {
                StatusMessage = "Found Service UUID";
                Debug.Log("Found Service UUID " + characteristicUUID);
                _foundCharacteristicUUID =
                  _foundCharacteristicUUID ||
                  IsEqual(characteristicUUID, characteristicUUID);
                if (_foundCharacteristicUUID)
                {
                  _connected = true;
                  SetState(States.Connected, 2f);
                }
              }
            }, (disconnectAddress) =>
            {
              StatusMessage = "Disconnected";
              Reset();
              SetState(States.Scan, 1f);
            });
            break;
          case States.Disconnect:
            StatusMessage = "Commanded disconnect.";
            if (_connected)
            {
              BluetoothLEHardwareInterface.DisconnectPeripheral(
              _deviceAddress, (address) =>
              {
                StatusMessage = "Device disconnected";
                BluetoothLEHardwareInterface.DeInitialize(() =>
  {
    _connected = false;
    _state = States.None;
  });
              });
            }
            else
            {
              BluetoothLEHardwareInterface.DeInitialize(() =>
              {
                _state = States.None;
              });
            }
            break;
        }
      }
    }
  }

  string FullUUID(string uuid)
  {
    string fullUUID = uuid;
    if (fullUUID.Length == 4)
      fullUUID = "0000" + uuid + "-0000-1000-8000-00805f9b34fb";
    return fullUUID;
  }

  bool IsEqual(string uuid1, string uuid2)
  {
    uuid1 = FullUUID(uuid1);
    uuid2 = FullUUID(uuid2);
    return (uuid1.ToUpper().Equals(uuid2.ToUpper()));
  }

  public void ReadByte()
  {
    if (!_connected) return;
    Debug.Log("Read bytes");
    BluetoothLEHardwareInterface.ReadCharacteristic(
    _deviceAddress, serviceUUID, readCharacteristicUUID,
    (characteristic, bytes) =>
    {
      BluetoothLEHardwareInterface.Log("Read Succeeded");
      string str = System.Text.Encoding.ASCII.GetString(bytes);
      readEvent.Invoke(str);
    });
  }

  public void SendByte(string value)
  {
    // TODO: Connect input field
    if (!_connected) return;
    Debug.Log("Send bytes");
    byte[] data = System.Text.Encoding.ASCII.GetBytes(value);
    Debug.Log(data);
    BluetoothLEHardwareInterface.WriteCharacteristic(
    _deviceAddress, serviceUUID, writeCharacteristicUUID, data, data.Length,
    true, (characteristicUUID) =>
    {
      BluetoothLEHardwareInterface.Log("Write Succeeded");
    });
  }
}

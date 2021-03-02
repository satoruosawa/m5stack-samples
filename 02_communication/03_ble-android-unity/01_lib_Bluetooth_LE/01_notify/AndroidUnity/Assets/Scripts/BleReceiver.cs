using UnityEngine.Events;
using UnityEngine;

public class BleReceiver : MonoBehaviour
{
  public string deviceName = "M5Stack";
  public string serviceUUID = "2220";
  public string characteristicUUID = "2221";
  [SerializeField] private UnityEvent receiveEvent = new UnityEvent();

  enum States
  {
    None,
    Scan,
    ScanRSSI,
    Connect,
    RequestMTU,
    Subscribe,
    Unsubscribe,
    Disconnect,
  }

  bool _connected = false;
  float _timeout = 0f;
  States _state = States.None;
  string _deviceAddress;
  bool _foundCharacteristicUUID = false;
  bool _rssiOnly = false;
  int _rssi = 0;

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
    _rssi = 0;
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
    receiveEvent.Invoke();
  }

  void Update()
  {
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
                if (!_rssiOnly)
                {
                  if (name.Contains(deviceName))
                  {
                    StatusMessage = "Found " + name;
                    BluetoothLEHardwareInterface.StopScan();
                    _deviceAddress = address;
                    SetState(States.Connect, 0.5f);
                  }
                }
              }, (address, name, rssi, bytes) =>
              {
                if (name.Contains(deviceName))
                {
                  StatusMessage = "Found " + name;
                  if (_rssiOnly)
                  {
                    _rssi = rssi;
                  }
                  else
                  {
                    BluetoothLEHardwareInterface.StopScan();
                    _deviceAddress = address;
                    SetState(States.Connect, 0.5f);
                  }
                }
              }, _rssiOnly);
            if (_rssiOnly)
              SetState(States.ScanRSSI, 0.5f);
            break;
          case States.ScanRSSI:
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
                  _foundCharacteristicUUID = _foundCharacteristicUUID || IsEqual(characteristicUUID, characteristicUUID);
                  if (_foundCharacteristicUUID)
                  {
                    _connected = true;
                    SetState(States.RequestMTU, 2f);
                  }
                }
              });
            break;
          case States.RequestMTU:
            StatusMessage = "Requesting MTU";
            BluetoothLEHardwareInterface.RequestMtu(_deviceAddress, 185,
              (address, newMTU) =>
            {
              StatusMessage = "MTU set to " + newMTU.ToString();
              SetState(States.Subscribe, 0.1f);
            });
            break;
          case States.Subscribe:
            StatusMessage = "Subscribing to characteristics...";
            BluetoothLEHardwareInterface
              .SubscribeCharacteristicWithDeviceAddress(
                _deviceAddress, serviceUUID, characteristicUUID,
                (notifyAddress, notifyCharacteristic) =>
            {
              StatusMessage = "Waiting for user action (1)...";
              _state = States.None;
              BluetoothLEHardwareInterface.ReadCharacteristic(
                _deviceAddress, serviceUUID, characteristicUUID,
                (characteristic, bytes) =>
              {
                DataReceived(bytes);
              });
            }, (address, characteristicUUID, bytes) =>
            {
              if (_state != States.None)
              {
                StatusMessage = "Waiting for user action (2)...";
                _state = States.None;
              }
              DataReceived(bytes);
            });
            break;
          case States.Unsubscribe:
            BluetoothLEHardwareInterface.UnSubscribeCharacteristic(
              _deviceAddress, serviceUUID, characteristicUUID, null);
            SetState(States.Disconnect, 4f);
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
    if (uuid1.Length == 4)
      uuid1 = FullUUID(uuid1);
    if (uuid2.Length == 4)
      uuid2 = FullUUID(uuid2);
    return (uuid1.ToUpper().Equals(uuid2.ToUpper()));
  }
}

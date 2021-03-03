using System;
using UnityEngine.Events;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections;

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
    NotInitialized,
    NotFound,
    Scanning,
    FoundButNotConnected,
    Connected,
  }

  States state = States.NotInitialized;
  string _deviceAddress;
  bool _foundCharacteristicUUID = false;
  bool _isWaitingCallback = false;

  async void Start()
  {
    await Initialize();
    await Scan();
    await Connect();
  }

  void Reset()
  {
    _deviceAddress = null;
    _foundCharacteristicUUID = false;
  }

  async UniTask Initialize()
  {
    Reset();
    _isWaitingCallback = true;
    BluetoothLEHardwareInterface.Initialize(true, false, () =>
    {
      state = States.NotFound;
      _isWaitingCallback = false;
    }, (error) =>
    {
      Debug.Log("Error during initialize: " + error);
    });
    await WaitUntilCallback();
    await UniTask.Delay(100);
  }

  async UniTask Scan()
  {
    Debug.Log("Scanning for " + deviceName);
    _isWaitingCallback = true;
    BluetoothLEHardwareInterface.ScanForPeripheralsWithServices(
    null, (address, name) =>
    {
      if (name.Contains(deviceName))
      {
        Debug.Log("Found " + name);
        BluetoothLEHardwareInterface.StopScan();
        _deviceAddress = address;
        state = States.FoundButNotConnected;
        _isWaitingCallback = false;
      }
    });
    await WaitUntilCallback();
    await UniTask.Delay(500);
  }

  async UniTask Connect()
  {
    Debug.Log("Connecting...");
    _isWaitingCallback = true;
    _foundCharacteristicUUID = false;
    BluetoothLEHardwareInterface.ConnectToPeripheral(
    _deviceAddress, null, null,
    (address, serviceUUID, characteristicUUID) =>
    {
      Debug.Log("Connected...");
      if (IsEqual(serviceUUID, serviceUUID))
      {
        Debug.Log("Found Service UUID");
        Debug.Log("Found Service UUID " + characteristicUUID);
        _foundCharacteristicUUID =
          _foundCharacteristicUUID ||
          IsEqual(characteristicUUID, characteristicUUID);
        if (_foundCharacteristicUUID)
        {
          state = States.Connected;
          _isWaitingCallback = false;
        }
      }
    }, (disconnectAddress) =>
    {
      Debug.Log("Disconnected");
      Reset();
      _isWaitingCallback = false;
    });
    await WaitUntilCallback();
    await UniTask.Delay(2000);
  }

  IEnumerator WaitUntilCallback()
  {
    while (_isWaitingCallback)
    {
      yield return null;
    }
  }

  public void Reconnect()
  {
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
    if (state != States.Connected) return;
    Debug.Log("Read bytes");
    BluetoothLEHardwareInterface.ReadCharacteristic(
    _deviceAddress, serviceUUID, readCharacteristicUUID,
    (characteristic, bytes) =>
    {
      Debug.Log("Read Succeeded");
      string str = System.Text.Encoding.ASCII.GetString(bytes);
      readEvent.Invoke(str);
    });
  }

  public void SendByte(string value)
  {
    // TODO: Connect input field
    if (state != States.Connected) return;
    Debug.Log("Send bytes");
    byte[] data = System.Text.Encoding.ASCII.GetBytes(value);
    Debug.Log(data);
    BluetoothLEHardwareInterface.WriteCharacteristic(
    _deviceAddress, serviceUUID, writeCharacteristicUUID, data, data.Length,
    true, (characteristicUUID) =>
    {
      Debug.Log("Write Succeeded");
    });
  }
}

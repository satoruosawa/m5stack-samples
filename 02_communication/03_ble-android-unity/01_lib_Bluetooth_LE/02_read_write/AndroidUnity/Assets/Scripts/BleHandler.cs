using System;
using UnityEngine.Events;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class BleHandler : MonoBehaviour
{
  public string deviceName = "M5Stack";
  public string serviceUUID = "2220";
  public string readCharacteristicUUID = "2221";
  public string writeCharacteristicUUID = "2222";
  float scanTimeout = 10.0f; //sec
  [Serializable] public class StepEvent : UnityEvent<string> { }
  [SerializeField] StepEvent readEvent = new StepEvent();
  public enum States
  {
    NotInitialized,
    Initializing,
    InitializationError,
    NotFound,
    Scaning,
    FoundButNotConnected,
    Connecting,
    Connected,
    Disconnecting,
    Deinitializing,
    Reading,
    Writing
  }
  public States state { get; private set; }

  string deviceAddress;
  bool foundServiceUUID = false;
  bool foundReadCharacteristicUUID = false;
  bool foundWriteCharacteristicUUID = false;

  async void Start()
  {
    state = States.NotInitialized;
    deviceAddress = null;
    foundServiceUUID = false;
    foundReadCharacteristicUUID = false;
    foundWriteCharacteristicUUID = false;
    await Initialize();
    await UniTask.Delay(5000);
    await Scan();
    await UniTask.Delay(5000);
    await Connect();
  }

  void Reset()
  {
  }

  public async void OnInitialize() { await Initialize(); }

  async UniTask Initialize()
  {
    if (state != States.NotInitialized)
    {
      Debug.LogWarning("Can't initialize. State = " + state);
      return;
    }
    state = States.Initializing;
    Debug.Log("[" + Time.time + "]: Start initialize.");
    BluetoothLEHardwareInterface.Initialize(true, false, () =>
    {
      state = States.NotFound;
      Debug.Log("[" + Time.time + "]: End initialize.");
    }, (error) =>
    {
      state = States.InitializationError;
      Debug.LogError("Error during initialize: " + error);
    });
    while (IsWaitingCallback()) await UniTask.Yield(PlayerLoopTiming.Update);
  }

  public async void OnDeinitialize() { await Deinitialize(); }

  async UniTask Deinitialize()
  {
    // TODO:
  }

  public async void OnScan() { await Scan(); }

  async UniTask Scan()
  {
    if (state != States.NotFound)
    {
      Debug.LogWarning("Can't scan. State = " + state);
      return;
    }
    Debug.Log("[" + Time.time + "]: Start Scanning for " + deviceName);
    state = States.Scaning;
    BluetoothLEHardwareInterface.ScanForPeripheralsWithServices(
    null, (address, name) =>
    {
      if (name.Contains(deviceName))
      {
        BluetoothLEHardwareInterface.StopScan();
        deviceAddress = address;
        state = States.FoundButNotConnected;
        Debug.Log("[" + Time.time + "]: Found " + name);
      }
    });
    float scanStart = Time.time;
    while (IsWaitingCallback())
    {
      if (Time.time > scanStart + scanTimeout)
      {
        BluetoothLEHardwareInterface.StopScan();
        Debug.LogWarning("[" + Time.time + "]: Scan timeout.");
        break;
      }
      await UniTask.Yield(PlayerLoopTiming.Update);
    }
  }

  public async void OnConnect() { await Connect(); }

  async UniTask Connect()
  {
    if (state != States.FoundButNotConnected)
    {
      Debug.LogWarning("Can't connect. State = " + state);
      return;
    }
    state = States.Connecting;
    Debug.Log("[" + Time.time + "]: Start connecting.");
    BluetoothLEHardwareInterface.ConnectToPeripheral(
    deviceAddress, (ad) =>
    {
      state = States.Connected;
      Debug.Log("[" + Time.time + "]: Connected. Address = " + ad);
    }, (ad, su) =>
    {
      foundServiceUUID = true;
      if (IsEqual(su, serviceUUID))
        Debug.Log("[" + Time.time + "]: Found Service UUID. UUID = " + su);
    },
    (address, su, cu) =>
    {
      if (IsEqual(su, serviceUUID))
      {
        if (IsEqual(cu, readCharacteristicUUID))
        {
          foundReadCharacteristicUUID = true;
          Debug.Log(
            "[" + Time.time + "]: Found Characteristic UUID. UUID = " + cu);
        }
        else if (IsEqual(cu, writeCharacteristicUUID))
        {
          foundWriteCharacteristicUUID = true;
          Debug.Log(
            "[" + Time.time + "]: Found Characteristic UUID. UUID = " + cu);
        }
      }
    }, (disconnectAddress) =>
    {
      // TODO: Check Algorythm.
      Debug.Log("Disconnected");
      // Reset();
    });
    while (IsWaitingCallback()) await UniTask.Yield(PlayerLoopTiming.Update);
  }

  public async void OnDisconnect() { await Disconnect(); }

  async UniTask Disconnect()
  {
    if (state != States.Connected)
    {
      Debug.LogWarning("Can't disconnect. State = " + state);
      return;
    }
    state = States.Disconnecting;
    Debug.Log("[" + Time.time + "]: Start disconnecting.");
    BluetoothLEHardwareInterface.DisconnectPeripheral(
      deviceAddress, (ad) =>
      {
        state = States.FoundButNotConnected;
        Debug.Log("[" + Time.time + "]: Disconnected. Address = " + ad);
      });
    while (IsWaitingCallback()) await UniTask.Yield(PlayerLoopTiming.Update);
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

  bool IsWaitingCallback()
  {
    return state == States.Initializing || state == States.Scaning ||
      state == States.Connecting || state == States.Disconnecting ||
      state == States.Deinitializing;
  }

  public void ReadCharacteristic()
  {
    if (state != States.Connected)
    {
      Debug.LogWarning("Can't read. State = " + state);
      return;
    }
    else if (!foundReadCharacteristicUUID)
    {
      Debug.LogWarning("ReadCharacteristic is not found.");
      return;
    }
    state = States.Reading;
    Debug.Log("Read bytes");
    BluetoothLEHardwareInterface.ReadCharacteristic(
    deviceAddress, serviceUUID, readCharacteristicUUID,
    (characteristic, bytes) =>
    {
      // Read action callback doesn't work in Editor mode.
      state = States.Connected;
      Debug.Log("Read Succeeded");
      string str = System.Text.Encoding.ASCII.GetString(bytes);
      readEvent.Invoke(str);
    });
  }

  public void WriteCharacteristic(string value)
  {
    // TODO: Connect input field
    if (state != States.Connected)
    {
      Debug.LogWarning("Can't write. State = " + state);
      return;
    }
    else if (!foundWriteCharacteristicUUID)
    {
      Debug.LogWarning("WriteCharacteristic is not found.");
      return;
    }
    state = States.Writing;
    Debug.Log("Write bytes");
    byte[] data = System.Text.Encoding.ASCII.GetBytes(value);
    Debug.Log(data);
    BluetoothLEHardwareInterface.WriteCharacteristic(
    deviceAddress, serviceUUID, writeCharacteristicUUID, data, data.Length,
    true, (characteristicUUID) =>
    {
      state = States.Connected;
      Debug.Log("Write Succeeded");
    });
  }
}

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
    FoundButNotConnected,
    Connected,
  }
  bool isWaitingCallback = false;

  States state = States.NotInitialized;
  string _deviceAddress;
  bool _foundCharacteristicUUID = false;

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
    if (state != States.NotInitialized || isWaitingCallback)
    {
      string s = "Already initialized. State = " + state;
      s = isWaitingCallback ? s + " [waiting process]" : s;
      Debug.LogWarning(s);
      return;
    }
    Reset();
    isWaitingCallback = true;
    // TODO: Add timeout.
    Debug.Log("[" + Time.time + "]: Start initialize.");
    BluetoothLEHardwareInterface.Initialize(true, false, () =>
    {
      state = States.NotFound;
      isWaitingCallback = false;
      Debug.Log("[" + Time.time + "]: End initialize.");
    }, (error) =>
    {
      Debug.LogError("Error during initialize: " + error);
    });
    await WaitUntilCallback();
    await UniTask.Delay(5000);
  }

  async UniTask Scan()
  {
    if (state != States.NotFound || isWaitingCallback)
    {
      string s = "You can't sacn. State = " + state;
      s = isWaitingCallback ? s + " [waiting process]" : s;
      Debug.LogWarning(s);
      return;
    }
    Debug.Log("[" + Time.time + "]: Start Scanning for " + deviceName);
    isWaitingCallback = true;
    // TODO: Add timeout.
    BluetoothLEHardwareInterface.ScanForPeripheralsWithServices(
    null, (address, name) =>
    {
      if (name.Contains(deviceName))
      {
        Debug.Log("[" + Time.time + "]: Found " + name);
        BluetoothLEHardwareInterface.StopScan();
        _deviceAddress = address;
        state = States.FoundButNotConnected;
        isWaitingCallback = false;
      }
    });
    await WaitUntilCallback();
    await UniTask.Delay(5000);
  }

  async UniTask Connect()
  {
    if (state != States.FoundButNotConnected || isWaitingCallback)
    {
      string s = "You can't connect. State = " + state;
      s = isWaitingCallback ? s + " [waiting process]" : s;
      Debug.LogWarning(s);
      return;
    }
    isWaitingCallback = true;
    _foundCharacteristicUUID = false;
    Debug.Log("[" + Time.time + "]: Start connecting.");
    // TODO: Add timeout.
    BluetoothLEHardwareInterface.ConnectToPeripheral(
    _deviceAddress, (ad) =>
    {
      Debug.Log("[" + Time.time + "]: Connected. Address = " + ad);
      state = States.Connected;
    }, (ad, su) =>
    {
      if (IsEqual(su, serviceUUID))
        Debug.Log(
          "[" + Time.time + "]: Found Service UUID. UUID = " + su);
    },
    (address, su, cu) =>
    {
      if (IsEqual(su, serviceUUID))
      {
        if (IsEqual(cu, readCharacteristicUUID))
        {
          Debug.Log(
            "[" + Time.time + "]: Found Characteristic UUID. UUID = " + cu);
        }
        else if (IsEqual(cu, writeCharacteristicUUID))
        {
          Debug.Log(
            "[" + Time.time + "]: Found Characteristic UUID. UUID = " + cu);
        }
        // TODO: Check Algorythm.
        //   Debug.Log("Found Service UUID " + cu);
        //   _foundCharacteristicUUID =
        //     _foundCharacteristicUUID ||
        //     IsEqual(cu, characteristicUUID);
        //   if (_foundCharacteristicUUID)
        //   {
        //     state = States.Connected;
        //     isWaitingCallback = false;
        //   }
      }
    }, (disconnectAddress) =>
    {
      // TODO: Check Algorythm.
      Debug.Log("Disconnected");
      Reset();
      isWaitingCallback = false;
    });
    await WaitUntilCallback();
    await UniTask.Delay(5000);
  }

  IEnumerator WaitUntilCallback()
  {
    while (isWaitingCallback)
    {
      yield return null;
    }
  }

  async public void Reconnect()
  {
    await Initialize();
    await Scan();
    await Connect();
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

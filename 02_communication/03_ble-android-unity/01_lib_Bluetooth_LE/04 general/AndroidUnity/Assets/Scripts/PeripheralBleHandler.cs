using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using UnityEngine;

namespace M5BLE
{
  [Serializable] public class BytesEvent : UnityEvent<byte[]> { }

  public class PeripheralBleHandler : MonoBehaviour
  {
    public enum States
    {
      NotFoundPeripheral,
      Scanning,
      FoundButNotConnected,
      Connecting,
      Connected,
      Disconnecting,
      Reading,
      Writing,
      Subscribing,
      Unsubscribing
    }

    public class Service
    {
      public string uuid;
      public Dictionary<string, Characteristic> characteristics;
      public Service(string serviceUuid)
      {
        uuid = serviceUuid;
        characteristics = new Dictionary<string, Characteristic>();
      }
    }

    public class Characteristic
    {
      public string uuid;
      public bool isSubscribing;
      public Characteristic(string characteristicUuid)
      {
        uuid = characteristicUuid;
        isSubscribing = false;
      }
    }

    // References
    [SerializeField] CentralBleHandler centralBleHandler = null;
    // Settings
    public string deviceName = "M5Stack BLE Sample";
    float scanTimeout = 10.0f; //sec

    // Public variable
    public States state { get; private set; }

    // Private variables
    string deviceAddress = null;
    public Dictionary<string, Service> services { get; private set; }

    PeripheralBleHandler()
    {
      services = new Dictionary<string, Service>();
      Reset();
    }

    void Update()
    { if (!IsProcessing() && !centralBleHandler.IsInitialized()) Reset(); }

    void Reset()
    {
      state = States.NotFoundPeripheral;
      deviceAddress = null;
      services.Clear();
    }

    public async void Scan() { await ScanTask(); }

    public async UniTask ScanTask()
    {
      if (!centralBleHandler.IsInitialized())
      {
        Debug.LogWarning("<Scan> Can't scan. Central is not initialized.");
        return;
      }
      else if (state != States.NotFoundPeripheral)
      {
        // TODO: Make sure it can NOT really scan.
        Debug.LogWarning("<Scan> Can't scan. State = " + state);
        return;
      }
      state = States.Scanning;
      bool isWaitingCallback = true;
      Debug.Log("[" + Time.time + "]: <Scan> Start Scanning for " + deviceName);
      BluetoothLEHardwareInterface.ScanForPeripheralsWithServices(
      null, (address, name) =>
      {
        if (name.Contains(deviceName))
        {
          BluetoothLEHardwareInterface.StopScan();
          deviceAddress = address;
          isWaitingCallback = false;
          Debug.Log("[" + Time.time + "]: <Scan> Found " + name);
        }
      });
      float scanStart = Time.time;
      while (isWaitingCallback)
      {
        if (!centralBleHandler.IsInitialized())
        {
          BluetoothLEHardwareInterface.StopScan();
          Reset();
          Debug.LogWarning("[" + Time.time +
            "]: <Scan> Cancel scan. Central is not initialized.");
          return;
        }
        else if (state != States.Scanning)
        {
          BluetoothLEHardwareInterface.StopScan();
          Debug.LogWarning("[" + Time.time +
            "]: <Scan> Cancel scan. State changed externally.");
          return;
        }
        else if (Time.time > scanStart + scanTimeout)
        {
          BluetoothLEHardwareInterface.StopScan();
          Reset();
          Debug.LogWarning("[" + Time.time + "]: <Scan> Cancel scan. Timeout.");
          return;
        }
        await UniTask.Yield(PlayerLoopTiming.Update);
      }
      await UniTask.Delay(500);
      state = States.FoundButNotConnected;
    }

    public async void Connect() { await ConnectTask(); }

    public async UniTask ConnectTask()
    {
      if (!centralBleHandler.IsInitialized())
      {
        Debug.LogWarning(
          "<Connect> Can't connect. Central is not initialized.");
        return;
      }
      else if (state != States.FoundButNotConnected)
      {
        Debug.LogWarning("<Connect> Can't connect. State = " + state);
        return;
      }
      state = States.Connecting;
      bool isWaitingCallback = true;
      Debug.Log("[" + Time.time + "]: <Connect> Start connecting.");
      BluetoothLEHardwareInterface.ConnectToPeripheral(
      deviceAddress, (ad) =>
      {
        isWaitingCallback = false;
        Debug.Log("[" + Time.time + "]: <Connect> Connected. Address = " + ad);
      }, null,
      (address, su, cu) =>
      {
        Debug.Log("[" + Time.time + "]: <Connect> Found Characteristics [" +
          su + "],[" + cu + "]");
        if (!services.ContainsKey(su))
        { services.Add(su, new Service(su)); }
        if (!services[su].characteristics.ContainsKey(cu))
        { services[su].characteristics.Add(cu, new Characteristic(cu)); }
      }, (ad) =>
      {
        Reset();
        Debug.Log("[" + Time.time +
          "]: <Connect> Callback disconnected action. Address = " + ad);
      });
      while (isWaitingCallback)
      {
        if (!centralBleHandler.IsInitialized())
        {
          Reset();
          Debug.LogWarning("[" + Time.time +
            "]: <Connect> Cancel connect. Central is not initialized.");
          return;
        }
        await UniTask.Yield(PlayerLoopTiming.Update);
      }
      await UniTask.Delay(2000);
      state = States.Connected;
    }

    public async void Disconnect() { await DisconnectTask(); }

    public async UniTask DisconnectTask()
    {
      if (!centralBleHandler.IsInitialized())
      {
        Debug.LogWarning(
          "<Disconnect> Can't disconnect. Central is not initialized.");
        return;
      }
      else if (state != States.Connected)
      {
        Debug.LogWarning("<Disconnect> Can't disconnect. State = " + state);
        return;
      }
      state = States.Disconnecting;
      bool isWaitingCallback = true;
      Debug.Log("[" + Time.time + "]: <Disconnect> Start disconnecting.");
      BluetoothLEHardwareInterface.DisconnectPeripheral(
        deviceAddress, (ad) =>
        {
          isWaitingCallback = false;
          Debug.Log("[" + Time.time +
            "]: <Disconnect> Disconnected. Address = " + ad);
        });
      while (isWaitingCallback)
      {
        if (!centralBleHandler.IsInitialized())
        {
          Reset();
          Debug.LogWarning("[" + Time.time +
              "]: <Disconnect> Cancel disconnect. Central is not initialized.");
          return;
        }
        await UniTask.Yield(PlayerLoopTiming.Update);
      }
      Reset();
    }

    public async void ReadCharacteristic(
      string serviceUuid, string characteristicUuid, BytesEvent readEvent)
    {
      await ReadCharacteristicTasl(
        FullUuid(serviceUuid), FullUuid(characteristicUuid), readEvent);
    }

    public async UniTask ReadCharacteristicTasl(
      string serviceUuid, string characteristicUuid, BytesEvent readEvent)
    {
      if (!centralBleHandler.IsInitialized())
      {
        Debug.LogWarning("<Read> Can't read. Central is not initialized.");
        return;
      }
      else if (state != States.Connected)
      {
        Debug.LogWarning("<Read> Can't read. State = " + state);
        return;
      }
      else if (!services.ContainsKey(serviceUuid))
      {
        Debug.LogWarning(
          "<Read> The service is not found. uuid = " + serviceUuid);
        return;
      }
      else if (
        !services[serviceUuid].characteristics.ContainsKey(characteristicUuid))
      {
        Debug.LogWarning("<Read> The characteristic is not found. uuid = " +
          characteristicUuid);
        return;
      }
      state = States.Reading;
      bool isWaitingCallback = true;
      Debug.Log("<Read> Read bytes");
      BluetoothLEHardwareInterface.ReadCharacteristic(
      deviceAddress, serviceUuid, characteristicUuid,
      (cu, bytes) =>
      {
        // Read action callback doesn't work in Editor mode.
        isWaitingCallback = false;
        Debug.Log("<Read> Read Succeeded.");
        readEvent.Invoke(bytes);
      });
      while (isWaitingCallback)
      {
        if (!centralBleHandler.IsInitialized())
        {
          Reset();
          Debug.LogWarning("[" + Time.time +
            "]: <Read> Cancel read. Central is not initialized.");
          return;
        }
        await UniTask.Yield(PlayerLoopTiming.Update);
        state = States.Connected;
      }
    }

    public async void WriteCharacteristic(
      string serviceUuid, string characteristicUuid, string value)
    {
      await WriteCharacteristicTask(
        FullUuid(serviceUuid), FullUuid(characteristicUuid), value);
    }

    public async UniTask WriteCharacteristicTask(
      string serviceUuid, string characteristicUuid, string value)
    {
      if (!centralBleHandler.IsInitialized())
      {
        Debug.LogWarning("<Write> Can't read. Central is not initialized.");
        return;
      }
      else if (state != States.Connected)
      {
        Debug.LogWarning("<Write> Can't write. State = " + state);
        return;
      }
      else if (!services.ContainsKey(serviceUuid))
      {
        Debug.LogWarning(
          "<Write> The service is not found. uuid = " + serviceUuid);
        return;
      }
      else if (
        !services[serviceUuid].characteristics.ContainsKey(characteristicUuid))
      {
        Debug.LogWarning("<Write> The characteristic is not found. uuid = " +
          characteristicUuid);
        return;
      }
      state = States.Writing;
      bool isWaitingCallback = true;
      Debug.Log("<Write> Write value");
      byte[] data = System.Text.Encoding.ASCII.GetBytes(value);
      Debug.Log(data);
      BluetoothLEHardwareInterface.WriteCharacteristic(
      deviceAddress, serviceUuid, characteristicUuid, data, data.Length,
      true, (cu) =>
      {
        isWaitingCallback = false;
        Debug.Log("<Write> Write Succeeded. " + value);
      });
      while (isWaitingCallback)
      {
        if (!centralBleHandler.IsInitialized())
        {
          Reset();
          Debug.LogWarning("[" + Time.time +
            "]: <Write> Cancel write. Central is not initialized.");
          return;
        }
        await UniTask.Yield(PlayerLoopTiming.Update);
        state = States.Connected;
      }
    }

    public async void Subscribe(
      string serviceUuid, string characteristicUuid, BytesEvent notifyEvent)
    {
      await SubscribeTask(
        FullUuid(serviceUuid), FullUuid(characteristicUuid), notifyEvent);
    }

    public async UniTask SubscribeTask(
      string serviceUuid, string characteristicUuid, BytesEvent notifyEvent)
    {
      if (!centralBleHandler.IsInitialized())
      {
        Debug.LogWarning(
          "<Subscribe> Can't subscribe. Central is not initialized.");
        return;
      }
      else if (state != States.Connected)
      {
        Debug.LogWarning("<Subscribe> Can't subscribe. State = " + state);
        return;
      }
      else if (!services.ContainsKey(serviceUuid))
      {
        Debug.LogWarning("<Subscribe> The service is not found. uuid = " +
          serviceUuid);
        return;
      }
      else if (
        !services[serviceUuid].characteristics.ContainsKey(characteristicUuid))
      {
        Debug.LogWarning(
          "<Subscribe> The characteristic is not found. uuid = " +
          characteristicUuid);
        return;
      }
      else if (
        services[serviceUuid].characteristics[characteristicUuid].isSubscribing)
      {
        Debug.LogWarning(
          "<Subscribe> The characteristic is already subscribed.");
        return;
      }
      state = States.Subscribing;
      bool isWaitingCallback = true;
      Debug.Log("<Subscribe> Start Subscribe.");
      BluetoothLEHardwareInterface.SubscribeCharacteristic(
        deviceAddress, serviceUuid, characteristicUuid,
        (cu) =>
        {
          // Notification action callback doesn't work in Editor mode.
          isWaitingCallback = false;
          services[serviceUuid].characteristics[characteristicUuid]
            .isSubscribing = true;
          Debug.Log("<Subscribe> Subscribe Succeeded.");
        }, (characteristicUUID, bytes) =>
        {
          string value = System.Text.Encoding.ASCII.GetString(bytes);
          Debug.Log("<Subscribe> Notified.");
          notifyEvent.Invoke(bytes);
        });
      while (isWaitingCallback)
      {
        if (!centralBleHandler.IsInitialized())
        {
          Reset();
          Debug.LogWarning("[" + Time.time +
            "]: <Subscribe> Cancel subscribe. Central is not initialized.");
          return;
        }
        await UniTask.Yield(PlayerLoopTiming.Update);
        state = States.Connected;
      }
    }

    public async void Unsubscribe(string serviceUuid, string characteristicUuid)
    {
      await UnsubscribeTask(
        FullUuid(serviceUuid), FullUuid(characteristicUuid));
    }

    public async UniTask UnsubscribeTask(
      string serviceUuid, string characteristicUuid)
    {
      if (!centralBleHandler.IsInitialized())
      {
        Debug.LogWarning(
          "<Unsubscribe> Can't unsubscrie. Central is not initialized.");
        return;
      }
      else if (state != States.Connected)
      {
        Debug.LogWarning("<Unsubscribe> Can't unsubscrie. State = " + state);
        return;
      }
      else if (!services.ContainsKey(serviceUuid))
      {
        Debug.LogWarning("<Unsubscribe> The service is not found. uuid = " +
          serviceUuid);
        return;
      }
      else if (
        !services[serviceUuid].characteristics.ContainsKey(characteristicUuid))
      {
        Debug.LogWarning(
          "<Unsubscribe> The characteristic is not found. uuid = " +
          characteristicUuid);
        return;
      }
      else if (
       !services[serviceUuid].characteristics[characteristicUuid].isSubscribing)
      {
        Debug.LogWarning("<Unsubscribe> The characteristic is not subscribed.");
        return;
      }
      state = States.Unsubscribing;
      bool isWaitingCallback = true;
      Debug.Log("<Unsubscribe> Start unsubscribe");
      BluetoothLEHardwareInterface.UnSubscribeCharacteristic(
        deviceAddress, serviceUuid, characteristicUuid,
        (name) =>
        {
          isWaitingCallback = false;
          services[serviceUuid].characteristics[characteristicUuid]
            .isSubscribing = false;
          Debug.Log("<Unsubscribe> Unsubscribe Succeeded. " + name);
        });
      while (isWaitingCallback)
      {
        if (!centralBleHandler.IsInitialized())
        {
          Reset();
          Debug.LogWarning("[" + Time.time +
            "]: <Unsubscribe> Cancel unsubscribe. Central is not initialized.");
          return;
        }
        await UniTask.Yield(PlayerLoopTiming.Update);
        state = States.Connected;
      }
      await UniTask.Delay(4000);
    }

    string FullUuid(string uuid)
    {
      // TODO: Move to interaction.
      string fullUUID = uuid;
      if (fullUUID.Length == 4)
        fullUUID = "0000" + uuid + "-0000-1000-8000-00805F9B34FB";
      return fullUUID;
    }

    bool IsEqual(string uuid1, string uuid2)
    {
      // TODO: Delete.
      uuid1 = FullUuid(uuid1);
      uuid2 = FullUuid(uuid2);
      return (uuid1.ToUpper().Equals(uuid2.ToUpper()));
    }

    bool IsProcessing()
    {
      return state == States.Scanning ||
        state == States.Connecting || state == States.Disconnecting ||
        state == States.Reading || state == States.Writing ||
        state == States.Subscribing || state == States.Unsubscribing;
    }
  }
}
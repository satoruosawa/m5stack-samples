using System;
using UnityEngine.Events;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace M5BLE
{
  [Serializable] public class ReadEvent : UnityEvent<string> { }

  public class BleHandler : MonoBehaviour
  {
    public string deviceName = "M5Stack";
    public string serviceUUID = "2220";
    float scanTimeout = 10.0f; //sec
    public enum States
    {
      NotInitialized,
      Initializing,
      Error,
      NotFound,
      Scaning,
      FoundButNotConnected,
      Connecting,
      Connected,
      Disconnecting,
      Deinitializing,
      Reading,
      Writing,
      Subscribing,
      Unsubscribing
    }

    public struct UUID
    {
      public string service;
      public string characteristic;
      public bool isSubscribing;
    }

    public States state { get; private set; }

    string deviceAddress = null;
    List<UUID> uuids;

    BleHandler()
    {
      uuids = new List<UUID>();
    }

    async void Start()
    {
      Reset();
      await InitializeTask();
      await UniTask.Delay(5000);
      await ScanTask();
      await UniTask.Delay(5000);
      await ConnectTask();
    }

    void Reset()
    {
      state = States.NotInitialized;
      deviceAddress = null;
      uuids.Clear();
    }

    public async void Initialize() { await InitializeTask(); }

    public async UniTask InitializeTask()
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
        state = States.Error;
        Debug.LogError("Error: " + error);
      });
      while (IsWaitingCallback()) await UniTask.Yield(PlayerLoopTiming.Update);
      await UniTask.Delay(100);
    }

    public async void Deinitialize() { await DeinitializeTask(); }

    public async UniTask DeinitializeTask()
    {
      if (state == States.NotInitialized)
      {
        Debug.LogWarning("NotInitialized");
        return;
      }
      while (IsWaitingCallback())
      {
        if (state == States.Deinitializing) return;
        await UniTask.Yield(PlayerLoopTiming.Update);
      }
      if (state == States.Connected)
      {
        await DisconnectTask();
      }
      state = States.Deinitializing;
      Debug.Log("[" + Time.time + "]: Start deinitialize.");
      BluetoothLEHardwareInterface.DeInitialize(() =>
      {
        Reset();
        Debug.Log("[" + Time.time + "]: End deinitialize.");
      });
      while (IsWaitingCallback()) await UniTask.Yield(PlayerLoopTiming.Update);
    }

    public async void Scan() { await ScanTask(); }

    public async UniTask ScanTask()
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
          Reset();
          state = States.NotFound;
          Debug.LogWarning("[" + Time.time + "]: Scan timeout.");
          break;
        }
        await UniTask.Yield(PlayerLoopTiming.Update);
      }
      await UniTask.Delay(500);
    }

    public async void Connect() { await ConnectTask(); }

    public async UniTask ConnectTask()
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
        if (IsEqual(su, serviceUUID))
          Debug.Log("[" + Time.time + "]: Found Service UUID. UUID = " + su);
      },
      (address, su, cu) =>
      {
        if (IsEqual(su, serviceUUID))
        {
          UUID uuid = new UUID()
          {
            service = su,
            characteristic = cu,
            isSubscribing = false
          };
          uuids.Add(uuid);
        }
      }, (ad) =>
      {
        Reset();
        state = States.NotFound;
        Debug.Log("[" + Time.time + "]: Disconnected. Address = " + ad);
      });
      while (IsWaitingCallback()) await UniTask.Yield(PlayerLoopTiming.Update);
      await UniTask.Delay(2000);
    }

    public async void Disconnect() { await DisconnectTask(); }

    public async UniTask DisconnectTask()
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
          Reset();
          state = States.NotFound;
          Debug.Log("[" + Time.time + "]: Disconnected. Address = " + ad);
        });
      while (IsWaitingCallback()) await UniTask.Yield(PlayerLoopTiming.Update);
    }

    public async void ReadCharacteristic(
      string characteristicUUID, ReadEvent readEvent)
    {
      await ReadCharacteristicTasl(characteristicUUID, readEvent);
    }

    public async UniTask ReadCharacteristicTasl(
      string characteristicUUID, ReadEvent readEvent)
    {
      if (state != States.Connected)
      {
        Debug.LogWarning("Can't read. State = " + state);
        return;
      }
      else if (uuids.FindIndex(uuid => (IsEqual(uuid.service, serviceUUID) &&
        IsEqual(uuid.characteristic, characteristicUUID))) == -1)
      {
        Debug.LogWarning("ReadCharacteristic is not found.");
        return;
      }
      state = States.Reading;
      Debug.Log("Read bytes");
      BluetoothLEHardwareInterface.ReadCharacteristic(
      deviceAddress, serviceUUID, characteristicUUID,
      (cu, bytes) =>
      {
        // Read action callback doesn't work in Editor mode.
        state = States.Connected;
        string value = System.Text.Encoding.ASCII.GetString(bytes);
        Debug.Log("Read Succeeded. " + value);
        readEvent.Invoke(value);
      });
      while (IsWaitingCallback()) await UniTask.Yield(PlayerLoopTiming.Update);
    }

    public async void WriteCharacteristic(
      string characteristicUUID, string value)
    {
      await WriteCharacteristicTask(characteristicUUID, value);
    }

    public async UniTask WriteCharacteristicTask(
      string characteristicUUID, string value)
    {
      if (state != States.Connected)
      {
        Debug.LogWarning("Can't write. State = " + state);
        return;
      }
      else if (uuids.FindIndex(uuid => (IsEqual(uuid.service, serviceUUID) &&
        IsEqual(uuid.characteristic, characteristicUUID))) == -1)
      {
        Debug.LogWarning("WriteCharacteristic is not found.");
        return;
      }
      state = States.Writing;
      Debug.Log("Write bytes");
      byte[] data = System.Text.Encoding.ASCII.GetBytes(value);
      Debug.Log(data);
      BluetoothLEHardwareInterface.WriteCharacteristic(
      deviceAddress, serviceUUID, characteristicUUID, data, data.Length,
      true, (cu) =>
      {
        state = States.Connected;
        Debug.Log("Read Succeeded. " + value);
      });
      while (IsWaitingCallback()) await UniTask.Yield(PlayerLoopTiming.Update);
    }

    public async void Subscribe(
      string characteristicUUID, ReadEvent notifyEvent)
    { await SubscribeTask(characteristicUUID, notifyEvent); }

    public async UniTask SubscribeTask(
      string characteristicUUID, ReadEvent notifyEvent)
    {
      if (state != States.Connected)
      {
        Debug.LogWarning("Can't subscribe. State = " + state);
        return;
      }
      int uuidIndex = uuids.FindIndex(
        uuid => (IsEqual(uuid.service, serviceUUID) &&
        IsEqual(uuid.characteristic, characteristicUUID)));
      if (uuidIndex == -1)
      {
        Debug.LogWarning("The notifyCharacteristic is not found.");
        return;
      }
      if (uuids[uuidIndex].isSubscribing)
      {
        Debug.LogWarning("Already subscribing.");
        return;
      }
      state = States.Subscribing;
      Debug.Log("Start Subscribe.");
      BluetoothLEHardwareInterface.SubscribeCharacteristic(
        deviceAddress, serviceUUID, characteristicUUID,
        (cu) =>
        {
          // Notification action callback doesn't work in Editor mode.
          state = States.Connected;
          UUID uuid = uuids[uuidIndex];
          uuid.isSubscribing = true;
          uuids[uuidIndex] = uuid;
          Debug.Log("Subscribe Succeeded.");
        }, (characteristicUUID, bytes) =>
        {
          string value = System.Text.Encoding.ASCII.GetString(bytes);
          Debug.Log("Received. value = " + value);
          notifyEvent.Invoke(value);
        });
      while (IsWaitingCallback()) await UniTask.Yield(PlayerLoopTiming.Update);
    }

    public async void Unsubscribe(string characteristicUUID)
    { await UnsubscribeTask(characteristicUUID); }

    public async UniTask UnsubscribeTask(string characteristicUUID)
    {
      if (state != States.Connected)
      {
        Debug.LogWarning("Can't unsubscrie. State = " + state);
        return;
      }
      int uuidIndex = uuids.FindIndex(
        uuid => (IsEqual(uuid.service, serviceUUID) &&
        IsEqual(uuid.characteristic, characteristicUUID)));
      if (uuidIndex == -1)
      {
        Debug.LogWarning("The notifyCharacteristic is not found.");
        return;
      }
      if (!uuids[uuidIndex].isSubscribing)
      {
        Debug.LogWarning("Not subscribed.");
        return;
      }
      state = States.Unsubscribing;
      Debug.Log("Unsubscribe");
      BluetoothLEHardwareInterface.UnSubscribeCharacteristic(
        deviceAddress, serviceUUID, characteristicUUID,
        (name) =>
        {
          state = States.Connected;
          UUID uuid = uuids[uuidIndex];
          uuid.isSubscribing = false;
          uuids[uuidIndex] = uuid;
          Debug.Log("Unsubscribe Succeeded. " + name);
        });
      while (IsWaitingCallback()) await UniTask.Yield(PlayerLoopTiming.Update);
      await UniTask.Delay(4000);
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
        state == States.Deinitializing || state == States.Reading ||
        state == States.Writing || state == States.Subscribing ||
        state == States.Unsubscribing;
    }
  }
}
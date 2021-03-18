using System;
using TMPro;
using UnityEngine.Events;
using UnityEngine;

namespace M5BLE
{
  [Serializable] public class TextEvent : UnityEvent<string> { }

  public class Interactions : MonoBehaviour
  {
    public string serviceCharacteristicUUID = "2220";
    public string readCharacteristicUUID = "2221";
    public string writeCharacteristicUUID = "2222";
    public string notifyCharacteristicUUID = "2223";
    [SerializeField] PeripheralBleHandler peripheralBleHandler = null;
    [SerializeField] TextEvent readEvent = new TextEvent();
    [SerializeField] TextEvent notifyEvent = new TextEvent();
    [SerializeField] TextMeshProUGUI writeTextUgui = null;

    public void ReadCharacteristic()
    {
      BytesEvent readBytesEvent = new BytesEvent();
      readBytesEvent.AddListener((bytes) =>
      {
        // if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
        int value = BitConverter.ToInt32(bytes, 0);
        Debug.Log("readBytesEvent");
        Debug.Log(value);
        readEvent.Invoke(value.ToString());
      });
      peripheralBleHandler.ReadCharacteristic(
        FullUuid(serviceCharacteristicUUID), FullUuid(readCharacteristicUUID),
        readBytesEvent);
    }

    public void WriteCharacteristic()
    {
      peripheralBleHandler.WriteCharacteristic(
        FullUuid(serviceCharacteristicUUID), FullUuid(writeCharacteristicUUID),
        writeTextUgui.text);
    }

    public void SubscribeCharacteristic()
    {
      BytesEvent notifyBytesEvent = new BytesEvent();
      notifyBytesEvent.AddListener((bytes) =>
      {
        // if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
        int value = BitConverter.ToInt32(bytes, 0);
        Debug.Log("notifyBytesEvent");
        Debug.Log(value);
        notifyEvent.Invoke(value.ToString());
      });
      peripheralBleHandler.Subscribe(
        FullUuid(serviceCharacteristicUUID), FullUuid(notifyCharacteristicUUID),
        notifyBytesEvent);
    }

    public void UnsubscribeCharacteristic()
    {
      peripheralBleHandler.Unsubscribe(
        FullUuid(serviceCharacteristicUUID),
        FullUuid(notifyCharacteristicUUID));
    }

    string FullUuid(string uuid)
    {
      string fullUUID = uuid;
      if (fullUUID.Length == 4)
        fullUUID = "0000" + uuid + "-0000-1000-8000-00805F9B34FB";
      return fullUUID;
    }
  }
}

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
    [SerializeField] TextMeshProUGUI ugui = null;

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
        serviceCharacteristicUUID, readCharacteristicUUID, readBytesEvent);
    }

    public void WriteCharacteristic()
    {
      peripheralBleHandler.WriteCharacteristic(
        serviceCharacteristicUUID, writeCharacteristicUUID, ugui.text);
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
        serviceCharacteristicUUID, notifyCharacteristicUUID, notifyBytesEvent);
    }

    public void UnsubscribeCharacteristic()
    {
      peripheralBleHandler.Unsubscribe(
        serviceCharacteristicUUID, notifyCharacteristicUUID);
    }
  }
}

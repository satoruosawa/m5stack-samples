using System;
using TMPro;
using UnityEngine.Events;
using UnityEngine;

namespace M5BLE
{
  [Serializable] public class TextEvent : UnityEvent<string> { }

  public class Interactions : MonoBehaviour
  {
    public string readCharacteristicUUID = "2221";
    public string writeCharacteristicUUID = "2222";
    public string notifyCharacteristicUUID = "2223";
    [SerializeField] PeripheralBleHandler peripheralBleHandler = null;
    [SerializeField] TextEvent readEvent = new TextEvent();
    [SerializeField] TextEvent notifyEvent = new TextEvent();
    [SerializeField] TextMeshProUGUI ugui = null;

    void Start()
    {

    }

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
      peripheralBleHandler.ReadCharacteristic(readCharacteristicUUID, readBytesEvent);
    }

    public void WriteCharacteristic()
    {
      peripheralBleHandler.WriteCharacteristic(writeCharacteristicUUID, ugui.text);
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
      peripheralBleHandler.Subscribe(notifyCharacteristicUUID, notifyBytesEvent);
    }

    public void UnsubscribeCharacteristic()
    {
      peripheralBleHandler.Unsubscribe(notifyCharacteristicUUID);
    }
  }
}

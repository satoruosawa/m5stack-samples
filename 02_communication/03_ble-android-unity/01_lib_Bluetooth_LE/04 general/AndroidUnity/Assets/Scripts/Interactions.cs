using M5BLE;
using TMPro;
using UnityEngine;

public class Interactions : MonoBehaviour
{
  public string readCharacteristicUUID = "2221";
  public string writeCharacteristicUUID = "2222";
  public string notifyCharacteristicUUID = "2223";
  [SerializeField] BleHandler bleHandler = null;
  [SerializeField] ReadEvent readEvent = new ReadEvent();
  [SerializeField] ReadEvent notifyEvent = new ReadEvent();
  [SerializeField] TextMeshProUGUI ugui = null;

  void Start()
  {

  }

  public void ReadCharacteristic()
  {
    bleHandler.ReadCharacteristic(readCharacteristicUUID, readEvent);
  }

  public void WriteCharacteristic()
  {
    bleHandler.WriteCharacteristic(writeCharacteristicUUID, ugui.text);
  }

  public void SubscribeCharacteristic()
  {
    bleHandler.Subscribe(notifyCharacteristicUUID, notifyEvent);
  }

  public void UnsubscribeCharacteristic()
  {
    bleHandler.Unsubscribe(notifyCharacteristicUUID);
  }
}

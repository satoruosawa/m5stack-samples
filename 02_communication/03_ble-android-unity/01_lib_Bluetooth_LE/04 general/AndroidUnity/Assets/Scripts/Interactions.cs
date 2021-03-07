using System.Collections;
using M5BLE;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Interactions : MonoBehaviour
{
  public string readCharacteristicUUID = "2221";
  public string writeCharacteristicUUID = "2222";
  [SerializeField] BleHandler bleHandler = null;
  [SerializeField] ReadEvent readEvent = new ReadEvent();
  [SerializeField] TextMeshProUGUI ugui = null;

  void Start()
  {

  }

  public void OnReadCharacteristic()
  {
    bleHandler.OnReadCharacteristic(readCharacteristicUUID, readEvent);
  }

  public void OnWriteCharacteristic()
  {
    bleHandler.OnWriteCharacteristic(writeCharacteristicUUID, ugui.text);
  }
}

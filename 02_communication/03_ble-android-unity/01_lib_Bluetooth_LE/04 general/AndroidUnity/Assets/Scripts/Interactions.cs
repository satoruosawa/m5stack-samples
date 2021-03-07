using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M5BLE;

public class Interactions : MonoBehaviour
{
  public string readCharacteristicUUID = "2221";
  [SerializeField] BleHandler bleHandler = null;
  [SerializeField] ReadEvent readEvent = new ReadEvent();

  void Start()
  {

  }

  public void OnReadCharacteristic()
  {
    bleHandler.OnReadCharacteristic(readCharacteristicUUID, readEvent);
  }
}

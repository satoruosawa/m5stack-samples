using System;
using TMPro;
using UnityEngine.Events;
using UnityEngine;

public class WriteInteraction : MonoBehaviour
{
  [Serializable] public class WriteEvent : UnityEvent<string> { }
  [SerializeField] WriteEvent writeEvent = new WriteEvent();
  [SerializeField] TextMeshProUGUI ugui = null;

  public void OnWriteCharacteristic()
  {
    writeEvent.Invoke(ugui.text);
  }
}

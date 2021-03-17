using UnityEngine;
using TMPro;
using M5BLE;

public class PeripheralStatusText : MonoBehaviour
{
  [SerializeField] TextMeshProUGUI statusUgui = null;
  [SerializeField] PeripheralBleHandler peripheralBleHandler = null;


  void Update()
  {
    switch (peripheralBleHandler.state)
    {
      case PeripheralBleHandler.States.NotFound:
        statusUgui.text = "NotFound";
        break;
      case PeripheralBleHandler.States.Scaning:
        statusUgui.text = "Scanning";
        break;
      case PeripheralBleHandler.States.FoundButNotConnected:
        statusUgui.text = "Found Device";
        break;
      case PeripheralBleHandler.States.Connecting:
        statusUgui.text = "Connecting";
        break;
      case PeripheralBleHandler.States.Connected:
        statusUgui.text = "Connected";
        break;
      case PeripheralBleHandler.States.Disconnecting:
        statusUgui.text = "Disconnecting";
        break;
      case PeripheralBleHandler.States.Reading:
        statusUgui.text = "Reading";
        break;
      case PeripheralBleHandler.States.Writing:
        statusUgui.text = "Writing";
        break;
      case PeripheralBleHandler.States.Subscribing:
        statusUgui.text = "Subscribing";
        break;
      case PeripheralBleHandler.States.Unsubscribing:
        statusUgui.text = "Unsubscribing";
        break;
      default:
        break;
    }
  }
}
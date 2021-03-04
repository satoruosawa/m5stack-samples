using UnityEngine;
using TMPro;

public class StatusText : MonoBehaviour
{
  [SerializeField] TextMeshProUGUI statusUgui = null;
  [SerializeField] BleHandler bleHandler = null;


  void Update()
  {
    switch (bleHandler.state)
    {
      case BleHandler.States.NotInitialized:
        statusUgui.text = "Not initialized";
        break;
      case BleHandler.States.Initializing:
        statusUgui.text = "Initializing";
        break;
      case BleHandler.States.InitializationError:
        statusUgui.text = "Initialization error";
        break;
      case BleHandler.States.NotFound:
        statusUgui.text = "Initialized";
        break;
      case BleHandler.States.Scaning:
        statusUgui.text = "Scanning";
        break;
      case BleHandler.States.FoundButNotConnected:
        statusUgui.text = "Found Device";
        break;
      case BleHandler.States.Connecting:
        statusUgui.text = "Connecting";
        break;
      case BleHandler.States.Connected:
        statusUgui.text = "Connected";
        break;
      case BleHandler.States.Disconnecting:
        statusUgui.text = "Disconnecting";
        break;
      case BleHandler.States.Deinitializing:
        statusUgui.text = "Deinitializing";
        break;
      case BleHandler.States.Subscribing:
        statusUgui.text = "Subscribing";
        break;
      case BleHandler.States.Unsubscribing:
        statusUgui.text = "Unsubscribing";
        break;
      default:
        break;
    }
  }
}
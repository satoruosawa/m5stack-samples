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
        if (!bleHandler.isWaitingCallback)
        {
          statusUgui.text = "Not initialized";
        }
        else
        {
          statusUgui.text = "Initializing";
        }
        break;
      case BleHandler.States.InitializationError:
        if (!bleHandler.isWaitingCallback)
        {
          statusUgui.text = "Initialization error";
        }
        else
        {
          statusUgui.text = "Error ie wc";
        }
        break;
      case BleHandler.States.NotFound:
        if (!bleHandler.isWaitingCallback)
        {
          statusUgui.text = "Initialized";
        }
        else
        {
          statusUgui.text = "Scanning";
        }
        break;
      case BleHandler.States.FoundButNotConnected:
        if (!bleHandler.isWaitingCallback)
        {
          statusUgui.text = "Found Device";
        }
        else
        {
          statusUgui.text = "Connecting";
        }
        break;
      case BleHandler.States.Connected:
        if (!bleHandler.isWaitingCallback)
        {
          statusUgui.text = "Connected";
        }
        else
        {
          statusUgui.text = "Disconnecting";
        }
        break;
      default:
        break;
    }
  }
}

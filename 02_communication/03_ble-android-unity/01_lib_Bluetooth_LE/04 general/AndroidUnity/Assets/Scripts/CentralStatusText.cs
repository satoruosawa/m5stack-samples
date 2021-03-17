using UnityEngine;
using TMPro;
using M5BLE;

public class CentralStatusText : MonoBehaviour
{
  [SerializeField] TextMeshProUGUI statusUgui = null;
  [SerializeField] CentralBleHandler centralBleHandler = null;


  void Update()
  {
    switch (centralBleHandler.state)
    {
      case CentralBleHandler.States.NotInitialized:
        statusUgui.text = "Not initialized";
        break;
      case CentralBleHandler.States.Initializing:
        statusUgui.text = "Initializing";
        break;
      case CentralBleHandler.States.Initialized:
        statusUgui.text = "Initialized";
        break;
      case CentralBleHandler.States.Deinitializing:
        statusUgui.text = "Deinitializing";
        break;
      case CentralBleHandler.States.Error:
        statusUgui.text = "Error";
        break;
      default:
        break;
    }
  }
}
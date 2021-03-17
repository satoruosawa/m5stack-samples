using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace M5BLE
{
  public class CentralBleHandler : MonoBehaviour
  {
    [SerializeField] List<PeripheralBleHandler> peripherals = null;
    public States state { get; private set; }
    public enum States
    {
      NotInitialized,
      Initializing,
      Initialized,
      Deinitializing,
      Error
    }

    CentralBleHandler() { Reset(); }

    void Reset() { state = States.NotInitialized; }

    public async void Initialize() { await InitializeTask(); }

    public async UniTask InitializeTask()
    {
      if (state != States.NotInitialized)
      {
        Debug.LogWarning("Can't initialize. State = " + state);
        return;
      }
      state = States.Initializing;
      bool isWaitingCallback = true;
      Debug.Log("[" + Time.time + "]: Start initialize.");
      BluetoothLEHardwareInterface.Initialize(true, false, () =>
      {
        isWaitingCallback = false;
        Debug.Log("[" + Time.time + "]: End initialize.");
      }, (error) =>
      {
        state = States.Error;
        Debug.LogError("Error: " + error);
      });
      while (isWaitingCallback)
      {
        if (IsError()) return;
        await UniTask.Yield(PlayerLoopTiming.Update);
      }
      await UniTask.Delay(500);
      state = States.Initialized;
    }

    public async void Deinitialize() { await DeinitializeTask(); }

    public async UniTask DeinitializeTask()
    {
      if (state != States.Initialized)
      {
        Debug.LogWarning("Can't deinitialize. Central is not initialized");
        return;
      }
      foreach (var peripheral in peripherals)
      {
        if (peripheral.state == PeripheralBleHandler.States.Connected)
        {
          Debug.LogWarning("Can't deinitialize. Peripheral is connected.");
          return;
        }
        else if (peripheral.IsProcessing())
        {
          Debug.LogWarning("Can't deinitialize. Peripheral is processing.");
          return;
        }
      }
      state = States.Deinitializing;
      bool isWaitingCallback = true;
      Debug.Log("[" + Time.time + "]: Start deinitialize.");
      BluetoothLEHardwareInterface.DeInitialize(() =>
      {
        isWaitingCallback = false;
        Debug.Log("[" + Time.time + "]: End deinitialize.");
      });
      while (isWaitingCallback)
      {
        if (IsError()) return;
        await UniTask.Yield(PlayerLoopTiming.Update);
      }
      await UniTask.Delay(500);
      state = States.NotInitialized;
    }

    bool IsError() { return state == States.Error; }

    public bool IsInitialized() { return state == States.Initialized; }
  }
}

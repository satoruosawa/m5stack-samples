using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using UnityEngine;

namespace M5BLE
{
  public class CentralBleHandler : MonoBehaviour
  {
    public States state { get; private set; }
    public enum States
    {
      NotInitialized,
      Initializing,
      Initialized,
      Deinitializing,
      Error
    }

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
      Debug.Log("[" + Time.time + "]: Start initialize.");
      BluetoothLEHardwareInterface.Initialize(true, false, () =>
      {
        state = States.Initialized;
        Debug.Log("[" + Time.time + "]: End initialize.");
      }, (error) =>
      {
        state = States.Error;
        Debug.LogError("Error: " + error);
      });
      while (IsProcessing()) await UniTask.Yield(PlayerLoopTiming.Update);
      await UniTask.Delay(100);
    }

    public async void Deinitialize() { await DeinitializeTask(); }

    public async UniTask DeinitializeTask()
    {
      if (state == States.NotInitialized)
      {
        Debug.LogWarning("NotInitialized");
        return;
      }
      while (IsProcessing())
      {
        if (state == States.Deinitializing) return;
        await UniTask.Yield(PlayerLoopTiming.Update);
      }
      state = States.Deinitializing;
      Debug.Log("[" + Time.time + "]: Start deinitialize.");
      BluetoothLEHardwareInterface.DeInitialize(() =>
      {
        Reset();
        Debug.Log("[" + Time.time + "]: End deinitialize.");
      });
      while (IsProcessing()) await UniTask.Yield(PlayerLoopTiming.Update);
    }

    bool IsProcessing()
    { return state == States.Initializing || state == States.Deinitializing; }
  }
}

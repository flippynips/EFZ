using System;

using Efz.Threading;

namespace Efz.Tools {
  
  /// <summary>
  /// Task timeout with function call.
  /// </summary>
  public class Timeout {
    
    //-------------------------------------------//
    
    public Timer Timer;
    
    public Action<bool> OnComplete;
    public Func<bool> GetComplete;
    
    //-------------------------------------------//
    
    public ActionAct _updater;
    public Lock _locker;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize with the timeout time in milliseconds, onComplete(false is timeout)
    /// getComplete get whether the task is complete.
    /// </summary>
    public Timeout(long time, Action<bool> onComplete, Func<bool> getComplete) {
      OnComplete = onComplete;
      GetComplete = getComplete;
      Timer = new Timer(time, OnTimeout);
      _locker = new Lock();
      
      _updater = new ActionAct(Update);
      ManagerUpdate.Polling.AddUpdate(_updater);
    }
    
    //-------------------------------------------//
    
    private void Update() {
      _locker.Take();
      if(_updater.ToRun && GetComplete()) {
        _updater.Remove = true;
        OnComplete(true);
      }
      _locker.Release();
    }
    
    private void OnTimeout() {
      _locker.Take();
      _updater.ToRun = false;
      _updater.Remove = true;
      OnComplete(false);
      _locker.Release();
    }
    
  }

}

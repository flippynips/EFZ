using System;
using System.Threading;

namespace Efz.Tools {
  
  /// <summary>
  /// Simple state machine class to handle situations where multiple threads complete a task.
  /// </summary>
  public class Ticker {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Action run when pulls exceed pushes.
    /// </summary>
    public IAction OnDone;
    /// <summary>
    /// Number of ticks.
    /// </summary>
    public int Ticks;
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    public Ticker() {
      Ticks = 1;
    }
    
    public Ticker(Action onDone, int ticks) {
      OnDone = new Act(onDone);
      Ticks = ticks;
    }
    
    public Ticker(IAction onDone, int ticks = 1) {
      OnDone = onDone;
      Ticks = ticks;
    }

    /// <summary>
    /// Increase required pulls by one.
    /// </summary>
    public void Push() {
      Interlocked.Increment(ref Ticks);
    }

    /// <summary>
    /// Decrease required pulls by one. If pulls have exceeded pushes, call onDone.
    /// </summary>
    public void Pull() {
      if(Interlocked.Decrement(ref Ticks) == 0) OnDone.Run();
    }
        
    //-------------------------------------------//
        
        
        
  }

}
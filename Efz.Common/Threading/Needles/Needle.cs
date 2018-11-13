using System;

namespace Efz.Threading {
  
  /// <summary>
  /// Handles a queue of tasks for threads to execute.
  /// </summary>
  public abstract class Needle : IDisposable {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Delta time in milliseconds between each complete cycle of tasks.
    /// Used for timing updates.
    /// </summary>
    public abstract long Delta { get; set; }
    /// <summary>
    /// Get the number of tasks waiting to be run.
    /// </summary>
    public abstract int Count { get; }
    /// <summary>
    /// Priority determines which order needles are checked for tasks.
    /// </summary>
    public readonly byte Priority = 50;
    /// <summary>
    /// Friendly name of the needle.
    /// </summary>
    public readonly string Name;
    /// <summary>
    /// Synchronization context to be used by the needle. These are accessed by thread handles
    /// as context switches are required.
    /// </summary>
    internal NeedleContext Context;
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Needles manage queues of tasks to be repeatedly executed after a length
    /// of time.
    /// </summary>
    protected Needle(string name, byte priority = 50) {
      Name = name;
      Priority = priority;
      Context = new NeedleContext(this);
    }
    
    /// <summary>
    /// Releases all resource used by the needle.
    /// </summary>
    public abstract void Dispose();
    /// <summary>
    /// Return a task if there is one.
    /// </summary>
    public abstract bool Next(out ActionAct task);
    
    /// <summary>
    /// Run all pending tasks. Doesn't update delta.
    /// </summary>
    public virtual void RunAll() {
      
      int count = Count;
      ActionAct task;
      while(--count >= 0 && Next(out task)) task.Run();
      
    }
    
    /// <summary>
    /// Add an Action to be called once in order on this needle.
    /// </summary>
    public virtual void AddSingle(Action action) {
      AddUpdate(new ActionAct(action, true));
    }
    
    /// <summary>
    /// Add an action to be called once in order on this needle.
    /// </summary>
    public virtual void AddSingle<A>(Action<A> action, A argA) {
      AddUpdate(new ActionAct(new ActionSet<A>(action, argA), true));
    }
    
    /// <summary>
    /// Add an action to be called once in order on this needle.
    /// </summary>
    public virtual void AddSingle<A,B>(Action<A,B> action, A argA, B argB) {
      AddUpdate(new ActionAct(new ActionSet<A,B>(action, argA, argB), true));
    }
    
    /// <summary>
    /// Add an action to be called once in order on this needle.
    /// </summary>
    public virtual void AddSingle<A,B,C>(Action<A,B,C> action, A argA, B argB, C argC) {
      AddUpdate(new ActionAct(new ActionSet<A,B,C>(action, argA, argB, argC), true));
    }
    
    /// <summary>
    /// Add an action to be called once in order on this needle.
    /// </summary>
    public virtual void AddSingle<A,B,C,D>(Action<A,B,C,D> action, A argA, B argB, C argC, D argD) {
      AddUpdate(new ActionAct(new ActionSet<A,B,C,D>(action, argA, argB, argC, argD), true));
    }
    
    /// <summary>
    /// Add an action to be called once in order on this needle.
    /// </summary>
    public virtual void AddSingle<A,B,C,D,E>(Action<A,B,C,D,E> action, A argA, B argB, C argC, D argD,
      E argE) {
      AddUpdate(new ActionAct(new ActionSet<A,B,C,D,E>(action, argA, argB, argC, argD, argE), true));
    }
    
    /// <summary>
    /// Add an action to be called once in order on this needle.
    /// </summary>
    public virtual void AddSingle<A,B,C,D,E,F>(Action<A,B,C,D,E,F> action, A argA, B argB, C argC, D argD,
      E argE, F argF) {
      AddUpdate(new ActionAct(new ActionSet<A,B,C,D,E,F>(action, argA, argB, argC, argD, argE, argF), true));
    }
    
    /// <summary>
    /// Add an action to be called once in order on this needle.
    /// </summary>
    public virtual void AddSingle<A,B,C,D,E,F,G>(Action<A,B,C,D,E,F,G> action, A argA, B argB, C argC, D argD,
      E argE, F argF, G argG) {
      AddUpdate(new ActionAct(new ActionSet<A,B,C,D,E,F,G>(action, argA, argB, argC, argD, argE, argF, argG), true));
    }
    
    /// <summary>
    /// Add an action to be called once in order on this needle.
    /// </summary>
    public virtual void AddSingle<A,B,C,D,E,F,G,H>(Action<A,B,C,D,E,F,G,H> action, A argA, B argB, C argC, D argD,
      E argE, F argF, G argG, H argH) {
      AddUpdate(new ActionAct(new ActionSet<A,B,C,D,E,F,G,H>(action, argA, argB, argC, argD, argE, argF, argG, argH), true));
    }
    
    /// <summary>
    /// Add an action to be called once in order on this needle.
    /// </summary>
    public virtual void AddSingle<A,B,C,D,E,F,G,H,I>(Action<A,B,C,D,E,F,G,H,I> action, A argA, B argB, C argC, D argD,
      E argE, F argF, G argG, H argH, I argI) {
      AddUpdate(new ActionAct(new ActionSet<A,B,C,D,E,F,G,H,I>(action, argA, argB, argC, argD, argE, argF, argG, argH, argI), true));
    }
    
    /// <summary>
    /// Add an action to be called once in order on this needle.
    /// </summary>
    public virtual void AddSingle<A,B,C,D,E,F,G,H,I,J>(Action<A,B,C,D,E,F,G,H,I,J> action, A argA, B argB, C argC, D argD,
      E argE, F argF, G argG, H argH, I argI, J argJ) {
      AddUpdate(new ActionAct(new ActionSet<A,B,C,D,E,F,G,H,I,J>(action, argA, argB, argC, argD, argE, argF, argG, argH, argI, argJ), true));
    }
    
    /// <summary>
    /// Add an ActionSet to be called once in order on this needle.
    /// </summary>
    public virtual void AddSingle(IRun action) {
      AddUpdate(new ActionAct(action, true));
    }
    
    /// <summary>
    /// Add an Task to be called in order on this needle.
    /// </summary>
    public abstract void AddUpdate(ActionAct task);
    
    /// <summary>
    /// Add an action to be called each update on this needle.
    /// </summary>
    public abstract ActionAct AddUpdate(Action action);
    
    /// <summary>
    /// Add an action to be called each update on this needle.
    /// </summary>
    public abstract ActionAct AddUpdate(IAction action);
    
    /// <summary>
    /// Pause this needle for the specified number of milliseconds.
    /// </summary>
    public abstract void Pause(long time);
    
    /// <summary>
    /// Reset the timing values used to calculate iterations.
    /// </summary>
    public virtual void ResetDelta() { }
    
    /// <summary>
    /// Join the a needle execution thread.
    /// </summary>
    public virtual void Join() {
      
      // iterate the thread handles
      foreach(var handle in ThreadHandle.Handles.TakeItem()) {
        
        // does the thread handle run this needle?
        if(handle.Needles.Contains(this)) {
          // yes, release the handles collection
          ThreadHandle.Handles.Release();
          // join the thread
          handle.Thread.Join();
          return;
        }
        
      }
      
      ThreadHandle.Handles.Release();
      throw new InvalidOperationException("No threads handle needle '"+Name+"'. Cannot join.");
    }
    
    //-------------------------------------------//
    
  }

}
using System;
using System.Collections.Generic;
using System.Threading;

using Efz.Collections;

namespace Efz.Threading {
  
  /// <summary>
  /// Handles one thread and the task currently active in it.
  /// </summary>
  public class ThreadHandle : IDisposable {
    
    //-------------------------------------------//
    
    /// <summary>
    /// The main process thread.
    /// </summary>
    public static ThreadHandle Main;
    /// <summary>
    /// All current thread handle.
    /// </summary>
    public static Shared<ArrayAir<ThreadHandle>> Handles;
    /// <summary>
    /// Number of active thread handles.
    /// </summary>
    public static int HandleCount {
      get { return _handleCount; }
    }
    
    /// <summary>
    /// Get the handle for this thread.
    /// </summary>
    public static ThreadHandle Current {
      get {
        ThreadHandle threadHandle;
        _handleMap.TakeItem().TryGetValue(Thread.CurrentThread.ManagedThreadId, out threadHandle);
        _handleMap.Release();
        return threadHandle;
      }
    }
    /// <summary>
    /// Get the current needle being executed.
    /// </summary>
    public static Needle Needle {
      get {
        // get the handle
        ThreadHandle threadHandle;
        if(_handleMap.TakeItem().TryGetValue(Thread.CurrentThread.ManagedThreadId, out threadHandle)) {
          _handleMap.Release();
          return threadHandle.Needles[threadHandle._index];
        }
        _handleMap.Release();
        return null;
      }
    }
    
    /// <summary>
    /// Managed thread id.
    /// </summary>
    public readonly int Id;
    
    /// <summary>
    /// The thread used by this handle.
    /// </summary>
    public readonly Thread Thread;
    /// <summary>
    /// Cords that contain ordered tasks to be completed. Some on update. Cord tasks are prioritized
    /// ahead of ThreadManager (order and thread agnostic) tasks.
    /// </summary>
    public readonly ArrayRig<Needle> Needles;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Handles mapped to threads.
    /// </summary>
    private static Shared<Dictionary<int, ThreadHandle>> _handleMap;
    /// <summary>
    /// Inner handle count.
    /// </summary>
    private static int _handleCount;
    
    /// <summary>
    /// Flag for the thread handle running.
    /// </summary>
    private bool _running;
    
    /// <summary>
    /// The next action task.
    /// </summary>
    private ActionAct _next;
    /// <summary>
    /// Should the thread handle advance a wait iteration?
    /// </summary>
    private bool _wait;
    /// <summary>
    /// Next needle index.
    /// </summary>
    private int _nextIndex;
    /// <summary>
    /// Current needle index.
    /// </summary>
    private int _index;
    
    /// <summary>
    /// The current iteration waiting for a needle task.
    /// </summary>
    private int _iteration;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize static members.
    /// </summary>
    static ThreadHandle() {
      Handles = new Shared<ArrayAir<ThreadHandle>>(new ArrayAir<ThreadHandle>());
      _handleMap = new Shared<Dictionary<int, ThreadHandle>>(new Dictionary<int, ThreadHandle>());
    }
    
    /// <summary>
    /// Initialize a new ThreadHandle that will start a new Thread.
    /// If memory parameter is 0 this handle will manage the thread that called the constructor.
    /// </summary>
    public ThreadHandle(long memory) {
      
      // initialize the required parameters
      Needles = new ArrayRig<Needle>();
      
      Handles.Take();
      if(memory == 0) {
        Thread = Thread.CurrentThread;
        Thread.Priority = ThreadPriority.AboveNormal;
        ThreadHandle.Main = this;
        Thread.Name = "Main";
      } else {
        Thread = new Thread(new ThreadStart(Run), (int)memory);
        Thread.Name = (Handles.Item.Count + 1).ToString();
      }
      
      Id = Thread.ManagedThreadId;
      Handles.Item.Add(this);
      _handleMap.TakeItem()[Id] = this;
      
      _handleMap.Release();
      Handles.Release();
      
      _iteration = -1;
      _index = -1;
    }
    
    /// <summary>
    /// Stops running the thread.
    /// </summary>
    public void Dispose() {
      _running = false;
      
      // remove this from the handles collection
      Interlocked.Decrement(ref _handleCount);
      
    }
    
    /// <summary>
    /// Main thread method.
    /// </summary>
    public void Run() {
      
      _running = true;
      Interlocked.Increment(ref _handleCount);
      
      while(_running) {
        
        //#if !DEBUG
        try {
        //#endif
          
          // main thread loop
          while(_running) {
            
            // iterate needles and get the next task
            _nextIndex = Needles.Count;
            while(--_nextIndex >= 0) {
              
              // is there a waiting task?
              if(Needles[_nextIndex].Next(out _next)) {
                
                // yes, has the context been changed?
                if(_index != _nextIndex) {
                  // yes, update
                  _index = _nextIndex;
                  // set the synchronization context to allow async operations to be joined
                  // back to the same threads assigned to the needle
                  SynchronizationContext.SetSynchronizationContext(Needles[_index].Context);
                }
                
                //Log.D("Running : "+Needles[_nextIndex].Name+" === " + _next);
                
                // try update the thread-local time values
                Time.Update();
                
                // run the task
                _next.Run();
                
                _iteration = 0;
                _wait = false;
                
                break;
              }
              
            }
            
            if(_wait) {
              // determine the thread action based on the current iteration count
              ++_iteration;
              Time.Update();
              if(_iteration % 5 == 0) {
                Thread.Sleep(1);
              } else if(_iteration > HandleCount) {
                _iteration = 0;
                Thread.Sleep(HandleCount);
              } else {
                Thread.Sleep(0);
              }
            } else _wait = true;
            
          }
          
        //#if !DEBUG
        } catch(Exception ex) {
          Log.Error("Unhandled exception", ex);
          if(!_wait) {
            _next.Stop();
            _wait = true;
          }
          continue;
        }
        //#endif
        
      }
      
      // remove from the handles collection
      Handles.TakeItem().Remove(this);
      Handles.Release();
      
      // remove this from the handles map
      _handleMap.TakeItem().Remove(Id);
      _handleMap.Release();
      
      // clear this handles needle collection
      Needles.Dispose();
      
    }
    
    /// <summary>
    /// Add a needle to this handle. The handle will retrieve tasks from the needles it knows about in order
    /// of priority.
    /// </summary>
    public void Add(Needle _needle) {
      Needles.Add(_needle);
      Needles.Sort((a, b) => a.Priority < b.Priority);
    }
    
    //-------------------------------------------//
    
  }
}


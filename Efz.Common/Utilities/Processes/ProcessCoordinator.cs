/*
 * User: FloppyNipples
 * Date: 02/06/2017
 * Time: 00:55
 */
using System;
using System.Diagnostics;
using Efz.Collections;
using Efz.Threading;

namespace Efz.Utilities.Processes {
  
  /// <summary>
  /// Controller of process tasks. Allows queueing of tasks to make use of
  /// the process resources.
  /// </summary>
  public class ProcessCoordinator {
    
    //------------------------------//
    
    /// <summary>
    /// Process this coordinator manages.
    /// </summary>
    public Process Process;
    
    //------------------------------//
    
    /// <summary>
    /// Lock used to disable concurrent processes with exiftool.
    /// </summary>
    protected readonly Lock _lock;
    /// <summary>
    /// Queue of metadata removal requests.
    /// </summary>
    protected readonly Queue<ProcessTask> _tasks;
    
    //------------------------------//
    
    /// <summary>
    /// Initialize a new process coordinator.
    /// </summary>
    public ProcessCoordinator() {
      _lock = new Lock();
      _tasks = new Queue<ProcessTask>();
    }
    
    /// <summary>
    /// Add a new task to the coordinator.
    /// </summary>
    public virtual void Add(ProcessTask task) {
      _lock.Take();
      if(_tasks.Current == null) _tasks.Current = task;
      else _tasks.Enqueue(task);
      _lock.Release();
    }
    
    /// <summary>
    /// Run the next process task.
    /// </summary>
    public virtual void Next() {
      ProcessTask task;
      _lock.Take();
      if(_tasks.Dequeue(out task)) {
        _lock.Release();
        task.Run();
      } else {
        _tasks.Current = null;
        _lock.Release();
      }
    }
    
    //------------------------------//
    
  }
  
}

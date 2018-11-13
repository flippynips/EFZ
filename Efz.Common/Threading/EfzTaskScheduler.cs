/*
 * User: FloppyNipples
 * Date: 09/05/2017
 * Time: 21:34
 */
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reflection;

namespace Efz.Threading {
  
  /// <summary>
  /// Implementation of the .NET TaskScheduler in order to run tasks in existing threads.
  /// </summary>
  public class EfzTaskScheduler : TaskScheduler {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Concurrency of the task schedule is equal to the assigned thread count.
    /// </summary>
    public override int MaximumConcurrencyLevel { get { return ManagerUpdate.ThreadCount; } }
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize a task scheduler.
    /// </summary>
    public EfzTaskScheduler() {
      // set this scheduler as the default task scheduler
      if(!System.Threading.ThreadPool.SetMinThreads(1, 1) ||
         !System.Threading.ThreadPool.SetMaxThreads(ManagerUpdate.PoolCount, ManagerUpdate.PoolCount)) {
        int maxWorkers;
        int maxCompletion;
        int minWorkers;
        int minCompletion;
        System.Threading.ThreadPool.GetMaxThreads(out maxWorkers, out maxCompletion);
        System.Threading.ThreadPool.GetMinThreads(out minWorkers, out minCompletion);
        Log.Warning("Setting default thread pool count failed. Going with defaults : Workers ("+ minWorkers + ", " + maxWorkers +
          ") Completion ("+minCompletion+","+maxCompletion+").");
      }
      /*
      try {
        typeof(TaskScheduler)
          .GetField("s_defaultTaskScheduler", BindingFlags.Static | BindingFlags.NonPublic)
          .SetValue(null, this);
        typeof(TaskFactory)
          .GetField("m_defaultScheduler", BindingFlags.Instance | BindingFlags.NonPublic)
          .SetValue(Task.Factory, this);
      } catch { }
      */
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// Execute the specified task. Throwing exceptions that occur.
    /// </summary>
    protected void Execute(Task task) {
      if(!TryExecuteTask(task) && task.Exception != null) throw task.Exception;
    }
    
    /// <summary>
    /// Retrieval of scheduled tasks isn't allowed.
    /// </summary>
    protected override IEnumerable<Task> GetScheduledTasks() {
      throw new NotImplementedException();
    }
    
    /// <summary>
    /// Queue a task.
    /// </summary>
    protected override void QueueTask(Task task) {
      ManagerUpdate.Control.AddSingle(Execute, task);
    }
    
    /// <summary>
    /// Dequeueing a task from this scheduler is not possible.
    /// </summary>
    protected override bool TryDequeue(Task task) {
      return false;
    }
    
    /// <summary>
    /// Attempt to execute the task synchronously.
    /// </summary>
    protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued) {
      return !taskWasPreviouslyQueued && TryExecuteTask(task);
    }
    
  }
  
}

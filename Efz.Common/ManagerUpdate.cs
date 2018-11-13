using System;
using System.Threading;

using Efz.Threading;
using Efz.Tools;

namespace Efz {
  
  /// <summary>
  /// Handler of updates.
  /// </summary>
  public class ManagerUpdate : Singleton<ManagerUpdate> {
    
    //-------------------------------------------//
    
    /// <summary>
    /// The number of threads used for the application.
    /// </summary>
    public const int ThreadCount = 10;
    /// <summary>
    /// The number of threads allowed in the .Net managed thread pool.
    /// </summary>
    public static int PoolCount = 10;
    
    /// <summary>
    /// The main needle for logic.
    /// </summary>
    public static Needle Control;
    /// <summary>
    /// The acurate time needle.
    /// </summary>
    public static Needle Polling;
    /// <summary>
    /// Low resolution timing needle.
    /// </summary>
    public static Needle Iterant;
    
    /// <summary>
    /// Task scheduler for .NET tasks to be run within the managed thread
    /// pool.
    /// </summary>
    public static EfzTaskScheduler TaskScheduler;
    
    /// <summary>
    /// On start task machine.
    /// </summary>
    public static TaskMachine OnStart;
    /// <summary>
    /// On end task machine.
    /// </summary>
    public static TaskMachine OnEnd;
    
    /// <summary>
    /// Flag indicating the program is ending and some tasks should become sychronous.
    /// </summary>
    public static bool Stopping { get; private set; }
    
    //-------------------------------------------//
    
    /// <summary>
    /// Manager update will be initialized first in likely all cases.
    /// </summary>
    protected override byte SingletonPriority { get { return 250; } }
    /// <summary>
    /// A spin wait for debug pausing of execution.
    /// </summary>
    private static SpinWait Pause;
    
    /// <summary>
    /// Integer flag indicating the stopping procedure.
    /// </summary>
    private static int _stopping;
    
    /// <summary>
    /// Destructor.
    /// </summary>
    private static Destructor _destructor = new Destructor();
    
    //-------------------------------------------//
    
    public ManagerUpdate() {
      
      // initialize the pause spinner
      Pause = new SpinWait();
      // initialize on start manager to log start events
      OnStart = new TaskMachine();
      // initialize on end manager to log end events
      OnEnd = new TaskMachine();
      
      Log.Line("RUNNING Efz PROGRAM");
      
      #if INFO
      OnStart.OnTask = new ActionSet<string>(s => { if(!string.IsNullOrEmpty(s)) Log.Info("Started '"+s+"'"); });
      OnStart.AddOnDone(() => Log.Line());
      #endif
      
      // create needles
      Polling = new NeedleRhythmic("Polling", 1000/10, 050); // 10 updates per second - time related tasks
      Iterant = new NeedleRhythmic("Iterant", 1000/01, 010); //  1 update  per second - low resolution updates
      Control = new NeedleDynamic("Control");                //     continual updates - non-update tasks
      
      // initialize the task scheduler to handle task execution by default
      TaskScheduler = new EfzTaskScheduler();
      
      // create and add thread handles
      for(int i = ThreadCount-2; i >= 0; --i) {
        new ThreadHandle(SystemInformation.Processor64Bit ? Global.Megabyte * 8 : Global.Megabyte * 4);
      }
      // main process thread handler
      new ThreadHandle(0);
      
      // add needles to handles
      foreach(ThreadHandle handle in ThreadHandle.Handles.TakeItem()) {
        handle.Add(Polling);
        handle.Add(Control);
        handle.Add(Iterant);
      }
      ThreadHandle.Handles.Release();
      
      
    }
    
    /// <summary>
    /// Start engine and time cord instances and run the main cord.
    /// </summary>
    public static void Run() {
      
      // set the initial ticks of the needles
      Control.ResetDelta();
      Polling.ResetDelta();
      Iterant.ResetDelta();
      
      // start all threads
      foreach(ThreadHandle handle in ThreadHandle.Handles.TakeItem()) {
        if(handle != ThreadHandle.Main) handle.Thread.Start();
      }
      ThreadHandle.Handles.Release();
      
      // start task machine
      OnStart.Run();
      
      // Finally start the main thread handler if it exists.
      if(ThreadHandle.Main != null) ThreadHandle.Main.Run();
    }
    
    /// <summary>
    /// Perform application ending functions, and then destroy managers.
    /// </summary>
    public static void EndProgram() {
      if(Interlocked.CompareExchange(ref _stopping, 1, 0) == 1) return;
      // flip the stopping flag
      Stopping = true;
      Log.Line("ENDING Efz PROGRAM");
      #if INFO
      OnEnd.OnTask = new ActionSet<string>(s => { if(!string.IsNullOrEmpty(s)) Log.Info("Ended '"+s+"'"); });
      #endif
      // add save configuration and final stop method
      OnEnd.AddOnDone(() => OnEnd.AddOnDone(Stop));
      OnEnd.Run();
    }
    
    /// <summary>
    /// Spin for the specified number of milliseconds.
    /// </summary>
    public static void Spin(long milliseconds) {
      long startTime = Time.Milliseconds;
      while(System.Diagnostics.Stopwatch.GetTimestamp() / Time.Frequency - startTime > milliseconds) {
        Pause.SpinOnce();
      }
      Pause.Reset();
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// Stop all thread handles resulting in the program leaving the 'Run'
    /// method and disposing of Manager classes.
    /// </summary>
    protected static void Stop() {
      // ensure the main thread handle (if it exists) is disposed of last
      for(int i = ThreadHandle.Handles.TakeItem().Count-1; i >= 0; --i) {
        if(ThreadHandle.Handles.Item[i] != ThreadHandle.Main) ThreadHandle.Handles.Item[i].Dispose();
      }
      ThreadHandle.Handles.Release();
      
      // dispose of the main thread handle
      if(ThreadHandle.Main != null) ThreadHandle.Main.Dispose();
      
      // flop flag
      Stopping = false;
    }
    
    /// <summary>
    /// Destructor class.
    /// </summary>
    private sealed class Destructor {
      ~Destructor() {
        // schedule the program is ending if not already
        if(!Stopping) EndProgram();
        // wait until the program has trully ended
        while(Stopping) Thread.Sleep(10);
      }
    }
    
    //-------------------------------------------//
    
  }
  
}
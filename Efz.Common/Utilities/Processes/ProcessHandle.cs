/*
 * User: FloppyNipples
 * Date: 01/01/2017
 * Time: 21:35
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Efz {
  
  /// <summary>
  /// Wrapper for starting and managing background processes.
  /// </summary>
  public class ProcessHandle : IDisposable {
    
    //-------------------------------------------//
    
    /// <summary>
    /// The process instance.
    /// </summary>
    public Process Process;
    /// <summary>
    /// Is the process currently running.
    /// </summary>
    public bool Running { get { return !Process.HasExited; } }
    
    /// <summary>
    /// Standard input of the process.
    /// </summary>
    public StreamWriter StandardInput { get { return Process.StandardInput; } }
    /// <summary>
    /// Standard output of the process.
    /// </summary>
    public StreamReader StandardOutput { get { return Process.StandardOutput; } }
    /// <summary>
    /// Standard output of the process errors.
    /// </summary>
    public StreamReader StandardError { get { return Process.StandardError; } }
    
    //-------------------------------------------//
    
    /// <summary>
    /// Callback on the process ending.
    /// </summary>
    protected IAction<ProcessHandle> _onEnded;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Create a process handle for the process specified by id.
    /// </summary>
    public static ProcessHandle Get(int id) {
      Process process;
      try {
        process = System.Diagnostics.Process.GetProcessById(id);
      } catch(Exception ex) {
        Log.Error("Process could not be found for id '"+id+"'.", ex);
        return null;
      }
      return new ProcessHandle(process);
    }
    
    /// <summary>
    /// Create a process handle for the process specified by name.
    /// If multiple processes have been started with the same name,
    /// the first will be handled. If no process is found, this
    /// handle will reference a 'Null' process.
    /// </summary>
    public static ProcessHandle Find(string name) {
      var processes = System.Diagnostics.Process.GetProcessesByName(name);
      return processes.Length == 0 ? null : new ProcessHandle(processes[0]);
    }
    
    /// <summary>
    /// Initialize a process handle for the specified process.
    /// </summary>
    public ProcessHandle(Process process) {
      Process = process;
      Process.Exited += OnExit;
      Process.EnableRaisingEvents = true;
    }
    
    /// <summary>
    /// Start a new process with the specified arguments.
    /// </summary>
    public ProcessHandle(string path, bool show, bool standardIO = true, Dictionary<string,string> arguments = null, char delimiter = Chars.Equal) {
      
      ProcessStartInfo startInfo = new ProcessStartInfo();
      
      // set the process args
      var split = path.Split(path.LastIndexOf(Path.DirectorySeparatorChar), 1);
      startInfo.FileName = path;
      startInfo.WorkingDirectory = split.ArgA;
      startInfo.ErrorDialog = false;
      startInfo.LoadUserProfile = true;
      startInfo.CreateNoWindow = !show;
      startInfo.UseShellExecute = show;
      
      startInfo.RedirectStandardError =
        startInfo.RedirectStandardInput =
        startInfo.RedirectStandardOutput =
        standardIO;
      
      startInfo.WindowStyle = show ? ProcessWindowStyle.Normal : ProcessWindowStyle.Hidden;
      
      // are the arguments set?
      if(arguments != null) {
        // yes, concatinate all the process arguments to be passed
        var sb = StringBuilderCache.Get();
        bool first = true;
        foreach(var entry in arguments) {
          if(first) first = false;
          else sb.Append(Chars.Space);
          sb.Append(entry.Key);
          if(entry.Value != null) {
            sb.Append(delimiter);
            sb.Append(entry.Value);
          }
        }
        // construct the start info including the arguments
        startInfo.Arguments = StringBuilderCache.SetAndGet(sb);
      }
      
      // start the process
      Process = System.Diagnostics.Process.Start(startInfo);
      Process.Exited += OnExit;
      Process.EnableRaisingEvents = true;
    }
    
    /// <summary>
    /// Dispose of the process handle.
    /// </summary>
    public void Dispose() {
      if(Process != null) {
        if(!Process.HasExited) Process.Kill();
        Process.Dispose();
        Process = null;
      }
    }
    
    /// <summary>
    /// Exit the associated process, optionally waiting for the process to end for the specified milliseconds.
    /// </summary>
    public bool End(int milliseconds) {
      if(Process.WaitForExit(milliseconds)) return true;
      Process.Kill();
      return false;
    }
    
    /// <summary>
    /// Exit the associated process.
    /// </summary>
    public void End() {
      Process.Kill();
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// On process exit.
    /// </summary>
    protected void OnExit(object sender, EventArgs e) {
      if(_onEnded != null) {
        _onEnded.ArgA = this;
        _onEnded.Run();
      }
      Dispose();
    }
    
  }
  
}

/*
 * User: Joshua
 * Date: 3/06/2016
 * Time: 9:35 PM
 */

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

using Efz.Logs;

namespace Efz {
  
  /// <summary>
  /// Manages (i mean really manages) logging to the console.
  /// </summary>
  public static class Log {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Space taken between the start of a new console line and message content.
    /// </summary>
    public const string PrefixBuffer = "                                               ";
    
    /// <summary>
    /// Width of the console window.
    /// </summary>
    public static int Width;
    /// <summary>
    /// Height of the console window.
    /// </summary>
    public static int Height;
    
    /// <summary>
    /// Event triggered when a log message is written.
    /// </summary>
    public static event Action<ILogEvent> OnLog;
    
    /// <summary>
    /// Standard output stream.
    /// </summary>
    public static System.IO.StreamWriter StandardOutput;
    /// <summary>
    /// Standard error stream.
    /// </summary>
    public static System.IO.StreamWriter StandardError;
    
    //-------------------------------------------//
    
    /// <summary>
    /// A space character.
    /// </summary>
    private const char _space           = ' ';
    /// <summary>
    /// End of scentence character.
    /// </summary>
    private const char _dot             = '.';
    /// <summary>
    /// Colon character. As opposed to a semi-colon.
    /// </summary>
    private const char _colon           = ':';
    /// <summary>
    /// Used to separate sources from messages.
    /// </summary>
    private const string _at            = " > ";
    
    /// <summary>
    /// Date time format for log messages.
    /// </summary>
    private const string _dateFormat    = "dd/MM/yyyy HH:mm:ss:fff";
    
    //-------------------------------------------//
    
    static Log() {
      Width = 200;
      Height = 50;
      StandardOutput = new System.IO.StreamWriter(Console.OpenStandardOutput(), Console.OutputEncoding);
      StandardError = new System.IO.StreamWriter(Console.OpenStandardError(), Console.OutputEncoding);
    }
    
    /// <summary>
    /// Draw the string value of an object in the center of the console window.
    /// </summary>
    public static void Line(string message = null, string span = "▓") {
      StringBuilder builder = StringBuilderCache.Get();
      if (!string.IsNullOrEmpty(message)) {
        int halfWidth = Width / 2 - message.Length / 2 - 1;
        while (builder.Length < halfWidth) {
          builder.Append(span);
        }
        builder.Append(_space);
        builder.Append(message);
        builder.Append(_space);
      }
      while(builder.Length < Width - span.Length) builder.Append(span);
      if(builder.Length < Width) builder.Append(span.Substring(0, Width - builder.Length));
      if(OnLog == null) new LogLine(StringBuilderCache.SetAndGet(builder)).Write();
      else OnLog(new LogLine(StringBuilderCache.SetAndGet(builder)));
    }
    
    // -------------------------------------------------------------------- Information
    
    /// <summary>
    /// Debugging stats.
    /// </summary>
    [Conditional("INFO")]
    public static void Stat(string str) {
      OnLog(new LogStat(str));
    }
    
    /// <summary>
    /// Log information about the current state of the application.
    /// </summary>
    [Conditional("INFO")]
    public static void Info(object message, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0) {
      StringBuilder builder = StringBuilderCache.Get();
      builder.Append(DateTime.Now.ToString(_dateFormat));
      builder.Append(_space);
      builder.Append(message ?? "Null");
      builder.Append(_at);
      builder.Append(System.IO.Path.GetFileNameWithoutExtension(sourceFilePath));
      builder.Append(_dot);
      builder.Append(memberName);
      builder.Append(_colon);
      builder.Append(sourceLineNumber);
      if(OnLog == null) new LogInfo(StringBuilderCache.SetAndGet(builder)).Write();
      else OnLog(new LogInfo(StringBuilderCache.SetAndGet(builder)));
    }
    
    /// <summary>
    /// Log information about the current state of the application.
    /// </summary>
    [Conditional("INFO")]
    public static void Info(object message, Exception ex, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0) {
      StringBuilder builder = StringBuilderCache.Get();
      builder.Append(DateTime.Now.ToString(_dateFormat));
      builder.Append(_space);
      builder.Append(message ?? "Null");
      builder.Append(_at);
      builder.Append(System.IO.Path.GetFileNameWithoutExtension(sourceFilePath));
      builder.Append(_dot);
      builder.Append(memberName);
      builder.Append(_colon);
      builder.Append(sourceLineNumber);
      builder.AppendLine();
      builder.Append(ex.Message);
      builder.AppendLine();
      builder.Append(ex.StackTrace);
      if(OnLog == null) new LogInfo(StringBuilderCache.SetAndGet(builder)).Write();
      else OnLog(new LogInfo(StringBuilderCache.SetAndGet(builder)));
    }
    
    // -------------------------------------------------------------------- Debug
    
    /// <summary>
    /// Log any non-critical conditions that are outside normal functionality.
    /// </summary>
    [Conditional("DEBUG")]
    public static void Debug(object message, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0) {
      StringBuilder builder = StringBuilderCache.Get();
      builder.Append(DateTime.Now.ToString(_dateFormat));
      builder.Append(_space);
      builder.Append(message ?? "Null");
      builder.Append(_at);
      builder.Append(System.IO.Path.GetFileNameWithoutExtension(sourceFilePath));
      builder.Append(_dot);
      builder.Append(memberName);
      builder.Append(_colon);
      builder.Append(sourceLineNumber);
      if(OnLog == null) new LogDebug(StringBuilderCache.SetAndGet(builder)).Write();
      else OnLog(new LogDebug(StringBuilderCache.SetAndGet(builder)));
    }
    
    /// <summary>
    /// Log any non-critical conditions that are outside normal functionality.
    /// </summary>
    [Conditional("DEBUG")]
    public static void Debug(object message, Exception ex, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0) {
      StringBuilder builder = StringBuilderCache.Get();
      builder.Append(DateTime.Now.ToString(_dateFormat));
      builder.Append(_space);
      builder.Append(message ?? "Null");
      builder.Append(_at);
      builder.Append(System.IO.Path.GetFileNameWithoutExtension(sourceFilePath));
      builder.Append(_dot);
      builder.Append(memberName);
      builder.Append(_colon);
      builder.Append(sourceLineNumber);
      builder.AppendLine();
      builder.Append(ex.Message);
      builder.AppendLine();
      builder.Append(ex.StackTrace);
      if(OnLog == null) new LogDebug(StringBuilderCache.SetAndGet(builder)).Write();
      else OnLog(new LogDebug(StringBuilderCache.SetAndGet(builder)));
    }
    
    // -------------------------------------------------------------------- Warning
    
    /// <summary>
    /// Log any incorrect usages or input parameters.
    /// </summary>
    [Conditional("WARNING")]
    public static void Warning(object message, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0) {
      StringBuilder builder = StringBuilderCache.Get();
      builder.Append(DateTime.Now.ToString(_dateFormat));
      builder.Append(_space);
      builder.Append(message ?? "Null");
      builder.Append(_at);
      builder.Append(System.IO.Path.GetFileNameWithoutExtension(sourceFilePath));
      builder.Append(_dot);
      builder.Append(memberName);
      builder.Append(_colon);
      builder.Append(sourceLineNumber);
      if(OnLog == null) new LogWarning(StringBuilderCache.SetAndGet(builder)).Write();
      else OnLog(new LogWarning(StringBuilderCache.SetAndGet(builder)));
    }
    
    /// <summary>
    /// LLog any incorrect usages or input parameters.
    /// </summary>
    [Conditional("WARNING")]
    public static void Warning(object message, Exception ex, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0) {
      StringBuilder builder = StringBuilderCache.Get();
      builder.Append(DateTime.Now.ToString(_dateFormat));
      builder.Append(_space);
      builder.Append(message ?? "Null");
      builder.Append(_at);
      builder.Append(System.IO.Path.GetFileNameWithoutExtension(sourceFilePath));
      builder.Append(_dot);
      builder.Append(memberName);
      builder.Append(_colon);
      builder.Append(sourceLineNumber);
      builder.AppendLine();
      builder.Append(ex.Message);
      builder.AppendLine();
      builder.Append(ex.StackTrace);
      if(OnLog == null) new LogWarning(StringBuilderCache.SetAndGet(builder)).Write();
      else OnLog(new LogWarning(StringBuilderCache.SetAndGet(builder)));
    }
    
    // -------------------------------------------------------------------- Error
    
    /// <summary>
    /// Log any potentially application breaking conditions.
    /// </summary>
    [Conditional("ERROR")]
    public static void Error(object message, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0) {
      StringBuilder builder = StringBuilderCache.Get();
      builder.Append(DateTime.Now.ToString(_dateFormat));
      builder.Append(_space);
      builder.Append(message ?? "Null");
      builder.Append(_at);
      builder.Append(System.IO.Path.GetFileNameWithoutExtension(sourceFilePath));
      builder.Append(_dot);
      builder.Append(memberName);
      builder.Append(_colon);
      builder.Append(sourceLineNumber);
      builder.AppendLine();
      builder.Append(new StackTrace(1, true));
      if(OnLog == null) new LogError(StringBuilderCache.SetAndGet(builder)).Write();
      else OnLog(new LogError(StringBuilderCache.SetAndGet(builder)));
    }
    
    /// <summary>
    /// Log any potentially application breaking conditions.
    /// </summary>
    [Conditional("ERROR")]
    public static void Error(object message, Exception ex, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0) {
      StringBuilder builder = StringBuilderCache.Get();
      builder.Append(DateTime.Now.ToString(_dateFormat));
      builder.Append(_space);
      builder.Append(message ?? "Null");
      builder.Append(_at);
      builder.Append(System.IO.Path.GetFileNameWithoutExtension(sourceFilePath));
      builder.Append(_dot);
      builder.Append(memberName);
      builder.Append(_colon);
      builder.Append(sourceLineNumber);
      builder.AppendLine();
      builder.Append(ex.Message);
      builder.AppendLine();
      builder.Append(ex.StackTrace);
      if(OnLog == null) new LogError(StringBuilderCache.SetAndGet(builder)).Write();
      else OnLog(new LogError(StringBuilderCache.SetAndGet(builder)));
    }
    
    // -------------------------------------------------------------------- Error
    
    /// <summary>
    /// Log a major change occurance. Blocks the thread for the specified amount of time, logging each second.
    /// </summary>
    public static void CriticalChange(string message, int seconds) {
      
      StringBuilder builder = StringBuilderCache.Get();
      const string span = ">> ";
      
      // iterate while there are seconds remaining
      while(seconds >= 0) {
        
        int halfWidth = Width / 2 - message.Length / 2 - 1;
        while (builder.Length < halfWidth) builder.Append(span);
        builder.Append(_space);
        builder.Append(message ?? "Null");
        builder.Append(_space);
        
        while(builder.Length < Width - span.Length) builder.Append(span);
        if(builder.Length < Width) builder.Append(span.Substring(0, Width - builder.Length));
        
        builder.Append(_space, 3);
        builder.Append(seconds);
        builder.Append(" seconds remaining . . . ");
        
        if(OnLog == null) new LogCriticalChange(builder.ToString()).Write();
        else OnLog(new LogCriticalChange(builder.ToString()));
        
        builder.Length = 0;
        
        // sleep for a second
        System.Threading.Thread.Sleep(1000);
        
        --seconds;
      }
      
      // pass the builder back to the string builder cache
      StringBuilderCache.Set(builder);
      
    }
    
    // -------------------------------------------------------------------- Input Helpers
    
    /// <summary>
    /// Read console input. Displays the specified message before reading input. Optionally specify
    /// a predicate that returns an error message for invalid input, or accepts by returning 'Null'.
    /// Optionally require confirmation of accepted input.
    /// </summary>
    public static string Read(string message, System.Func<string, string> predicate = null, bool confirm = true) {
      string result = null;
      
      Console.WriteLine("__________________________________________");
      Console.WriteLine();
      
      // end the input if confirmed
      bool confirmed = false;
      
      while(!confirmed) {
        
        // write the requirement message
        Console.Write(message + " : ");
        
        // read the input
        result = Console.ReadLine();
        
        // was the predicate specified?
        if(predicate != null) {
          // yes, run the predicate function
          string error = predicate(result);
          
          // was there a problem with the input?
          if(error != null) {
            // yes, write the error
            Console.WriteLine(error);
            continue;
          }
        }
        
        // is confirmation required?
        if(confirm) {
          // iterate while not confirmed
          while(true) {
            
            // write query of confirmation
            Console.Write("Confirm '"+result+"'? (Y/N) : ");
            
            // get a confirmation key
            var keyInfo = Console.ReadKey();
            
            // confirmed?
            if(keyInfo.Key == ConsoleKey.Y) {
              // yes, end input
              Console.WriteLine(" Confirmed");
              confirmed = true;
              break;
            }
            if(keyInfo.Key == ConsoleKey.N) {
              // no, start again
              Console.WriteLine(" Cancelled");
              confirmed = false;
              break;
            }
            
            // notify of invalid character
            Console.WriteLine("Invalid key. Please press 'Y' to confirm or 'N' to cancel and re-enter.");
            
          }
        } else {
          confirmed = true;
        }
        
      }
      
      Console.WriteLine("__________________________________________");
      
      return result;
    }
    
    // -------------------------------------------------------------------- Debugging Helpers
    
    /// <summary>
    /// Short hand live console logging. This will not be sent to the
    /// 'OnLog' callback.
    /// </summary>
    [Conditional("DEBUG")]
    public static void D(object message) {
      StringBuilder builder = StringBuilderCache.Get();
      builder.Append(DateTime.Now.ToString(_dateFormat));
      builder.Append(_space);
      builder.Append(message == null ? "null" : message.ToString());
      if(OnLog == null) new LogLiveDebug(StringBuilderCache.SetAndGet(builder)).Write();
      else OnLog(new LogLiveDebug(StringBuilderCache.SetAndGet(builder)));
    }
    
    /// <summary>
    /// Short hand live console logging with stacktrace. This will not be sent to the
    /// 'OnLog' callback.
    /// </summary>
    [Conditional("DEBUG")]
    public static void F(object message) {
      StringBuilder builder = StringBuilderCache.Get();
      builder.Append(DateTime.Now.ToString(_dateFormat));
      builder.Append(_space);
      builder.AppendLine(message == null ? "null" : message.ToString());
      builder.Append(new StackTrace(1, true));
      if(OnLog == null) new LogLiveDebug(StringBuilderCache.SetAndGet(builder)).Write();
      else OnLog(new LogLiveDebug(StringBuilderCache.SetAndGet(builder)));
    }
    
    //-------------------------------------------//
    
  }
}

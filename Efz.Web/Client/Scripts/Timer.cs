/*
 * User: Bob
 * Date: 14/11/2016
 * Time: 10:50
 */
using System;
using System.Text;
using Efz.Web.Display;

namespace Efz.Web.Client.Scripts {
  
  /// <summary>
  /// A javascript.
  /// </summary>
  public class Timer : Script {
    
    //----------------------------------//
    
    /// <summary>
    /// The time before the ending script executes.
    /// </summary>
    public long Milliseconds;
    /// <summary>
    /// Repeat the timer.
    /// </summary>
    public bool Repeat;
    /// <summary>
    /// Script executed after the timer completes.
    /// </summary>
    public Script Script;
    
    /// <summary>
    /// Script to run to clear this timeout.
    /// </summary>
    public Script Stop {
      get {
        if(_stop == null) {
          _var = "TODO";
          _stop = new CustomScript(Element, "clearTimeout(" + _var + ");");
        }
        return _stop;
      }
    }
    
    //----------------------------------//
    
    /// <summary>
    /// Inner script to stop the timer.
    /// </summary>
    protected Script _stop;
    /// <summary>
    /// Var name of the timer variable.
    /// </summary>
    protected string _var;
    
    //----------------------------------//
    
    /// <summary>
    /// Initialize a timer that will execute the specified Script after the
    /// specified amount of time.
    /// </summary>
    public Timer(Element element, long milliseconds, bool repeat, Script script) : base(element) {
      Milliseconds = milliseconds;
      Repeat = repeat;
      Script = script;
    }
    
    //----------------------------------//
    
    /// <summary>
    /// Get this script component as javascript.
    /// </summary>
    protected override void Append(StringBuilder builder) {
      const string iterate = "setInterval(";
      const string timeout = "setTimeout(";
      
      // has the stop script been assigned?
      if(Stop != null) {
        // yes, append a variable that can be assigned to the timer
        builder.Append(_var);
        builder.Append(Chars.Equal);
      }
      
      // is the timer to iterate?
      if(Repeat) {
        // yes, append the interval timer
        builder.Append(iterate);
        builder.Append(Script.Name);
        builder.Append(Chars.Comma);
        builder.Append(Milliseconds);
        builder.Append(_closeFuncion);
      } else {
        // no, append the single timeout script
        builder.Append(timeout);
        builder.Append(Script.Name);
        builder.Append(Chars.Comma);
        builder.Append(Milliseconds);
        builder.Append(_closeFuncion);
      }
    }
    
  }
  
}

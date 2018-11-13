/*
 * User: Bob
 * Date: 10/11/2016
 * Time: 00:39
 */
/*
 * User: Joshua
 * Date: 6/08/2016
 * Time: 6:11 PM
 */
using System;

using Efz.Tools;

namespace Efz.Web
{
  /// <summary>
  /// Description of NetAccessMonitor.
  /// </summary>
  public class WebAccessMonitor {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Interval between web access checks. A value of '0' removes auto-checks.
    /// </summary>
    public long Interval {
      get {
        return _interval;
      }
      set {
        _interval = value;
        if(_interval > 0) {
          if(_watch == null) _watch = new Watch(_interval, true, Check);
          else _watch.Time = _interval;
        } else if(_watch != null) {
          _watch.Dispose();
          _watch = null;
        }
      }
    }
    
    /// <summary>
    /// Is web access available.
    /// </summary>
    public bool HasAccess {
      get {
        // are auto-updates enabled?
        if(_interval > 0) {
          // yes, return
          return _hasAccess;
        }
        // no, run a check
        Check();
        return _hasAccess;
      }
    }
    
    /// <summary>
    /// Callback when web access has been established or lost.
    /// </summary>
    public IAction<bool> OnAccessChanged;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Inner flag for internet access being available.
    /// </summary>
    private bool _hasAccess;
    /// <summary>
    /// Inner interval between access checks.
    /// </summary>
    private long _interval;
    /// <summary>
    /// Watch that controls incrementally checking web access.
    /// </summary>
    private Watch _watch;
    
    /// <summary>
    /// The urls used to check for internet access.
    /// </summary>
    private readonly string[] _urls;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Construct a new web access monitor.
    /// </summary>
    public WebAccessMonitor(int interval, IAction<bool> onAccessChanged, params string[] urls) {
      Interval = interval;
      OnAccessChanged = onAccessChanged;
      _urls = urls;
      if(_urls.Length == 0) throw new ArgumentException("Urls are required to check internet access.");
      Check();
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// Check for web access.
    /// </summary>
    private void Check() {
      string url = _urls.Random();
      try {
        // attempt a connection to a random url
        using (var client = new System.Net.WebClient())
        using (var stream = client.OpenRead(url)) {
          if(!_hasAccess && OnAccessChanged != null) {
            OnAccessChanged.ArgA = true;
            OnAccessChanged.Run();
          }
          _hasAccess = true;
        }
      } catch {
        if(_hasAccess && OnAccessChanged != null) {
          OnAccessChanged.ArgA = false;
          OnAccessChanged.Run();
        }
        _hasAccess = false;
      }
    }
    
  }
  
}

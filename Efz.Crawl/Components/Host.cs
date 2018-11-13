/*
 * User: Joshua
 * Date: 1/08/2016
 * Time: 7:29 PM
 */
using System;
using Efz.Collections;
using Efz.Threading;
using Efz.Tools;
using Efz.Web;

namespace Efz.Crawl {
  
  /// <summary>
  /// Information associated with an address crawlers may attempt to 
  /// visit and parse.
  /// </summary>
  public class Host : IDisposable {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Number of milliseconds a host can remain idle before submitting DB changes.
    /// </summary>
    public static long Timeout = 5000;
    /// <summary>
    /// The max urls to keep in memory before committing.
    /// </summary>
    public static int MaxUrls = 20;
    
    /// <summary>
    /// The domain host.
    /// </summary>
    public readonly string Name;
    /// <summary>
    /// Is the host new?
    /// </summary>
    public bool New;
    
    /// <summary>
    /// Has the host been disposed of?
    /// </summary>
    public bool Disposed;
    
    /// <summary>
    /// The score is a figure basically representing;
    /// a global starting domain score +
    /// times an asset has been discovered * 2 -
    /// times there was a failed attempt at accessing this domain * 2 -
    /// number of sub-urls that have been accessed
    /// </summary>
    public int Score {
      get {
        _lock.Take();
        int s = _score;
        _lock.Release();
        return s;
      }
      set {
        _lock.Take();
        _score = value;
        // is the host a dud?
        if(_score < 0) {
          if(_scoreLog) {
            _scoreLog = false;
            //Log.D("Host '" + Name + "' has been found to be a dud.");
          }
          // yes, remove unrequired urls
          _session.OnIgnoreHost(this);
        } else if (_score > _session.ParseControl.HostMaxScore) {
          if(_scoreLog) {
            _scoreLog = false;
            //Log.D("Host '" + Name + "' has hit max score with " + _score + ".");
          }
          _score = _session.ParseControl.HostMaxScore;
        }
        _changed = true;
        _lock.Release();
      }
    }
    
    /// <summary>
    /// The number of urls parsed from this host.
    /// </summary>
    public int Count {
      get {
        _lock.Take();
        int c = _count;
        _lock.Release();
        return c;
      }
      set {
        _lock.Take();
        _count = value;
        _changed = true;
        _lock.Release();
      }
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// New Urls to be parsed.
    /// </summary>
    private ArrayRig<Url> _newUrls;
    /// <summary>
    /// Old Urls that have been parsed.
    /// </summary>
    private ArrayRig<Url> _oldUrls;
    
    /// <summary>
    /// The current host score.
    /// </summary>
    private int _score;
    /// <summary>
    /// The current url count.
    /// </summary>
    private int _count;
    
    /// <summary>
    /// Has the host been changed?
    /// </summary>
    private bool _changed;
    
    /// <summary>
    /// Lock for external manipulation.
    /// </summary>
    private readonly Lock _lock;
    
    /// <summary>
    /// Task to commit changes.
    /// </summary>
    private Act _commit;
    /// <summary>
    /// Is the commit task currently running?
    /// </summary>
    private bool _committing;
    
    /// <summary>
    /// Has the score log been used?
    /// </summary>
    private bool _scoreLog;
    
    /// <summary>
    /// Parse controller for this host.
    /// </summary>
    private readonly CrawlSession _session;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize a host instance with the specified name and score.
    /// </summary>
    public Host(CrawlSession session, string name, int score, int count, bool isNew = false) {
      _session = session;
      
      Name = name;
      _score = score;
      _count = count;
      New = isNew;
      
      _newUrls = new ArrayRig<Url>();
      _oldUrls = new ArrayRig<Url>();
      
      _changed = true;
      _lock = new Lock();
      _commit = new Act(Commit);
      
      _scoreLog = true;
    }
    
    /// <summary>
    /// Disposes of the host, committing any DB changes.
    /// </summary>
    public void Dispose() {
      if(!Disposed) {
        Disposed = true;
        // remove the host from url control
        _session.OnDisposeHost(this);
      }
      
      _lock.Take();
      if(_changed && !_committing) {
        _committing = true;
        _lock.Release();
        _commit.Run();
        return;
      }
      _newUrls.Dispose();
      _oldUrls.Dispose();
      _newUrls = null;
      _oldUrls = null;
      _commit  = null;
      _lock.Release();
    }
    
    /// <summary>
    /// Add a new Url to be parsed.
    /// </summary>
    public void AddNew(Url url) {
      _lock.Take();
      _newUrls.Add(url);
      _changed = true;
      if(_oldUrls.Count + _newUrls.Count == MaxUrls && !_committing) {
        _commit.Run();
        _committing = true;
      }
      _lock.Release();
    }
    
    /// <summary>
    /// Add an old url that has been parsed.
    /// </summary>
    public void AddOld(Url url) {
      _lock.Take();
      _oldUrls.Add(url);
      _changed = true;
      if(_oldUrls.Count + _newUrls.Count == MaxUrls && !_committing) {
        _commit.Run();
        _committing = true;
      }
      _lock.Release();
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// Commit urls and score to the DB.
    /// </summary>
    private void Commit() {
      _lock.Take();
      ArrayRig<Url> newUrls = _newUrls;
      _newUrls = new ArrayRig<Url>();
      ArrayRig<Url> oldUrls = _oldUrls;
      _oldUrls = new ArrayRig<Url>();
      int score = _score;
      _changed = false;
      _committing = false;
      _lock.Release();
      
      // iterate new urls
      foreach(Url url in newUrls) {
        // add to new urls table
        _session.OnNewUrl(url);
      }
      // iterate old urls
      foreach(Url url in oldUrls) {
        // add to old urls table
        _session.OnUrlParsed(url);
      }
      
      // commit host score changes
      _session.OnHostUpdate(this);
      
      if(Disposed) {
        Dispose();
      }
    }
    
  }
  
}

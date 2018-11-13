/*
 * User: Joshua
 * Date: 31/10/2016
 * Time: 6:46 PM
 */
using System;
using System.Collections.Generic;

using System.Text;
using Efz.Threading;
using Efz.Tools;

namespace Efz.Crawl {
  
  /// <summary>
  /// Crawler stats.
  /// </summary>
  public class Stats {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Time in milliseconds between each statistical log message.
    /// </summary>
    public int LogTime {
      get {
        return _logTime;
      }
      set {
        _logTime = value;
        if(_logTime > 0) {
          if(_logTimer == null) _logTimer = new Timer(_logTime, Show);
          else _logTimer.Reset(_logTime);
        } else _logTimer = null;
      }
    }
    
    /// <summary>
    /// Total time used by crawlers to connect to web pages.
    /// </summary>
    public long ConnectTime;
    /// <summary>
    /// Total number of connections made.
    /// </summary>
    public long ConnectCount;
    
    /// <summary>
    /// Total number of failed connections.
    /// </summary>
    public long AttemptCount;
    
    /// <summary>
    /// Total time used by crawlers to successfully crawl web pages.
    /// </summary>
    public long ProcessTime;
    /// <summary>
    /// Total number of processed web pages.
    /// </summary>
    public long ProcessCount;
    
    /// <summary>
    /// Total time for urls being read from files.
    /// </summary>
    public long ReadTime;
    /// <summary>
    /// Total number of urls read from a file.
    /// </summary>
    public long ReadCount;
    /// <summary>
    /// Last time a url was read from a file.
    /// </summary>
    public long ReadTicks;
    
    /// <summary>
    /// Collection of asset count.
    /// </summary>
    public Dictionary<string, Teple<long, long>> Assets;
    /// <summary>
    /// Last time an asset of a particular type was created.
    /// </summary>
    public Dictionary<string, long> AssetTicks;
    
    /// <summary>
    /// Total number of assets discovered.
    /// </summary>
    public long AssetCount;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Time from the start of the stats.
    /// </summary>
    private readonly Timekeeper _time;
    /// <summary>
    /// The that the stats began.
    /// </summary>
    private DateTime _startTime;
    
    /// <summary>
    /// Lock for connection statistics.
    /// </summary>
    private Lock _connectLock;
    /// <summary>
    /// Lock for attempt statistics.
    /// </summary>
    private Lock _attemptLock;
    /// <summary>
    /// Lock for process statistics.
    /// </summary>
    private Lock _processLock;
    
    /// <summary>
    /// Lock for read urls from files.
    /// </summary>
    private Lock _readLock;
    
    /// <summary>
    /// Lock for asset statistics.
    /// </summary>
    private Lock _assetLock;
    
    /// <summary>
    /// Time between each log of the statistics.
    /// </summary>
    private int _logTime;
    /// <summary>
    /// Timer for the display of log stats.
    /// </summary>
    private Timer _logTimer;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize a new stats 
    /// </summary>
    public Stats(int logTime) {
      
      _connectLock = new Lock();
      _attemptLock = new Lock();
      _processLock = new Lock();
      _assetLock   = new Lock();
      _readLock    = new Lock();
      Assets     = new Dictionary<string, Teple<long, long>>();
      AssetTicks = new Dictionary<string, long>();
      
      _startTime = DateTime.Now;
      _time = new Timekeeper();
      _time.Start();
      
      LogTime = logTime;
    }
    
    public void UpdateConnect(long connectTime) {
      _connectLock.Take();
      ConnectTime += connectTime;
      ++ConnectCount;
      _connectLock.Release();
    }
    
    public void UpdateAttempt() {
      _attemptLock.Take();
      ++AttemptCount;
      _attemptLock.Release();
    }
    
    public void UpdateProcess(long processTime) {
      _processLock.Take();
      ProcessTime += processTime;
      ++ProcessCount;
      _processLock.Release();
    }
    
    public void UpdateAsset(string name) {
      _assetLock.Take();
      long milliseconds = _time.Milliseconds;
      if(Assets.ContainsKey(name)) {
        Teple<long, long> stats = Assets[name];
        ++stats.ArgA;
        stats.ArgB += milliseconds - AssetTicks[name];
        AssetTicks[name] = milliseconds;
      } else {
        Assets.Add(name, new Teple<long, long>(1, 0));
        AssetTicks.Add(name, milliseconds);
      }
      ++AssetCount;
      _assetLock.Release();
    }
    
    public void UpdateRead() {
      _readLock.Take();
      long milliseconds = _time.Milliseconds;
      if(ReadTicks != 0) ReadTime += milliseconds - ReadTicks;
      ReadTicks = milliseconds;
      ++ReadCount;
      _readLock.Release();
    }
    
    public void Show() {
      StringBuilder builder = StringBuilderCache.Get();
      
      builder.Append("------------- Crawling Stats ---------------\n");
      if(ReadCount != 0) {
        builder.Append("Avg Read Time       : ");
        builder.Append(ReadTime/ReadCount);
        builder.Append("ms");
        builder.AppendLine();
        builder.Append("Total Reads         : ");
        builder.Append(ReadCount);
      }
      
      if(ConnectCount != 0) {
        builder.AppendLine();
        builder.Append("Avg Connection Time : ");
        builder.Append(ConnectTime/ConnectCount);
        builder.Append("ms");
        builder.AppendLine();
        builder.Append("Total Connections   : ");
        builder.Append(ConnectCount);
      }
      
      builder.AppendLine();
      builder.Append("Total Attempts      : ");
      builder.Append(AttemptCount);
      
      if(ProcessCount != 0) {
        builder.AppendLine();
        builder.Append("Avg Processing Time : ");
        builder.Append(ProcessTime/ProcessCount);
        builder.Append("ms");
        builder.AppendLine();
        builder.Append("Total Processed     : ");
        builder.Append(ProcessCount);
        
      }
      
      if(AssetCount != 0) {
        builder.AppendLine();
        builder.Append("Avg Time Per Asset  : ");
        builder.Append((_time.Milliseconds/AssetCount));
        builder.Append("ms");
        builder.AppendLine();
        builder.Append("Total Assets        : ");
        builder.Append(AssetCount);
        
        builder.AppendLine();
        builder.Append("Assets Found");
        foreach(KeyValuePair<string, Teple<long, long>> entry in Assets) {
          builder.AppendLine();
          builder.Append(entry.Key);
          builder.AppendLine();
          builder.Append("   Avg Time Per Asset : ");
          builder.Append(entry.Value.ArgB/entry.Value.ArgA);
          builder.Append("ms");
          builder.AppendLine();
          builder.Append("   Total Assets       : ");
          builder.Append(entry.Value.ArgA);
        }
      }
      
      builder.Append("\n------------- -------------- ---------------\n");
      
      Log.Stat(StringBuilderCache.SetAndGet(builder));
      
      // is the log timer set? yes, restart
      if(_logTime > 0) _logTimer.Reset(_logTime);
    }
    
    //-------------------------------------------//
    
  }
}

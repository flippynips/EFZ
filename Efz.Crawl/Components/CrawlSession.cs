/*
 * User: Joshua
 * Date: 26/06/2016
 * Time: 12:10 AM
 */
using System;
using System.Collections.Generic;

using Efz.Collections;
using Efz.Data;
using Efz.Text;
using Efz.Threading;
using Efz.Tools;
using Efz.Web;

namespace Efz.Crawl {
  
  /// <summary>
  /// Web scraper main class. Manages the initialization of crawlers and domain handling.
  /// </summary>
  public abstract class CrawlSession {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Is the web crawl currently running.
    /// </summary>
    public bool Running;
    
    /// <summary>
    /// Domains loading and saving.
    /// </summary>
    public UrlControl UrlControl;
    /// <summary>
    /// Processor of crawler assets.
    /// </summary>
    public ParseControl ParseControl;
    
    /// <summary>
    /// The max number of urls each crawler can retain.
    /// </summary>
    public int CrawlerUrlBufferSize;
    /// <summary>
    /// The max number of characters to process in any single stream buffer.
    /// </summary>
    public int CrawlerByteBuffer;
    /// <summary>
    /// The maximum number of bytes to stream from a single url.
    /// </summary>
    public int CrawlerMaxBytes;
    /// <summary>
    /// The minimum number of bytes to stream from a single url.
    /// </summary>
    public int CrawlerMinBytes;
    /// <summary>
    /// Max timeout milliseconds allowed to establish a connection.
    /// </summary>
    public int CrawlerMaxConnectTimeout;
    /// <summary>
    /// Min timeout milliseconds allowed to establish a connection.
    /// </summary>
    public int CrawlerMinConnectTimeout;
    /// <summary>
    /// Max timeout milliseconds allowed to download and process a single page.
    /// </summary>
    public int CrawlerMaxProcessTimeout;
    /// <summary>
    /// Min timeout milliseconds allowed to download and process a single page.
    /// </summary>
    public int CrawlerMinProcessTimeout;
    /// <summary>
    /// The number of cookies each Crawler can maintain.
    /// </summary>
    public int CrawlerMaxCookieCount;
    /// <summary>
    /// The max size of each cookie in the Crawler container.
    /// </summary>
    public int CrawlerMaxCookieSize;
    /// <summary>
    /// Crawler time limit.
    /// </summary>
    public int CrawlerTimeLimit {
      get {
        return _timeLimit;
      }
      set {
        if(_timeLimit == value) return;
        _lock.Take();
        _timeLimit = value;
        foreach(var crawler in Crawlers) crawler.TimeLimit = _timeLimit;
        _lock.Release();
      }
    }
    
    /// <summary>
    /// Path to a configuration file for persistance the crawl. Can be relative.
    /// </summary>
    public string Path;
    
    /// <summary>
    /// Optional callback on a url being discovered by a Crawler.
    /// </summary>
    public Func<Url, bool> OnUrl;
    /// <summary>
    /// Optional extractor for the web pages.
    /// </summary>
    public Extract PageExtractor;
    
    /// <summary>
    /// Should crawlers traverse domains?
    /// </summary>
    public bool TraverseDomains;
    
    /// <summary>
    /// Number of crawlers.
    /// </summary>
    public int CrawlerCount {
      get {
        return _crawlerCount;
      }
      set {
        _lock.Take();
        _crawlerCount = value;
        _lock.Release();
      }
    }
    
    /// <summary>
    /// Current crawler collection.
    /// </summary>
    internal ArrayAir<Crawler> Crawlers;
    /// <summary>
    /// Url assets and their relative score attribution.
    /// </summary>
    internal Dictionary<string, byte> FileAssets;
    
    /// <summary>
    /// Monitor for internet access to manage crawlers connections.
    /// </summary>
    internal WebAccessMonitor WebAccess;
    
    /// <summary>
    /// Has the session been paused for some reason.
    /// </summary>
    internal bool Paused;
    
    /// <summary>
    /// Collection of hosts that are accepted when traverse domains is false.
    /// </summary>
    internal HashSet<string> SeedHosts;
    
    //-------------------------------------------//
    
    /// <summary>
    /// The number of crawlers.
    /// </summary>
    private int _crawlerCount;
    /// <summary>
    /// Update timer to check the status of the crawlers.
    /// </summary>
    private Watch _updater;
    
    /// <summary>
    /// Time limit.
    /// </summary>
    private int _timeLimit;
    
    /// <summary>
    /// Extractor for root url files. Will be 'Null' if not parsing root url paths.
    /// </summary>
    private FileExtractor _fileExtractor;
    
    /// <summary>
    /// Lock for external access to crawl parameters.
    /// </summary>
    private readonly Lock _lock;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize a new crawling session, optionally with a custom path.
    /// Default is a relative path to the application.
    /// </summary>
    protected CrawlSession(string path = "crawler.Efz") {
      
      Path = path;
      
      SeedHosts = new HashSet<string>();
      Crawlers = new ArrayAir<Crawler>();
      FileAssets = new Dictionary<string, byte>();
      _lock = new Lock();
      
      ParseControl = new ParseControl(this);
      UrlControl = new UrlControl(this);
      
      WebAccess = new WebAccessMonitor(5000, null, "http://www.google.com", "https://www.duckduckgo.com/", "https://www.startpage.com/", "http://www.codeproject.com");
      
    }
    
    /// <summary>
    /// Start crawling the web with the current parameters.
    /// </summary>
    public void Start() {
      
      _lock.Take();
      // has the crawl started?
      if(Running) {
        // yes, skip the start
        _lock.Release();
        return;
      }
      Running = true;
      
      Log.Debug("Starting crawl");
      
      _lock.Release();
      
      // load the configuration
      ManagerResources.LoadConfig(Path, new Act<Configuration>(OnConfigLoad));
      
    }
    
    /// <summary>
    /// End the processing of web pages.
    /// </summary>
    public void Stop() {
      
      _lock.Take();
      // if not running - skip
      if(!Running) {
        _lock.Release();
        return;
      }
      // no longer running
      Running = false;
      
      _updater.Run = false;
      
      // stop and dispose of all Crawlers
      foreach(Crawler crawler in Crawlers) {
        crawler.Dispose();
      }
      // dispose of the collection
      Crawlers.Dispose();
      
      _lock.Release();
      
      // load the configuration
      ManagerResources.LoadConfig(Path, new Act<Configuration>(OnConfigSave));
      
    }
    
    /// <summary>
    /// Add a url asset to scrape for.
    /// </summary>
    public void AddFileAsset(string extension, byte score = 5) {
      FileAssets.Add(extension.ToUpper(), score);
    }
    
    /// <summary>
    /// Add a url to parse.
    /// </summary>
    public void AddUrl(string url) {
      Url u = Url.Parse(url.ToLowercase());
      if(u == Url.Empty) {
        Log.Warning("Url couldn't be parsed '"+url+"'.");
        return;
      }
      _lock.Take();
      UrlControl.AddUrl(u.ToString());
      SeedHosts.Add(u.Host.ToLowercase());
      _lock.Release();
    }
    
    /// <summary>
    /// Add a path to a url file to parse.
    /// </summary>
    public void AddUrlFile(string path) {
      _lock.Take();
      
      // has the extractor been setup?
      if(_fileExtractor == null) {
        
        // no, create an extractor of urls
        Extract extract = new ExtractUrl(
          new ActionSet<string, Crawler>(ParseControl.OnUrl),
          new ArrayRig<Protocol>(ParseControl.Protocols));
        
        // start a file extractor for the files with the url extractor
        _fileExtractor = new FileExtractor(new ArrayRig<string>(new [] { path }), extract);
        _fileExtractor.OnFile = new ActionSet<string>(OnCompleteUrlFile);
        _fileExtractor.Run();
        
      } else {
        
        // yes, add the file to the extractor
        _fileExtractor.AddFile(path);
        
      }
      
      _lock.Release();
    }
    
    /// <summary>
    /// On completion of a urls file being parsed completely. Saves the completed
    /// state in the configuration.
    /// </summary>
    internal void OnCompleteUrlFile(string path) {
      
      _lock.Take();
      
      // load the configuration
      Configuration config = ManagerResources.LoadConfig(Path);
      Node node = config.Node;
      
      // have any root url files been defined?
      if(node["Files"].ArraySet) {
        
        // yes, iterate the specified url files
        foreach(Node pathNode in node["Files"].Array) {
          
          // does the node represent the completed file path?
          if(pathNode.String == path) {
            
            // yes, set the parsed state
            pathNode["Parsed"].Bool = true;
            // save the config
            config.Save();
            
            _lock.Release();
            return;
          }
        }
      } else {
        
        // no, set the first element of the node array
        Node pathNode = node["Files"][0];
        pathNode.String = path;
        pathNode["Parsed"].Bool = true;
        
        // save the config
        config.Save();
        
      }
      
      Log.Warning("Completed urls file was not found in the crawler configuration.");
      
      _lock.Release();
      
    }
    
    /// <summary>
    /// Method run when a host record is created or updated.
    /// </summary>
    public abstract void OnHostUpdate(Host host);
    /// <summary>
    /// Method run when a host is being disposed and can be removed from caches.
    /// </summary>
    public abstract void OnDisposeHost(Host host);
    /// <summary>
    /// Function used to retrieve a host record using a url reference.
    /// </summary>
    public abstract Host GetHost(Url url);
    /// <summary>
    /// Initial check for whether a url should be skipped.
    /// </summary>
    public abstract bool ShouldParse(Url url);
    /// <summary>
    /// Method run on a host score falling below the threshold.
    /// </summary>
    public abstract void OnIgnoreHost(Host host);
    /// <summary>
    /// Method run when a url is deemed parse-worthy.
    /// </summary>
    public abstract void OnNewUrl(Url url);
    /// <summary>
    /// Method run on a url being parsed successfully.
    /// </summary>
    public abstract void OnUrlParsed(Url url);
    /// <summary>
    /// Method run to retrieve a new set of urls to be parsed. The desired number
    /// is passed as the second parameter.
    /// </summary>
    public abstract IEnumerable<Url> GetUrls(int count);
    
    //-------------------------------------------//
    
    /// <summary>
    /// Check the status of the Crawlers.
    /// </summary>
    protected void Update() {
      
      Log.Debug("Update called.");
      
      // do we have web access?
      if(WebAccess.HasAccess) {
        // yes, are we running
        if(Paused) {
          // no, start running again
          Paused = false;
        }
        
        // ensure crawlers have been started
        foreach(Crawler crawler in Crawlers) {
          if(!crawler.Running) crawler.Start();
        }
        
      } else {
        
        // no, stop all crawlers
        foreach(Crawler crawler in Crawlers) {
          if(crawler.Running) crawler.Stop();
        }
        
      }
      
      // do any crawlers need to be created or removed?
      if(Crawlers.Count != _crawlerCount) {
        
        // yes, remove as required
        while(Crawlers.Count > _crawlerCount) {
          Crawlers.Pop().Dispose();
        }
        
        // add as required
        while(Crawlers.Count < _crawlerCount) {
          Crawler crawler = new Crawler(this);
          crawler.TimeLimit = _timeLimit;
          Crawlers.Add(crawler);
          if(!Paused) crawler.Start();
        }
        
      }
      
    }
    
    /// <summary>
    /// Setup the session with a configuration Node.
    /// </summary>
    protected void OnConfigLoad(Configuration config) {
      
      _lock.Take();
      
      // skip if no longer running
      if(!Running) {
        _lock.Release();
        return;
      }
      
      // persist the config node
      Node node = config.Node;
      
      // setup updater watch
      _updater = new Watch(1000, true, Update, false);
      
      // number of crawlers to initialize with
      _crawlerCount = node.Default((ThreadHandle.HandleCount + 1) / 2, "Crawler_Count");
      
      // url control parameters
      UrlControl.UrlBufferSize        = node.Default(20, "Url_Buffer");
      
      // parse control parameters
      ParseControl.HostNewScore       = node.Default(50, "Host_New_Score");
      ParseControl.HostParseScore     = node.Default(-20, "Host_Parse_Score");
      ParseControl.HostAttemptScore   = node.Default(-40, "Host_Attempt_Score");
      ParseControl.HostAssetScore     = node.Default(10, "Host_Asset_Score");
      ParseControl.HostMaxScore       = node.Default(1000, "Host_Max_Score");
      
      // crawler parameters
      CrawlerUrlBufferSize     = node.Default(1000, "Url_Crawl_Buffer");
      CrawlerByteBuffer        = node.Default(4096, "Byte_Buffer");
      CrawlerMaxBytes          = (int)node.Default(Global.Kilobyte * 100, "Max_Bytes");
      CrawlerMinBytes          = (int)node.Default(Global.Kilobyte * 55, "Min_Bytes");
      CrawlerMaxConnectTimeout = node.Default(5000, "Max_Connect_Timeout");
      CrawlerMinConnectTimeout = node.Default(3000, "Min_Connect_Timeout");
      CrawlerMaxProcessTimeout = node.Default(10000, "Max_Process_Timeout");
      CrawlerMinProcessTimeout = node.Default(7000, "Min_Process_Timeout");
      CrawlerMaxCookieCount    = node.Default(8, "Max_Cookie_Count");
      CrawlerMaxCookieSize     = node.Default(3435, "Max_Cookie_Size");
      
      // were any identities specified?
      if(node["Identities"].ArraySet) {
        
        // iterate the defined headers
        foreach(Node identityNode in node["Identities"].Array) {
          if(identityNode.DictionarySet) {
            
            // start the name value collection
            Identity identity = new Identity(true);
            
            // iterate the headers in the identity
            foreach(KeyValuePair<string,Node> entry in identityNode.Dictionary) {
              identity.Add(entry.Key, entry.Value.String);
            }
            
          }
          
        }
        
      }
      
      // have the url root files been set?
      if(node["Files"].ArraySet) {
        
        // yes, add the root file paths to the file extractor
        foreach(Node pathNode in node["Files"].Array) {
          if(!pathNode["Parsed"].Bool) {
            _lock.Release();
            AddUrlFile(pathNode.String);
            _lock.Take();
          }
        }
        
      }
      
      // run the task machine to start the session
      TaskMachine tm = new TaskMachine();
      tm.AddOnDone(() => _updater.Run = true);
      tm.Add("Set up parse control", ParseControl.Start);
      tm.Add("Set up URL control", UrlControl.Start);
      
      #if INFO
      tm.OnTask = new ActionSet<string>(s => Log.Info(s));
      #endif
      
      tm.Run();
      
      config.Save();
      
      _lock.Release();
      
      Log.Debug("Crawler session configuration loaded.");
    }
    
    /// <summary>
    /// Save the configuration.
    /// </summary>
    protected void OnConfigSave(Configuration config) {
      
      _lock.Take();
      if(Running) {
        _lock.Release();
        return;
      }
      
      Node node = config.Node;
      
      // set configuration parameters
      node["Crawler_Count"].Int32 = CrawlerCount;
      
      node["Url_Buffer"].Int32 = UrlControl.UrlBufferSize;
      
      node["Host_New_Score"].Int32 = ParseControl.HostNewScore;
      node["Host_Parse_Score"].Int32 = ParseControl.HostParseScore;
      node["Host_Attempt_Score"].Int32 = ParseControl.HostAttemptScore;
      node["Host_Asset_Score"].Int32 = ParseControl.HostAssetScore;
      node["Host_Max_Score"].Int32 = ParseControl.HostMaxScore;
      
      node["Url_Crawl_Buffer"].Int32 = CrawlerUrlBufferSize;
      node["Byte_Buffer"].Int32 = CrawlerByteBuffer;
      node["Max_Bytes"].Int32 = CrawlerMaxBytes;
      node["Min_Bytes"].Int32 = CrawlerMinBytes;
      node["Max_Connection_Timeout"].Int32 = CrawlerMaxConnectTimeout;
      node["Min_Connection_Timeout"].Int32 = CrawlerMinConnectTimeout;
      node["Max_Process_Timeout"].Int32 = CrawlerMaxProcessTimeout;
      node["Min_Process_Timeout"].Int32 = CrawlerMinProcessTimeout;
      node["Max_Cookie_Count"].Int32 = CrawlerMaxCookieCount;
      node["Max_Cookie_Size"].Int32 = CrawlerMaxCookieSize;
      
      // save the configuration
      config.Save();
      
      _lock.Release();
    }
    
    //-------------------------------------------//
    
  }
  
}

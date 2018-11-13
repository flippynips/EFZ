/*
 * User: Joshua
 * Date: 1/08/2016
 * Time: 7:29 PM
 */
using System;

using Efz.Collections;
using Efz.Threading;
using Efz.Web;

namespace Efz.Crawl {
  
  /// <summary>
  /// Manages the retrieval and saving or urls.
  /// </summary>
  public class UrlControl {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Method used by Crawlers to get a url.
    /// </summary>
    internal Url NextUrl {
      get {
        
        // does the buffer need to be filled?
        if(_urls.Count < UrlBufferSize && _loadingDomains.TryTake) {
          // yes, start a task to populate the buffer
          ManagerUpdate.Control.AddSingle(PopulateUrlBuffer);
        }
        
        // take the lock
        _urlsLock.Take();
        
        // is there a url in the buffer?
        if(_urls.Dequeue()) {
          
          // yes, unlock
          var url = _urls.Current;
          _urlsLock.Release();
          
          if(url.Host.Contains("endchan")) {
            Log.D("Crawling endchan url : " + url);
          }
          
          // return the next url
          return url;
        }
        
        // unlock
        _urlsLock.Release();
        
        // no url in the buffer
        return Url.Empty;  
      }
    }
    
    // -------------- Serialized
    
    /// <summary>
    /// The minimum number of urls to have in reserve.
    /// The collection will roughly remain double this.
    /// </summary>
    public int UrlBufferSize    = 20;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Lock to indicate domains ar loading into the buffer.
    /// </summary>
    private readonly Lock _loadingDomains;
    
    /// <summary>
    /// The urls that are queued to be processed by any Crawler.
    /// </summary>
    private readonly Queue<Url> _urls;
    /// <summary>
    /// Lock for accessing urls.
    /// </summary>
    private readonly Lock _urlsLock;
    
    /// <summary>
    /// The master crawl session for this url control.
    /// </summary>
    private readonly CrawlSession _session;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize, start reading domain files from the path specified.
    /// </summary>
    public UrlControl(CrawlSession session) {
      
      _session = session;
      // initialize the queue of domains to process
      _urls = new Queue<Url>();
      _urlsLock = new Lock();
      _loadingDomains = new Lock();
      
    }
    
    /// <summary>
    /// Start processing domains to be crawled.
    /// </summary>
    public void Start() {
      _loadingDomains.Take();
      PopulateUrlBuffer();
    }
    
    /// <summary>
    /// Add a url to be parsed.
    /// </summary>
    public void AddUrl(string url) {
      Url parsed = Url.Parse(url);
      
      // ensure the host won't be skipped
      var host = _session.GetHost(parsed);
      if(host == null) host = new Host(_session, parsed.Host, _session.ParseControl.HostNewScore, 0, true);
      host.Score = _session.ParseControl.HostNewScore;
      _urlsLock.Take();
      _urls.Enqueue(parsed);
      _urlsLock.Release();
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// Populate the url buffer from the DB.
    /// </summary>
    private void PopulateUrlBuffer() {
      
      // populate the urls collection from the top of the NewUrls collection
      foreach(Url url in _session.GetUrls(UrlBufferSize)) {
        _urls.Enqueue(url);
      }
      
      // release the populating urls lock
      _loadingDomains.Release();
      
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
  }
  
}

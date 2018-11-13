/*
 * User: Joshua
 * Date: 6/08/2016
 * Time: 6:11 PM
 */
using System;

using Efz.Collections;
using Efz.Data;
using Efz.Text;
using Efz.Tools;
using Efz.Web;

namespace Efz.Crawl {
  
  /// <summary>
  /// Handles processed addresses and the results of scraping.
  /// </summary>
  public class ParseControl {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Default new host score. The max score for any Domain is
    /// twice this number.
    /// </summary>
    public int HostNewScore     = 100;
    /// <summary>
    /// Score modifier on a discovered asset on top of the asset score.
    /// </summary>
    public int HostAssetScore   = 20;
    /// <summary>
    /// Score modifier on an unsuccessful connection attempt.
    /// </summary>
    public int HostAttemptScore = -80;
    /// <summary>
    /// Score modifier on a url being crawled.
    /// </summary>
    public int HostParseScore   = -30;
    /// <summary>
    /// Upper limit to the score a host can achieve.
    /// </summary>
    public int HostMaxScore     = 500;
    
    /// <summary>
    /// Protocols to be processed.
    /// </summary>
    public Protocol[] Protocols = { Protocol.Http, Protocol.Https };
    
    /// <summary>
    /// Statistics about the current crawling session.
    /// </summary>
    public Stats Stats;
    
    /// <summary>
    /// Extractor of urls.
    /// </summary>
    internal ExtractUrl ExtractorUrl;
    /// <summary>
    /// Recent urls that have been parsed and can be skipped.
    /// </summary>
    internal System.Collections.Generic.HashSet<Url> Skip;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Cache of urls recently discovered by crawlers.
    /// </summary>
    private System.Collections.Generic.Dictionary<Crawler, CacheValue<Url>> _caches;
    
    /// <summary>
    /// The crawl session this parse control helps manage.
    /// </summary>
    private readonly CrawlSession _session;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize the address processing which retrieves the processed addresses.
    /// </summary>
    public ParseControl(CrawlSession session) {
      
      _session = session;
      
      // initialize the skip collection
      _caches = new System.Collections.Generic.Dictionary<Crawler, CacheValue<Url>>();
      Stats = new Stats(10000);
      
      _session.Crawlers.OnAdd += OnAddCrawler;
      _session.Crawlers.OnRemove += OnRemoveCrawler;
      
    }
    
    /// <summary>
    /// Start the parse control.
    /// </summary>
    public void Start() {
      // nothing currently
    }
    
    /// <summary>
    /// End the processing of addresses. Serialize the processed addresses.
    /// </summary>
    public void End() {
      ExtractorUrl.Dispose();
      Skip.Clear();
      Skip = null;
    }
    
    /// <summary>
    /// On a url being parsed from a webpage.
    /// </summary>
    internal void OnUrl(string urlStr, Crawler crawler) {
      
      // parse the url
      Url url = Url.Parse(urlStr);
      
      // was the url valid?
      if(url == Url.Empty) {
        // no, skip
        //Log.Debug("Parsed an invalid url '" + urlStr + "'.");
        return;
      }
      
      // should the url be parsed?
      if(!_session.TraverseDomains && !_session.SeedHosts.Contains(url.Host.ToLowercase())) {
        // no, return
        return;
      }
      
      // is the on url callback set? yes, run
      if(_session.OnUrl != null && !_session.OnUrl(url)) return;
      
      // is the url from a crawler?
      if(crawler != null) {
        
        // has the url been parsed recently? yes, skip
        var cache = _caches[crawler];
        if(cache.Contains(url)) return;
        
        // add the url to the skip cache
        cache.Add(url);
        
        // yes, is the extension empty?
        if(Url.IsWebPageExtension(url.Extension)) {
          ManagerUpdate.Control.AddSingle(OnUrlWebPage, crawler, url);
        } else {
          ManagerUpdate.Control.AddSingle(OnUrlAsset, crawler, url);
        }
        
      } else {
        // add a read index to the stats
        Stats.UpdateRead();
        
        // add the url to the new urls table
        _session.OnNewUrl(url);
      }
      
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// On a successful connection to a url.
    /// </summary>
    private void OnConnect(long connectionTime, Crawler crawler) {
      Stats.UpdateConnect(connectionTime);
    }
    
    /// <summary>
    /// On a new crawler being added.
    /// </summary>
    private void OnAddCrawler(Crawler crawler) {
      
      var parsers = GetParsers(crawler);
      
      // supply the crawler with parsers
      crawler.SetParsers(parsers.ArgA, parsers.ArgB);
      
      // set the crawler actions
      crawler.OnAttempt = new ActionSet<Crawler>(OnAttempt, crawler);
      crawler.OnConnect = new ActionSet<long, Crawler>(OnConnect, 0L, crawler);
      crawler.OnProcess = new ActionSet<long, Crawler>(OnProcess, 0L, crawler);
      crawler.GetIdentity = new FuncSet<Identity>(() => Identity.Next);
      
      // add the crawler cache
      _caches.Add(crawler, new CacheValue<Url>(1024));
      
    }
    
    /// <summary>
    /// On a crawler being removed.
    /// </summary>
    private void OnRemoveCrawler(Crawler crawler) {
      
      // remove the crawler cache
      var cache = _caches[crawler];
      cache.Dispose();
      _caches.Remove(crawler);
      
    }
    
    /// <summary>
    /// On a successful parse of a url.
    /// </summary>
    private void OnProcess(long processingTime, Crawler crawler) {
      // update the stats
      Stats.UpdateProcess(processingTime);
      
      // alter the host score
      var host = _session.GetHost(crawler.Url);
      if(host == null) host = new Host(_session, crawler.Url.Host, _session.ParseControl.HostNewScore, 0, true);
      host.Score += HostParseScore;
      
      // add the url to the OldUrls table
      _session.OnUrlParsed(crawler.Url);
    }
    
    /// <summary>
    /// On an unsuccessful attempt to parse a url.
    /// </summary>
    private void OnAttempt(Crawler crawler) {
      // has the session been paused?
      if(!_session.Paused) {
        
        // no, update the stats
        Stats.UpdateAttempt();
        
        // alter the host score
        Host host = _session.GetHost(crawler.Url);
        if(host == null) host = new Host(_session, crawler.Url.Host, _session.ParseControl.HostNewScore, 0, true);
        host.Score += HostAttemptScore;
        // should the host be ignored?
        if(host.Score < 0) {
          // yes remove from to be parsed urls
          _session.OnIgnoreHost(host);
        }
        
      }
    }
    
    /// <summary>
    /// When a url is discovered with a file extension.
    /// </summary>
    private void OnUrlAsset(Crawler crawler, Url url) {
      
      if(!_session.ShouldParse(url)) return;
      
      byte score;
      // is the url an asset?
      if(_session.FileAssets.TryGetValue(url.Extension, out score)) {
        // yes, update stats
        Stats.UpdateAsset(url.Extension);
        // update the host score
        var host = _session.GetHost(crawler.Url);
        if(host == null) host = new Host(_session, crawler.Url.Host, _session.ParseControl.HostNewScore, 0, true);
        host.Score += HostAssetScore + score;  
      } else {
        // add the url to be parsed
        _session.OnNewUrl(url);
      }
      
    }
    
    /// <summary>
    /// When a url is discovered without an extension.
    /// </summary>
    private void OnUrlWebPage(Crawler crawler, Url url) {
      
      // has the url been parsed already? yes, skip
      if(!_session.ShouldParse(url)) return;
      
      // is the crawler parsing a matching host?
      if(crawler.Url.Host == url.Host && crawler.Add(url)) {
        // yes, the url will be parsed by the Crawler
        return;
      }
      
      // add the url to be parsed
      _session.OnNewUrl(url);
      
    }
    
    /// <summary>
    /// Create the extraction structure for the current crawling session.
    /// </summary>
    private Teple<ParseUrl,Parse> GetParsers(Crawler crawler) {
      
      // have the extractors been created?
      if(ExtractorUrl == null) {
        // no, create an extractor of urls
        ExtractorUrl = new ExtractUrl(null, new ArrayRig<Protocol>(Protocols));
      }
      
      ParseUrl parseUrl = (ParseUrl)ExtractorUrl.GetParser();
      parseUrl.OnUrl = new ActionSet<string, Crawler>(OnUrl, null, crawler);
      
      Parse parseElse = _session.PageExtractor == null ? null : _session.PageExtractor.GetParser();
      
      return new Teple<ParseUrl, Parse>(parseUrl, parseElse);
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
  }
}

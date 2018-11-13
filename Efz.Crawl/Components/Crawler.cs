/*
 * User: Joshua
 * Date: 1/08/2016
 * Time: 7:29 PM
 */
using System;
using System.IO;
using System.Net;
using System.Text;

using Efz.Collections;
using Efz.Text;
using Efz.Tools;
using Efz.Threading;
using Efz.Web;

namespace Efz.Crawl {
  
  /// <summary>
  /// Streams a webpage, parses and stores select content based on a ruleset.
  /// Threadsafe.
  /// </summary>
  public class Crawler {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Is the crawler running.
    /// </summary>
    public bool Running { get; protected set; }
    
    /// <summary>
    /// Flag for currently processing a web page.
    /// </summary>
    public bool Processing { get; protected set; }
    /// <summary>
    /// Flag for currently connecting to a url.
    /// </summary>
    public bool Connecting { get; protected set; }
    
    /// <summary>
    /// When the Crawler is stopped. This happens after each page.
    /// </summary>
    public IAction OnStop;
    /// <summary>
    /// Action on a url being successfully parsed.
    /// Passes the processing time.
    /// </summary>
    public IAction<long> OnProcess;
    /// <summary>
    /// Callback on a url being successfully connected to.
    /// Passes the connection time.
    /// </summary>
    public IAction<long> OnConnect;
    /// <summary>
    /// Callback on a crawler being unable to connect to a url.
    /// </summary>
    public IAction OnAttempt;
    
    /// <summary>
    /// Method called to retrieve an set of headers.
    /// </summary>
    public IFunc<Identity> GetIdentity;
    
    /// <summary>
    /// The collection of headers used for the requests.
    /// </summary>
    public Identity Identity;
    /// <summary>
    /// Collection of cookies.
    /// </summary>
    public CookieContainer Cookies;
    
    /// <summary>
    /// Time limit to how often this crawler can make requests.
    /// </summary>
    public int TimeLimit;
    
    /// <summary>
    /// The current url being parsed.
    /// </summary>
    internal Url Url { get; private set; }
    /// <summary>
    /// The current host being parsed.
    /// </summary>
    internal Host Host { get; private set; }
    /// <summary>
    /// The current host score.
    /// </summary>
    internal int HostScore { get; private set; }
    
    //-------------------------------------------//
    
    /// <summary>
    /// Urls pending for this Crawler.
    /// </summary>
    protected Queue<Url> _urls;
    /// <summary>
    /// Was the next url specified to move to?
    /// </summary>
    protected bool _nextUrlSet;
    /// <summary>
    /// Was the current crawler task aborted?
    /// </summary>
    protected bool _abort;
    
    /// <summary>
    /// Timing of processes.
    /// </summary>
    protected Timekeeper _time;
    
    /// <summary>
    /// Timeout timer.
    /// </summary>
    protected Timer _timer;
    /// <summary>
    /// The parser of urls.
    /// </summary>
    protected ParseUrl _parseUrl;
    /// <summary>
    /// The current parser used to extract content from webpages.
    /// </summary>
    protected Parse _parseAll;
    /// <summary>
    /// Whether the current parser has been set.
    /// </summary>
    protected bool _parseAllSet;
    
    /// <summary>
    /// Current request.
    /// </summary>
    protected HttpWebRequest _request;
    /// <summary>
    /// Current response.
    /// </summary>
    protected HttpWebResponse _response;
    /// <summary>
    /// Current response stream.
    /// </summary>
    protected Stream _stream;
    
    /// <summary>
    /// Decoder for the current stream.
    /// </summary>
    protected Decoder _decoder;
    /// <summary>
    /// Current limit to the streamed bytes.
    /// </summary>
    protected int _byteCountMax;
    /// <summary>
    /// Current bytes read from the current stream.
    /// </summary>
    protected int _byteCount;
    
    /// <summary>
    /// Task for the stopped method.
    /// </summary>
    protected Act _stopped;
    /// <summary>
    /// Task for the process timing out.
    /// </summary>
    protected Act _timeout;
    
    /// <summary>
    /// Task for getting the next url to connect to.
    /// </summary>
    protected Act _preConnect;
    /// <summary>
    /// Task for connecting to the current url.
    /// </summary>
    protected Act _connect;
    
    /// <summary>
    /// Task for pre processing a web page.
    /// </summary>
    protected Act _preProcess;
    /// <summary>
    /// Task for all processing.
    /// </summary>
    protected Act _process;
    /// <summary>
    /// Task for post processing a web page.
    /// </summary>
    protected Act _postProcess;
    
    /// <summary>
    /// Byte buffer that is populated with each read.
    /// </summary>
    protected byte[] _bytes;
    /// <summary>
    /// Character buffer that is populated with decoded bytes.
    /// </summary>
    protected char[] _chars;
    
    /// <summary>
    /// Cached decoder for utf8.
    /// </summary>
    protected Decoder _utf8;
    /// <summary>
    /// Cached decoder for ascii.
    /// </summary>
    protected Decoder _ascii;
    
    /// <summary>
    /// Lock for any extenal access to the crawler.
    /// </summary>
    protected Lock _lock;
    /// <summary>
    /// The session this crawler belongs to.
    /// </summary>
    protected CrawlSession _session;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize a new Web Crawler.
    /// </summary>
    public Crawler(CrawlSession session) {
      
      _session = session;
      
      _timer = new Timer(3000, OnTimeout, true);
      _time = new Timekeeper();
      
      // init the lock
      _lock = new Lock();
      
      // create the common tasks
      _stopped     = new Act(OnStopped);
      _timeout     = new Act(OnTimeout);
      
      _preConnect  = new Act(PreConnect);
      _connect     = new Act(Connect);
      
      _preProcess  = new Act(PreProcess);
      _process     = new Act(Process);
      _postProcess = new Act(PostProcess);
      
      // cache common decoders
      _utf8 = Encoding.UTF8.GetDecoder();
      _ascii = Encoding.ASCII.GetDecoder();
      
      // start byte buffer
      _bytes = new byte[_session.CrawlerByteBuffer];
      // start the initial char buffer
      _chars = new char[Encoding.UTF8.GetMaxCharCount(_session.CrawlerByteBuffer)];
      // start the urls collection
      _urls = new Queue<Url>();
      
      // init the cookie container
      Cookies = new CookieContainer(_session.CrawlerMaxCookieCount, _session.CrawlerMaxCookieCount, _session.CrawlerMaxCookieSize);
      // set a default url
      Url = Url.Empty;
      
    }
    
    /// <summary>
    /// Stop this Crawler and dispose of its resources.
    /// </summary>
    public void Dispose() {
      if(_lock == null) return;
      if(Running) {
        OnStop = new ActionSet(Dispose);
        _abort = true;
        _nextUrlSet = false;
      }
      _lock.Take();
      _timer.Run = false;
      _time.Stop();
      Connecting = false;
      Processing = false;
      _request = null;
      _response = null;
      _parseUrl = null;
      _ascii = null;
      _utf8 = null;
      _lock.Release();
      _lock = null;
    }
    
    /// <summary>
    /// Start the Crawler.
    /// </summary>
    public void Start() {
      _lock.Take();
      if(!Running) {
        Running = true;
        _abort = false;
        // disable once ObjectCreationAsStatement
        if(TimeLimit < 200) _preConnect.Run();
        else new Timer(TimeLimit, _preConnect);
      }
      _lock.Release();
    }
    
    /// <summary>
    /// Assign an extractor to the crawler.
    /// </summary>
    public void SetParsers(ParseUrl parseUrl, Parse parseAll) {
      _lock.Take();
      
      // abort if running to refresh parser
      _abort |= Running;
      _parseUrl = parseUrl;
      _parseAll = parseAll;
      _parseAllSet = _parseAll != null;
      
      _lock.Release();
    }
    
    /// <summary>
    /// Stop the processing of this Crawler.
    /// </summary>
    public void Stop() {
      _lock.Take();
      if(Running) {
        _abort = true;
        _nextUrlSet = false;
        _lock.Release();
      } else {
        _lock.Release();
        _stopped.Run();
      }
    }
    
    /// <summary>
    /// Add a url to be parsed by this Crawler. This is used to have a single Crawler identity
    /// crawl all pages of a unique host.
    /// </summary>
    internal bool Add(Url url) {
      _lock.Take();
      // is the buffer filled?
      if(_urls.Count != _session.CrawlerUrlBufferSize) {
        // no, add the url
        _urls.Enqueue(url);
        _lock.Release();
        return true;
      }
      _lock.Release();
      return false;
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// On Crawler stopped or aborted.
    /// </summary>
    protected void OnStopped() {
      
      // was the Crawler aborted?
      if(_abort) {
        // yes, was the next url assigned?
        if(_nextUrlSet) {
          // have stop events been subscribed to? run
          if(OnStop != null) OnStop.Run();
          
          // yes, process the next url
          _nextUrlSet = false;
          
          // disable once ObjectCreationAsStatement
          if(TimeLimit < 200) _preConnect.Run();
          else new Timer(TimeLimit, _preConnect);
          
        } else {
          // no, the Crawler is no longer running
          Running = false;
          
          // have stop events been subscribed to? run
          if(OnStop != null) OnStop.Run();
        }
      } else if(Running) {
        // have stops been subscribed to? run
        if(OnStop != null) OnStop.Run();
        
        // no, run the next url
        if(TimeLimit < 200) _preConnect.Run();
        else new Timer(TimeLimit, _preConnect);
      }
      
    }
    
    /// <summary>
    /// Start processing the specified address. This stops current processing.
    /// </summary>
    protected void PreConnect() {
      
      // stop timeout
      _timer.Run = false;
      
      // if the next url hasn't been set - get one from the crawler
      Url next = _urls.Dequeue() ? _urls.Current : _session.UrlControl.NextUrl;
      
      // was the next url retrieved?
      if(next == Url.Empty) {
        
        // set the timer to move to the next url on timeout
        _timer.OnDone = _stopped;
        _timer.Reset(500);
        
        return;
      }
      
      // does the next host match the last?
      if(next.Host != Url.Host) {
        // no, get a new identity
        Identity = GetIdentity.Run();
      }
      
      // set the next url
      Url = next;
      
      // ensure the url is valid
      Uri uri;
      if(!Uri.TryCreate(Url.ToString(), UriKind.Absolute, out uri)) {
        // start again
        _stopped.Run();
        return;
      }
      
      // connecting
      Connecting = true;
      
      // create a web request
      _request = System.Net.WebRequest.CreateHttp(Url.ToString());
      _request.AuthenticationLevel = System.Net.Security.AuthenticationLevel.None;
      // ensure no caching
      _request.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
      
      // set the request cookies
      _request.CookieContainer = Cookies;
      
      // set the request identity
      Identity.UpdateRequest(_request);
      
      // reset timeout
      _request.Timeout = Randomize.Range(_session.CrawlerMinConnectTimeout, _session.CrawlerMaxConnectTimeout);
      // start the timer
      _time.Start();
      
      // connect to the url
      _connect.Run();
    }
    
    /// <summary>
    /// Request the current url and get the response.
    /// </summary>
    protected void Connect() {
      
      try {
        
        // perform the request and get the response
        _response = (HttpWebResponse)_request.GetResponse();
        
      } catch(WebException ex) {
        
        // timeout or aborted...
        _time.Stop();
        
        // get the status
        switch(ex.Status) {
          // did the request time out?
          case WebExceptionStatus.Timeout:
            
            // callback
            if(OnAttempt != null) OnAttempt.Run();
            
            break;
          // was the request cancelled or an unknown error occur?
          case WebExceptionStatus.RequestCanceled:
            // was the request aborted?
            if(!_abort) {
              
              // no, callback
              if(OnAttempt != null) OnAttempt.Run();
              
            }
            break;
          // did any other error occur?
          default:
            // no, callback
            if(OnAttempt != null) OnAttempt.Run();
            break;
        }
        
        // run on stopped
        _stopped.Run();
        
        return;
      } catch(Exception ex) {
        _time.Stop();
        
        Log.Error("Crawler web request exception. " + ex.Messages());
        
        // callback
        if(OnAttempt != null) OnAttempt.Run();
        
        // run on stopped
        _stopped.Run();
        return;
      }
      
      // run pre process
      _preProcess.Run();
      
    }
    
    /// <summary>
    /// Begin processing a web page via an incoming stream.
    /// </summary>
    protected void PreProcess() {
      
      // stop the time keeper
      _time.Stop();
      
      // add to the connection time
      if(OnConnect != null) {
        OnConnect.ArgA = _time.Milliseconds;
        OnConnect.Run();
      }
      
      // continue processing
      Processing = true;
      Connecting = false;
      
      // has the request been aborted?
      if(_abort) {
        // yes, stop the request
        _request.Abort();
        _response.Dispose();
        
        // run on stop
        _stopped.Run();
        return;
      }
      
      try {
        
        // add the response cookies
        Cookies.Add(_response.Cookies);
        
      } catch {
        
        // the cookies were invalid - init a new container
        Cookies = new CookieContainer(_session.CrawlerMaxCookieCount, _session.CrawlerMaxCookieCount, _session.CrawlerMaxCookieSize);
        
      }
      
      // get the stream
      _stream = _response.GetResponseStream();
      
      // was the response stream retrieved?
      if(_stream == null) {
        // no, stop and dispose of the resources
        _request.Abort();
        _response.Dispose();
        
        Log.Debug("No stream retrieved from request to '" + Url + "'.");
        
        // callback
        if(OnAttempt != null) OnAttempt.Run();
        
        // run on stop
        _stopped.Run();
        return;
      }
      
      // get the encoding
      string encoder = _response.ContentType;
      _decoder = null;
      if(encoder != null && encoder.Length > 3) {
        int index = encoder.IndexOf("charset=", StringComparison.OrdinalIgnoreCase);
        if(index != -1 && index + 9 == encoder.Length) {
          encoder = encoder.Substring(index + 9, encoder.Length - index - 9);
          if(encoder.Length != 0) {
            int count;
            switch(encoder[0]) {
              case 'u':
                if(encoder.Length > 4) {
                  switch(encoder[4]) {
                    case '3':
                      _decoder = Encoding.UTF32.GetDecoder();
                      // set the char buffer
                      count = Encoding.UTF32.GetMaxCharCount(_session.CrawlerByteBuffer);
                      if(_chars.Length != count) {
                        _chars = new char[count];
                      }
                      break;
                    case '7':
                      _decoder = Encoding.UTF7.GetDecoder();
                      // set the char buffer
                      count = Encoding.UTF7.GetMaxCharCount(_session.CrawlerByteBuffer);
                      if(_chars.Length != count) {
                        _chars = new char[count];
                      }
                      break;
                  }
                }
                break;
              case 'a':
                _decoder = _ascii;
                // set the char buffer
                count = Encoding.ASCII.GetMaxCharCount(_session.CrawlerByteBuffer);
                if(_chars.Length != count) {
                  _chars = new char[count];
                }
                break;
            }
          }
        }
      }
      
      // is the decoder the default UTF8?
      if(_decoder == null) {
        // yes, set the decoder
        _decoder = _utf8;
        // set the char buffer
        int count = Encoding.UTF8.GetMaxCharCount(_session.CrawlerByteBuffer);
        if(_chars.Length != count) {
          _chars = new char[count];
        }
      }
      
      // is the content encoded?
      if(!string.IsNullOrEmpty(_response.ContentEncoding)) {
        try {
          switch(_response.ContentEncoding.ToLowercase()) {
            case "gzip":
              _stream = new System.IO.Compression.GZipStream(_stream, System.IO.Compression.CompressionMode.Decompress, false);
              break;
            case "deflate":
              _stream = new System.IO.Compression.DeflateStream(_stream, System.IO.Compression.CompressionMode.Decompress, false);
              break;
          }
        } catch {
          // the base stream was unreadable - run on attempt
          if(OnAttempt != null) OnAttempt.Run();
          
          // stop and dispose of the resources
          _request.Abort();
          _response.Dispose();
          
          // run on stopped
          _stopped.Run();
          
          return;
        }
      }
      
      // has the request been aborted?
      if(_abort) {
        // yes, stop and dispose of the resources
        _request.Abort();
        _response.Dispose();
        
        // run on stopped
        _stopped.Run();
        return;
      }
      
      // the number of bytes to read from the current url
      _byteCountMax = Randomize.Range(_session.CrawlerMinBytes, _session.CrawlerMaxBytes);
      _byteCount = 0;
      // start the timers
      _time.Start();
      _timer.OnDone = _timeout;
      _timer.Reset(Randomize.Range(_session.CrawlerMinProcessTimeout, _session.CrawlerMaxProcessTimeout));
      
      // start processing the stream
      _process.Run();
      
    }
    
    /// <summary>
    /// Parse a buffer from the current stream.
    /// </summary>
    private void Process() {
      
      int count;
      try {
        
        // read the next byte buffer
        count = _stream.Read(_bytes, 0, _session.CrawlerByteBuffer);
        
      } catch {
        
        // run on attempt
        if(OnAttempt != null) OnAttempt.Run();
        
        // stop and dispose of the resources
        _request.Abort();
        _response.Dispose();
        
        // run on stopped
        _stopped.Run();
        
        return;
      }
    
      // is the stream empty?
      if(count == 0) {
        // yes, end
        _postProcess.Run();
        return;
      }
      
      // add to the current byte count
      _byteCount += count;
      
      // get the characters
      count = _decoder.GetChars(_bytes, 0, count, _chars, 0);
      
      // parse the next set
      _parseUrl.Next(_chars, 0, count);
      if(_parseAllSet) _parseAll.Next(_chars, 0, count);
      
      // has the processing been aborted or has the quota been filled?
      if(_abort || _byteCount > _byteCountMax) {
        // yes, end
        _postProcess.Run();
      } else {
        // no, continue
        _process.Run();
      }
      
    }
    
    /// <summary>
    /// After the parsing of a web page.
    /// </summary>
    private void PostProcess() {
      
      // flush the decoder
      _decoder.Reset();
      
      // no longer processing
      Processing = false;
      
      // stop and dispose of the resources
      _request.Abort();
      _response.Dispose();
      
      // stop the timers
      _timer.Run = false;
      _time.Stop();
      
      // run on process if it timed out or wasn't aborted
      if(OnProcess != null && !_abort || !_timer.Run) {
        OnProcess.ArgA = _time.Milliseconds;
        OnProcess.Run();
      }
      
      // run on stopped
      _stopped.Run();
      
    }
    
    /// <summary>
    /// If the crawler times out connecting to a url or waiting for
    /// new urls.
    /// </summary>
    protected void OnTimeout() {
      
      // was the current request aborted?
      if(_abort) {
        // yes, do nothing
        return;
      }
      
      // start aborting
      _abort = true;
      
    }
    
    
    
    
  }
  
}

/*
 * User: Joshua
 * Date: 30/10/2016
 * Time: 7:50 PM
 */
using System;
using System.Collections.Specialized;
using System.Net;

using Efz.Collections;
using Efz.Threading;
using Efz.Tools;

namespace Efz.Web {
  
  /// <summary>
  /// Describes an identity.
  /// </summary>
  public class Identity {
    
    /// <summary>
    /// Get a random identity from the current collection.
    /// </summary>
    public static Identity Next {
      get {
        _identitiesLock.Take();
        var identity = _identities[Randomize.Range(0, _identities.Count-1)];
        _identitiesLock.Release();
        return identity;
      }
    }
    
    /// <summary>
    /// The accept header.
    /// </summary>
    public string Accept;
    /// <summary>
    /// The accept header.
    /// </summary>
    public string UserAgent;
    /// <summary>
    /// The connection header.
    /// </summary>
    public string Connection;
    /// <summary>
    /// The keep-alive header.
    /// </summary>
    public bool KeepAlive;
    
    /// <summary>
    /// The other headers.
    /// </summary>
    public NameValueCollection Headers = new NameValueCollection();
    
    /// <summary>
    /// Collection of identities that can used.
    /// </summary>
    protected static ArrayRig<Identity> _identities;
    /// <summary>
    /// Lock for the inner collection of identities.
    /// </summary>
    protected static Lock _identitiesLock;
    
    static Identity() {
      _identities = new ArrayRig<Identity>();
      _identitiesLock = new Lock();
      Initialize();
    }
    
    /// <summary>
    /// Construct a new identity. Optionally the identity will be added to the internal
    /// collection.
    /// </summary>
    public Identity(bool persist) {
      if(persist) {
        _identitiesLock.Take();
        _identities.Add(this);
        _identitiesLock.Release();
      }
    }
    
    /// <summary>
    /// Setup the identity on the specified request.
    /// </summary>
    public void UpdateRequest(HttpWebRequest request) {
      request.Accept    = Accept;
      request.UserAgent = UserAgent;
      request.KeepAlive = KeepAlive;
      request.Connection = Connection;
      request.Headers.Add(Headers);
    }
    
    /// <summary>
    /// Add a header value to the identity.
    /// </summary>
    public void Add(string key, string value) {
      switch(key.ToLowercase()) {
        case "accept":
          Accept = value;
          break;
        case "user-agent":
          UserAgent = value;
          break;
        case "connection":
          KeepAlive |= value.ToLowercase() == "keep-alive";
          break;
        default:
          Headers.Add(key, value);
          break;
      }
    }
    
    /// <summary>
    /// Get all headers.
    /// </summary>
    public ArrayRig<Teple<string,string>> GetAll() {
      ArrayRig<Teple<string,string>> collection = new ArrayRig<Teple<string, string>>();
      if(Accept != null) collection.Add(new Teple<string, string>("Accept", Accept));
      
      foreach(object key in Headers) {
        collection.Add(new Teple<string, string>(key.ToString(), Headers[key.ToString()]));
      }
      return collection;
    }
    
    /// <summary>
    /// Initialize default identities.
    /// </summary>
    protected static void Initialize() {
      
      // add default identities
      Identity identity = new Identity(true);
      identity.Add("Accept", "image/jpeg, application/x-ms-application, image/gif, application/xaml+xml, image/pjpeg, application/x-ms-xbap, application/x-shockwave-flash, application/msword, */*");
      identity.Add("User-Agent", "Mozilla/5.0 (compatible; Konqueror/3.5; Linux; X11; i686; en_US) KHTML/3.5.3 (like Gecko)");
      identity.Add("Accept-Language", "en-US,en;q=0.5");
      identity.Add("If-None-Match", "mJ4imx1p");
      
      identity = new Identity(true);
      identity.Add("Accept", "text/html, application/xml;q=0.9, application/xhtml xml, image/png, image/jpeg, image/gif, image/x-xbitmap, */*;q=0.1");
      identity.Add("User-Agent", "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US) AppleWebKit/525.13 (KHTML, like Gecko) Version/3.1 Safari/525.13");
      identity.Add("Accept-Language", "en-US,en;q=0.5");
      identity.Add("If-None-Match", "g2ed3k5u");
      
      identity = new Identity(true);
      identity.Add("Accept", "text/html, image/png, image/jpeg, image/gif, image/x-xbitmap, */*;q=0.1");
      identity.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.0; WOW64) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.835.186 Safari/535.1");
      identity.Add("Accept-Language", "en-US,en;q=0.5");
      identity.Add("If-None-Match", "34vn9p8m");
      
      identity = new Identity(true);
      identity.Add("Accept", "application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,image/png,*/*;q=0.5");
      identity.Add("User-Agent", "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.2.11) Gecko/20101023 Firefox/3.6.11 (Palemoon/3.6.11) ( .NET CLR 3.5.30729; .NET4.0E)");
      identity.Add("Accept-Language", "en-US,en;q=0.5");
      identity.Add("If-None-Match", "iu753id7");
      
      // set common headers
      for(int i = _identities.Count-1; i >= 0; --i) {
        identity = _identities[i];
        // spoof the 'Via' heading
        var builder = StringBuilderCache.Get(64);
        builder.Append("1.1 ");
        builder.Append(Randomize.Byte);
        builder.Append(Chars.Stop);
        builder.Append(Randomize.Byte);
        builder.Append(Chars.Stop);
        builder.Append(Randomize.Byte);
        builder.Append(Chars.Stop);
        builder.Append(Randomize.Byte);
        identity.Add("Via", builder.ToString());
        
        // spoof the 'X-Forwarded-For' heading
        builder.Length = 0;
        builder.Append(Randomize.Byte);
        builder.Append(Chars.Stop);
        builder.Append(Randomize.Byte);
        builder.Append(Chars.Stop);
        builder.Append(Randomize.Byte);
        builder.Append(Chars.Stop);
        builder.Append(Randomize.Byte);
        identity.Add("X-Forwarded-For", StringBuilderCache.SetAndGet(builder));
        
        identity.Add("Accept-Encoding", "gzip, deflate");
        identity.KeepAlive = true;
      }
    }
    
  }
  
}

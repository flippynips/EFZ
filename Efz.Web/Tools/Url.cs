/*
 * User: Joshua
 * Date: 27/10/2016
 * Time: 6:14 PM
 */
using System;

using System.Collections.Generic;
using Efz.Tools;

namespace Efz.Web {
  
  /// <summary>
  /// The constituants of a url.
  /// </summary>
  public struct Url {
    
    //-------------------------------------------//
    
    /// <summary>
    /// An empty url.
    /// </summary>
    public static Url Empty {
      get { return new Url(true); }
    }
    
    public static IEqualityComparer<Url> Comparer {
      get { return new UrlComparer(); }
    }
    
    /// <summary>
    /// Get a string representation of the protocol, server and host section of the url.
    /// </summary>
    public string PrePath {
      get {
        if(Server.Length == 0) {
          return Protocols.Get(Protocol) + Host;
        }
        return Protocols.Get(Protocol) + Server + Chars.Stop + Host;
      }
    }
    
    /// <summary>
    /// Protocol of the url.
    /// </summary>
    public readonly Protocol Protocol;
    /// <summary>
    /// Any value pre-host i.e. nmf83 of nmf83.webinar.com
    /// </summary>
    public readonly string Server;
    /// <summary>
    /// The host component of the url.
    /// </summary>
    public readonly string Host;
    /// <summary>
    /// The path component of the path.
    /// </summary>
    public readonly string Path;
    /// <summary>
    /// The extension if set of the url.
    /// </summary>
    public readonly string Extension;
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Does the extension likely represent a web page?
    /// </summary>
    public static bool IsWebPageExtension(string extension) {
      switch(extension.ToUpper()) {
        case "":
        case "HTM":
        case "HTML":
        case "JHTML":
        case "XHTML":
        case "CSS":
        case "ASP":
        case "ASPX":
        case "AXD":
        case "ASMX":
        case "ASHX":
        case "CFM":
        case "SHTML":
        case "PHP":
        case "PHP4":
        case "PHP3":
        case "PHTML":
        case "PY":
        case "RHTML":
        case "RB":
        case "XML":
        case "RSS":
        case "SVG":
        case "CGI":
        case "DLL":
        case "PL":
        case "JS":
        case "JSP":
        case "JSPX":
        case "WSS":
        case "DO":
        case "ACTION":
          return true;
      }
      return false;
    }
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    private Url(bool a) {
      Protocol = Protocol.Unknown;
      Server = Host = Path = Extension = string.Empty;
    }
    
    /// <summary>
    /// Initialize a url with the specified components.
    /// </summary>
    public Url(Protocol protocol, string pre, string host, string path, string extension) {
      Protocol = protocol;
      Server = pre;
      Host = host;
      Path = path;
      Extension = extension;
    }
    
    /// <summary>
    /// Initialize a url with the specified components. The host will be parsed.
    /// </summary>
    public Url(string host, string path, string extension) {
      int index = 0;
      switch(host[0]) {
        case Chars.h:
        case Chars.H:
          if(host.Length < 5) {
            Protocol = Protocol.Unknown;
            break;
          }
          if(host[4] == Chars.s || host[4] == Chars.S) {
            Protocol = Protocol.Https;
            index = 8;
          } else {
            Protocol = Protocol.Http;
            index = 7;
          }
          break;
        case Chars.w:
        case Chars.W:
          Protocol = Protocol.WebSocket;
          index = 5;
          break;
        case Chars.f:
        case Chars.F:
          Protocol = Protocol.File;
          index = 7;
          break;
        default:
          Protocol = Protocol.Unknown;
          break;
      }
      
      int indexA = index;
      int indexB = 0;
      bool preHostFound = false;
      while(index < host.Length) {
        if(indexB == 0) {
          if(host[index] == Chars.Stop) indexB = index;
        } else if(host[index] == Chars.Stop) {
          preHostFound = true;
          break;
        }
        ++index;
      }
      
      // was a pre-host defined?
      if(preHostFound) {
        // yes, get the pre-host definition
        Server = host.Substring(indexA, indexB - indexA);
        // derive the host
        Host = host.Substring(indexB + 1);
      } else {
        // no, get the host
        Host = host.Substring(indexA);
        Server = string.Empty;
      }
      
      Path = path;
      Extension = extension;
    }
    
    /// <summary>
    /// Get the url components as a full address.
    /// </summary>
    public override string ToString() {
      if(Server.Length == 0) {
        if(Path.Length == 0) return Protocols.Get(Protocol) + Host;
        if(Extension.Length == 0) return Protocols.Get(Protocol) + Host + Chars.ForwardSlash + Path;
        return Protocols.Get(Protocol) + Host + Chars.ForwardSlash + Path + Chars.Stop + Extension;
      }
      if(Path.Length == 0) return Protocols.Get(Protocol) + Server + Chars.Stop + Host;
      if(Extension.Length == 0) return Protocols.Get(Protocol) + Server + Chars.Stop + Host + Chars.ForwardSlash + Path;
      return Protocols.Get(Protocol) + Server + Chars.Stop + Host + Chars.ForwardSlash + Path + Chars.Stop + Extension;
    }
    
    /// <summary>
    /// Equality of urls.
    /// </summary>
    public override bool Equals(object obj) {
      return (obj is Url) && Equals((Url)obj);
    }
    
    /// <summary>
    /// Equality of urls.
    /// </summary>
    public bool Equals(Url other) {
      if(this.Protocol == other.Protocol &&
         this.Server.Length == other.Server.Length &&
         this.Host.Length == other.Host.Length &&
         this.Path.Length == other.Path.Length &&
         this.Extension.Length == other.Extension.Length) {
        return this.Server == other.Server &&
               this.Host == other.Host &&
               this.Path == other.Path &&
               this.Extension == other.Extension;
      }
      return false;
    }
    
    /// <summary>
    /// Hashcode generation of the url.
    /// </summary>
    public override int GetHashCode() {
      unchecked {
        return Protocol.GetHashCode() + Server.GetHashCode() + Host.GetHashCode() + Path.GetHashCode() + Extension.GetHashCode();
      }
    }
    
    /// <summary>
    /// Equality operator for urls.
    /// </summary>
    public static bool operator ==(Url lhs, Url rhs) {
      if(lhs.Protocol == rhs.Protocol &&
         lhs.Server == rhs.Server &&
         lhs.Host.Length == rhs.Host.Length &&
         lhs.Path.Length == rhs.Path.Length &&
         lhs.Extension.Length == rhs.Extension.Length) {
        return lhs.Server == rhs.Server &&
               lhs.Host == rhs.Host &&
               lhs.Path == rhs.Path &&
               lhs.Extension == rhs.Extension;
      }
      return false;
    }
    
    /// <summary>
    /// Inequality operator for urls.
    /// </summary>
    public static bool operator !=(Url lhs, Url rhs) {
      if(lhs.Protocol != rhs.Protocol ||
         lhs.Server.Length != rhs.Server.Length ||
         lhs.Host.Length != rhs.Host.Length ||
         lhs.Path.Length != rhs.Path.Length ||
         lhs.Extension.Length != rhs.Extension.Length) {
        return true;
      }
      return lhs.Server != rhs.Server &&
             lhs.Host != rhs.Host &&
             lhs.Path != rhs.Path &&
             lhs.Extension != rhs.Extension;;
    }
    
    /// <summary>
    /// Parse a url into its host, path and extension components.
    /// Returns null if the url was invalid.
    /// </summary>
    public static Url Parse(string path) {
      int index = path.Length;
      
      // skip urls of insignificant length
      if(index < 5) return Url.Empty;
      
      // get the extension if any
      string extension = null;
      // iterate backwards through the url
      while(--index >= path.Length - 5) {
        switch(path[index]) {
          case Chars.Stop:
            ++index;
            extension = path.Substring(index, path.Length - index);
            // is the extension a domain extension?
            if(Domains.Contains(extension)) {
              // yea, invalid
              extension = null;
            }
            index = 0;
            break;
          case Chars.Colon:
          case Chars.ForwardSlash:
          case Chars.Equal:
            // no extension break
            index = 0;
            break;
        }
      }
      
      // get a search for the domains
      var domainSearch = Domains.DomainSearch.SearchProgressive();
      
      index = 6;
      // get the host
      int pathLength = path.Length;
      // iterate forwards through the url
      while(++index != pathLength) {
        
        // has a domain been found? i.e. .com
        if(domainSearch.Next(Char.ToUpper(path[index]))) {
          
          // is this index the last in the path?
          if(index + 1 == pathLength) {
            // yes, just a host
            return new Url(path, string.Empty, string.Empty);
          }
          
          // has a forward slash been found?
          if(path[index + 1] == Chars.ForwardSlash) {
            if(pathLength == index + 2) {
              // just a host
              return new Url(path.Substring(0, index + 1), string.Empty, string.Empty);
            }
            // host and path
            if(extension == null) {
              return new Url(path.Substring(0, index + 1), path.Substring(index + 2, pathLength - index - 2), string.Empty);
            }
            return new Url(path.Substring(0, index + 1), path.Substring(index + 2, pathLength - index - extension.Length - 3), extension);
          }
          
          // is the url possibly escaped?
          if(path[index + 1] == Chars.BackSlash) {
            // yes, try escaping
            return Parse(path.Replace("\\", string.Empty));
          }
          
        } else if(path[index] == Chars.Colon) {
          
          // could be an ip address
          int portIndex = 0;
          while(++index != pathLength && Char.IsDigit(path[index])) {
            ++portIndex;
          }
          
          // did the path have a port?
          if(portIndex > 1) {
            // yes, add it
            if(index == pathLength) {
              // just a host
              return new Url(path, string.Empty, string.Empty);
            }
            if(path[index] == Chars.ForwardSlash) {
              if(pathLength == index + 1) {
                // just a host
                return new Url(path.Substring(0, index), string.Empty, string.Empty);
              }
              // host and path
              if(extension == null) {
                return new Url(path.Substring(0, index), path.Substring(index + 1, pathLength - index - 1), string.Empty);
              }
              return new Url(path.Substring(0, index), path.Substring(index + 1, pathLength - index - extension.Length - 2), extension);
            }
          } else {
            return Url.Empty;
          }
        }
        
      }
      
      // the host wasn't found, the url is invalid
      return Url.Empty;
    }
    
    /// <summary>
    /// Get the parameters from the path of a web request.
    /// </summary>
    public static Dictionary<string,string> GetParams(ref string path) {
      
      // try get the index separating the path from the parameters
      int paramsIndex = path.IndexOf(Chars.Question);
      Dictionary<string,string> parameters = new Dictionary<string, string>();
      
      // were parameters added?
      if(paramsIndex >= 0) {
        
        var split = System.Net.WebUtility.UrlDecode(path).Split(paramsIndex);
        
        path = split.ArgA;
        
        string key = null;
        int indexStart = 0;
        int index = 0;
        while(index < split.ArgB.Length) {
          
          switch(split.ArgB[index]) {
            case Chars.Equal:
              
              key = split.ArgB.Section(indexStart, index);
              indexStart = index + 1;
              
              break;
            case Chars.And:
              
              parameters.Add(key, split.ArgB.Section(indexStart, index));
              indexStart = index + 1;
              key = null;
              
              break;
          }
          
          ++index;
        }
        
        if(key != null) {
          if(indexStart == index) {
            parameters.Add(key, null);
          } else {
            parameters.Add(key, split.ArgB.Section(indexStart, index));
          }
        }
        
      }
      
      return parameters;
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// Comparer used by collections of urls.
    /// </summary>
    public class UrlComparer : IEqualityComparer<Url> {
      /// <summary>
      /// Compare two urls for equality.
      /// </summary>
      public bool Equals(Url x, Url y) {
        return x.Equals(y);
      }
      /// <summary>
      /// Get the hashcode of a url.
      /// </summary>
      public int GetHashCode(Url url) {
        return url.GetHashCode();
      }
    }
    
  }
  
}

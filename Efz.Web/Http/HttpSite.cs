using System;
using System.Collections.Generic;

using Efz.Data;
using Efz.Collections;
using Efz.Web.Display;

namespace Efz.Web {
  
  /// <summary>
  /// Represents the provisioning of web content to a web server.
  /// </summary>
  public class HttpSite : IDisposable {
    
    //----------------------------------//
    
    /// <summary>
    /// Local root path of the web site resources.
    /// </summary>
    public readonly string Path;
    
    /// <summary>
    /// Web server instance this client belongs to.
    /// </summary>
    public HttpServer Server { get; protected set; }
    
    /// <summary>
    /// Flag to indicate the server is currently running.
    /// </summary>
    public bool Running { get; protected set; }
    
    /// <summary>
    /// Callback on a client being denied access to a resource.
    /// </summary>
    public IAction<HttpRequest> OnAccessDenied {
      get {
        if(_onAccessDenied == null) return null;
        return _onAccessDenied.Action;
      }
      set {
        if(value == null) _onAccessDenied = null;
        if(_onAccessDenied == null) _onAccessDenied = new ActionPop<HttpRequest>(value);
        else _onAccessDenied.Action = value;
      }
    }
    /// <summary>
    /// Callback on a client attempting to access a resource that doesn't exist.
    /// </summary>
    public IAction<HttpRequest> OnInvalidResource {
      get {
        if(_onInvalidResource == null) return null;
        return _onInvalidResource.Action;
      }
      set {
        if(value == null) _onInvalidResource = null;
        if(_onInvalidResource == null) _onInvalidResource = new ActionPop<HttpRequest>(value);
        else _onInvalidResource.Action = value;
      }
    }
    /// <summary>
    /// Callback on a new client connection.
    /// </summary>
    public IAction<HttpConnection> OnNewConnection {
      get {
        if(_onConnection == null) return null;
        return _onConnection.Action;
      }
      set {
        if(value == null) _onConnection = null;
        if(_onConnection == null) _onConnection = new ActionPop<HttpConnection>(value);
        else _onConnection.Action = value;
      }
    }
    /// <summary>
    /// Callback on a client being redirected. The target path is passed along with the
    /// request.
    /// </summary>
    public IAction<HttpRequest, string> OnRedirect;
    
    /// <summary>
    /// Authentication requirements for certain file system locations within the site folder.
    /// </summary>
    public readonly Dictionary<string, HttpRequirement> Authentication;
    /// <summary>
    /// Redirects for the web resources.
    /// </summary>
    public readonly Dictionary<string, HttpRedirect> Redirects;
    
    //----------------------------------//
    
    /// <summary>
    /// Configuration of the web site.
    /// </summary>
    protected Configuration _configuration;
    
    /// <summary>
    /// Cache of web resources.
    /// </summary>
    protected Cache<string, WebResource> _cache;
    
    /// <summary>
    /// Inner callback on a request being denied.
    /// </summary>
    protected ActionPop<HttpRequest> _onAccessDenied;
    /// <summary>
    /// Inner callback on an invalid resource request.
    /// </summary>
    protected ActionPop<HttpRequest> _onInvalidResource;
    /// <summary>
    /// Inner callback on a new client.
    /// </summary>
    protected ActionPop<HttpConnection> _onConnection;
    
    /// <summary>
    /// Default send options for html elements.
    /// </summary>
    protected HttpSendOptions _defaultSendOptions;
    
    //----------------------------------//
    
    /// <summary>
    /// Start a web site and server with the specified path as the root directory.
    /// </summary>
    public HttpSite(string path) {
      if(string.IsNullOrEmpty(path)) {
        Log.Warning("No path specified for site. A path should be set.");
      }
      
      // persist and clean the path
      Path = Fs.Combine(path);
      
      // get the configuration for the web site
      ManagerResources.LoadConfig(Fs.Combine(Path, "site.config"), new Act<Configuration>(OnConfiguration));
      
      // create the authentication dictionary
      Authentication = new Dictionary<string, HttpRequirement>();
      // create the redirects collection
      Redirects = new Dictionary<string, HttpRedirect>();
      
      // create the cache
      _cache = new Cache<string, WebResource>(Global.Megabyte * 100, r => r.Size);
      
      _onAccessDenied = new ActionPop<HttpRequest>();
      _onInvalidResource = new ActionPop<HttpRequest>();
      
      _defaultSendOptions = new HttpSendOptions{ContentType = "text/html"};
    }
    
    /// <summary>
    /// Close any existing connection with the client and disposes of any unmanaged
    /// resources.
    /// </summary>
    /// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="Efz.Web.HttpWebClient"/>. The
    /// <see cref="Dispose"/> method leaves the <see cref="Efz.Web.HttpWebClient"/> in an unusable state. After calling
    /// <see cref="Dispose"/>, you must release all references to the <see cref="Efz.Web.HttpWebClient"/> so the garbage
    /// collector can reclaim the memory that the <see cref="Efz.Web.HttpWebClient"/> was occupying.</remarks>
    public void Dispose() {
      
      // dispose of the server
      Server.Dispose();
      
      var authNode = _configuration["Authentication"];
      authNode.Clear();
      // set the authentication values
      foreach(var authentication in Authentication) {
        
        // create a node for authentication
        var auth = new Node();
        // save the authentication values
        authentication.Value.Save(auth);
        // add the node to the main auth node
        authNode.Add(auth);
        
      }
      
      var redirectsNode = _configuration["Redirects"];
      redirectsNode.Clear();
      
      // set the redirect values
      foreach(var redirect in Redirects) {
        
        // set each redirects node
        var redirectNode = new Node();
        // save the path
        redirectNode["Path"].String = redirect.Key;
        // save the redirect in the redirect node
        redirect.Value.Save(redirectNode);
        
      }
      
      // save the configuration
      _configuration.Save();
      
    }
    
    /// <summary>
    /// Get a string representation of the web client.
    /// </summary>
    public override string ToString() {
      return "[HttpSite Path="+Path+" Clients="+Server.Connections.Count+"]";
    }
    
    /// <summary>
    /// Resolve the resource at the specified path.
    /// </summary>
    public virtual WebResource GetResource(string path) {
      
      // does the path indicate a directory?
      if(path.Length == 0 || path[path.Length-1] == Chars.ForwardSlash) {
        // yes, get the default html page of that directory
        path = Fs.Combine(path, "default.html");
      }
      
      // attempt to get the web resource from the cache
      WebResource resource = _cache.Get(path);
      
      // was the resource found in the cache?
      if(resource == null) {
        // no, create a new resource
        resource = new WebResource(this, path);
        // add the resource to the cache
        _cache.Add(path, resource);
      }
      
      return resource;
    }
    
    /// <summary>
    /// Send the sites generic error page to the client with an error message.
    /// </summary>
    public virtual void SendErrorPage(HttpConnection client, string error) {
      
      // has the error page been specified?
      if(_configuration["Pages"]["Error"].Set) {
        // yes, send the specified page
        new ElementParser(_configuration["Pages"]["Error"].String, new ActionSet<ElementParser, HttpConnection, string>(OnErrorPageParsed, null, client, error));
      } else {
        // no, send a default error page
        SendDefaultError(client, error);
      }
      
    }
    
    //----------------------------------//
    
    /// <summary>
    /// On a web request from a client.
    /// </summary>
    protected virtual void OnRequest(HttpRequest request) {
      
      // is the client allowed the specified resource?
      if(!ResolvePath(request)) {
        
        // no, is the not authenticated callback set?
        if(_onAccessDenied != null) {
          // yes, run it
          _onAccessDenied.Run(request);
          return;
        }
        
        // log the access denial
        Log.Warning("Access to unauthorized resource '"+request.RequestPath+"' from '"+request.Connection+"'.");
        
        // send the default error page to the client
        SendErrorPage(request.Connection, "Invalid page request '"+request.RequestPath+"'.");
        
        // skip the remaining process
        return;
      }
      
      // get the page web resource
      WebResource resource = GetResource(request.RequestPath);
      
      // dispose of the web request
      request.Dispose();
      
      // is the resource valid?
      if(resource.Valid) {
        
        // yes, is there a default wrapper for the resource?
        if(_configuration["Wrappers"].DictionarySet) {
          // get the wrapper node for the resource mime type
          Node wrapperNode = _configuration["Wrappers"][resource.MimeType];
          if(wrapperNode.Set) {
            
            // yes, get the default wrapper
            new ElementParser(ResolvePath(wrapperNode.String), new ActionSet<ElementParser, HttpConnection, WebResource>(OnWrapperParsed)).Run();
            
            return;
          }
        }
        
        // just start sending the resource
        request.Connection.Send(resource);
        
      } else {
        
        // no, has the invalid resource callback been assigned?
        if(_onInvalidResource.Action != null) {
          
          // yes, run the callback
          _onInvalidResource.ArgA = request;
          _onInvalidResource.Run();
          
          // skip error page
          return;
        }
        
        // send an error page in response
        SendErrorPage(request.Connection, "Invalid page request '"+resource.Path+"'");
        
      }
    }
    
    /// <summary>
    /// Check if the WebRequest should be redirected and authenticate the
    /// web request. Returns if the request path was resolved successfully.
    /// </summary>
    protected virtual bool ResolvePath(HttpRequest request) {
      
      HttpRedirect redirect = null;
      HttpRequirement auth;
      
      // split the request path into directory sections
      var sections = request.RequestPath.Split(Chars.ForwardSlash);
      
      // persistance of the current path
      string path = "/";
      
      // the first iteration?
      bool first = true;
      // redirected path is absolute?
      bool absolute = false;
      // is the request path authenticated?
      bool authentication = true;
      
      // iterate the request path sections
      for (int i = 0; i < sections.Length; ++i) {
        
        // is the first iteration?
        if(!first) {
          // is the redirect absolute? yes, end the redirects
          if(absolute) break;
          
          // is the section empty? yes, skip the section
          if(sections[i].Length == 0) continue;
          
          // append the section
          path = path + Chars.ForwardSlash + sections[i];
        }
        
        // does authentication exist for the request path?
        if (Authentication.TryGetValue(path, out auth)) {
          // update the authentication
          authentication = auth.Allow(request);
        }
        
        // should the path be redirected?
        if(Redirects.TryGetValue(path, out redirect)) {
          
          // update the current path
          path = redirect.Target;
          absolute = redirect.Absolute;
          
          // does authentication exist for the request path?
          if (Authentication.TryGetValue(path, out auth)) {
            // update the authentication
            authentication = auth.Allow(request);
          }
          
          int iteration = 1;
          // while the current path is to be redirected
          while(Redirects.TryGetValue(path, out redirect)) {
            
            // update the current path
            path = redirect.Target;
            absolute = redirect.Absolute;
            
            // does authentication exist for the request path?
            if (Authentication.TryGetValue(path, out auth)) {
              // update the authentication
              authentication = auth.Allow(request);
            }
            
            // have there been an excessive number of redirects?
            if(++iteration == 20) {
              
              // yes, start maintaining a collection of redirected paths
              ArrayRig<string> redirects = new ArrayRig<string>();
              redirects.Add(path);
              
              // while the current path is to be redirected
              while(Redirects.TryGetValue(path, out redirect)) {
                
                // update the current path
                path = redirect.Target;
                absolute = redirect.Absolute;
                
                // has the redirect path already been hit?
                if(redirects.Contains(path)) {
                  // yes, return false, a redirect loop was hit
                  Log.Warning("Redirect loop detected. Request denied.");
                  request.RequestPath = null;
                  return false;
                }
                
                // add to the redirected collection
                redirects.Add(path);
                
                // does authentication exist for the request path?
                if (Authentication.TryGetValue(path, out auth)) {
                  // update the authentication
                  authentication = auth.Allow(request);
                }
              }
              break;
            }
          }
          
          first = false;
          
        } else if(first) {
          
          path = string.Empty;
          first = false;
          
        }
      }
      
      // has a redirect occured?
      if(!request.RequestPath.Equals(path)) {
        
        // has the redirect method been assigned?
        if(OnRedirect != null) {
          OnRedirect.ArgA = request;
          OnRedirect.ArgB = path;
          OnRedirect.Run();
        }
        
        // update the web request path
        request.RequestPath = path;
      }
      
      // return the authentication value
      return authentication;
    }
    
    /// <summary>
    /// Resolve the path using redirects. No authentication.
    /// </summary>
    protected virtual string ResolvePath(string path) {
      
      HttpRedirect redirect = null;
      string[] sections;
      int index;
      if(path[0] == '/') {
        // split the request path into directory sections
        sections = path.Split(Chars.ForwardSlash);
        index = -1;
      } else {
        sections = path.Split(Chars.ForwardSlash);
        index = -2;
      }
      
      // persistance of the current path
      path = "/";
      
      // the first iteration?
      bool first = true;
      // redirected path is absolute?
      bool absolute = false;
      
      // iterate the request path sections
      while(++index < sections.Length) {
        
        // is the first iteration?
        if(!first) {
          // is the redirect absolute? yes, end the redirects
          if(absolute) break;
          
          // is the section empty? yes, skip the section
          if(sections[index].Length == 0) continue;
          
          // append the section
          path = path + Chars.ForwardSlash + sections[index];
        }
        
        // should the path be redirected?
        if(Redirects.TryGetValue(path, out redirect)) {
          
          // update the current path
          path = redirect.Target;
          absolute = redirect.Absolute;
          
          int iteration = 1;
          // while the current path is to be redirected
          while(Redirects.TryGetValue(path, out redirect)) {
            
            // update the current path
            path = redirect.Target;
            absolute = redirect.Absolute;
            
            // have there been an excessive number of redirects?
            if(++iteration == 20) {
              
              // yes, start maintaining a collection of redirected paths
              ArrayRig<string> redirects = new ArrayRig<string>();
              redirects.Add(path);
              
              // while the current path is to be redirected
              while(Redirects.TryGetValue(path, out redirect)) {
                
                // update the current path
                path = redirect.Target;
                absolute = redirect.Absolute;
                
                // has the redirect path already been hit?
                if(redirects.Contains(path)) {
                  // yes, return false, a redirect loop was hit
                  Log.Warning("Redirect loop detected. Request denied.");
                  return null;
                }
                
                // add to the redirected collection
                redirects.Add(path);
                
              }
              break;
            }
          }
          
          first = false;
          
        } else if(first) {
          
          path = string.Empty;
          first = false;
          
        }
      }
      
      // return the authentication value
      return path;
    }
    
    /// <summary>
    /// On a resource wrapper being parsed.
    /// </summary>
    protected void OnWrapperParsed(ElementParser parser, HttpConnection client, WebResource resource) {
      
      // get the element that is to be replaced
      var content = parser.Root.Build();
      
      // replace text where required
      string replaceString = "{ReplaceText(" + resource.MimeType + ")}";
      int replaceIndex = content.IndexOf(replaceString, StringComparison.Ordinal);
      while(replaceIndex >= 0) {
        var sections = content.Split(replaceIndex, 0, replaceString.Length);
        
        content = sections.ArgA + resource.GetString() + sections.ArgB;
        
        replaceIndex = content.IndexOf(replaceString, StringComparison.Ordinal);
      }
      
      // replace source strings where required
      replaceString = "{ReplaceSource(" + resource.MimeType + ")}";
      replaceIndex = content.IndexOf(replaceString, StringComparison.Ordinal);
      
      while(replaceIndex >= 0) {
        var sections = content.Split(replaceIndex, 0, replaceString.Length);
        
        content = sections.ArgA + resource.Path + sections.ArgB;
        
        replaceIndex = content.IndexOf(replaceString, StringComparison.Ordinal);
      }
      
      // just start sending the resource
      client.Send(parser.Root, true, _defaultSendOptions);
      
    }
    
    /// <summary>
    /// On the error page being parsed.
    /// </summary>
    protected void OnErrorPageParsed(ElementParser parser, HttpConnection client, string error) {
      
      // was the page parsed successfully?
      if(parser.Root == null) {
        // no, log the error
        Log.Error(parser.Error);
        return;
      }
      
      // yes, replace the error string placeholder with the error string
      var errorElement = parser.Root.FindChild("error");
      
      if(errorElement != null) {
        // append the error to the element content
        errorElement.ContentString = errorElement.ContentString + error;
      }
      
      // send the web page
      client.Send(parser.Root.Build());
      
    }
    
    /// <summary>
    /// Send the default error web page.
    /// </summary>
    protected void SendDefaultError(HttpConnection client, string error) {
      client.Send(
        "<html xmlns=\"http://www.w3.org/1999/xhtml\">" +
        "  <head>" +
        "    <meta content=\"text/html;charset=utf-8\" http-equiv=\"Content-Type\">" +
        "    <meta content=\"utf-8\" http-equiv=\"encoding\">" +
        "    <title id=\"titleHeader\">Error</title>" +
        "  </head>" +
        "  <body>" +
        "    <h1 id=\"titleBody\">There was an Error.</h1>" +
        "    <div>" +
        "      <p>An error occured with your request. "+error+"</p>" +
        "    </div>" +
        "  </body>" +
        "</html>", new HttpSendOptions {ContentType = "text/html"});
    }
    
    /// <summary>
    /// On a client encountering an error while receiving a web request.
    /// </summary>
    protected virtual void OnReceiveError(Exception ex, HttpConnection client) {
      Log.Info("Client receive error.", ex);
      client.Dispose();
    }
    
    /// <summary>
    /// On a web client encountering an error sending data.
    /// </summary>
    protected virtual void OnSendError(string error, HttpConnection client) {
      Log.Info("Client send error. " + error);
      client.Dispose();
    }
    
    /// <summary>
    /// On a new web client connecting to the site server.
    /// </summary>
    protected virtual void OnConnection(HttpConnection connection) {
      
      if(connection == null) return;
      
      try {
        
        if(connection.Client == null) {
          connection.Dispose();
          return;
        }
        
        // subscribe to the client web requests
        connection.Client.OnReceive = new ActionSet<HttpRequest>(OnRequest);
        connection.Client.OnError = new ActionSet<Exception, HttpConnection>(OnReceiveError, null, connection);
        
        // has the callback been assigned? yes, run it
        if(_onConnection != null) _onConnection.Run(connection);
        
      } catch(Exception ex) {
        
        Log.Error("Error OnConnection.", ex);
        
      }
      
    }
    
    /// <summary>
    /// On the site configuration being loaded.
    /// </summary>
    protected virtual void OnConfiguration(Configuration configuration) {
      
      // persist the configuration
      _configuration = configuration;
      
      // is the name set?
      if(!_configuration["Name"].Set) {
        // no, get the name from the path
        int lastIndex = Path.LastIndexOf(Chars.ForwardSlash);
        
        if(lastIndex == Path.Length-1) {
          int startIndex = Path.LastIndexOf(Chars.ForwardSlash, lastIndex-1);
          _configuration["Name"].String = Path.Substring(startIndex + 1, lastIndex - startIndex - 1);
        } else {
          _configuration["Name"].String = Path.Substring(lastIndex + 1);
        }
      }
      
      // have authentication paths been defined?
      if(_configuration["Authentication"].ArraySet) {
        // iterate defined authentications
        foreach(Node auth in _configuration["Authentication"].Array) {
          if(auth.DictionarySet) {
            // get the path
            var authentication = HttpRequirement.Load(auth);
            
            // add the authentication to the collection
            Authentication.Add(authentication.Path, authentication);
          }
          
        }
      }
      
      // have redirects been defined?
      if(_configuration["Redirects"].ArraySet) {
        // yes, iterate the defined redirects
        foreach(var redirect in _configuration["Redirects"].Array) {
          // add each redirect
          Redirects.Add(redirect["Path"].String, new HttpRedirect(redirect["Target"].String, redirect["Absolute"].Bool));
        }
      }
      
      _configuration.Node.Object = null;
      
      // save the configuration
      _configuration.Save();
      
      // start the web server
      Server = new HttpServer(_configuration, OnConnection);
      Server.Start();
      
    }
    
  }

}


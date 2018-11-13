/*
 * User: FloppyNipples
 * Date: 26/02/2017
 * Time: 19:09
 */
using System;
using System.Collections.Generic;

using Efz.Collections;
using Efz.Tools;

namespace Efz.Web.Display {
  
  /// <summary>
  /// Builder of multiple elements.
  /// </summary>
  public class ElementBuilder {
    
    //----------------------------------//
    
    /// <summary>
    /// Actions that can be taken by an element builder.
    /// </summary>
    internal enum BuilderAction {
      Alter = 0,
      Add = 1,
      Replace = 2,
      Remove = 3,
    }
    
    /// <summary>
    /// Key of the element to retrieve from the Elements instance.
    /// Either 'Key', 'GetElement' or 'Element' should be specified.
    /// </summary>
    public string Key;
    /// <summary>
    /// Element of the builder instance.
    /// Either 'Key', 'GetElement' or 'Element' should be specified.
    /// </summary>
    public IFunc<Element> GetElement;
    /// <summary>
    /// Static element to be inserted in place of this builder instance.
    /// Either 'Key', 'GetElement' or 'Element' should be specified.
    /// </summary>
    public Element Element;
    /// <summary>
    /// Attributes to be applied to the element this builder represents.
    /// </summary>
    public Dictionary<string, Teple<string, IFunc<string>>> Attributes;
    /// <summary>
    /// Content to be set on the element.
    /// </summary>
    public ArrayRig<Teple<string, IFunc<string>>> Content;
    
    /// <summary>
    /// Builders of child elements referenced by id.
    /// </summary>
    public Dictionary<string, ElementBuilder> SingleChildren {
      get {
        if(_singleChildren == null) _singleChildren = new Dictionary<string, ElementBuilder>();
        return _singleChildren;
      }
    }
    
    /// <summary>
    /// Collections of builders of child elements referenced by id.
    /// </summary>
    public Dictionary<string, ArrayRig<ElementBuilder>> MultiChildren {
      get {
        if(_multiChildren == null) _multiChildren = new Dictionary<string, ArrayRig<ElementBuilder>>();
        return _multiChildren;
      }
    }
    
    /// <summary>
    /// Parent of this element builder.
    /// </summary>
    public ElementBuilder Parent;
    
    /// <summary>
    /// Flag indicating whether the builder will make any changes.
    /// </summary>
    public bool Empty {
      get {
        return (_singleChildren == null || _singleChildren.Count == 0) &&
          (Attributes == null || Attributes.Count == 0);
      }
    }
    
    /// <summary>
    /// Action to be taken by the builder.
    /// </summary>
    internal BuilderAction Action;
    
    //----------------------------------//
    
    /// <summary>
    /// Inner collection of children that represent single elements.
    /// </summary>
    protected Dictionary<string, ElementBuilder> _singleChildren;
    /// <summary>
    /// Inner collection of children that represent elements to be added.
    /// </summary>
    protected Dictionary<string, ArrayRig<ElementBuilder>> _multiChildren;
    
    /// <summary>
    /// Optional callback on built.
    /// </summary>
    protected IAction<Element> _onBuilt;
    
    //----------------------------------//
    
    /// <summary>
    /// Initialize a new instance of the element builder.
    /// </summary>
    public ElementBuilder(string key) {
      Key = key;
      Action = BuilderAction.Alter;
    }
    /// <summary>
    /// Initialize a new instance of the element builder.
    /// </summary>
    public ElementBuilder(string key, IAction<Element> onBuilt) {
      Key = key;
      _onBuilt = onBuilt;
      Action = BuilderAction.Alter;
    }
    /// <summary>
    /// Initialize a new instance of the element builder.
    /// </summary>
    public ElementBuilder(Element element, IAction<Element> onBuilt) {
      Element = element;
      _onBuilt = onBuilt;
      Action = BuilderAction.Alter;
    }
    /// <summary>
    /// Inner constructor.
    /// </summary>
    private ElementBuilder() {
      Action = BuilderAction.Alter;
    }
    
    /// <summary>
    /// Add a child to the element builder to replace an existing id.
    /// </summary>
    public ElementBuilder ReplaceChild(string id, string key, Action<Element> onElement) {
      return ReplaceChild(id, key, new ActionSet<Element>(onElement));
    }
    
    /// <summary>
    /// Add a child to the element builder to replace an existing id.
    /// </summary>
    public ElementBuilder ReplaceChild(string id, string key, IAction<Element> onElement = null) {
      ElementBuilder builder;
      if(!SingleChildren.TryGetValue(id, out builder)) _singleChildren.Add(id, builder = new ElementBuilder());
      builder.Action = BuilderAction.Replace;
      builder.Key = key;
      builder.GetElement = null;
      if(onElement != null) builder._onBuilt = onElement;
      return builder;
    }
    
    /// <summary>
    /// When built, an element of the specified id will be replaced by the specified element.
    /// </summary>
    public ElementBuilder ReplaceChild(string id, Func<Element> getElement, Action<Element> onElement) {
      return ReplaceChild(id, new FuncSet<Element>(getElement), new ActionSet<Element>(onElement));
    }
    
    /// <summary>
    /// Add a child to the element builder to replace an existing id.
    /// </summary>
    public ElementBuilder ReplaceChild(string id, IFunc<Element> getElement, IAction<Element> onElement = null) {
      ElementBuilder builder;
      if(!SingleChildren.TryGetValue(id, out builder)) _singleChildren.Add(id, builder = new ElementBuilder());
      builder.Action = BuilderAction.Replace;
      builder.GetElement = getElement;
      if(onElement != null) builder._onBuilt = onElement;
      return builder;
    }
    
    /// <summary>
    /// When built, an element of the specified id will be replaced by the specified element.
    /// </summary>
    public ElementBuilder ReplaceChild(string id, Element element, Action<Element> onElement) {
      return ReplaceChild(id, element, new ActionSet<Element>(onElement));
    }
    
    /// <summary>
    /// Add a child to the element builder to replace an existing id.
    /// </summary>
    public ElementBuilder ReplaceChild(string id, Element element, IAction<Element> onElement = null) {
      ElementBuilder builder;
      if(!SingleChildren.TryGetValue(id, out builder)) _singleChildren.Add(id, builder = new ElementBuilder());
      builder.Action = BuilderAction.Replace;
      builder.Element = element;
      if(onElement != null) builder._onBuilt = onElement;
      return builder;
    }
    
    /// <summary>
    /// Add a child to the current element builder.
    /// </summary>
    public ElementBuilder AddChild(string id, string key, Action<Element> onElement) {
      return AddChild(id, key, Act.New(onElement));
    }
    /// <summary>
    /// Add a child to the current element builder.
    /// </summary>
    public ElementBuilder AddChild(string id, string key, IAction<Element> onElement = null) {
      ArrayRig<ElementBuilder> builders;
      if(!MultiChildren.TryGetValue(id, out builders)) _multiChildren.Add(id, builders = new ArrayRig<ElementBuilder>());
      var builder = new ElementBuilder();
      builders.Add(builder);
      builder.Action = BuilderAction.Add;
      builder.Key = key;
      if(onElement != null) builder._onBuilt = onElement;
      return builder;
    }
    
    /// <summary>
    /// Add a child to the current element builder. The specified callback function will
    /// be called if an element cannot be found for the specified key.
    /// </summary>
    public ElementBuilder AddChild(string id, string key, Func<Element> getFallback, Action<Element> onElement = null) {
      return AddChild(id, key, new FuncSet<Element>(getFallback), Act.New(onElement));
    }
    /// <summary>
    /// Add a child to the current element builder. The specified callback function will
    /// be called if an element cannot be found for the specified key.
    /// </summary>
    public ElementBuilder AddChild(string id, string key, IFunc<Element> getFallback, IAction<Element> onElement = null) {
      ArrayRig<ElementBuilder> builders;
      if(!MultiChildren.TryGetValue(id, out builders)) _multiChildren.Add(id, builders = new ArrayRig<ElementBuilder>());
      var builder = new ElementBuilder();
      builders.Add(builder);
      builder.Action = BuilderAction.Add;
      builder.GetElement = getFallback;
      builder.Key = key;
      if(onElement != null) builder._onBuilt = onElement;
      return builder;
    }
    
    /// <summary>
    /// Add a child to the current element builder.
    /// </summary>
    public ElementBuilder AddChild(string id, Func<Element> getElement, Action<Element> onElement) {
      return AddChild(id, new FuncSet<Element>(getElement), Act.New(onElement));
    }
    /// <summary>
    /// Add a child to the current element builder.
    /// </summary>
    public ElementBuilder AddChild(string id, IFunc<Element> getElement, IAction<Element> onElement = null) {
      ArrayRig<ElementBuilder> builders;
      if(!MultiChildren.TryGetValue(id, out builders)) _multiChildren.Add(id, builders = new ArrayRig<ElementBuilder>());
      var builder = new ElementBuilder();
      builders.Add(builder);
      builder.Action = BuilderAction.Add;
      builder.GetElement = getElement;
      if(onElement != null) builder._onBuilt = onElement;
      return builder;
    }
    
    /// <summary>
    /// Add a child to the current element builder.
    /// </summary>
    public ElementBuilder AddChild(string id, Element element, Action<Element> onElement) {
      return AddChild(id, element, Act.New(onElement));
    }
    /// <summary>
    /// Add a child to the current element builder.
    /// </summary>
    public ElementBuilder AddChild(string id, Element element, IAction<Element> onElement = null) {
      ArrayRig<ElementBuilder> builders;
      if(!MultiChildren.TryGetValue(id, out builders)) _multiChildren.Add(id, builders = new ArrayRig<ElementBuilder>());
      var builder = new ElementBuilder();
      builders.Add(builder);
      builder.Action = BuilderAction.Add;
      builder.Element = element;
      if(onElement != null) builder._onBuilt = onElement;
      return builder;
    }
    
    /// <summary>
    /// Add an attribute to be applied to the element of the specified id.
    /// </summary>
    public ElementBuilder SetAttribute(string id, string key, string value, IAction<Element> onElement = null) {
      ElementBuilder builder;
      if(!SingleChildren.TryGetValue(id, out builder)) _singleChildren.Add(id, builder = new ElementBuilder());
      if(builder.Attributes == null) builder.Attributes = new Dictionary<string, Teple<string, IFunc<string>>>();
      Teple<string, IFunc<string>> attribute;
      if(builder.Attributes.TryGetValue(key, out attribute)) {
        attribute.ArgA = value;
      } else {
        builder.Attributes.Add(key, new Teple<string, IFunc<string>>(value, null));
      }
      if(onElement != null) builder._onBuilt = onElement;
      return builder;
    }
    
    /// <summary>
    /// Set the content of the element the builder represents.
    /// </summary>
    public ElementBuilder SetContent(string content) {
      if(Content == null) Content = new ArrayRig<Teple<string, IFunc<string>>>(1);
      else Content.Clear();
      Content.Add(new Teple<string, IFunc<string>>(content, null));
      return this;
    }
    
    /// <summary>
    /// Set the content of the element the builder represents.
    /// </summary>
    public ElementBuilder SetContent(Func<string> getContent) {
      if(Content == null) Content = new ArrayRig<Teple<string, IFunc<string>>>(1);
      else Content.Clear();
      Content.Add(new Teple<string, IFunc<string>>(null, new FuncSet<string>(getContent)));
      return this;
    }
    
    /// <summary>
    /// Add an attribute to be applied to the element of the specified id.
    /// </summary>
    public ElementBuilder SetContent(string id, string content, IAction<Element> onElement = null) {
      ElementBuilder builder;
      if(!SingleChildren.TryGetValue(id, out builder)) _singleChildren.Add(id, builder = new ElementBuilder());
      if(builder.Content == null) builder.Content = new ArrayRig<Teple<string, IFunc<string>>>();
      else builder.Content.Clear();
      builder.Content.Add(new Teple<string, IFunc<string>>(content, null));
      if(onElement != null) builder._onBuilt = onElement;
      return builder;
    }
    
    /// <summary>
    /// Add an attribute to be applied to the element of the specified id.
    /// </summary>
    public ElementBuilder AddContent(string id, string content, IAction<Element> onElement = null) {
      ElementBuilder builder;
      if(!SingleChildren.TryGetValue(id, out builder)) _singleChildren.Add(id, builder = new ElementBuilder());
      if(builder.Content == null) builder.Content = new ArrayRig<Teple<string, IFunc<string>>>();
      builder.Content.Add(new Teple<string, IFunc<string>>(content, null));
      if(onElement != null) builder._onBuilt = onElement;
      return builder;
    }
    
    /// <summary>
    /// Add an attribute to be applied to the element of the specified id.
    /// </summary>
    public ElementBuilder SetContent(string id, Func<string> getContent, IAction<Element> onElement = null) {
      ElementBuilder builder;
      if(!SingleChildren.TryGetValue(id, out builder)) _singleChildren.Add(id, builder = new ElementBuilder());
      if(builder.Content == null) builder.Content = new ArrayRig<Teple<string, IFunc<string>>>();
      else builder.Content.Clear();
      builder.Content.Add(new Teple<string, IFunc<string>>(null, new FuncSet<string>(getContent)));
      if(onElement != null) builder._onBuilt = onElement;
      return builder;
    }
    
    /// <summary>
    /// Add an attribute to be applied to the element of the specified id.
    /// </summary>
    public ElementBuilder AddContent(string id, Func<string> content, IAction<Element> onElement = null) {
      ElementBuilder builder;
      if(!SingleChildren.TryGetValue(id, out builder)) _singleChildren.Add(id, builder = new ElementBuilder());
      if(builder.Content == null) builder.Content = new ArrayRig<Teple<string, IFunc<string>>>();
      builder.Content.Add(new Teple<string, IFunc<string>>(null, new FuncSet<string>(content)));
      if(onElement != null) builder._onBuilt = onElement;
      return builder;
    }
    
    /// <summary>
    /// Add an attribute to be applied to the element of the specified id.
    /// </summary>
    public ElementBuilder SetAttribute(string id, string key, Func<string> getValue) {
      ElementBuilder builder;
      // get the builder of the child if already assigned
      if(SingleChildren.TryGetValue(id, out builder)) {
        Teple<string, IFunc<string>> value;
        if(builder.Attributes.TryGetValue(key, out value)) value.ArgB = new FuncSet<string>(getValue);
        else builder.Attributes.Add(key, new Teple<string, IFunc<string>>(null, new FuncSet<string>(getValue)));
      } else {
        builder = new ElementBuilder();
        _singleChildren.Add(id, builder = new ElementBuilder());
        builder.Attributes.Add(id, new Teple<string, IFunc<string>>(null, new FuncSet<string>(getValue)));
      }
      return builder;
    }
    
    /// <summary>
    /// Get a builder for the element specified by id.
    /// </summary>
    public ElementBuilder Get(string id) {
      ElementBuilder builder;
      if(!SingleChildren.TryGetValue(id, out builder)) _singleChildren.Add(id, builder = new ElementBuilder());
      return builder;
    }
    
    /// <summary>
    /// Remove the specified child by id.
    /// </summary>
    public void RemoveChild(string id) {
      ElementBuilder builder;
      if(SingleChildren.TryGetValue(id, out builder)) {
        builder.Action = BuilderAction.Remove;
      } else {
        builder = new ElementBuilder();
        builder.Action = BuilderAction.Remove;
        _singleChildren.Add(id, builder);
      }
    }
    
    /// <summary>
    /// Remove the attribute from being applied to the element.
    /// </summary>
    public void RemoveAttribute(string id, string key) {
      if(_singleChildren == null) return;
      ElementBuilder builder;
      if(_singleChildren.TryGetValue(id, out builder) && builder.Attributes.ContainsKey(key)) {
        builder.Attributes.Remove(key);
      }
    }
    
    /// <summary>
    /// Build the elements retrieving as required from the specified elements instance.
    /// </summary>
    public async void Build(Elements elements) {
      if(GetElement == null) {
        elements.Get(Key,
          new Act<Element, Elements, IAction<Element>>(OnRootLoaded, null, elements, null));
      } else {
        OnRootLoaded(await GetElement.RunAsync(), elements, null);
      }
    }
    
    /// <summary>
    /// Build the elements retrieving as required from the specified elements instance.
    /// </summary>
    public async void Build(Elements elements, Action<Element> onBuilt) {
      if(GetElement == null) {
        elements.Get(Key, Act.New(OnRootLoaded, (Element)null, elements, new ActionSet<Element>(onBuilt)));
      } else {
        OnRootLoaded(await GetElement.RunAsync(), elements, new ActionSet<Element>(onBuilt));
      }
    }
    
    /// <summary>
    /// Build the elements retrieving as required from the specified elements instance.
    /// Don't pass a delayed IAction.
    /// </summary>
    public async void Build(Elements elements, IAction<Element> onBuilt) {
      if(GetElement == null) {
        elements.Get(Key, Act.New(OnRootLoaded, (Element)null, elements, onBuilt));
      } else {
        OnRootLoaded(await GetElement.RunAsync(), elements, onBuilt);
      }
    }
    
    //----------------------------------//
    
    /// <summary>
    /// On the element builders root load.
    /// </summary>
    protected void OnRootLoaded(Element element, Elements elements, IAction<Element> onBuilt) {
      
      // determine the requirements
      Ticker ticker;
      if(onBuilt == null) {
        if(_onBuilt == null) {
          ticker = new Ticker();
        } else {
          _onBuilt.ArgA = element;
          ticker = new Ticker(_onBuilt);
        }
      } else {
        if(_onBuilt == null) {
          onBuilt.ArgA = element;
          ticker = new Ticker(Act.New(onBuilt));
        } else {
          _onBuilt.ArgA = element;
          onBuilt.ArgA = element;
          ticker = new Ticker(Act.New(new ActionPair(onBuilt, _onBuilt)));
        }
      }
      
      // load this element builders children
      Build(ticker, element, elements);
      
    }
    
    /// <summary>
    /// Inner build method for children pages.
    /// </summary>
    protected async void BuildReplaceChild(Ticker ticker, Elements elements, Element parent, string id) {
      if(Key != null) {
        elements.Get(Key, Act.New(OnReplaceChildLoaded, (Element)null, ticker, elements, id, parent));
      } else if(Element == null) {
        OnReplaceChildLoaded(await GetElement.RunAsync(), ticker, elements, id, parent);
      } else {
        OnReplaceChildLoaded(Element, ticker, elements, id, parent);
      }
    }
    
    /// <summary>
    /// Inner build method for children pages.
    /// </summary>
    protected async void BuildAddedChild(Ticker ticker, Elements elements, Element parent, int index) {
      if(Key != null) {
        elements.Get(Key, Act.New(OnAddedChildLoaded, (Element)null, ticker, elements, parent, index));
      } else if(Element == null) {
        OnAddedChildLoaded(await GetElement.RunAsync(), ticker, elements, parent, index);
      } else {
        OnAddedChildLoaded(Element, ticker, elements, parent, index);
      }
    }
    
    /// <summary>
    /// Inner on page loaded method for replacement children pages.
    /// </summary>
    protected async void OnReplaceChildLoaded(Element element, Ticker ticker, Elements elements, string id, Element parent) {
      
      if(element == null) {
        if(GetElement != null) element = await GetElement.RunAsync();
        if(element == null) Log.Warning("Element to replace id '"+id+"' was Null.");
      }
      
      // replace the element of the specified id
      var replace = parent.FindChild(id);
      if(replace == null) {
        Log.Error("ID '"+id+"' not found in element '"+parent+"'.");
        return;
      }
      replace.Replace(element);
      
      // has the on built callback been assigned?
      if(_onBuilt == null) {
        // no, load this element builders children
        Build(ticker, element, elements);
      } else {
        // yes, run the callback, load this element builders children and move the state closer to completion
        _onBuilt.ArgA = element;
        ManagerUpdate.Control.AddSingle(new ActionPair(_onBuilt, ActionSet.New(Build, ticker, element, elements)));
      }
    }
    
    /// <summary>
    /// Inner on page loaded method for children pages to be added.
    /// </summary>
    protected async void OnAddedChildLoaded(Element element, Ticker ticker, Elements elements, Element parent, int index) {
      
      if(element == null && GetElement != null) element = await GetElement.RunAsync();
      
      // add the page as a child of the parent page
      parent.SetChild(element, index);
      
      // has the on built callback been assigned?
      if(_onBuilt == null) {
        // no, load this element builders children
        Build(ticker, element, elements);
      } else {
        // yes, run the callback, load this element builders children and move the state closer to completion
        _onBuilt.ArgA = element;
        ManagerUpdate.Control.AddSingle(new ActionPair(_onBuilt, ActionSet.New(Build, ticker, element, elements)));
      }
      
    }
    
    /// <summary>
    /// Load this elements children. This is the final function of all built elements.
    /// </summary>
    protected async void Build(Ticker ticker, Element element, Elements elements) {
      
      if(_singleChildren != null) {
        
        // initiaize the parent cache used to keep track of the desired indices of added elements.
        // Once an element is added to the cache an index is used to insert each new child as they
        // are built
        Dictionary<string, Teple<Element, int>> parentCache = new Dictionary<string, Teple<Element, int>>();
        
        // iterate the children to action
        foreach(var child in _singleChildren) {
          ticker.Push();
          switch(child.Value.Action) {
            case BuilderAction.Replace:
              
              ManagerUpdate.Control.AddSingle(child.Value.BuildReplaceChild, ticker, elements, element, child.Key);
              
              break;
            case BuilderAction.Add:
              
              if(parentCache == null) parentCache = new Dictionary<string, Teple<Element, int>>();;
              Teple<Element, int> next;
              if (parentCache.TryGetValue(child.Key, out next)) {
                ++next.ArgB;
              } else {
                next = new Teple<Element, int>(element.FindChild(child.Key), 0);
                next.ArgB = next.ArgA.ChildCount;
                parentCache.Add(child.Key, next);
              }
              ManagerUpdate.Control.AddSingle(child.Value.BuildAddedChild, ticker, elements, next.ArgA, next.ArgB);
              
              break;
            default:
              
              var childElement = element.FindChild(child.Key);
              if(childElement == null) Log.Error("Child element '"+child.Key+"' not found in '"+element+"'.");
              ManagerUpdate.Control.AddSingle(child.Value.Build, ticker, childElement, elements);
              
              break;
          }
        }
      }
      
      if(_multiChildren != null) {
        
        // initiaize the parent cache used to keep track of the desired indices of added elements.
        // Once an element is added to the cache an index is used to insert each new child as they
        // are built
        Dictionary<string, Teple<Element, int>> parentCache = new Dictionary<string, Teple<Element, int>>();
        
        // iterate the children to action
        foreach(var collection in _multiChildren) {
          foreach(var child in collection.Value) {
            ticker.Push();
            
            switch(child.Action) {
              case BuilderAction.Replace:
                
                // add an action to build the replacement child
                ManagerUpdate.Control.AddSingle(child.BuildReplaceChild, ticker, elements, element, collection.Key);
                
                break;
              case BuilderAction.Add:
                
                // create a cache of references
                if(parentCache == null) parentCache = new Dictionary<string, Teple<Element, int>>();;
                Teple<Element, int> next;
                if (parentCache.TryGetValue(collection.Key, out next)) {
                  ++next.ArgB;
                } else {
                  next = new Teple<Element, int>(element.FindChild(collection.Key), 0);
                  next.ArgB = next.ArgA.ChildCount;
                  parentCache.Add(collection.Key, next);
                }
                ManagerUpdate.Control.AddSingle(child.BuildAddedChild, ticker, elements, next.ArgA, next.ArgB);
                
                break;
              case BuilderAction.Remove:
                
                var removeElement = element.FindChild(collection.Key);
                if(removeElement == null) Log.Error("Child element '"+collection.Key+"' was not found in '"+element+"' to be removed.");
                else removeElement.Remove();
                
                break;
              default:
                
                var childElement = element.FindChild(collection.Key);
                if(childElement == null) Log.Error("Child element '"+collection.Key+"' not found in '"+element+"' to be altered.");
                else ManagerUpdate.Control.AddSingle(child.Build, ticker, childElement, elements);
                
                break;
            }
          }
        }
      }
      
      // have any attributes been specified?
      if(Attributes != null) {
        // yes, iterate the attributes
        foreach(var attribute in Attributes) {
          if(attribute.Value.ArgB == null) {
            element.SetAttribute(attribute.Key, attribute.Value.ArgA);
          } else if(attribute.Value.ArgA == null) {
            element.RemoveAttribute(attribute.Key);
          } else {
            element.SetAttribute(attribute.Key, await attribute.Value.ArgB.RunAsync() ?? attribute.Value.ArgA);
          }
        }
      }
      
      // has the content been specified?
      if(Content != null) {
        // yes, iterate and add the content
        foreach(var content in Content) {
          element.Content.Add(content.ArgB == null ? content.ArgA : await content.ArgB.RunAsync() ?? content.ArgA);
        }
      }
      
      ticker.Pull();
    }
    
  }
  
  
}

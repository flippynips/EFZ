/*
 * User: FloppyNipples
 * Date: 26/03/2017
 * Time: 15:47
 */
using System;

using System.Collections.Generic;
using System.Text.RegularExpressions;
using Efz.Collections;

namespace Efz.Web.Display {
  
  /// <summary>
  /// Represents a form element with input controls.
  /// </summary>
  public class ElementInput : Element {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Type of input element.
    /// </summary>
    public InputType Type {
      get {
        if(_type == InputType.None) _type = ExtendInputType.GetInputType(this["type"]);
        return _type;
      }
      set {
        _type = value;
        this["type"] = _type.GetString();
      }
    }
    
    /// <summary>
    /// Optional callback in order to validate the input.
    /// </summary>
    public SafeAct<IHttpPostParam, ArrayRig<string>> OnValidate;
    
    /// <summary>
    /// Optional friendly name for the input element that is used if the input is invalid.
    /// Default is the 'name' attribute.
    /// </summary>
    public string FriendlyName;
    /// <summary>
    /// Flag that indicates whether to apply default validation to post parameters.
    /// Default is 'true'. The 'required' attribute should also be set.
    /// </summary>
    public bool DefaultValidation = true;
    
    /// <summary>
    /// Optional length check for parameters.
    /// </summary>
    public int MaxLength = int.MaxValue;
    /// <summary>
    /// Optional length check for parameters.
    /// </summary>
    public int MinLength = int.MinValue;
    /// <summary>
    /// Regex expression automatically populated from the 'pattern' attribute.
    /// </summary>
    public Regex Regex;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Input type.
    /// </summary>
    protected InputType _type;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Create a new form element.
    /// </summary>
    public ElementInput(Tag tag = Tag.Input) {
      Tag = tag;
    }
    
    /// <summary>
    /// Clone the element.
    /// </summary>
    public override Element Clone() {
      // create a new element table
      ElementInput clone = new ElementInput();
      
      clone.Tag = this.Tag;
      if(this._content != null) clone._content = new ArrayRig<string>(this._content);
      if(this._attributes != null) clone._attributes = new Dictionary<string, string>(this._attributes);
      if(this._style != null) clone._style = new Style(this._style);
      
      if(this.Children != null) {
        // iterate the children of this element
        foreach(var child in this.Children) {
          clone.AddChild(child.Clone());
        }
      }
      
      clone._type = this._type;
      clone.OnValidate = this.OnValidate;
      clone.FriendlyName = this.FriendlyName;
      clone.DefaultValidation = this.DefaultValidation;
      clone.MinLength = this.MinLength;
      clone.MaxLength = this.MaxLength;
      
      return clone;
    }
    
    /// <summary>
    /// Set an attribute of the element. The value may be 'Null' in order to set a flag attribute e.g. 'required'.
    /// </summary>
    public override void SetAttribute(string key, string value) {
      if(key.ToLowercase() == "type") _type = ExtendInputType.GetInputType(value);
      if(key.ToLowercase() == "pattern") Regex = new Regex(value, RegexOptions.Compiled, TimeSpan.FromMilliseconds(2));
      base.SetAttribute(key, value);
    }
    
    /// <summary>
    /// Validate the parameter. Error messages are added to the errors collection
    /// if there are issues with the parameter.
    /// </summary>
    public void Validate(IHttpPostParam parameter, ArrayRig<string> errors) {
      
      // is default validatation enabled?
      if(DefaultValidation) {
        
        // yes, is the parameter required?
        bool required = this._attributes.ContainsKey("required");
        
        if(required) {
          // yes, was the parameter retrieved?
          if(parameter == null) {
            // no, add an error
            errors.Add("Missing a required parameter.");
            
            // is the custom validation set?
            if(OnValidate != null) {
              // yes, run the custom validation
              OnValidate.Add(parameter, errors);
              OnValidate.Run();
            }
            
            return;
          }
        } else if(parameter == null) {
          return;
        }
        
        string name;
        
        // validate the input value name
        if(parameter.Params.TryGetValue("name", out name)) {
          if(!name.Equals(this["name"], StringComparison.Ordinal)) {
            errors.Add("Input element name mismatch.");
            return;
          }
        } else {
          name = this["name"];
          if(name == null) {
            errors.Add("Attribute 'name' missing.");
            return;
          }
        }
        
        // has the friendly name been assigned? yes, replace it in messages.
        if(!string.IsNullOrEmpty(FriendlyName)) name = FriendlyName;
        
        // get the post parameter as its correct type
        HttpPostParam<string> paramString = parameter as HttpPostParam<string>;
        HttpPostParam<ArrayRig<byte>> paramBinary = null;
        if(paramString == null) {
          paramBinary = parameter as HttpPostParam<ArrayRig<byte>>;
        } else if(string.IsNullOrEmpty(paramString.Value)) {
          // is the parameter required? yes, add an error
          if(required) errors.Add(name + " is required.");
          // no, skip validation
          else return;
        } else if(paramString.Value.Length > MaxLength) {
          errors.Add("Exceeded maximum characters ("+MaxLength+") for "+name+".");
          return;
        } else if(paramString.Value.Length < MinLength) {
          errors.Add("Entered value was shorter than the minimum length ("+MinLength+") for "+name+".");
          return;
        } else if(Regex != null && !Regex.IsMatch(paramString.Value)) {
          // has the title for the input been specified?
          errors.Add(this["title"] ?? "Invalid entry for "+name+".");
          return;
        }
        
        if((paramString == null || paramString.Value == string.Empty) &&
           (paramBinary == null || paramBinary.Value.Count == 0)) {
          if(required || _type == InputType.Hidden) errors.Add(name + " is required.");
          return;
        }
        
        // perform default validation based on type
        switch(_type) {
          case InputType.Checkbox:
            if(paramString == null) {
              errors.Add(name + " is required.");
              return;
            }
            if(!paramString.Value.Equals(this["value"])) {
              errors.Add(name + " has an incorrect value.");
              return;
            }
            break;
          case InputType.Color:
            if(paramString == null) {
              errors.Add("Please select a color for " + name + ".");
              return;
            }
            WebColor color;
            if(!WebColor.TryParse(paramString.Value, out color)) {
              errors.Add("Incorrect hexadecimal color format. Should be #XXXXXXXX or #XXXXXX or #XXX.");
              return;
            }
            break;
          case InputType.Date:
            if(paramString == null) {
              errors.Add("Please select " + name + ".");
              return;
            }
            DateTime date;
            if(!DateTime.TryParse(paramString.Value, out date)) {
              errors.Add("Received an incorrect date format.");
              return;
            }
            break;
          case InputType.DatetimeLocal:
            if(paramString == null) {
              errors.Add("Please select a date-time for " + name + ".");
              return;
            }
            DateTime dateTime;
            if(!DateTime.TryParse(paramString.Value, out dateTime)) {
              errors.Add("Received an incorrect date-time format.");
              return;
            }
            break;
          case InputType.Email:
            if(paramString == null) {
              errors.Add(name + " is required.");
              return;
            }
            if(!Efz.Validate.Email(paramString.Value)) {
              errors.Add("Email appears malformed.");
              return;
            }
            break;
          case InputType.File:
            if(paramBinary == null) {
              errors.Add("You didn't choose a file for " + name + ".");
              return;
            }
            if(paramBinary.Value.Count == 0) {
              errors.Add("Required file parameter is missing.");
            }
            string filename;
            if(!paramBinary.Params.TryGetValue("filename", out filename) ||
               !Fs.ParseFileName(ref filename)) {
              errors.Add("The file name of "+name+" was invalid.");
              return;
            }
            // ensure the content type matches accepted types
            string contentType;
            if(!paramBinary.Params.TryGetValue("content-type", out contentType)) {
              errors.Add("Content type of "+name+" was not provided.");
              return;
            }
            // check the accepted file types
            string accepted;
            if(_attributes.TryGetValue("accept", out accepted)) {
              var extension = Mime.GetExtension(contentType);
              if(extension == string.Empty) {
                errors.Add("Content type '"+contentType+"' was invalid.");
                return;
              }
              if(!accepted.Contains(Chars.Stop + extension + Chars.Comma) && !accepted.EndsWith(Chars.Stop + extension, StringComparison.Ordinal)) {
                errors.Add("Accepted file types include '"+accepted+"'.");
                return;
              }
            }
            break;
          case InputType.Month:
            if(paramString == null) {
              errors.Add("You didn't choose a month for " + name + ".");
              return;
            }
            if(paramString.Value.Length != 7 || !RegexMonth.IsMatch(paramString.Value)) {
              errors.Add(name + " seems malformed.");
              return;
            }
            if(paramString.Value[4] == Chars.Dash) {
              paramString.Value = paramString.Value.Substring(5) + Chars.Dash + paramString.Value.Substring(0, 4);
            }
            break;
          case InputType.Number:
            int number;
            if(!int.TryParse(paramString.Value, out number)) {
              errors.Add("The "+name+" seems malformed.");
              return;
            }
            if(_attributes.ContainsKey("min")) {
              int min = _attributes["min"].ToInt32();
              if(number < min) {
                errors.Add(name + " was not within a valid range.");
                return;
              }
            }
            if(_attributes.ContainsKey("max")) {
              int max = _attributes["max"].ToInt32();
              if(number > max) {
                errors.Add(name + " was not within a valid range.");
                return;
              }
            }
            if(_attributes.ContainsKey("step")) {
              int step = _attributes["step"].ToInt32();
              if(number % step != 0) {
                errors.Add(name + " was not a valid value.");
                return;
              }
            }
            break;
          case InputType.Password:
            if(paramString == null) {
              errors.Add(name + " is required.");
              return;
            }
            if(!RegexPassword.IsMatch(paramString.Value)) {
              errors.Add("Enter a password with at least four(4) characters.");
              return;
            }
            break;
          case InputType.Range:
            if(paramString == null) {
              errors.Add("Value required for "+name+".");
              return;
            }
            int range;
            if(!int.TryParse(paramString.Value, out range)) {
              errors.Add("Invalid "+name+" value received.");
              return;
            }
            if(_attributes.ContainsKey("min")) {
              int min = _attributes["min"].ToInt32();
              if(range < min) {
                errors.Add(name + " was not within a valid range.");
                return;
              }
            }
            if(_attributes.ContainsKey("max")) {
              int max = _attributes["max"].ToInt32();
              if(range > max) {
                errors.Add(name + " was not within a valid range.");
                return;
              }
            }
            if(_attributes.ContainsKey("step")) {
              int step = _attributes["step"].ToInt32();
              if(range % step != 0) {
                errors.Add(name + " was not a valid value.");
                return;
              }
            }
            break;
          case InputType.Tel:
            if(paramString == null) {
              errors.Add(name + " is required.");
              return;
            }
            if(!Efz.Validate.PhoneNumber(paramString.Value)) {
              errors.Add(name + " was suspected of being invalid.");
              return;
            }
            break;
          case InputType.Text:
            if(paramString == null || paramString.Value.Length == 0) {
              errors.Add(name + " is required.");
              return;
            }
            break;
          case InputType.Time:
            if(paramString == null) {
              errors.Add(name + " is required.");
              return;
            }
            if(paramString.Value.Length != 5 || !RegexTime.IsMatch(paramString.Value)) {
              errors.Add(name + " was found to be invalid.");
              return;
            }
            break;
          case InputType.Url:
            if(paramString == null) {
              errors.Add(name + " is required.");
              return;
            }
            if(paramString.Value.Length < 4 || paramString.Value.Length > 200 ||
               !RegexUrl.IsMatch(paramString.Value)) {
              errors.Add("Url was invalid.");
              return;
            }
            break;
          case InputType.Week:
            if(paramString == null) {
              errors.Add("Please select " + name + ".");
              return;
            }
            if(paramString.Value.Length != 8 || !RegexWeek.IsMatch(paramString.Value)) {
              errors.Add(name + " was malformed.");
              return;
            }
            break;
        }
      }
      
      // is the custom validation set? no, complete
      if(OnValidate == null) return;
      
      // run the custom validation
      OnValidate.Add(parameter, errors);
      OnValidate.Run();
    }
    
    //-------------------------------------------//
    
  }
}

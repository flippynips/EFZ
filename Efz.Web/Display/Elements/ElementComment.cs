/*
 * User: Bob
 * Date: 13/11/2016
 * Time: 10:53
 */
/*
 * User: Joshua
 * Date: 6/08/2016
 * Time: 6:11 PM
 */
using System;
using System.Text;

namespace Efz.Web.Display {
  
  /// <summary>
  /// Comment element that gets written to a new line. Only the Content
  /// is taken into account.
  /// </summary>
  public class ElementComment : Element {
    
    //----------------------------------//
    
    public bool Comment;
    
    //----------------------------------//
    
    //----------------------------------//
    
    public ElementComment(bool comment = true) {
      Comment = comment;
    }
    
    /// <summary>
    /// Append the element to the specified string builder.
    /// </summary>
    public override void Build(StringBuilder builder) {
      builder.AppendLine();
      if(Comment) builder.Append("<!-- ");
      else builder.Append(Chars.LessThan);
      builder.Append(ContentString);
      if(Comment) builder.Append(" -->");
      else builder.Append(Chars.GreaterThan);
      
      if(Children != null) {
        builder.Append(Chars.NewLine);
        // build and append the children elements
        for (int i = 0; i < Children.Count; ++i) {
          Children[i].Build(builder);
        }
      }
    }
    
    //----------------------------------//
    
  }
  
}

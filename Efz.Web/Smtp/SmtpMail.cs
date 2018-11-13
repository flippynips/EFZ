/*
 * User: FloppyNipples
 * Date: 28/08/2018
 * Time: 8:54 PM
 */
using System;
using System.Collections.Generic;
using System.Net.Mail;
using Efz.Web.Display;

namespace Efz.Web.Smtp {
  
  /// <summary>
  /// Wrapper for smtp mail messages.
  /// </summary>
  public class SmtpMail {
    
    //----------------------------------------//
    
    /// <summary>
    /// Source mail address.
    /// </summary>
    public string From;
    
    /// <summary>
    /// Destination emails.
    /// </summary>
    public List<string> To;
    /// <summary>
    /// Secondary destination emails.
    /// </summary>
    public List<string> Bcc {
      get {
        if(_bcc == null) _bcc = new List<string>();
        return _bcc;
      }
    }
    /// <summary>
    /// Tertiary destination emails.
    /// </summary>
    public List<string> Cc {
      get {
        if(_cc == null) _cc = new List<string>();
        return _cc;
      }
    }
    
    /// <summary>
    /// Subject of the email message.
    /// </summary>
    public string Subject;
    
    /// <summary>
    /// Plain text body string.
    /// </summary>
    public string BodyString;
    /// <summary>
    /// Html email body.
    /// </summary>
    public ElementBuilder BodyElement;
    
    //----------------------------------------//
    
    /// <summary>
    /// Inner secondary email addresses.
    /// </summary>
    private List<string> _bcc;
    /// <summary>
    /// Inner tertiary email addresses.
    /// </summary>
    private List<string> _cc;
    
    //----------------------------------------//
    
    /// <summary>
    /// Create a new smtp message.
    /// </summary>
    public SmtpMail(string subject, string fromEmail = null, params string[] to) {
      Subject = subject;
      From = fromEmail;
      To = new List<string>(to);
    }
    
    /// <summary>
    /// Build the mail message.
    /// </summary>
    public void Build(IAction<MailMessage> onBuilt, Elements elements = null) {
      
      MailMessage mail = new MailMessage();
      
      mail.From = new MailAddress(From);
      if(To != null) {
        foreach(var to in To) {
          mail.To.Add(to);
        }
      }
      if(_bcc != null) {
        foreach(var bcc in _bcc) {
          mail.Bcc.Add(bcc);
        }
      }
      if(_cc != null) {
        foreach(var cc in _cc) {
          mail.CC.Add(cc);
        }
      }
      
      mail.BodyEncoding = System.Text.Encoding.UTF8;
      mail.Subject = Subject;
      
      if(BodyString != null) {
        mail.Body = BodyString;
      } else if(BodyElement != null) {
        mail.IsBodyHtml = true;
        BodyElement.Build(elements, Act.New(OnBuilt, (Element)null, mail, onBuilt));
        return;
      } else {
        throw new InvalidOperationException("Neither BodyString nor BodyElement was set.");
      }
      
      onBuilt.ArgA = mail;
      ManagerUpdate.Control.AddSingle(onBuilt.Run);
      
    }
    
    /// <summary>
    /// Send the mail message.
    /// </summary>
    public void Send(SmtpClient client, Elements elements = null) {
      
      Build(Act.New(SendMessage, (MailMessage)null, client), elements);
      
    }
    
    //----------------------------------------//
    
    /// <summary>
    /// On the mail message built.
    /// </summary>
    private void OnBuilt(Element element, MailMessage message, IAction<MailMessage> onBuilt) {
      
      onBuilt.ArgA = message;
      onBuilt.Run();
      
    }
    
    /// <summary>
    /// Send the mail message on being built.
    /// </summary>
    private void SendMessage(MailMessage message, SmtpClient client) {
      
      client.Send(message);
      
    }
    
  }
  
}

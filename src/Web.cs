using System;

/// <summary>
/// Wrapped Quick Web Utilities
/// <summary>
namespace Utility.Web
{
  interface IMail
  {
    void SetCredentials(string username, string password);

    bool Validate();

    bool SendMail();
  }

  public class GMail : IMail
  {
    private System.Net.NetworkCredential i_CurrentCredential;
    private int i_DefaultPort;
    private string i_DefaultHost;
    private bool i_EnableSSL;
    private string i_FromAddress;
    private string i_ToAddress;
    private string i_SubjectStr;
    private string i_ContentStr;

    public GMail() 
    {
      i_DefaultPort = 587;
      i_DefaultHost = "smtp.gmail.com";
      i_EnableSSL = true;
      i_SubjectStr = "[Sample Subject]";
      i_ContentStr = "[Sample Content]";
    }

    public GMail(string username, string password):this()
    {
      SetCredentials(username, password);
    }

    #region IMail Members

    public void SetCredentials(string username, string password)
    {
      i_CurrentCredential = new System.Net.NetworkCredential(username, password);
      if (username.Contains("@")) { i_FromAddress = username; }
    }

    public bool Validate()
    {
      if (i_CurrentCredential == null) { return false; }
      if (string.IsNullOrEmpty(i_FromAddress) || string.IsNullOrEmpty(i_ToAddress)) { return false; }
      return true;
    }

    public bool SendMail()
    {
      if (!Validate()) { return false; }
      System.Net.Mail.SmtpClient mailClient = new System.Net.Mail.SmtpClient();
      mailClient.Credentials = i_CurrentCredential;
      mailClient.Port = i_DefaultPort;
      mailClient.Host = i_DefaultHost;
      mailClient.EnableSsl = i_EnableSSL;

      System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
      message.From = new System.Net.Mail.MailAddress(i_FromAddress, i_FromAddress, System.Text.Encoding.UTF8);
      message.To.Add(i_ToAddress);
      message.Subject = i_SubjectStr;
      message.Body = i_ContentStr;
      message.DeliveryNotificationOptions = System.Net.Mail.DeliveryNotificationOptions.OnFailure;
      try
      {
        mailClient.Send(message);
      }
      catch (System.Net.Mail.SmtpException ex)
      {
        throw ex;
      }
      return true;
    }

    #endregion

    public System.Net.NetworkCredential CurrentCredential
    {
      get { return i_CurrentCredential; }
    }

    public string FromAddress
    {
      get { return i_FromAddress; }
      set { i_FromAddress = value; }
    }

    public string ToAddress
    {
      get { return i_ToAddress; }
      set { i_ToAddress = value; }
    }

    public string Subject
    {
      get { return i_SubjectStr; }
      set { i_SubjectStr = value; }
    }

    public string Content
    {
      get { return i_ContentStr; }
      set { i_ContentStr = value; }
    }
  }

  interface IContent
  {
    void SetCredentials(string username, string password);

    string GetContent(string url);
  }

  public class HttpContent : IContent
  {
    private System.Net.NetworkCredential i_CurrentCredential;

    public HttpContent() { }

    public HttpContent(string username, string password) : this()
    {
      SetCredentials(username, password);
    }

    #region IContent Members

    public void SetCredentials(string username, string password)
    {
      i_CurrentCredential = new System.Net.NetworkCredential(username, password);
    }

    public string GetContent(string url)
    {
      string content = string.Empty;
      System.Net.WebRequest webRequest = System.Net.WebRequest.Create(url);
      System.Net.HttpWebRequest httpRequest = (System.Net.HttpWebRequest)webRequest;
      httpRequest.Method = "GET";
      if (i_CurrentCredential != null) 
      {
          httpRequest.Credentials = i_CurrentCredential; 
          httpRequest.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(new System.Text.ASCIIEncoding().GetBytes(string.Format("{0}:{1}", i_CurrentCredential.UserName, i_CurrentCredential.Password))));
      }
      try
      {
        System.Net.WebResponse webResponse = httpRequest.GetResponse();
        System.IO.Stream receivedStream = webResponse.GetResponseStream();
        using (System.IO.StreamReader reader = new System.IO.StreamReader(receivedStream, System.Text.Encoding.UTF8))
        {
          content = reader.ReadToEnd().Trim();
          reader.Close();
          receivedStream.Close();
        }
      }
      catch(System.Net.WebException ex)
      {
        throw ex;
      }
      return content;
    }

    public string PostContent(string url, string requestContent)
    {
        string content = string.Empty;
        System.Net.WebRequest webRequest = System.Net.WebRequest.Create(url);
        System.Net.HttpWebRequest httpRequest = (System.Net.HttpWebRequest)webRequest;
        httpRequest.Method = "POST";
        httpRequest.ContentType = "application/x-www-form-urlencoded";
        if (i_CurrentCredential != null) 
        { 
            httpRequest.Credentials = i_CurrentCredential;
            httpRequest.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(new System.Text.ASCIIEncoding().GetBytes(string.Format("{0}:{1}", i_CurrentCredential.UserName, i_CurrentCredential.Password))));
        }
        try
        {
            System.Text.Encoding code = System.Text.Encoding.ASCII;
            byte[] bytesToPost = code.GetBytes(requestContent);
            httpRequest.ContentLength = bytesToPost.Length;
            using (System.IO.Stream requestStream = httpRequest.GetRequestStream())
            { 
                 requestStream.Write(bytesToPost, 0, bytesToPost.Length);
                 requestStream.Close();
            }

            System.Net.WebResponse webResponse = httpRequest.GetResponse();
            System.IO.Stream receivedStream = webResponse.GetResponseStream();
            using (System.IO.StreamReader reader = new System.IO.StreamReader(receivedStream, System.Text.Encoding.UTF8))
            {
                content = reader.ReadToEnd().Trim();
                reader.Close();
                receivedStream.Close();
            }
        }
        catch (System.Net.WebException ex)
        {
            throw ex;
        }
        return content;
    }

    public static string GetQuickContent(string url)
    {
      HttpContent content = new HttpContent();
      return content.GetContent(url);
    }

    #endregion

    public System.Net.NetworkCredential CurrentCredential
    {
      get { return i_CurrentCredential; }
    }
  }
}

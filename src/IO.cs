using System;

/// <summary>
/// Wrapped Quick IO Utilities
/// <summary>
namespace Utility.IO
{
  public class File
  {
    public static string ReadContent(string filePath)
    {
      string content = string.Empty;
      if (System.IO.File.Exists(filePath))
      {
        try
        {
          using (System.IO.StreamReader reader = new System.IO.StreamReader(filePath))
          {
            content = reader.ReadToEnd();
            reader.Close();
          }
        }
        catch (System.IO.IOException ex)
        {
          throw ex;
        }
      }
      return content;
    }

    public static bool WriteContent(string filePath, string appendedContent, bool append)
    {
      bool writeSuccess = false;
      if (!System.IO.File.Exists(filePath))
      {
        try
        {
          using (System.IO.FileStream fs = System.IO.File.Create(filePath)) { }
        }
        catch (System.IO.IOException ex)
        {
          throw ex;
        }
      }
      try
      {
        /// <note>
        /// TODO 
        /// Add synclock to prevent multi-thread conflict
        /// <note>
        using (System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath, append))
        {
          writer.WriteLine(appendedContent);
          writer.Close();
          writeSuccess = true;
        }
      }
      catch (System.IO.IOException ex)
      {
        throw ex;
      }
      return writeSuccess;
    }

    public static bool DeleteFile(string filePath)
    {
      bool deleteSuccess = false;
      if (System.IO.File.Exists(filePath))
      {
        try
        {
          System.IO.File.Delete(filePath);
          deleteSuccess = true;
        }
        catch (System.IO.IOException ex)
        {
          throw ex;
        }
      }
      return deleteSuccess;
    }
  }
}

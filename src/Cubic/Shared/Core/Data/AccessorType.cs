namespace Cubic.Core.Data
{
  public enum AccessorType
  {
    INDIVIDUAL = 0,



    #region Database

    Database = 20,

    #endregion



    #region Files

    #region MS Office

    MSEXCEL = 60,

    MSWORD = 70,

    #endregion

    CSV = 80,

    BINARY = 81,

    XML = 82,

    JSON = 83,

    #endregion



    #region FTP

    FTP = 100,

    SFTP = 101,

    #endregion


    #region Email

    IMAP = 200,

    POP3 = 210,

    SMTP = 250,

    #endregion


    #region Http

    SIMPLEHTTPTRANSPORT = 300,

    #endregion

  }
}
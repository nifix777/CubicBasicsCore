namespace Cubic.Core.Data
{
  public enum AccessorType
  {
    INDIVIDUAL = 0,



    #region Database

    Database = 20,

    #endregion



    #region Files

    File = 80,

    #endregion



    #region FTP

    FTP = 100,

    #endregion


    #region Email

    SMTP = 200,

    Postbox = 250,

    #endregion


    #region Http

    HTTP = 300

    #endregion

  }
}
namespace Cubic.Core
{

  public delegate void GenericEventHandler<in TSender, in TArgs>(TSender sender, TArgs args);
}
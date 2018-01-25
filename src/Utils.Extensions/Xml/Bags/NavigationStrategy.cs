namespace Utils.Extensions.Xml.Bags
{

  /// <summary>
  /// Strategie di navigazione dell'xml
  /// </summary>
  public enum NavigationStrategy
  {
    /// <summary>
    /// Strategia che utilizza XPathNavigator
    /// </summary>
    XPathNavigator, 

    /// <summary>
    /// Strategia che utilizza XElement 
    /// </summary>
    XElementNavigator
  }
}
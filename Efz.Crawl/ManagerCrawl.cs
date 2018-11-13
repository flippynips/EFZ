//*******************************************************//
//                      Web Crawler                      //
//                                                       //
//*******************************************************//
//       Joshua Graham 2015   -    Happy Coding :)       //
//                                                       //
//*******************************************************//


namespace Efz {
  
  /// <summary>
  /// Application initialization.
  /// </summary>
  public class ManagerCrawl : Singleton<ManagerCrawl> {
		
    //-------------------------------------------//
		
		/// <summary>
    /// The crawler should be initialized late.
    /// </summary>
    protected override byte SingletonPriority { get { return 10; } }
		
    //-------------------------------------------//
		
    //-------------------------------------------//
		
    
  }
}
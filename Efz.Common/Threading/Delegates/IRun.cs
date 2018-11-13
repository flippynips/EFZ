using System;
using System.Threading.Tasks;

namespace Efz {
  
  /// <summary>
  /// Common interface for runnables. Not super helpful - should improve.
  /// </summary>
  public interface IRuns {}
  
  public interface IRun : IRuns {
    
    //-------------------------------------------//
    
    void Run();
    
    //-------------------------------------------//
    
  }
  
  public interface IRun<T> : IRuns {
    
    //-------------------------------------------//
    
    T Run();
    Task<T> RunAsync();
    
    //-------------------------------------------//
    
  }
  
}

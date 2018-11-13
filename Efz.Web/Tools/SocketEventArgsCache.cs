/*
 * User: FloppyNipples
 * Date: 05/05/2017
 * Time: 23:54
 */
using System;
using System.Net.Sockets;

using Efz.Collections;

namespace Efz.Web {
  
  /// <summary>
  /// A repository for asynchronous event args.
  /// </summary>
  internal static class SocketEventArgsCache {
    
    /// <summary>
    /// Event args to be used for send operations
    /// </summary>
    private static readonly SafeQueue<SocketAsyncEventArgs> _eventArgsSend = new SafeQueue<SocketAsyncEventArgs>();
    /// <summary>
    /// Event args to be used for receive operations
    /// </summary>
    private static readonly SafeQueue<SocketAsyncEventArgs> _eventArgsReceive = new SafeQueue<SocketAsyncEventArgs>();
    
    /// <summary>
    /// Allocate an event arg for a send operation
    /// </summary>        
    public static SocketAsyncEventArgs AllocateForSend(EventHandler<SocketAsyncEventArgs> ioCompletedHandler) {
      SocketAsyncEventArgs result;
      
      if (!_eventArgsSend.Dequeue(out result)) result = new SocketAsyncEventArgs();
      
      result.Completed += ioCompletedHandler;
      return result;
    }

    /// <summary>
    /// Allocate an event arg for a receive operation
    /// </summary>        
    public static SocketAsyncEventArgs AllocateForReceive(EventHandler<SocketAsyncEventArgs> ioCompletedHandler) {
      SocketAsyncEventArgs result;
      
      if (!_eventArgsReceive.Dequeue(out result)) {
        result = new SocketAsyncEventArgs();
        result.SetBuffer(BufferCache.Get(), 0, Global.BufferSizeLocal);
      }
      
      result.Completed += ioCompletedHandler;
      return result;
    }
    
    /// <summary>
    /// De-Allocate the given event arg to it can be used for another send operation
    /// </summary>   
    public static void DeallocateForSend(SocketAsyncEventArgs eventArgs, EventHandler<SocketAsyncEventArgs> ioCompletedHandler) {
      eventArgs.Completed -= ioCompletedHandler;
      _eventArgsSend.Enqueue(eventArgs);
    }
    
    /// <summary>
    /// De-Allocate the given event arg to it can be used for another receive operation
    /// </summary>   
    public static void DeallocateForReceive(SocketAsyncEventArgs eventArgs, EventHandler<SocketAsyncEventArgs> ioCompletedHandler) {
      eventArgs.Completed -= ioCompletedHandler;
      _eventArgsReceive.Enqueue(eventArgs);
    }
  }
}

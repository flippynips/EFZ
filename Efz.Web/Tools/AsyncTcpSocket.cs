/*
 * User: FloppyNipples
 * Date: 06/05/2017
 * Time: 17:57
 */
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using Efz.Collections;
using Efz.Threading;
using Efz.Tools;

namespace Efz.Web {
  
  /// <summary>
  /// Implements async and helper functionality for sockets.
  /// </summary>
  public class AsyncTcpSocket : IDisposable {
    
    //----------------------------------//
    
    /// <summary>
    /// Action run on an error occuring with the socket.
    /// </summary>
    public Action<Exception> OnError;
    /// <summary>
    /// Action run on an error occuring with the socket.
    /// </summary>
    public Action<byte[], int> OnReceive;
    
    /// <summary>
    /// Provides access to the underlying socket.
    /// </summary>
    internal Socket Socket;
    /// <summary>
    /// Gets a value to determine if the socket is connected to the remote host as at the
    /// last send or receive operation.
    /// </summary>
    internal bool Connected { get { return Socket.Connected; } }
    /// <summary>
    /// Gets the remote endpoint. This works even after the socket has been closed/disposed.
    /// </summary>
    internal readonly EndPoint RemoteEndPoint;
    /// <summary>
    /// Gets the local endpoint.  This works even after the socket has been closed/disposed.
    /// </summary>
    internal readonly EndPoint LocalEndPoint;
    
    //----------------------------------//
    
    /// <summary>
    /// Raised when a message is received from the socket.
    /// </summary>
    private ActionSequence _onReceive;
    
    /// <summary>
    /// Set to 1 if Dispose has been called
    /// </summary>
    private bool _disposing;
    /// <summary>
    /// Value of 1 if Start() has been called.
    /// </summary>
    private int _started;
    
    /// <summary>
    /// Queue for buffering data to be sent.
    /// </summary>
    private readonly Shared<BufferQueue> _sendBuffer;
    /// <summary>
    /// Async event args instance for sending.
    /// </summary>
    private SocketAsyncEventArgs _sendSocketArgs;
    /// <summary>
    /// Used for synchronisation so we can't dispose when actions are in progress.
    /// </summary>
    private readonly LockReadWrite _syncLock;
    /// <summary>
    /// Lock used to retrict sending to a single thread.
    /// </summary>
    private readonly Lock _sendLock;
    /// <summary>
    /// Buffer for received data
    /// </summary>
    private readonly Shared<BufferQueue> _receivedBuffer;
    
    /// <summary>
    /// Lock for read data being processed.
    /// </summary>
    private Lock _readLock;
    
    /// <summary>
    /// Callback collection on sent bytes.
    /// </summary>
    private Queue<Teple<int, IAction>> _callbacks;
    /// <summary>
    /// Last callback byte index.
    /// </summary>
    private int _lastCallbackIndex;
    /// <summary>
    /// Current callback byte index.
    /// </summary>
    private int _currentCallbackIndex;
    /// <summary>
    /// Current callback action.
    /// </summary>
    private IAction _currentCallbackAction;
    /// <summary>
    /// Callback lock used to safely apply callback changes.
    /// </summary>
    private Lock _callbackLock;
    
    //----------------------------------//
    
    /// <summary>
    /// Creates a new AsyncSocket.  You must call Start() after creating the AsyncSocket
    /// in order to begin receive data.
    /// </summary>
    internal AsyncTcpSocket(Socket socket, Action<byte[], int> onReceive, Action<Exception> onError) {
      
      Socket = socket;
      
      OnReceive = onReceive;
      OnError = onError;
      
      RemoteEndPoint = Socket.RemoteEndPoint;
      LocalEndPoint = Socket.LocalEndPoint;
      
      _sendLock = new Lock();
      _readLock = new Lock();
      _callbackLock = new Lock();
      
      _sendBuffer = new Shared<BufferQueue>(new BufferQueue());
      _syncLock = new LockReadWrite();
      _receivedBuffer = new Shared<BufferQueue>(new BufferQueue());
      _onReceive = new ActionSequence();
      _callbacks = new Queue<Teple<int, IAction>>();
      
      _sendSocketArgs = SocketEventArgsCache.AllocateForSend(OnSocketSend);
      _sendSocketArgs.SendPacketsFlags = TransmitFileOptions.UseKernelApc | TransmitFileOptions.UseSystemThread;
      _sendSocketArgs.RemoteEndPoint = RemoteEndPoint;
      
      _disposing = false;
      
    }
    
    /// <summary>
    /// Creates a new AsyncSocket.  You must call Start() after creating the AsyncSocket
    /// in order to begin receive data.
    /// </summary>
    internal AsyncTcpSocket(Socket socket, IPEndPoint remoteEndpoint, IPEndPoint localEndpoint,
       Action<byte[], int> onReceive, Action<Exception> onError) {
      
      Socket = socket;
      
      OnReceive = onReceive;
      OnError = onError;
      
      RemoteEndPoint = remoteEndpoint;
      LocalEndPoint = localEndpoint;
      
      _sendLock = new Lock();
      _readLock = new Lock();
      _callbackLock = new Lock();
      
      _sendBuffer = new Shared<BufferQueue>(new BufferQueue());
      _syncLock = new LockReadWrite();
      _receivedBuffer = new Shared<BufferQueue>(new BufferQueue());
      _onReceive = new ActionSequence();
      _callbacks = new Queue<Teple<int, IAction>>();
      
      _sendSocketArgs = SocketEventArgsCache.AllocateForSend(OnSocketSend);
      _sendSocketArgs.SendPacketsFlags = TransmitFileOptions.UseKernelApc | TransmitFileOptions.UseSystemThread;
      _sendSocketArgs.RemoteEndPoint = RemoteEndPoint;
      
      _disposing = false;
      
    }
    
    /// <summary>
    /// Start receiving data from the socket.
    /// </summary>
    public void Start() {
      
      // don't start if disposing
      if (_disposing) return;
      
      // ensure we don't start multiple times
      if (Interlocked.CompareExchange(ref _started, 1, 0) == 1) {
        // already started! can't call Start() twice.
        throw new InvalidOperationException("Cannot call AsyncSocket.Start() more than once.");
      }
      
      // configure for receive
      var args = SocketEventArgsCache.AllocateForReceive(OnSocketReceive);
      
      // start a receive request and immediately check to see if the receive is already complete
      // otherwise OnIOCompleted will get called when the receive is complete
      if (!Socket.ReceiveAsync(args)) ReceiveSocketData(args);
      
    }
    
    /// <summary>
    /// Dispose this AsyncSocket. Closes the underlying socket if required.
    /// </summary>
    public void Dispose() {
      
      // ensure we haven't already disposed
      _syncLock.TakeWrite();
      if (_disposing) {
        _syncLock.ReleaseWrite();
        return;
      }
      _disposing = true;
      _syncLock.ReleaseWrite();
      
      // run sender dispose code
      if (_sendSocketArgs != null) {
        SocketEventArgsCache.DeallocateForSend(_sendSocketArgs, OnSocketSend);
        _sendSocketArgs = null;
      }
      
    }
    
    /// <summary>
    /// Send bytes from the specified stream to the remote host.
    /// </summary>
    public void Send(Stream stream, int length, IAction onSent = null) {
      
      // skip 0 length
      if(length == 0) return;
      
      if(onSent != null) {
        
        _callbackLock.Take();
        
        // enqueue bytes from the stream
        var count = _sendBuffer.TakeItem().EnqueueGetCount(stream, length);
        _sendBuffer.Release();
        if(_currentCallbackAction == null) {
          _currentCallbackAction = onSent;
          _currentCallbackIndex = _lastCallbackIndex = count;
        } else {
          _lastCallbackIndex = count - _lastCallbackIndex;
          _callbacks.Enqueue(Teple.New(_lastCallbackIndex, onSent));
        }
        
        _callbackLock.Release();
        
      } else {
        
        // enqueue bytes from the stream
        _sendBuffer.TakeItem().Enqueue(stream, length);
        _sendBuffer.Release();
      }
      
      // is the socket currently sending?
      if (_sendLock.TryTake) {
        
        // ensure not disposing
        if (_disposing) {
          _sendLock.Release();
          return;
        }
        
        try {
          
          // start sending the bytes in the buffer
          StartSend();
          
        } catch (SocketException ex) {
          
          _sendLock.Release();
          if(ex.SocketErrorCode == SocketError.ConnectionReset) return;
          ProcessError(ex);
          
        } catch (Exception ex) {
          
          _sendLock.Release();
          // handle the error outside the lock
          ProcessError(ex);
          
        }
        
      }
      
    }
    
    /// <summary>
    /// Send data to the remote host
    /// </summary>
    public void Send(byte[] buffer, int index, int count, IAction onSent = null) {
      
      // skip 0 bytes
      if(count == 0) return;
      
      if(onSent != null) {
        _callbackLock.Take();
        
        // enqueue bytes in the buffer
        var length = _sendBuffer.TakeItem().EnqueueGetCount(buffer, index, count);
        _sendBuffer.Release();
        if(_currentCallbackAction == null) {
          _currentCallbackAction = onSent;
          _currentCallbackIndex = _lastCallbackIndex = length;
        } else {
          _lastCallbackIndex = length - _lastCallbackIndex;
          _callbacks.Enqueue(Teple.New(_lastCallbackIndex, onSent));
        }
        
        _callbackLock.Release();
      } else {
        
        // enqueue bytes from the stream
        _sendBuffer.TakeItem().Enqueue(buffer, index, count);
        _sendBuffer.Release();
        
      }
      
      // is the socket currently sending?
      if (_sendLock.TryTake) {
        
        // ensure not disposing
        if (_disposing) {
          _sendLock.Release();
          return;
        }
        
        try {
          
          StartSend();
          
        } catch (SocketException ex) {
          
          _sendLock.Release();
          if(ex.SocketErrorCode == SocketError.ConnectionReset) return;
          ProcessError(ex);
          
        } catch (Exception ex) {
          
          _sendLock.Release();
          // handle the error outside the lock
          ProcessError(ex);
          
        }
      }
    }
    
    /// <summary>
    /// Send data to the remote host
    /// </summary>
    public void Send(byte[] data, int count) {
      Send(data, 0, count);
    }
    
    /// <summary>
    /// Send data to the remote host
    /// </summary>
    public void Send(byte[] data) {
      Send(data, 0, data.Length);
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// Start sending data to the socket. Make sure slimLock is entered in read mode before calling this,
    /// and that disposing is not true. Ensure an error handler is implemented which forwards to ProcessError.
    /// </summary>
    private void StartSend() {
      
      if(_disposing) {
        _sendLock.Release();
        return;
      }
      
      byte[] buffer = BufferCache.Get();
      
      // iterate while there are more bytes to be sent
      for (;;) {
        
        // get data without removing it from the buffer and set buffer on socket
        int count;
        
        _sendBuffer.Take();
        
        if(_currentCallbackAction == null) {
          
          count = _sendBuffer.Item.Dequeue(buffer, 0, Global.BufferSizeLocal);
          
          if(count == 0) {
            
            _sendLock.Release();
            
            // more data to send? yes, take the lock and return positive
            if (_sendBuffer.Item.Length > 0 && _sendLock.TryTake) {
              count = _sendBuffer.Item.Dequeue(buffer, 0, Global.BufferSizeLocal);
            } else {
              _sendBuffer.Release();
              BufferCache.Set(buffer);
              return;
            }
          }
        } else {
          
          _callbackLock.Take();
          
          count = _sendBuffer.Item.Dequeue(buffer, 0, Global.BufferSizeLocal);
          
          if(count == 0) {
            
            _sendLock.Release();
            
            // more data to send? yes, take the lock and return positive
            if (_sendBuffer.Item.Length > 0 && _sendLock.TryTake) {
              count = _sendBuffer.Item.Dequeue(buffer, 0, Global.BufferSizeLocal);
            } else {
              _sendBuffer.Release();
              _callbackLock.Release();
              BufferCache.Set(buffer);
              return;
            }
          }
          
          _currentCallbackIndex -= count;
          
          _callbackLock.Release();
          
        }
        
        _sendBuffer.Release();
        
        _sendSocketArgs.SetBuffer(buffer, 0, count);
        
        if(_disposing) {
          _sendLock.Release();
          return;
        }
        
        if (Socket.Connected) {
          
          // sendAsync returns true if the I/O operation is pending. An event will be raised upon completion.
          // returns false if the I/O operation completed synchronously
          if (Socket.SendAsync(_sendSocketArgs)) {
            // an event is going to be raised to OnIOComplete_Send, so let's exit right now because we're done
            return;
          }
          
          // send -- is there more data to send now? no, break iteration
          if(!CompleteSend(_sendSocketArgs)) return;
          
        } else {
          
          _sendLock.Release();
          BufferCache.Set(buffer);
          if(_disposing) return;
          ProcessError("Socket was disconnected mid-send.");
          return;
          
        }
        
      }
      
      // in case loop is ever broken
      //_syncLock.ExitReadLock();
      
    }
    
    /// <summary>
    /// Remove data that has been sent from the queue. Returns true if we should send more.
    /// Make sure slimLock is entered in read mode before calling this, and that disposing is not 1.
    /// Ensure an error handler is implemented which forwards to ProcessError.
    /// </summary>
    private bool CompleteSend(SocketAsyncEventArgs args) {
      
      // did it fail?
      if (args.SocketError != SocketError.Success) {
        
        _sendLock.Release();
        BufferCache.Set(args.Buffer);
        
        // it failed, let's invoke the error handler
        if(!_disposing) {
          ProcessError("Socket error '"+args.SocketError+"'.");
        }
        
        return false;
        
      }
      
      // is the current callback set?
      if(_currentCallbackAction != null) {
        
        // yes, take the lock
        _callbackLock.Take();
        
        if(_currentCallbackAction != null) {
          
          while(_currentCallbackIndex <= 0) {
            
            _currentCallbackAction.Run();
            
            Teple<int, IAction> nextCallback;
            if(_callbacks.Dequeue(out nextCallback)) {
              
              _currentCallbackIndex = nextCallback.ArgA + _currentCallbackIndex;
              _currentCallbackAction = nextCallback.ArgB;
              
            } else {
              
              _currentCallbackAction = null;
              
            }
            
          }
          
        }
        
        // release the lock
        _callbackLock.Release();
        
      }
      
      int length = _sendBuffer.TakeItem().Length;
      _sendBuffer.Release();
      
      // more data to send? yes, return positive
      if (length > 0) return true;
      
      _sendLock.Release();
      
      length = _sendBuffer.TakeItem().Length;
      _sendBuffer.Release();
      
      // more data to send? yes, take the lock and return positive
      if (length > 0 && _sendLock.TryTake) return true;
      
      BufferCache.Set(args.Buffer);
      return false;
    }

    /// <summary>
    /// This method is called when there is no more data to send to a connected client
    /// </summary>
    private void OnSocketSend(object sender, SocketAsyncEventArgs e) {
      // skip if disposing -- not really needed, but nice for consistency
      if (_disposing) return;
      
      // determine which type of operation just completed and call the associated handler
      if(e.LastOperation == SocketAsyncOperation.Send) {
        
        // ensure not disposing
        _syncLock.TakeRead();
        if (_disposing) {
          _syncLock.ReleaseRead();
          return;
        }
        _syncLock.ReleaseRead();
        
        try {
          
          // this send has completed are there more bytes to send?
          if (CompleteSend(e)) {
            // yes, send again
            StartSend();
          }
          
        } catch(SocketException ex) {
          
          _sendLock.Release();
          if(ex.SocketErrorCode == SocketError.ConnectionReset) {
            _syncLock.ReleaseRead();
            return;
          }
          _syncLock.ReleaseRead();
          ProcessError(ex);
          
        } catch (Exception ex) {
          
          _sendLock.Release();
          _syncLock.ReleaseRead();
          // handle error outside lock
          ProcessError(ex);
          
        }
      } else {
        if(_disposing) return;
        ProcessError("Unknown operation completed");
      }
    }
    
    /// <summary>
    /// Read data from socket into buffer and process.  Check if disposed after calling this, because
    /// this method will temporarily release the lock.
    /// </summary>
    private void ReceiveSocketData(SocketAsyncEventArgs args) {
      try {
        
        _syncLock.TakeRead();
        
        // exit if we're disposing
        if (_disposing) {
          _syncLock.ReleaseRead();
          return;
        }
        
        if(args.SocketError == SocketError.Success) {
          
          // how many bytes did we receive?
          int count = args.BytesTransferred;
          
          if(count > 0) {
            
            // enqueue these bytes to read
            _receivedBuffer.TakeItem().Enqueue(args.Buffer, 0, count);
            _receivedBuffer.Release();
            
            if(_readLock.TryTake) ProcessReceivedData();
          }
          
          // deallocate the arguments
          _syncLock.ReleaseRead();
          
        } else {
          // deallocate the arguments
          _syncLock.ReleaseRead();
        
          if(_disposing) return;
          ProcessError("Socket Receive error '"+args.SocketError+"'.");
        }
        
        SocketEventArgsCache.DeallocateForReceive(args, OnSocketReceive);
        
      } catch(SocketException ex) {
        
        if(ex.SocketErrorCode == SocketError.ConnectionReset) {
          
          SocketEventArgsCache.DeallocateForReceive(args, OnSocketReceive);
          Log.Warning("Socket connection to '"+RemoteEndPoint+"' was reset.");
          
        }
        
        _syncLock.ReleaseRead();
        ProcessError(ex);
        
        
      } catch (Exception ex) {
        
        // we want to catch exceptions outside of the lock
        _syncLock.ReleaseRead();
        ProcessError(ex);
        
      }
    }
    
    /// <summary>
    /// Buffer processed data and send to the receive callback.
    /// </summary>
    private void ProcessReceivedData() {
      
      // get a buffer to dequeue into
      var buffer = BufferCache.Get();
      
      try {
        
        // dequeue a buffer from the pending received data
        int count = _receivedBuffer.TakeItem().Dequeue(buffer, 0, Global.BufferSizeLocal);
        _receivedBuffer.Release();
        if(count == 0) {
          
          // allocate the buffer cache once more
          BufferCache.Set(buffer);
          
        } else {
          
          // remove
          //Log.Debug(new string(System.Text.Encoding.UTF8.GetChars(buffer, 0, count)));
          
          // run the callback action
          _onReceive.AddRun(ActionSet.New(OnReceive, buffer, count));
          
          if(count == Global.BufferSizeLocal) {
            ManagerUpdate.Control.AddSingle(ProcessReceivedData);
            return;
          }
        }
        
        // no bytes to receive
        CompleteReceive();
        
      } catch (SocketException ex) {
        
        _readLock.Release();
        BufferCache.Set(buffer);
        if(ex.SocketErrorCode == SocketError.ConnectionReset) return;
        ProcessError(ex);
        
      } catch (Exception ex) {
        
        _readLock.Release();
        BufferCache.Set(buffer);
        ProcessError(ex);
        
      }
    }
    
    /// <summary>
    /// Complete processing a buffer of bytes read from the socket.
    /// </summary>
    private void CompleteReceive() {
      
      // ReadSocketData temporarily releases the lock (and then enters it again), so we need to ensure
      // we haven't disposed in the meantime
      if (_disposing) {
        _readLock.Release();
        return;
      }
      
      if(_receivedBuffer.TakeItem().Length > 0) {
        _receivedBuffer.Release();
        ManagerUpdate.Control.AddSingle(ProcessReceivedData);
        return;
      }
      _receivedBuffer.Release();
      
      // flop the processing flag
      _readLock.Release();
      
      // any bytes in the queue?
      if(_receivedBuffer.TakeItem().Length > 0 && _readLock.TryTake) {
        _receivedBuffer.Release();
        ManagerUpdate.Control.AddSingle(ProcessReceivedData);
        return;
      }
      _receivedBuffer.Release();
      
      // socket may have been closed already due to connection error.
      // if not try receiving more data
      if (Socket.Connected) {
        
        // receiveAsync returns true if the I/O operation is pending. An event will be raised upon completion.
        // returns false if the I/O operation completed synchronously.
        var args = SocketEventArgsCache.AllocateForReceive(OnSocketReceive);
        if (!Socket.ReceiveAsync(args)) ReceiveSocketData(args);
        
      } else {
        if(_disposing) return;
        ProcessError("Cannot receive, socket is not connected.");
      }
    }
    
    /// <summary>
    /// This method is called when there is no more data to read from a connected client
    /// </summary>
    private void OnSocketReceive(object sender, SocketAsyncEventArgs args) {
      // process the received data if the expected operation has completed
      if(args.LastOperation == SocketAsyncOperation.Receive && !_disposing) ReceiveSocketData(args);
    }
    
    /// <summary>
    /// Called internally when an error occurs.
    /// </summary>
    private void ProcessError(Exception ex) {
      if(OnError == null) Log.Error("Socket exception.", ex);
      else OnError(ex);
    }
    
    /// <summary>
    /// Called internally when an error occurs.
    /// </summary>
    private void ProcessError(string msg) {
      ProcessError(new Exception(msg));
    }
    
  }
  
}

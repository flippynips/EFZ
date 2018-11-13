/*
 * User: FloppyNipples
 * Date: 06/05/2017
 * Time: 17:57
 */
using System;
using System.Collections.Generic;
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
  public class AsyncUdpSocket : IDisposable {
    
    //----------------------------------//
    
    /// <summary>
    /// Datagram that has been broken into chunks that is being received.
    /// </summary>
    private class ChunkedGram {
      /// <summary>
      /// Chunks that have been received.
      /// </summary>
      public ArrayRig<Teple<int, byte[]>> Chunks;
      /// <summary>
      /// Total length of the gram.
      /// </summary>
      public int Length;
      
      /// <summary>
      /// Timestamp the last chunk was received. Used for timeouts.
      /// </summary>
      public long Timestamp;
    }
    
    /// <summary>
    /// Buffer queued to be sent by this udp socket.
    /// </summary>
    private class QueuedBuffer {
      /// <summary>
      /// Data queued.
      /// </summary>
      public BufferQueue Buffer;
      /// <summary>
      /// End point to be sent to.
      /// </summary>
      public IPEndPoint EndPoint;
    }
    
    /// <summary>
    /// Maximum number of bytes in a udp datagram.
    /// </summary>
    public const int MaxDatagramSize = 8192 - 20;
    /// <summary>
    /// Number of bytes in each chunk header.
    /// </summary>
    public const int ChunkHeaderSize = 13;
    /// <summary>
    /// Ticks between receiving data chunks for the chunked data to be considered timed out
    /// and disposed of.
    /// </summary>
    public static readonly long ChunkedGramTimeout = Time.Frequency * Time.Minute * 5;
    
    /// <summary>
    /// Action run on an error occuring with the socket.
    /// </summary>
    public Action<Exception> OnError;
    /// <summary>
    /// Action run on an error occuring with the socket.
    /// </summary>
    public Action<IPEndPoint, byte[], int> OnReceive;
    
    /// <summary>
    /// Provides access to the underlying socket.
    /// </summary>
    public Socket Socket;
    
    /// <summary>
    /// Gets a value to determine if the socket is connected to the remote host as at the
    /// last send or receive operation.
    /// </summary>
    internal bool Connected { get { return Socket.Connected; } }
    /// <summary>
    /// Current target endpoint for sent data. Get the last remote endpoint sent to or
    /// set the next remote endpoint to send to.
    /// </summary>
    internal IPEndPoint TargetEndPoint;
    /// <summary>
    /// EndPoint to receive data from. Cannot be null.
    /// </summary>
    internal IPEndPoint RemoteEndPoint;
    /// <summary>
    /// Gets the local endpoint.
    /// </summary>
    internal readonly IPEndPoint LocalEndPoint;
        
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
    /// Current buffer being filled before sent.
    /// </summary>
    private BufferQueue _enqueueBuffer;
    /// <summary>
    /// Current queue of buffers to be sent.
    /// </summary>
    private readonly Shared<ArrayRig<QueuedBuffer>> _sendQueue;
    /// <summary>
    /// Current buffer being sent.
    /// </summary>
    private BufferQueue _sendBuffer;
    
    /// <summary>
    /// Used for synchronisation so we can't dispose when actions are in progress.
    /// </summary>
    private readonly LockReadWrite _syncLock;
    /// <summary>
    /// Lock used to retrict sending to a single thread.
    /// </summary>
    private readonly Lock _sendLock;
    
    /// <summary>
    /// Lock for read data being processed.
    /// </summary>
    private Lock _readLock;
    
    /// <summary>
    /// Current callback action.
    /// </summary>
    private IAction _onSent;
    
    /// <summary>
    /// Buffer used to receive bytes asynchronously.
    /// </summary>
    private byte[] _receiveBuffer;
    /// <summary>
    /// Currently receiving from this endpoint.
    /// </summary>
    private EndPoint _receiveEndPoint;
    
    /// <summary>
    /// Chunks of data that are being received on this socket.
    /// </summary>
    private Dictionary<int, ChunkedGram> _receivedChunks;
    /// <summary>
    /// Current chunk index of the data being sent.
    /// </summary>
    private int _chunkIndex;
    /// <summary>
    /// Current chunk count of the data being sent.
    /// </summary>
    private int _chunkCount;
    /// <summary>
    /// Current chunked id of the data being sent.
    /// </summary>
    private int _chunkedId;
    /// <summary>
    /// Flag indicating whether the current data being sent is chunked.
    /// </summary>
    private bool _chunked;
    
    //----------------------------------//
    
    /// <summary>
    /// Creates a new AsyncSocket.  You must call Start() after creating the AsyncSocket
    /// in order to begin receive data.
    /// </summary>
    internal AsyncUdpSocket(Socket socket, Action<IPEndPoint, byte[], int> onReceive, Action<Exception> onError) {
      
      Socket = socket;
      
      OnReceive = onReceive;
      OnError = onError;
      
      LocalEndPoint = (IPEndPoint)Socket.LocalEndPoint;
      RemoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
      TargetEndPoint = RemoteEndPoint;
      
      _sendLock = new Lock();
      _readLock = new Lock();
      
      _enqueueBuffer = new BufferQueue();
      _sendQueue = new Shared<ArrayRig<QueuedBuffer>>(new ArrayRig<QueuedBuffer>());
      
      _syncLock = new LockReadWrite();
      _onReceive = new ActionSequence();
      
      _receiveBuffer = BufferCache.Get();
      _receivedChunks = new Dictionary<int, ChunkedGram>();
      
      _disposing = false;
      
    }
    
    /// <summary>
    /// Creates a new AsyncSocket.  You must call Start() after creating the AsyncSocket
    /// in order to begin receive data.
    /// </summary>
    internal AsyncUdpSocket(Socket socket, IPEndPoint localEndpoint,
       Action<IPEndPoint, byte[], int> onReceive, Action<Exception> onError) {
      
      Socket = socket;
      
      OnReceive = onReceive;
      OnError = onError;
      
      LocalEndPoint = localEndpoint;
      RemoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
      TargetEndPoint = RemoteEndPoint;
      
      _sendLock = new Lock();
      _readLock = new Lock();
      
      _enqueueBuffer = new BufferQueue();
      _sendQueue = new Shared<ArrayRig<QueuedBuffer>>(new ArrayRig<QueuedBuffer>());
      
      _syncLock = new LockReadWrite();
      _onReceive = new ActionSequence();
      
      _receiveBuffer = BufferCache.Get();
      _receivedChunks = new Dictionary<int, ChunkedGram>();
      
      _disposing = false;
      
    }
    
    /// <summary>
    /// Dispose this AsyncSocket. Does not close the underlying socket.
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
      
    }
    
    /// <summary>
    /// Start receiving data from the socket.
    /// </summary>
    public void StartReceiving() {
      
      // don't start if disposing
      if (_disposing) return;
      
      // ensure we don't start multiple times
      if (Interlocked.CompareExchange(ref _started, 1, 0) == 1) {
        // already started! can't call Start() twice.
        throw new InvalidOperationException("Cannot start the async socket more than once.");
      }
      
      try {
        
        // bind the socket to the local endpoint
        Socket.Bind(LocalEndPoint);
        
      } catch(Exception ex) {
        
        ProcessError(ex);
        return;
        
      }
      
      // create the endpoint for receiving data
      _receiveEndPoint = new IPEndPoint(RemoteEndPoint.Address, RemoteEndPoint.Port);
      
      try {
         
        //Log.D("'" + LocalEndPoint+ "' receiving from '"+RemoteEndPoint+"'.");
        
        // start receiving from the socket
        Socket.BeginReceiveFrom(_receiveBuffer, 0, Global.BufferSizeLocal, SocketFlags.None, ref _receiveEndPoint, OnReceiveSocketData, null);
        
      } catch(Exception ex) {
        
        ProcessError(ex);
        
      }
      
    }
    
    /// <summary>
    /// Send to the specified endpoint.
    /// </summary>
    public void Send(IPEndPoint endPoint, IAction onSent = null) {
      
      if(!_sendLock.TryTake) {
        
        _sendQueue.Take();
        
        _sendQueue.Item.Add(new QueuedBuffer {
          EndPoint = endPoint,
          Buffer = _enqueueBuffer
        });
        _enqueueBuffer = null;
        
        _sendQueue.Release();
        
        return;
      }
      
      if(_disposing) {
        _sendLock.Release();
        return;
      }
      
      _onSent = onSent;
      
      SetSendBuffer(_enqueueBuffer, endPoint);
      
      _enqueueBuffer = null;
      
      try {
        
        // send
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
    
    /// <summary>
    /// Start sending the enqueued bytes to the remote endpoint.
    /// </summary>
    public void Send(IAction onSent = null) {
      
      if(!_sendLock.TryTake) {
        
        _sendQueue.Take();
        
        _sendQueue.Item.Add(new QueuedBuffer {
          EndPoint = TargetEndPoint,
          Buffer = _enqueueBuffer
        });
        _enqueueBuffer = null;
        
        _sendQueue.Release();
        
        return;
      }
      
      // ensure not disposing
      if (_disposing) {
        _sendLock.Release();
        return;
      }
      
      _onSent = onSent;
      
      SetSendBuffer(_enqueueBuffer, TargetEndPoint);
      _enqueueBuffer = null;
      
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
    
    /// <summary>
    /// Enqueue bytes from the specified stream to be sent.
    /// </summary>
    public void Enqueue(Stream stream, int length) {
      
      // skip 0 length
      if(length == 0) return;
      
      // does the enqueue buffer need to be created? yes, create it
      if(_enqueueBuffer == null) {
        _sendQueue.Take();
        if(_enqueueBuffer == null) _enqueueBuffer = new BufferQueue();
        _sendQueue.Release();
      }
      
      // enqueue bytes from the stream
      _enqueueBuffer.Enqueue(stream, length);
      
    }
    
    /// <summary>
    /// Enqueue data to be sent to the remote host
    /// </summary>
    public void Enqueue(byte[] buffer, int index, int count) {
      
      // skip 0 bytes
      if(count == 0) return;
      
      // does the enqueue buffer need to be created? yes, create it
      if(_enqueueBuffer == null) {
        _sendQueue.Take();
        if(_enqueueBuffer == null) _enqueueBuffer = new BufferQueue();
        _sendQueue.Release();
      }
      
      // enqueue bytes from the stream
      _enqueueBuffer.Enqueue(buffer, index, count);
      
    }
    
    /// <summary>
    /// Enqueue data to be sent to the remote host
    /// </summary>
    public void Enqueue(byte[] data, int count) {
      Enqueue(data, 0, count);
    }
    
    /// <summary>
    /// Enqueue data to be sent to the remote host
    /// </summary>
    public void Enqueue(byte[] data) {
      Enqueue(data, 0, data.Length);
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
      
      // get a buffer used to send data
      byte[] buffer = BufferCache.Get(MaxDatagramSize);
      
      // iterate while there are more bytes to be sent
      for (;;) {
        
        // get data without removing it from the buffer and set buffer on socket
        int count = TryGetBuffer(buffer);
        
        // any bytes in the buffer?
        if(count == 0) {
          
          _sendLock.Release();
          
          // more data to send? yes, take the lock and return positive
          if (_enqueueBuffer.Length == 0 || !_sendLock.TryTake ||
              (count = TryGetBuffer(buffer)) == 0) {
            BufferCache.Set(buffer);
            return;
          }
          
        }
        
        //Log.Debug("Sending buffer '"+count+"' : " + buffer.ToString(0, count, ','));
        
        var args = new SocketAsyncEventArgs();
        args.Completed += OnSocketSend;
        args.SendPacketsFlags = TransmitFileOptions.UseSystemThread;
        
        // set the remote endpoint of the data to be sent
        args.RemoteEndPoint = TargetEndPoint;
        // set the data
        args.SetBuffer(buffer, 0, count);
        
        if(_disposing) {
          _sendLock.Release();
          args.Dispose();
          return;
        }
        
        if (Socket.SendToAsync(args)) {
          // an event is going to be raised to OnIOComplete_Send, so let's exit right now because we're done
          return;
        }
        
        // send -- is there more data to send now? no, break iteration
        if(!CompleteSend(args)) return;
        
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
        if(!_disposing) ProcessError("Socket error '"+args.SocketError+"'.");
        
        args.Dispose();
        
        return false;
        
      }
      
      BufferCache.Set(args.Buffer);
      args.Dispose();
      
      // is there a callback? yes, run it
      if(_onSent != null) _onSent.Run();
      
      // more data to send? yes, return positive
      if (_sendBuffer.Length > 0) return true;
      
      // more buffers in queue?
      if(_sendQueue.TakeItem().Count > 0) {
        // yes, recycle send buffer if appropriate
        if(_enqueueBuffer == null) _enqueueBuffer = _sendBuffer;
        
        // get the next buffer to send
        QueuedBuffer queuedBuffer = _sendQueue.Item.RemoveReturn(0);
        _sendQueue.Release();
        
        // set the send buffer
        SetSendBuffer(queuedBuffer.Buffer, queuedBuffer.EndPoint);
        
        return true;
      }
      _sendQueue.Release();
      
      _sendLock.Release();
      
      // more data to send? yes, take the lock and return positive
      if (_sendBuffer.Length > 0 && _sendLock.TryTake) return true;
      
      // more buffers in the queue? yes, take the lock and return positive
      if(_sendQueue.TakeItem().Count > 0 && _sendLock.TryTake) {
        // yes, recycle send buffer if appropriate
        if(_enqueueBuffer == null) _enqueueBuffer = _sendBuffer;
        
        // get the next buffer to send
        QueuedBuffer queuedBuffer = _sendQueue.Item.RemoveReturn(0);
        _sendQueue.Release();
        
        // set the send buffer
        SetSendBuffer(queuedBuffer.Buffer, queuedBuffer.EndPoint);
        
        return true;
      }
      _sendQueue.Release();
      
      return false;
    }

    /// <summary>
    /// This method is called when there is no more data to send to a connected client
    /// </summary>
    private void OnSocketSend(object sender, SocketAsyncEventArgs e) {
      
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
    }
    
    /// <summary>
    /// On socket data being received.
    /// </summary>
    private unsafe void OnReceiveSocketData(IAsyncResult ar) {
      
      int count = 0;
      try {
        
        // end the receive
        count = Socket.EndReceiveFrom(ar, ref _receiveEndPoint);
        
      } catch(Exception ex) {
        
        // release the read lock
        _syncLock.ReleaseRead();
        
        ProcessError(ex);
        return;
        
      }
      
      if(count > 0) {
        
        // is the received buffer chunked?
        byte flag = _receiveBuffer[0];
        if(flag < 60 || flag >= 188) {
          
          count -= ChunkHeaderSize;
          
          int chunkId;
          int chunkIndex;
          int chunkCount;
          
          // sanity check for correct number of bytes received
          if(count < 0) {
            _syncLock.ReleaseRead();
            ProcessError("Socket didn't receive enough data for chunked information.");
            return;
          }
          
          // yes, read the chunk details
          fixed(byte* intP = &_receiveBuffer[1]) {
            chunkId = *(int*)intP;
          }
          fixed(byte* intP = &_receiveBuffer[5]) {
            chunkIndex = *(int*)intP;
          }
          fixed(byte* intP = &_receiveBuffer[9]) {
            chunkCount = *(int*)intP;
          }
          
          // sanity check for the chunk data being valid
          if(chunkIndex >= chunkCount) {
            _syncLock.ReleaseRead();
            ProcessError("Socket received invalid chunk index and count information.");
            return;
          }
          
          // write
          ChunkedGram chunkedGram;
          if(_receivedChunks.TryGetValue(chunkId, out chunkedGram)) {
            chunkedGram.Length += count;
            chunkedGram.Chunks.Insert(Teple.New(count, _receiveBuffer), chunkIndex);
            
            // have all chunks been added?
            if(chunkedGram.Chunks.Count == chunkCount) {
              
              // yes, remove from the collection
              _receivedChunks.Remove(chunkId);
              
              // create a byte buffer for the entire message
              byte[] result = new byte[chunkedGram.Length];
              int index = 0;
              foreach(var chunk in chunkedGram.Chunks) {
                int length = chunk.ArgA;
                Micron.CopyMemory(chunk.ArgB, ChunkHeaderSize, result, index, length);
                index += length;
              }
              
              // reference the endpoint from which the data was received
              IPEndPoint endpoint = (IPEndPoint)_receiveEndPoint;
              
              // run the callback
              _onReceive.AddRun(ActionSet.New(OnReceive,
                endpoint,
                result,
                chunkedGram.Length));
              
            } else {
              
              // no, create a new receive buffer
              _receiveBuffer = BufferCache.Get();
              
            }
            
          } else {
            chunkedGram = new ChunkedGram {
              Chunks = new ArrayRig<Teple<int, byte[]>>(chunkCount),
              Timestamp = Time.Timestamp
            };
            
            _receivedChunks.Add(chunkId, chunkedGram);
            
            chunkedGram.Chunks.Add(Teple.New(count, _receiveBuffer));
            chunkedGram.Length += count;
            
            // create a new receive buffer
            _receiveBuffer = BufferCache.Get();
          }
          
          
          
        } else {
          
          // no, copy the received buffer
          --count;
          byte[] buffer = BufferCache.Get(count);
          Micron.CopyMemory(_receiveBuffer, 1, buffer, 0, count);
          
          // reference the endpoint from which the data was received
          IPEndPoint endpoint = (IPEndPoint)_receiveEndPoint;
          
          // run the callback
          _onReceive.AddRun(ActionSet.New(OnReceive,
            endpoint,
            buffer,
            count));
          
        }
        
      }
      
      if(_receivedChunks.Count > 0) {
        // check for any chunked data timeouts
        ArrayRig<int> toRemove = null;
        foreach(var chunkedGram in _receivedChunks) {
          if(Time.Timestamp - chunkedGram.Value.Timestamp > ChunkedGramTimeout) {
            if(toRemove == null) toRemove = new ArrayRig<int>();
            toRemove.Add(chunkedGram.Key);
          }
        }
        if(toRemove != null) {
          foreach(var chunkId in toRemove) {
            ChunkedGram chunked;
            if(_receivedChunks.TryGetValue(chunkId, out chunked)) {
              _receivedChunks.Remove(chunkId);
              chunked.Chunks.Dispose();
            }
          }
        }
      }
      
      // release the read lock
      _syncLock.ReleaseRead();
      
      // create the endpoint for receiving data
      _receiveEndPoint = new IPEndPoint(RemoteEndPoint.Address, RemoteEndPoint.Port);
      
      try {
        
        // start receiving again
        Socket.BeginReceiveFrom(_receiveBuffer, 0, Global.BufferSizeLocal, SocketFlags.None,
          ref _receiveEndPoint, OnReceiveSocketData, null);
        
      } catch(Exception ex) {
        
        ProcessError(ex);
        return;
        
      }
      
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
    
    /// <summary>
    /// Set the buffer to be sent.
    /// </summary>
    private void SetSendBuffer(BufferQueue buffer, IPEndPoint endPoint) {
      
      _sendBuffer = buffer;
      TargetEndPoint = endPoint;
      
      // should the buffer be chunked?
      if(_sendBuffer.Length > MaxDatagramSize) {
        // yes, set some of the chunked details
        _chunked = true;
        _chunkIndex = 0;
        _chunkedId = Randomize.Range(int.MinValue, int.MaxValue);
        _chunkCount = (int)Math.Ceiling((double)_sendBuffer.Length / (MaxDatagramSize-ChunkHeaderSize));
      } else {
        // no
        _chunked = false;
      }
      
    }
    
    /// <summary>
    /// Try get a send buffer from the current queue taking chunking into account.
    /// </summary>
    private unsafe int TryGetBuffer(byte[] buffer) {
      
      int count;
      if(_chunked) {
        
        // dequeue a buffer from the bytes to send
        count = _sendBuffer.Dequeue(buffer, ChunkHeaderSize, MaxDatagramSize-ChunkHeaderSize);
        
        if(count == 0) return 0;
        
        // write a positive flag
        buffer[0] = (byte)(Randomize.Range(188, 188 + 127) % 255);
        
        // print the chunked id in the buffer
        fixed (byte* byteP = &buffer[1]) {
          *(int*)byteP = _chunkedId;
        }
        
        // print the chunk index in the buffer
        fixed (byte* byteP = &buffer[5]) {
          *(int*)byteP = _chunkIndex;
        }
        
        // print the chunk count in the buffer
        fixed (byte* byteP = &buffer[9]) {
          *(int*)byteP = _chunkCount;
        }
        
        // increment the bytes to send by the number of bytes in the header
        count += ChunkHeaderSize;
        
        // increment the current chunk index
        ++_chunkIndex;
        
      } else {
        
        // dequeue a buffer from the bytes to send
        count = _sendBuffer.Dequeue(buffer, 1, MaxDatagramSize-1);
        
        if(count == 0) return 0;
        
        // write a negative flag
        buffer[0] = (byte)(Randomize.Range(188-128, 187) % 255);
        
        ++count;
        
      }
      
      return count;
      
    }
    
  }
  
}

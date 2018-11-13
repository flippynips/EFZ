/*
 * User: Bob
 * Date: 23/11/2016
 * Time: 11:11
 */
using System;

namespace Efz {
  
  /// <summary>
  /// Func delegate.
  /// </summary>
  public delegate TReturn Func<TReturn>();
  /// <summary>
  /// Func delegate.
  /// </summary>
  public delegate TReturn Func<A,TReturn>(A a);
  /// <summary>
  /// Func delegate.
  /// </summary>
  public delegate TReturn Func<A,B,TReturn>(A a, B b);
  /// <summary>
  /// Func delegate.
  /// </summary>
  public delegate TReturn Func<A,B,C,TReturn>(A a, B b, C c);
  /// <summary>
  /// Func delegate.
  /// </summary>
  public delegate TReturn Func<A,B,C,D,TReturn>(A a, B b, C c, D d);
  /// <summary>
  /// Func delegate.
  /// </summary>
  public delegate TReturn Func<A,B,C,D,E,TReturn>(A a, B b, C c, D d, E e);
  /// <summary>
  /// Func delegate.
  /// </summary>
  public delegate TReturn Func<A,B,C,D,E,F,TReturn>(A a, B b, C c, D d, E e, F f);
  /// <summary>
  /// Func delegate.
  /// </summary>
  public delegate TReturn Func<A,B,C,D,E,F,G,TReturn>(A a, B b, C c, D d, E e, F f, G g);
  /// <summary>
  /// Func delegate.
  /// </summary>
  public delegate TReturn Func<A,B,C,D,E,F,G,H,TReturn>(A a, B b, C c, D d, E e, F f, G g, H h);
  /// <summary>
  /// Func delegate.
  /// </summary>
  public delegate TReturn Func<A,B,C,D,E,F,G,H,I,TReturn>(A a, B b, C c, D d, E e, F f, G g, H h, I i);
  /// <summary>
  /// Func delegate.
  /// </summary>
  public delegate TReturn Func<A,B,C,D,E,F,G,H,I,J,TReturn>(A a, B b, C c, D d, E e, F f, G g, H h, I i, J j);
  
}

/*
 * User: Joshua
 * Date: 4/08/2016
 * Time: 11:01 PM
 */
using System;

using Efz.Collections;
using Efz.Threading;

namespace Efz.Tools {
  
  /// <summary>
  /// Structure to search a collection of TItems using equatable TKeys.
  /// Efficient at searching, not adding search items. Threadsafe.
  /// </summary>
  public class PatternSearch<TKey, TValue> : IDisposable where TKey : IEquatable<TKey> {
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// The initial branch.
    /// </summary>
    protected Branch _root;
    /// <summary>
    /// Lock used for the tree search.
    /// </summary>
    protected Lock _lock;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize an empty tree search, optionally specifying an expected leaf collection
    /// size.
    /// </summary>
    public PatternSearch() {
      _root = new Branch();
      _lock = new Lock();
    }
    
    /// <summary>
    /// Dispose of the components of the tree search.
    /// </summary>
    public void Dispose() {
      _root = null;
    }
    
    public DynamicSearch SearchDynamic() {
      return new DynamicSearch(this);
    }
    
    public ProgressiveSearch SearchProgressive() {
      return new ProgressiveSearch(this);
    }
    
    /// <summary>
    /// Search using the specified keys and return the best fit.
    /// Returns default(TValue) on no result.
    /// </summary>
    public TValue SearchBest(TKey[] keys, int start = 0, int end = -1) {
      if(end == -1) end = keys.Length;
      // set the initial value
      TValue value = _root.ValueSet ? _root.Value : default(TValue);
      Branch currentBranch = _root;
      
      while(start < end) {
        
        bool gotBranch = false;
        // iterate through branches
        foreach(Branch branch in currentBranch.Leaves) {
          
          bool match = false;
          foreach(TKey k in branch.Keys) {
            if(keys[start].Equals(k)) {
              match = true;
              break;
            }
          }
          
          if(match != branch.Not) {
            // move down a segment
            currentBranch = branch;
            gotBranch = true;
            // set the new value
            if(currentBranch.ValueSet) {
              value = currentBranch.Value;
            }
            break;
          }
        }
        
        // if no branch - return the latest value
        if(!gotBranch) {
          return value;
        }
        ++start;
        
      }
      
      // return the result
      return value;
    }
    
    /// <summary>
    /// Search using the specified keys. Returns default(TValue) on no result.
    /// </summary>
    public TValue Search(TKey[] keys, int start = 0, int end = -1) {
      if(end == -1) end = keys.Length;
      
      Branch currentBranch = _root;
      while(start < end) {
        bool gotBranch = false;
        // iterate through branches
        foreach(Branch branch in currentBranch.Leaves) {
          
          bool match = false;
          foreach(TKey k in branch.Keys) {
            if(keys[start].Equals(k)) {
              match = true;
              break;
            }
          }
          
          if(match != branch.Not) {
            // move down a segment
            currentBranch = branch;
            gotBranch = true;
            break;
          }
        }
        if(!gotBranch) {
          return default(TValue);
        }
        ++start;
      }
      
      // return the result
      return currentBranch.Value;
    }
    
    /// <summary>
    /// Add or Set a search node to the current search branches.
    /// </summary>
    public void Add(TValue value, params Struct<TKey[], bool>[] keySets) {
      var sets = new ArrayRig<Struct<ArrayRig<TKey>, bool>>(keySets.Length);
      foreach(var s in keySets) sets.Add(new Struct<ArrayRig<TKey>, bool>(new ArrayRig<TKey>(s.ArgA), s.ArgB));
      Add(value, sets);
    }
    
    /// <summary>
    /// Add or Set a search node to the current search branches.
    /// </summary>
    public void Add(TValue value, ArrayRig<Struct<TKey[], bool>> keySets) {
      var sets = new ArrayRig<Struct<ArrayRig<TKey>, bool>>(keySets.Count);
      foreach(var s in keySets) sets.Add(new Struct<ArrayRig<TKey>, bool>(new ArrayRig<TKey>(s.ArgA), s.ArgB));
      Add(value, sets);
    }
    
    /// <summary>
    /// Add or Set a search node to the current search branches.
    /// </summary>
    public void Add(TValue value, ArrayRig<Struct<ArrayRig<TKey>, bool>> keySets) {
      // get the lock
      _lock.Take();
      
      Branch currentBranch = _root;
      Branch next;
      bool gotKey;
      int index = 0;
      
      // iterate through keys
      foreach(var keySet in keySets) {
        ++index;
        // if children not yet set
        if(currentBranch.LeavesSet) {
          gotKey = false;
          // iterate through leaves of current branch
          foreach(Branch leaf  in currentBranch.Leaves) {
            
            // if the 'Not' value doesn't match - skip
            if(leaf.Not != keySet.ArgB) continue;
            
            bool match = true;
            foreach(TKey k in leaf.Keys) {
              bool found = false;
              foreach(TKey p in keySet.ArgA) {
                if(k.Equals(p)) {
                  found = true;
                  break;
                }
              }
              if(!found) {
                match = false;
                break;
              }
            }
            
            if(match) {
              // move to the next branch
              gotKey = true;
              currentBranch = leaf;
              break;
            }
            
          }
          
          // if last key index
          if(index == keySets.Count) {
            
            if(gotKey) {
              
              // key exists - set the value
              currentBranch.Value = value;
              currentBranch.ValueSet = true;
              
            } else {
              
              // add new branch
              Branch[] leaves = new Branch[currentBranch.Leaves.Length+1];
              for(int i = currentBranch.Leaves.Length-1; i >= 0; --i) {
                leaves[i] = currentBranch.Leaves[i];
              }
              leaves[leaves.Length-1] = next = new Branch {
                Keys = keySet.ArgA.ToArray(),
                Not = keySet.ArgB,
                Value = value,
                ValueSet = true };
              currentBranch.Leaves = leaves;
              
            }
            
          } else if(!gotKey) {
            
            // add new branch
            Branch[] leaves = new Branch[currentBranch.Leaves.Length+1];
            for(int i = currentBranch.Leaves.Length-1; i >= 0; --i) {
              leaves[i] = currentBranch.Leaves[i];
            }
            leaves[leaves.Length-1] = next = new Branch { Keys = keySet.ArgA.ToArray(), Not = keySet.ArgB };
            currentBranch.Leaves = leaves;
            currentBranch = next;
          }
          
        } else {
          
          // no leaves on current branch
          
          // if last key
          if(index == keySets.Count) {
            // setup leaves array with the search value
            currentBranch.Leaves = new []{ new Branch {
              Keys = keySet.ArgA.ToArray(),
              Not = keySet.ArgB,
              Value = value,
              ValueSet = true
            } };
            currentBranch.LeavesSet = true;
          } else {
            // setup leaves array
            currentBranch.Leaves = new []{ next = new Branch {
              Keys = keySet.ArgA.ToArray(),
              Not = keySet.ArgB
            } };
            currentBranch.LeavesSet = true;
            currentBranch = next;
          }
          
        }
        
      }
      
      // release the lock
      _lock.Release();
    }
    
    /// <summary>
    /// Remove the value at the path of keys.
    /// </summary>
    public void Remove(ArrayRig<TKey> keys) {
      // get the lock
      _lock.Take();
      
      Branch currentBranch = _root;
      ArrayRig<Branch> branches = new ArrayRig<Branch>();
      bool gotKey = false;
      int index = 0;
      
      // iterate through keys in the branch to be set
      foreach(TKey keySearch in keys) {
        ++index;
        // if children not yet set
        if(currentBranch.LeavesSet) {
          // iterate through branches
          foreach(Branch branch in currentBranch.Leaves) {
            
            bool match = false;
            foreach(TKey k in branch.Keys) {
              match |= keySearch.Equals(k);
            }
            
            if(match != branch.Not) {
              // move down a segment
              gotKey = true;
              branches.Add(currentBranch);
              currentBranch = branch;
              break;
            }
            
          }
          
          // if last key index
          if(index == keys.Count) {
            
            if(gotKey) {
              
              // key exists - remove the value
              currentBranch.Value = default(TValue);
              currentBranch.ValueSet = false;
              
              // iterate path of removed node to the root
              // and remove unrequired branches
              Branch last;
              for(int i = branches.Count-1; i >= 0; --i) {
                currentBranch = branches[i];
                // if no leaves - remove the branch
                if(currentBranch.Leaves.Length == 1) {
                  currentBranch.Leaves = null;
                  currentBranch.LeavesSet = false;
                } else {
                  
                  // remove unrequired leaf
                  int count = currentBranch.Leaves.Length;
                  Branch[] leaves = new Branch[count-1];
                  for(int j = currentBranch.Leaves.Length-1; j > -1; --j) {
                    
                    bool match = false;
                    foreach(TKey k in currentBranch.Leaves[j].Keys) {
                      if(keys[i].Equals(k)) {
                        match = true;
                        break;
                      }
                    }
                    
                    if(match != currentBranch.Not) {
                      leaves[count] = currentBranch.Leaves[j];
                      ++count;
                    }
                    
                  }
                  // replace leaves
                  currentBranch.Leaves = leaves;
                }
                last = currentBranch;
              }
              
            } else {
              // key doesn't exist
              // release lock and return
              _lock.Release();
              return;
            }
            
          } else if(!gotKey) {
            // key doesn't exist
            // release lock and return
            _lock.Release();
            return;
            
          }
        } else {
          // current branch doesn't have any leaves
          // release lock and return
          _lock.Release();
          return;
        }
        
      }
      
      // release the lock
      _lock.Release();
    }

    //-------------------------------------------//
    
    /// <summary>
    /// Search that returns all matching values of any number of keys entered.
    /// </summary>
    public class DynamicSearch {
      
      //-------------------------------------------//
      
      /// <summary>
      /// Retrieve values of the current branches.
      /// </summary>
      public readonly ArrayRig<TValue> Values;
      
      /// <summary>
      /// Current active branches in the search.
      /// </summary>
      public Queue<Branch> _branches;
      /// <summary>
      /// The tree used for this search.
      /// </summary>
      private Branch _root;
      
      /// <summary>
      /// Initialize a new dynamic search for the specified tree.
      /// </summary>
      internal DynamicSearch(PatternSearch<TKey, TValue> tree) {
        _root = tree._root;
        _branches = new Queue<Branch>();
        Values = new ArrayRig<TValue>();
      }
      
      /// <summary>
      /// Reset this dynamic search.
      /// </summary>
      public void Reset() {
        _branches.Reset();
        Values.Reset();
      }
      
      /// <summary>
      /// Move all current branches to the next search node that corresponds to the specified key.
      /// Returns if there is at least one value on the current branches.
      /// </summary>
      public bool Next(TKey key) {
        
        // skip if no branches
        if(!_root.LeavesSet) return false;
        
        // reset the current values
        Values.Reset();
        
        // get the branches to iterate
        int count = _branches.Count;
        
        // always check the root branch
        foreach(Branch leaf in _root.Leaves) {
          
          bool match = false;
            
          // iterate the keys
          foreach(TKey k in leaf.Keys) {
            // does the key match?
            if(key.Equals(k)) {
              // no, flip the match flag
              match = true;
              break;
            }
          }
          
          // does the match state equal the leaf exclusivity?
          if(match != leaf.Not) {
            
            // yes, add next branch
            _branches.Enqueue(leaf);
            // if value - add it
            if(leaf.ValueSet) Values.Add(leaf.Value);
            
          }
        }
        
        // get the number of branches to process
        if(count > 0) {
          // iterate branches to search
          do {
            _branches.Dequeue();
            if(_branches.Current.LeavesSet) {
              // iterate leaves of current branches
              foreach(Branch leaf in _branches.Current.Leaves) {
                bool match = false;
                
                // iterate the keys and check for matches
                foreach(TKey k in leaf.Keys) {
                  // does the key match?
                  if(key.Equals(k)) {
                    // no, flip the match flag
                    match = true;
                    break;
                  }
                }
                
                // does the match state equal the leaf exclusivity?
                if(match != leaf.Not) {
                  
                  // yes, add next branch
                  _branches.Enqueue(leaf);
                  // if value - add it
                  if(leaf.ValueSet) Values.Add(leaf.Value);
                  
                }
              }
            }
          } while(--count != 0);
        }
        
        return Values.Count != 0;
      }
      
      //-------------------------------------------//
      
    }
    
    /// <summary>
    /// Progressive search which moves one branch at a time.
    /// </summary>
    public class ProgressiveSearch {
      
      //-------------------------------------------//
      
      /// <summary>
      /// Get the current value.
      /// </summary>
      public TValue Value { get { return _current.Value; } }
      
      /// <summary>
      /// The current branch.
      /// </summary>
      protected Branch _current;
      /// <summary>
      /// The root branch to reset to.
      /// </summary>
      protected Branch _root;
      
      /// <summary>
      /// Initialize a new progressive search for the specified tree.
      /// </summary>
      internal ProgressiveSearch(PatternSearch<TKey, TValue> tree) {
        _current = _root = tree._root;
      }
      
      /// <summary>
      /// Reset this progressive search instance.1
      /// </summary>
      public void Reset() {
        _current = _root;
      }
      
      /// <summary>
      /// Move to the next search node.
      /// </summary>
      public bool Next(TKey key) {
        // does the current branch have keys to move to?
        if(_current.LeavesSet) {
          // yes, iterate the leaves of the current branch
          foreach(Branch leaf in _current.Leaves) {
            
            // is the leaf exclusive?
            if(leaf.Not) {
              // yes, iterate the leaf keys
              foreach(TKey k in leaf.Keys) {
                // does the key match? yes, the leaf doesn't match
                if(key.Equals(k)) return false;
              }
              
              // set the current leaf
              _current = leaf;
              return leaf.ValueSet;
              
            } else {
              
              // iterate the leaf keys
              foreach(TKey k in leaf.Keys) {
                // does the key match?
                if(key.Equals(k)) {
                  // yes, set the current leaf
                  _current = leaf;
                  return leaf.ValueSet;
                }
              }
              
            }
            
          }
        }
        
        // iterate the root leaves
        foreach(Branch leaf in _root.Leaves) {
          
          // is the leaf exclusive?
          if(leaf.Not) {
            // yes, iterate the leaf keys
            foreach(TKey k in leaf.Keys) {
              // does the key match? yes, the leaf doesn't match
              if(key.Equals(k)) return false;
            }
            
            // set the current leaf
            _current = leaf;
            return leaf.ValueSet;
            
          } else {
            
            // iterate the leaf keys
            foreach(TKey k in leaf.Keys) {
              
              // does the key match?
              if(key.Equals(k)) {
                // yes, set the current leaf
                _current = leaf;
                return leaf.ValueSet;
              }
            }
            
          }
        }
        
        // no matching leaves
        return false;
      }
      
      //-------------------------------------------//
      
    }
    
    /// <summary>
    /// A branch of the pattern search tree.
    /// </summary>
    public class Branch {
      
      //-------------------------------------------//
      
      /// <summary>
      /// The keys that represent the branch.
      /// </summary>
      public TKey[] Keys;
      /// <summary>
      /// Whether the banch should exclude the defined keys.
      /// </summary>
      public bool Not;
      
      /// <summary>
      /// The following leaves.
      /// </summary>
      public Branch[] Leaves;
      /// <summary>
      /// Have the leaves been set?
      /// </summary>
      public bool LeavesSet;
      
      /// <summary>
      /// Value of the branch if set.
      /// </summary>
      public TValue Value;
      /// <summary>
      /// Was the branch value set?
      /// </summary>
      public bool ValueSet;
      
      //-------------------------------------------//
      
    }
    
  }
  
  
  
  
}

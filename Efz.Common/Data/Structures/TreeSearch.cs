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
  public class TreeSearch<TKey, TValue> : IDisposable where TKey : IEquatable<TKey> {
    
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
    public TreeSearch() {
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
      TValue value = _root.valueSet ? _root.value : default(TValue);
      Branch currentBranch = _root;
      
      while(start < end) {
        
        bool gotBranch = false;
        // iterate through branches
        foreach(Branch branch in currentBranch.leaves) {
          // if the segment key matches
          if(branch.key.Equals(keys[start])) {
            // move down a segment
            currentBranch = branch;
            gotBranch = true;
            // set the new value
            if(currentBranch.valueSet) {
              value = currentBranch.value;
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
        foreach(Branch branch in currentBranch.leaves) {
          // if the segment key matches
          if(branch.key.Equals(keys[start])) {
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
      return currentBranch.value;
    }
    
    /// <summary>
    /// Add or Set a search node to the current search branches.
    /// </summary>
    public void Add(TValue value, ArrayRig<TKey> keys) {
      // get the lock
      _lock.Take();
      
      Branch currentBranch = _root;
      Branch next;
      bool gotKey;
      int index = 0;
      
      // iterate through keys
      foreach(TKey keySearch in keys) {
        ++index;
        // if children not yet set
        if(currentBranch.leavesSet) {
          gotKey = false;
          // iterate through leavs of current branch
          foreach(Branch leaf  in currentBranch.leaves) {
            
            // if the leaf key matches
            if(leaf.key.Equals(keySearch)) {
              // move to the next branch
              gotKey = true;
              currentBranch = leaf;
              break;
            }
            
          }
          
          // if last key index
          if(index == keys.Count) {
            
            if(gotKey) {
              
              // key exists - set the value
              currentBranch.value = value;
              currentBranch.valueSet = true;
              
            } else {
              
              // add new branch
              Branch[] leaves = new Branch[currentBranch.leaves.Length+1];
              for(int i = currentBranch.leaves.Length-1; i >= 0; --i) {
                leaves[i] = currentBranch.leaves[i];
              }
              leaves[leaves.Length-1] = next = new Branch { key = keySearch, value = value, valueSet = true };
              currentBranch.leaves = leaves;
              
            }
            
          } else if(!gotKey) {
            
            // add new branch
            Branch[] leaves = new Branch[currentBranch.leaves.Length+1];
            for(int i = currentBranch.leaves.Length-1; i >= 0; --i) {
              leaves[i] = currentBranch.leaves[i];
            }
            leaves[leaves.Length-1] = next = new Branch { key = keySearch };
            currentBranch.leaves = leaves;
            currentBranch = next;
          }
        } else {
          
          // no leaves on current branch
          
          // if last key
          if(index == keys.Count) {
            // setup leaves array with the search value
            currentBranch.leaves = new []{ new Branch { key = keySearch, value = value, valueSet = true } };
            currentBranch.leavesSet = true;
          } else {
            // setup leaves array
            currentBranch.leaves = new []{ next = new Branch { key = keySearch } };
            currentBranch.leavesSet = true;
            currentBranch = next;
          }
          
        }
        
      }
      
      // release the lock
      _lock.Release();
    }
    
    /// <summary>
    /// Add or Set a search node to the current search branches.
    /// </summary>
    public void Add(TValue value, TKey[] keys) {
      // get the lock
      _lock.Take();
      
      Branch currentBranch = _root;
      Branch next;
      bool gotKey;
      int index = 0;
      
      // iterate through keys
      foreach(TKey keySearch in keys) {
        ++index;
        // if the current branch has leaves
        if(currentBranch.leavesSet) {
          gotKey = false;
          // iterate through leaves of current branch
          foreach(Branch leaf  in currentBranch.leaves) {
            
            // if the leaf key matches
            if(leaf.key.Equals(keySearch)) {
              // move to the next branch
              gotKey = true;
              currentBranch = leaf;
              break;
            }
            
          }
          
          // if last key index
           if(index == keys.Length) {
            
            if(gotKey) {
              
              // key exists - set the value
              currentBranch.value = value;
              currentBranch.valueSet = true;
              
            } else {
            
              // add new branch
              Branch[] leaves = new Branch[currentBranch.leaves.Length+1];
              for(int i = currentBranch.leaves.Length-1; i >= 0; --i) {
                leaves[i] = currentBranch.leaves[i];
              }
              leaves[leaves.Length-1] = next = new Branch { key = keySearch, value = value, valueSet = true };
              currentBranch.leaves = leaves;
              
            }
            
          } else if(!gotKey) {
            
            // add new branch
            Branch[] leaves = new Branch[currentBranch.leaves.Length+1];
            for(int i = currentBranch.leaves.Length-1; i >= 0; --i) {
              leaves[i] = currentBranch.leaves[i];
            }
            leaves[leaves.Length-1] = next = new Branch { key = keySearch };
            currentBranch.leaves = leaves;
            currentBranch = next;
          }
        } else {
          
          // no leaves on current branch
          
          // if last key
          if(index == keys.Length) {
            // setup leaves array with the search value
            currentBranch.leaves = new []{ new Branch { key = keySearch, value = value, valueSet = true } };
            currentBranch.leavesSet = true;
          } else {
            // setup leaves array
            currentBranch.leaves = new []{ next = new Branch { key = keySearch } };
            currentBranch.leavesSet = true;
            currentBranch = next;
          }
          
        }
        
      }
      
      // release the lock
      _lock.Release();
    }
    
    /// <summary>
    /// Add an enumerable collection as keys.
    /// </summary>
    public void Add(TValue value, System.Collections.Generic.IEnumerable<TKey> keys) {
      ArrayRig<TKey> rig = new ArrayRig<TKey>();
      rig.AddCollection(keys);
      Add(value, rig);
    }
    
    /// <summary>
    /// Remove the value at the path of keys.
    /// </summary>
    public void Remove(TKey[] keys) {
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
        if(currentBranch.leavesSet) {
          // iterate through branches
          foreach(Branch branch in currentBranch.leaves) {
            
            // if the segment key matches
            if(branch.key.Equals(keySearch)) {
              // move down a segment
              gotKey = true;
              branches.Add(currentBranch);
              currentBranch = branch;
              break;
            }
            
          }
          
          // if last key index
          if(index == keys.Length) {
            
            if(gotKey) {
              
              // key exists - remove the value
              currentBranch.value = default(TValue);
              currentBranch.valueSet = false;
              
              // iterate path of removed node to the root
              // and remove unrequired branches
              Branch last;
              for(int i = branches.Count-1; i >= 0; --i) {
                currentBranch = branches[i];
                // if no leaves - remove the branch
                if(currentBranch.leaves.Length == 1) {
                  currentBranch.leaves = null;
                  currentBranch.leavesSet = false;
                } else {
                  // remove unrequired leaf
                  int count = currentBranch.leaves.Length;
                  Branch[] leaves = new Branch[count-1];
                  for(int j = currentBranch.leaves.Length-1; j > -1; --j) {
                    if(currentBranch.leaves[j].key.Equals(keys[i])) {
                      leaves[count] = currentBranch.leaves[j];
                      ++count;
                    }
                  }
                  // replace leaves
                  currentBranch.leaves = leaves;
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
        if(currentBranch.leavesSet) {
          // iterate through branches
          foreach(Branch branch in currentBranch.leaves) {
            
            // if the segment key matches
            if(branch.key.Equals(keySearch)) {
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
              currentBranch.value = default(TValue);
              currentBranch.valueSet = false;
              
              // iterate path of removed node to the root
              // and remove unrequired branches
              Branch last;
              for(int i = branches.Count-1; i >= 0; --i) {
                currentBranch = branches[i];
                // if no leaves - remove the branch
                if(currentBranch.leaves.Length == 1) {
                  currentBranch.leaves = null;
                  currentBranch.leavesSet = false;
                } else {
                  // remove unrequired leaf
                  int count = currentBranch.leaves.Length;
                  Branch[] leaves = new Branch[count-1];
                  for(int j = currentBranch.leaves.Length-1; j > -1; --j) {
                    if(currentBranch.leaves[j].key.Equals(keys[i])) {
                      leaves[count] = currentBranch.leaves[j];
                      ++count;
                    }
                  }
                  // replace leaves
                  currentBranch.leaves = leaves;
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
      public Queue<Branch> Branches;
      
      /// <summary>
      /// The tree used for this search.
      /// </summary>
      private Branch _root;
      
      /// <summary>
      /// Initialize a new dynamic search for the specified tree.
      /// </summary>
      internal DynamicSearch(TreeSearch<TKey, TValue> _tree) {
        _root = _tree._root;
        Branches = new Queue<Branch>();
        Values = new ArrayRig<TValue>();
      }
      
      /// <summary>
      /// Reset this dynamic search.
      /// </summary>
      public void Reset() {
        Branches.Reset();
        Values.Reset();
      }
      
      /// <summary>
      /// Move all current branches to the next search node that corresponds to the specified key.
      /// Returns if there is at least one value on the current branches.
      /// </summary>
      public bool Next(TKey key) {
        
        // skip if no branches
        if(!_root.leavesSet) return false;
        
        // reset the current values
        Values.Reset();
        
        // get the branches to iterate
        int count = Branches.Count;
        
        // always check the root branch
        foreach(Branch leaf in _root.leaves) {
          // if key equals search key
          if(leaf.key.Equals(key)) {
            // add next branch
            Branches.Enqueue(leaf);
            // if value - add it
            if(leaf.valueSet) Values.Add(leaf.value);
          }
        }
        
        // iterate branches to search
        while(--count >= 0) {
          Branches.Dequeue();
          if(Branches.Current.leavesSet) {
            // iterate leaves of current branches
            foreach(Branch leaf in Branches.Current.leaves) {
              // if key equals search key
              if(leaf.key.Equals(key)) {
                // add next branch to the end of the queue
                Branches.Enqueue(leaf);
                // if value - add it
                if(leaf.valueSet) Values.Add(leaf.value);
              }
            }
          }
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
      public TValue Value { get { return _current.value; } }
      
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
      internal ProgressiveSearch(TreeSearch<TKey, TValue> _tree) {
        _current = _root = _tree._root;
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
      public bool Next(TKey _key) {
        if(_current.leavesSet) {
          foreach(Branch leaf in _current.leaves) {
            if(leaf.key.Equals(_key)) {
              _current = leaf;
              return leaf.valueSet;
            }
          }
        }
        foreach(Branch leaf in _root.leaves) {
          if(leaf.key.Equals(_key)) {
            _current = leaf;
            return leaf.valueSet;
          }
        }
        return false;
      }
      
      //-------------------------------------------//
      
    }
    
    /// <summary>
    /// A branch of the search tree.
    /// </summary>
    public class Branch {
      
      //-------------------------------------------//
      
      public TKey key;
      public Branch[] leaves;
      public bool leavesSet;
      public TValue value;
      public bool valueSet;
      
      //-------------------------------------------//
      
    }
    
  }
  
  
  
  
}

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
  /// Structure to search a collection of TItems using comparable TKeys.
  /// Efficient at searching, adding and removing items. Uses slightly more
  /// memory than the TreeSearch. Threadsafe searching.
  /// </summary>
  public class SnapSearch<TKey, TValue> where TKey : IComparable<TKey> {
    
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
    
    /// <summary>
    /// Initial array size of branch leaves.
    /// </summary>
    protected int _initialBranchCapacity;
    
    /// <summary>
    /// Function used to get an average between two TKeys.
    /// </summary>
    protected Func<TKey, TKey, TKey> _getAverage;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize an empty tree search.
    /// </summary>
    public SnapSearch(Func<TKey, TKey, TKey> getAverage, int initialBranchCapacity = 10) {
      _getAverage = getAverage;
      _root = new Branch();
      _lock = new Lock();
      _initialBranchCapacity = initialBranchCapacity;
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
        
        // get the next branch
        currentBranch = currentBranch.Leaves[keys[start]];
        
        // if no branch - return the latest value
        if(currentBranch == null) {
          return value;
        }
        
        if(currentBranch.ValueSet) {
          // that is the next best
          value = currentBranch.Value;
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
        
        // get the next branch
        currentBranch = currentBranch.Leaves[keys[start]];
        
        if(currentBranch == null) {
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
    public void Add(TValue value, TKey[] keys) {
      // get the lock
      _lock.Take();
      
      Branch currentBranch = _root;
      int index = 0;
      
      // iterate through keys
      foreach(TKey keySearch in keys) {
        
        ++index;
        // if leaves have been set
        if(currentBranch.LeavesSet) {
          
          // get the next branch
          currentBranch = currentBranch.Leaves[keySearch];
          
          // if the next branch wasn't found
          if(currentBranch == null) {
            // if the last key
            if(index == keys.Length) {
              
              // add a new branch with the value
              currentBranch.Leaves[keySearch] = new Branch {
                Key = keySearch,
                Value = value,
                ValueSet = true
              };
              
            } else {
              
              // add a new branch
              currentBranch = currentBranch.Leaves[keySearch] = new Branch {
                Key = keySearch
              };
              
            }
          } else if(index == keys.Length) {
            
            // key exists - set the value
            currentBranch.Value = value;
            currentBranch.ValueSet = true;
            
          }
        } else {
          
          // no leaves on current branch
          
          // if last key
          if(index == keys.Length) {
            
            // setup leaves array with the search value
            currentBranch.Leaves = new SortedRig<TKey, Branch>(_getAverage, _initialBranchCapacity);
            currentBranch.LeavesSet = true;
            currentBranch.Leaves[keySearch] = new Branch {
              Key = keySearch,
              Value = value,
              ValueSet = true
            };
            
          } else {
            
            // setup leaves array
            currentBranch.Leaves = new SortedRig<TKey, Branch>(_getAverage, _initialBranchCapacity);
            currentBranch.LeavesSet = true;
            
            // create next branch
            currentBranch = currentBranch.Leaves[keySearch] = new Branch {
              Key = keySearch
            };
            
          }
          
        }
        
      }
      
      // release the lock
      _lock.Release();
    }
    
    /// <summary>
    /// Add or Set a search node to the current search branches.
    /// </summary>
    public void Add(TValue value, TKey[] keys, int start, int end) {
      // get the lock
      _lock.Take();
      
      Branch currentBranch = _root;
      int index = 0;
      
      // iterate through keys
      for(int k = start; k < end; ++k) {
        TKey keySearch = keys[k];
        
        ++index;
        // if children not yet set
        if(currentBranch.LeavesSet) {
          
          // get the next branch
          currentBranch = currentBranch.Leaves[keySearch];
          
          // if the next branch didn't exist
          if(currentBranch == null) {
            
            // if last key index
            if(index == keys.Length) {
            
              // add a new branch with the value
              currentBranch.Leaves[keySearch] = new Branch {
                Key = keySearch,
                Value = value,
                ValueSet = true
              };
              
            } else {
              
              // add a new branch
              currentBranch = currentBranch.Leaves[keySearch] = new Branch {
                Key = keySearch
              };
            }
            
          } else if(index == keys.Length) {
            
            // key exists - set the value
            currentBranch.Value = value;
            currentBranch.ValueSet = true;
            
          }
          
        } else {
          
          // no leaves on current branch
          
          // if last key
          if(index == keys.Length) {
            
            // setup leaves array with the search value
            currentBranch.Leaves = new SortedRig<TKey, Branch>(_getAverage, _initialBranchCapacity);
            currentBranch.LeavesSet = true;
            currentBranch.Leaves[keySearch] = new Branch {
              Key = keySearch,
              Value = value,
              ValueSet = true
            };
            
          } else {
            
            // setup leaves array
            currentBranch.Leaves = new SortedRig<TKey, Branch>(_getAverage, _initialBranchCapacity);
            currentBranch.LeavesSet = true;
            // create next branch
            currentBranch = currentBranch.Leaves[keySearch] = new Branch {
              Key = keySearch
            };
            
          }
          
        }
        
      }
      
      // release the lock
      _lock.Release();
    }
    
    /// <summary>
    /// Remove the value at the path of keys.
    /// This removes branches with no branches or values due
    /// to the target branch removal.
    /// </summary>
    public void Remove(TKey[] keys) {
      // get the lock
      _lock.Take();
      
      Branch currentBranch = _root;
      int earliestRemoveIndex = 0;
      Branch[] branchPath = new Branch[keys.Length];
      
      // iterate through keys in the branch to be set
      for(int i = 0, keyCount = keys.Length; i < keyCount; ++i) {
        
        // if children not yet set
        if(currentBranch.LeavesSet) {
          
          // if the current branch has only one leaf
          if(currentBranch.Leaves.Count == 1) {
            
            // get the only branch
            currentBranch = currentBranch.Leaves.Array[0].ArgB;
            // does the branch have the key required
            if(currentBranch.Key.Equals(keys[i])) {
              
              // if branch index to remove hasn't been set
              if(currentBranch.ValueSet) {
                // unset the remove index
                earliestRemoveIndex = -1;
              } else {
                if(earliestRemoveIndex == -1) earliestRemoveIndex = i;
              }
              
            } else {
              // release lock and return
              _lock.Release();
              return;
            }
            
          } else {
            
            // unset the remove index
            earliestRemoveIndex = -1;
            
            // search for required key
            currentBranch = currentBranch.Leaves[keys[i]];
            
            // if the next branch doesn't exist
            if(currentBranch == null) {
              // release lock and return
              _lock.Release();
              return;
            }
            
          }
          
          // add index to branch path
          branchPath[i] = currentBranch;
          
        } else {
          // target value doesn't exist
          // release lock and return
          _lock.Release();
          return;
        }
      }
      
      // key exists - remove its value
      currentBranch.Value = default(TValue);
      currentBranch.ValueSet = false;
      
      // if the current branch has no leaves and can be removed
      if(currentBranch.Leaves.Count == 0) {
        
        // special case if root has a single branch
        if(earliestRemoveIndex == 0) {
          
          _root.LeavesSet = false;
          _root.Leaves = null;
          
        } else {
          
          // if zero unrequired branches (other than the last)
          if(earliestRemoveIndex == -1) earliestRemoveIndex = keys.Length-1;
          
          // remove the last empty branch from its parent
          currentBranch = branchPath[earliestRemoveIndex];
          
          // remove the unneeded branch closest to the root branch
          branchPath[earliestRemoveIndex-1].Leaves.Remove(branchPath[earliestRemoveIndex].Key);
          
          // dispose
          currentBranch.Leaves.Dispose();
          currentBranch.Value = default(TValue);
          
          // null all child branches that are unrequired
          while(++earliestRemoveIndex < keys.Length) {
            branchPath[earliestRemoveIndex].Leaves.Dispose();
          }
        }
      }
      
      // release the lock
      _lock.Release();
    }
    
    /// <summary>
    /// Remove the value at the path of keys.
    /// This removes branches with no branches or values due
    /// to the target branch removal.
    /// </summary>
    public void Remove(TKey[] keys, int start, int end) {
      // get the lock
      _lock.Take();
      
      Branch currentBranch = _root;
      int earliestRemoveIndex = 0;
      Branch[] branchPath = new Branch[end - start];
      
      // iterate through keys in the branch to be set
      for(int i = start; i < end; ++i) {
        
        // if children not yet set
        if(currentBranch.LeavesSet) {
          
          // if the current branch has only one leaf
          if(currentBranch.Leaves.Count == 1) {
            
            // get the only branch
            currentBranch = currentBranch.Leaves.Array[0].ArgB;
            // does the branch have the key required
            if(currentBranch.Key.Equals(keys[i])) {
              
              // if branch index to remove hasn't been set
              if(currentBranch.ValueSet) {
                // unset the remove index
                earliestRemoveIndex = -1;
              } else {
                if(earliestRemoveIndex == -1) earliestRemoveIndex = i;
              }
              
            } else {
              // release lock and return
              _lock.Release();
              return;
            }
            
          } else {
            
            // unset the remove index
            earliestRemoveIndex = -1;
            
            // search for required key
            currentBranch = currentBranch.Leaves[keys[i]];
            
            // if the next branch doesn't exist
            if(currentBranch == null) {
              // release lock and return
              _lock.Release();
              return;
            }
            
          }
          
          // add index to branch path
          branchPath[i] = currentBranch;
          
        } else {
          // target value doesn't exist
          // release lock and return
          _lock.Release();
          return;
        }
      }
      
      // key exists - remove its value
      currentBranch.Value = default(TValue);
      currentBranch.ValueSet = false;
      
      // if the current branch has no leaves and can be removed
      if(currentBranch.Leaves.Count == 0) {
        
        // special case if root has a single branch 
        if(earliestRemoveIndex == 0) {
          
          _root.Leaves = null;
          _root.LeavesSet = false;
          
        } else {
          
          // if zero unrequired branches (other than the last)
          if(earliestRemoveIndex == -1) earliestRemoveIndex = end - start - 1;
          
          // remove the last empty branch from its parent
          currentBranch = branchPath[earliestRemoveIndex];
          
          // remove the unneeded branch closest to the root branch
          branchPath[earliestRemoveIndex-1].Leaves.Remove(branchPath[earliestRemoveIndex].Key);
          
          // dispose
          currentBranch.Leaves.Dispose();
          currentBranch.Value = default(TValue);
          
          // null all child branches that are unrequired
          while(++earliestRemoveIndex < keys.Length) {
            branchPath[earliestRemoveIndex].Leaves.Dispose();
          }
          
        }
      }
      
      // release the lock
      _lock.Release();
    }
    
    
    /// <summary>
    /// Remove the branch and sub-branches at the path of keys.
    /// This removes branches with no branches or values due
    /// to the target branch removal.
    /// </summary>
    public void RemoveBranch(TKey[] keys) {
      // get the lock
      _lock.Take();
      
      Branch currentBranch = _root;
      int earliestRemoveIndex = 0;
      Branch[] branchPath = new Branch[keys.Length];
      
      // iterate through keys in the branch to be set
      for(int i = 0, keyCount = keys.Length; i < keyCount; ++i) {
        
        // if leaves have been setup
        if(currentBranch.LeavesSet) {
          
          // if the current branch has only one leaf
          if(currentBranch.Leaves.Count == 1) {
            
            // get the only branch
            currentBranch = currentBranch.Leaves.Array[0].ArgB;
            // does the branch have the key required
            if(currentBranch.Key.Equals(keys[i])) {
              
              // if branch index to remove hasn't been set
              if(currentBranch.ValueSet) {
                // unset the remove index
                earliestRemoveIndex = -1;
              } else {
                if(earliestRemoveIndex == -1) earliestRemoveIndex = i;
              }
              
            } else {
              // if no matching key
              _lock.Release();
              return;
            }
            
          } else {
            
            // unset the remove index
            earliestRemoveIndex = -1;
            
            // check if leaf exists
            currentBranch = currentBranch.Leaves[keys[i]];
            
            // if no matching key
            if(currentBranch == null) {
              // release lock and return
              _lock.Release();
              return;
            }
            
          }
          
          // add index to branch path
          branchPath[i] = currentBranch;
          
        } else {
          // target value doesn't exist
          // release lock and return
          _lock.Release();
          return;
        }
      }
      
      // special case if root had a single branch
      if(earliestRemoveIndex == 0) {
        
        _root.Leaves = null;
        _root.LeavesSet = false;
        
      } else {
        
        // if zero unrequired branches (other than the last)
        if(earliestRemoveIndex == -1) earliestRemoveIndex = keys.Length-1;
        
        // remove the last empty branch from its parent
        currentBranch = branchPath[earliestRemoveIndex];
        
        // remove the unneeded branch closest to the root branch
        branchPath[earliestRemoveIndex-1].Leaves.Remove(branchPath[earliestRemoveIndex].Key);
        
        // dispose
        currentBranch.Leaves.Dispose();
        currentBranch.Value = default(TValue);
        
        // null all child branches that are unrequired
        while(++earliestRemoveIndex < keys.Length) {
          branchPath[earliestRemoveIndex].Leaves.Dispose();
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
      public ArrayRig<TValue> Values {
        get {
          return values;
        }
      }
      
      /// <summary>
      /// Current active branches in the search.
      /// </summary>
      public Queue<Branch> branches;
      
      /// <summary>
      /// The root branch used for this search.
      /// </summary>
      private Branch root;
      /// <summary>
      /// The matching branches values.
      /// </summary>
      private ArrayRig<TValue> values;
      
      /// <summary>
      /// Initialize a new dynamic search for the specified tree.
      /// </summary>
      public DynamicSearch(SnapSearch<TKey, TValue> _tree) {
        root = _tree._root;
        branches = new Queue<Branch>();
        values = new ArrayRig<TValue>();
      }
      
      /// <summary>
      /// Reset this dynamic search.
      /// </summary>
      public void Reset() {
        branches.Reset();
        values.Reset();
      }
      
      /// <summary>
      /// Move all current branches to the next search node that corresponds to the specified key.
      /// Returns if there is at least one value on the current branches.
      /// </summary>
      public bool Next(TKey key) {
        
        // skip if no branches
        if(!root.LeavesSet) return false;
        
        // reset the current values
        values.Reset();
        
        // get the branches to iterate
        int branchCount = branches.Count;
        
        // always check the root branch
        Branch leaf = root.Leaves[key];
        if(leaf != null) {
          branches.Enqueue(leaf);
        }
        
        // iterate through results of previous search
        while(--branchCount != -1) {
          
          // iterate branches to search
          branches.Dequeue();
          if(branches.Current.LeavesSet) {
            // check specified branch
            leaf = branches.Current.Leaves[key];
            if(leaf != null) {
              branches.Enqueue(leaf);
            }
          }
          
        }
        
        return values.Count != 0;
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
      public TValue Value { get { return current.Value; } }
      
      /// <summary>
      /// The current branch.
      /// </summary>
      protected Branch current;
      /// <summary>
      /// The root branch to reset to.
      /// </summary>
      protected Branch root;
      
      /// <summary>
      /// Initialize a new progressive search for the specified tree.
      /// </summary>
      public ProgressiveSearch(SnapSearch<TKey, TValue> tree) {
        current = root = tree._root;
      }
      
      /// <summary>
      /// Reset this progressive search instance.1
      /// </summary>
      public void Reset() {
        current = root;
      }
      
      /// <summary>
      /// Move to the next search node.
      /// </summary>
      public bool Next(TKey key) {
        if(current.LeavesSet) {
          current = current.Leaves[key];
          if(current != null) {
            return true;
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
      
      /// <summary>
      /// The key this branch represents.
      /// </summary>
      public TKey Key;
      
      /// <summary>
      /// Associated child branches.
      /// </summary>
      public SortedRig<TKey, Branch> Leaves;
      /// <summary>
      /// Whether the child branges been set.
      /// </summary>
      public bool LeavesSet;
      
      /// <summary>
      /// Searchable value of this branch.
      /// </summary>
      public TValue Value;
      /// <summary>
      /// Whether the value of this branch has been set.
      /// </summary>
      public bool ValueSet;
      
      //-------------------------------------------//
      
    }
    
  }
  
  
  
  
}

using System;

using Efz.Collections;

namespace Efz.Maths {
  
  /// <summary>
  /// A class to provide a simple method of finding the average
  /// value of a large data set. A certain number of values are
  /// added to a collection of which averaged once it reaches
  /// the 'batch size' the result of that calculation is added
  /// to a collection of average results which is used for the
  /// rolling average calculation.
  /// 
  /// Not threadsafe.
  /// </summary>
  public class Average {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Returns the average of every batch calculation.
    /// </summary>
    public double GetAverage {
      get {
        if(refresh) {
          refresh = false;
          Calculate();
        }
        return average;
      }
    }
    
    /// <summary>
    /// The number of items in each batch of calculations.
    /// </summary>
    public int BatchSize {
      set {
        batchSize = value;
        batch.SetCapacity(batchSize);
      }
    }
    
    /// <summary>
    /// The number of batches to keep at any one time.
    /// </summary>
    public int BatchCount {
      get {
        return batches.Count;
      }
      set {
        batchCount = value;
        batches.SetCapacity(batchCount);
        filled = batches.Count == batchCount;
        if(filled) {
          if(index > batches.Count) {
            index = 0;
          }
        }
      }
    }
    
    /// <summary>
    /// The weight of each batch value in the average calculation.
    /// </summary>
    public double BatchWeight {
      get {
        return batchWeight;
      }
      set {
        batchWeight = value;
        batchWeightSet = batchWeight != 1.0;
      }
    }
    
    //-------------------------------------------//
    
    protected ArrayRig<double> batch;
    protected ArrayRig<double> batches;
    protected int index;
    protected int batchSize;
    protected int batchCount;
    protected bool batchWeightSet;
    protected double batchWeight;
    
    protected double average;
    
    protected bool refresh;
    protected bool filled;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize with the functions necessary to perform calculations on a generic type.
    /// </summary>
    public Average(int _batchSize, int _batchNumber, double _batchWeight = 1.0) {
      batchSize   = _batchSize;
      batchCount = _batchNumber;
      BatchWeight = _batchWeight;
      batch   = new ArrayRig<double>(batchSize);
      batches = new ArrayRig<double>(batchCount);
    }
    
    /// <summary>
    /// Add an item to be considered for the average.
    /// </summary>
    virtual public void Add(double _item) {
      // add the item to the batch
      batch.Add(_item);
      if(batch.Count == batchSize) {
        // calculate the current batch average
        average = 0;
        foreach(double item in batch) {
          average += item;
        }
        average /= batch.Count;
        batch.Reset();
        if(filled) {
          // set a batch average
          batches[index] = average/batch.Count;
          ++index;
          if(index == batches.Count) index = 0;
        } else {
          // add a new batch average
          batches.Add(average/batch.Count);
          filled = batches.Count == batchCount;
        }
      }
      refresh = true;
    }
    
    //-------------------------------------------//
    
    virtual protected void Calculate() {
      average = 0;
      // add current batch
      foreach(double item in batch) {
        average += item;
      }
      // add previous batches, optionally with weights.
      if(batchWeightSet) {
        foreach(double item in batches) {
          average += item * batchWeight;
        }
        average /= (batch.Count + batches.Count * batchWeight);
      } else {
        foreach(double item in batches) {
          average += item;
        }
        average /= (batch.Count + batches.Count);
      }
    }
    
  }

}

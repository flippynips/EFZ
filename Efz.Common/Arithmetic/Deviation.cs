using System;

namespace Efz.Maths {
  
  /// <summary>
  /// A class to provide a simple method of finding a rolling
  /// standard deviation for a dataset.
  /// Not threadsafe.
  /// </summary>
  public class Deviation : Average {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Returns the average of every batch calculation.
    /// </summary>
    public double GetDeviation {
      get {
        if(refresh) {
          refresh = false;
          Calculate();
        }
        return average;
      }
    }
    
    //-------------------------------------------//
    
    protected double deviation;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize with the functions necessary to perform calculations on a generic type.
    /// </summary>
    public Deviation(int _batchSize, int _batchNumber, double _batchWeight = 1.0) : base(_batchSize, _batchNumber, _batchWeight) {
    }
    
    /// <summary>
    /// Add an item to be considered for the average.
    /// </summary>
    override public void Add(double _item) {
      // add the item to the batch
      batch.Add(_item);
      if(batch.Count == batchSize) {
        // get the average of the complete batch
        average = 0;
        foreach(double item in batch) {
          average += item;
        }
        average /= batch.Count;
        
        // calculate the deviation of the complete batch
        deviation = 0;
        foreach(double item in batch) {
          deviation += Meth.Square(item - average);
        }
        deviation = Math.Sqrt(deviation/(batch.Count-1));
        
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
    
    override protected void Calculate() {
      average = 0;
      // add items from current unfinished batch
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
      // calculate the deviation
      deviation = 0;
      foreach(double item in batch) {
        deviation += Meth.Square(item - average);
      }
      foreach(double item in batches) {
        deviation += Meth.Square(item - average);
      }
      deviation = Math.Sqrt(deviation/(batch.Count + batches.Count-1));
    }
    
  }

}

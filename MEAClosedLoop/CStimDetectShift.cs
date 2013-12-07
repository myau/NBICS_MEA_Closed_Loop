using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MEAClosedLoop
{

  using TRawData = UInt16;
  using TData = Double;
  using TStimIndex = System.Int16;
  using TTime = System.UInt64;

  public class CStimDetectShift
  {
    public const int ErrorState = -3303;
    private const TStimIndex FILTER_DEPTH = 16;
    private const TStimIndex default_offset = 8;
    private const TStimIndex start_offset = 16;
    private const TRawData Defaul_Zero_Point = 32768; 
    private List<TStimGroup> m_expectedStims;
    private int MissedStimsCount; // how many stims wasn't found at preview Data Pocket
    private Int32[] FullData; //Includes CurrentPacket & PreViewsPacket
    private Int32[] PreViewData;// temporary Solution, includes PreView Data Pocket
    private int CallCount;
    private object LockStimList = new object();
    TestGraph GrafForm;
    //Way Nums:
    //1:Deault, Find all pegs by using ExpectedStims
    //2: Get all pegs by Abstract reserch
    public void SetExpectedStims(TStimGroup StimGroupToAdd)
    {
      lock (LockStimList) 
      {

      }
    }
    public bool IsStimulusExpected(TTime TestimgTime)
    {
      bool Flag = false;
      lock (LockStimList)
      {
      }
      return Flag;
    }
    public CStimDetectShift()
    {
      FullData = null;
      CallCount = 0;
      PreViewData = new Int32[0];
      GrafForm = new TestGraph();
      GrafForm.Show();
      // TODO: Complete member initialization
    }
    public void ClearBuff()
    {
      FullData = null;
      PreViewData = new Int32[0];
    }
    public List<TStimIndex> GetStims(TRawData[] DataPacket, TRawData[] PrevDataPacket, TStimGroup ExpectedStim)
    {
      #region FullData Array Prepearing

      //Old to new Massiv Copy
      FullData = new Int32[PreViewData.Length + DataPacket.Length];
      for (int i = 0; i < PreViewData.Length; i++)
      {
        FullData[i] = PreViewData[i];
      }
      //Input to new massive copy
      for (int i = PreViewData.Length; i < PreViewData.Length + DataPacket.Length; i++)
      {
        FullData[i] = -Defaul_Zero_Point + DataPacket[i - PreViewData.Length];
      }
      #endregion
      CallCount++;
      List<TStimIndex> FoundPegs = new List<TStimIndex>();
      
      return GetStims(DataPacket);

      #region Temporary Solution to use expected stims 
      int ValidateCount = 0;
      // Process current packet
      int DataPacketLength = DataPacket.Length - FILTER_DEPTH - 1;

      
      //[TOTO] Оптимизировать цикл по Expected Stims

      for (TStimIndex i = default_offset; i < DataPacketLength; i++)
      {
        if (TrueValidateSingleStimInT(i, default_offset))
        {
          ValidateCount++;
          FoundPegs.Add(i);
        }
      }
      MissedStimsCount = ExpectedStim.count - ValidateCount;
      return FoundPegs;
      #endregion
    }

    public List<TStimIndex> GetStims(TRawData[] DataPacket)
    {
      List<TStimIndex> FindedPegs = new List<TStimIndex>();
      List<TStimIndex> ErrorList = new List<TStimIndex> { 0 };
      int ValidateCount = 0;
      int FirstUseIndex = 0;
      if (PreViewData.Length == 0) FirstUseIndex = 1;
      /////
      GrafForm.DrawRawData(DataPacket, "packet № " + CallCount.ToString());
      ////
      for (short i = (short)(PreViewData.Length + FirstUseIndex*start_offset); i < FullData.LongLength - FILTER_DEPTH; i++)
      {
        if (TrueValidateSingleStimInT(i, default_offset))
        //if (BasicValidateSingleStimInT(i))
        {
          ValidateCount++;
          FindedPegs.Add(i);
        }
      }
      // ноль - временно для настройки поиска только в одном пакете
      MissedStimsCount = 0;//  ExpectedStim.count - ValidateCount;

      #region Previous Data Array Prepearing 
      //Input to old massive copy
      PreViewData = new Int32[DataPacket.Length];
      for (int i = 0; i < DataPacket.Length; i++)
      {
        PreViewData[i] = - Defaul_Zero_Point + DataPacket[i];
      }
      #endregion
      return FindedPegs;
    }
    private bool BasicValidateSingleStimInT(long t)
    {
      if (FullData[t + 1] - FullData[t] > 65 &&
          FullData[t + 2] - FullData[t + 1] > 55 &&
          FullData[t + 3] - FullData[t + 2] > 35 &&
          FullData[t + 4] - FullData[t + 3] > 30)
      {
        return true;
      }
      else
      {
        return false;
      }

    }
    private bool TrueValidateSingleStimInT(long t, int maximum_offset)
    {

      try
      {
        Average pre_average = new Average();
        Average post_average = new Average();

        for (int i = 0; i < maximum_offset; i++)
        {
          pre_average.AddValueElem(this.FullData[t - i]);
        }
        for (int i = 0; i < FILTER_DEPTH; i++)
        {
          post_average.AddValueElem(this.FullData[t + i]);
        }

        pre_average.Calc();
        post_average.Calc();
        GrafForm.DrawSigma(t - maximum_offset, t, (float) pre_average.Value, (float)pre_average.TripleSigma);
        if (pre_average.IsInArea(FullData[t - 1])
          && pre_average.IsInArea(FullData[t - 2])
          && !pre_average.IsInArea(FullData[t])
          && !pre_average.IsInArea(FullData[t + 1])
          && !pre_average.IsInArea(post_average.Value - post_average.Sigma)
          && !pre_average.IsInArea(post_average.Value + post_average.Sigma)
          ) return true;
        return false;
      }
      catch (Exception e)
      {
        return false;
      }

    }

  }
  class Average
  {
    private List<int> values;
    public double Value;
    public double TripleSigma;
    public double Sigma;
    public void AddValueElem(int ValueToAdd)
    {
      values.Add(ValueToAdd);
    }
    public void Calc()
    {
      double summ = 0;
      for (int i = 0; i < values.Count; i++)
      {
        summ += values[i];
      }
      Value = summ / values.Count;
      double QuarteSumm = 0;
      for (int i = 0; i < values.Count; i++)
      {
        QuarteSumm += Math.Pow(Value - values[i], 2);
      }
      Sigma = Math.Sqrt(QuarteSumm / values.Count);
      TripleSigma = 3.5 * Sigma;
    }
    public bool IsInArea(double Value)
    {
      this.Calc();
      if (Math.Abs(Value - Value) < TripleSigma) return true;
      return false;
    }
    public Average()
    {
      values = new List<int>();
      Value = 0;
      TripleSigma = 0;
    }
  }
}

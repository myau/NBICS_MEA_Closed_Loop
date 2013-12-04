using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MEAClosedLoop
{

  using TRawData = UInt16;
  using TData = Double;
  using TStimIndex = System.Int16;

  public class CStimDetectShift
  {
    public const int ErrorState = -3303;
    private const TStimIndex FILTER_DEPTH = 8;
    private const TStimIndex default_offset = 8;
    private int shift;
    private int DoubleStimPeriod;
    private int sigma;
    private int AVD;// Average value of Derivative
    private const double MinDRatio = .5;// Min ration between AVG & Current Derivative  
    private Average Current;
    private Average Previews;
    private List<int> ExpectedStims;
    private int WayNum;
    private TRawData[] F;
    private List<TStimGroup> m_expectedStims;
    private int MissedStimsCount; // how many stims wasn't found at preview Data Pocket
    private TRawData[] m_prevDataPoints;

    //Way Nums:
    //1:Deault, Find all pegs by using ExpectedStims
    //2: Get all pegs by Abstract reserch

    public CStimDetectShift()
    {
      // TODO: Complete member initialization
      m_prevDataPoints = new TRawData[6];
    }
    public List<TStimIndex> GetStims(TRawData[] DataPacket, TStimGroup ExpectedStim)
    {
      List<TStimIndex> FoundPegs = new List<TStimIndex>();
      //List<TStimIndex> ErrorList = new List<TStimIndex> { 0 };
      int ValidateCount = 0;
      #region strange
      // Process last FILTER_DEPTH points of the previous packet
      this.F = m_prevDataPoints;

      // [TODO] Миша, если тебе нужен этот switch по WayNum, повтори его тут сам
      /*
        for (TStimIndex i = 0; i < FILTER_DEPTH; i++)
      {
        m_prevDataPoints[FILTER_DEPTH + i] = DataPacket[i];
        if (BasicValidateSingleStimInT(i))
        {
          ValidateCount++;
          FoundPegs.Add((TStimIndex)(i - FILTER_DEPTH));
        }
      }
       * */
      #endregion
      // Process current packet
      this.F = DataPacket;

      WayNum = 2;
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

      for (int i = 0; i < FILTER_DEPTH; i++)
      {
        //m_prevDataPoints[i] = DataPacket[DataPacketLength + i];
      }

      return FoundPegs;
      //If all is realy bad;
      //if(ValidateCount == 0) return null;
    }

    public List<TStimIndex> GetStims(TRawData[] DataPacket)
    {
      List<TStimIndex> FindedPegs = new List<TStimIndex>();
      List<TStimIndex> ErrorList = new List<TStimIndex> { 0 };
      this.F = DataPacket;
      int ValidateCount = 0;

      //TODO find all by hard research;
      //Opimization cycle
      int DataPacketLength = DataPacket.Length - 4;
      //EndOpimization

      for (Int16 i = 0; i < DataPacketLength; i++)
      {
        if (TrueValidateSingleStimInT(i, default_offset))
        {
          ValidateCount++;
          FindedPegs.Add(i);
        }
      }

      //MissedStimsCount = ExpectedStim.count - ValidateCount;


      return FindedPegs;
    }
    private bool BasicValidateSingleStimInT(long t)
    {
      switch (WayNum)
      {
        case 1:
          break;
        case 2:
          if (F[t + 1] - F[t] > 45 &&
              F[t + 2] - F[t + 1] > 45 &&
              F[t + 3] - F[t + 2] > 45)
          {
            return true;
          }
          else
          {
            return false;
          }
      }
      return false;
    }
    private bool TrueValidateSingleStimInT(long t, int maximum_offset)
    {

      try
      {
        Average pre_average = new Average();
        Average post_average = new Average();

        for (int i = 0; i < maximum_offset; i++)
        {
          pre_average.AddValueElem(this.F[t - i]);
        }
        for (int i = 0; i < FILTER_DEPTH; i++)
        {
          post_average.AddValueElem(this.F[t + i]);
        }

        pre_average.Calc();
        post_average.Calc();
        if (pre_average.IsInArea(F[t - 1])
          && pre_average.IsInArea(F[t - 2])
          && !pre_average.IsInArea(F[t])
          && !pre_average.IsInArea(F[t + 1])
          && pre_average.IsInArea(post_average.Value - post_average.Sigma)
          && pre_average.IsInArea(post_average.Value + post_average.Sigma)
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

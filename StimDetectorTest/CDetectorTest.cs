﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Neurorighter;
using Common;
using MEAClosedLoop;


namespace StimDetectorTest
{
  using TData = System.Double;
  using TTime = System.UInt64;
  using TStimIndex = System.Int16;
  using TRawDataPacket = Dictionary<int, ushort[]>;
  using StimuliList = List<TStimGroup>;
  using MSTime = UInt64;

  public class CDetectorTest
  {
    const int DEFAULT_VALUE = 32767;
    const int MIN_PACKET_SIZE = 150;
    const int BLOCK_SIZE = 2500;

    private CInputStream m_inputStream;
    private TRawDataPacket m_prevPacket;
    //private CStimDetector m_stimDetector;
    private int m_artifChannel;
    private StimuliList m_expectedStims;
    public List<TStimIndex> m_stimIndices;
    private TStimGroup m_nextExpectedStim;
    private OnStreamKillDelegate m_onStreamKill = null;
    public OnStreamKillDelegate OnStreamKill { set { m_onStreamKill = value; } }

    private Int64 m_squareError;
    private CStimDetectShift m_stimDetector;
    private volatile bool m_kill;

    private bool m_CalmMode;


    public CDetectorTest(string fileName, StimuliList sl)
    {
      List<int> channelList = new List<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59 });

      m_inputStream = new CInputStream(fileName, channelList, BLOCK_SIZE);

      m_inputStream.OnStreamKill = Dismiss;
      m_inputStream.ConsumerList.Add(ReceiveData);


      m_stimIndices = new List<TStimIndex>();

      //m_stimDetector = new CStimDetector(15, 35, 150); //old
      m_stimDetector = new CStimDetectShift();
      m_artifChannel = m_inputStream.ChannelList[0];

      if (sl != null)
      {
        m_CalmMode = false;
        m_expectedStims = sl;
        m_nextExpectedStim = sl[0];
        sl.RemoveAt(0);
      }
      else m_CalmMode = true;
      m_kill = false;
    }

    public void ReceiveData(TRawDataPacket currPacket)  //TODO: make absolute stimIndices
    {
      TTime endOfPacket = m_inputStream.TimeStamp + (TTime)currPacket[currPacket.Keys.Min()].Length;

      if (m_nextExpectedStim.stimTime < endOfPacket)
      {
        if (m_CalmMode) m_stimIndices.AddRange(m_stimDetector.GetStims(currPacket[m_artifChannel]));
        else
        {
          m_stimIndices.AddRange(m_stimDetector.GetStims(currPacket[m_artifChannel], m_nextExpectedStim)); //old version //new vers. used like old
          m_nextExpectedStim = m_expectedStims[0];
          m_expectedStims.RemoveAt(0);
        }
      }
      else
      {
        m_stimIndices.AddRange(m_stimDetector.GetStims(currPacket[m_artifChannel]));
      }

      // Calculate error
      // m_squareError += error;

      m_prevPacket = currPacket;
    }

    public UInt64 RunTest(List<TStimIndex> realStimIndices)
    {
      MSTime squareError = 0; //not really square


      m_inputStream.Start();
      m_inputStream.WaitEOF();

      //comparing m_stimIndices with realStimIndices

      if (realStimIndices == null)
      {
        return Convert.ToUInt64(m_stimIndices.Count());
      }
      else
      {
        for (int i = 0; i < realStimIndices.Count(); i++)
        {
          squareError += Helpers.Int2Time(Convert.ToUInt64(Math.Abs(m_stimIndices[i] - realStimIndices[i])));
        }
      }

      return squareError;
      //return 0;
    }
    private void Dismiss()
    {
      m_kill = true;
      if (m_onStreamKill != null) m_onStreamKill();
    }
  }
}

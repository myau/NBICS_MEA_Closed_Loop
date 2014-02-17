using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using MEAClosedLoop;
using System.Threading;
namespace MEAClosedLoop
{
  #region Definitions
  using TData = System.Double;
  using TTime = System.UInt64;
  using TStimIndex = System.Int16;
  using TAbsStimIndex = System.UInt64;
  using TRawData = UInt16;
  using TRawDataPacket = Dictionary<int, ushort[]>;
  using TFltDataPacket = Dictionary<int, System.Double[]>;
  #endregion
  public class CStatCalc
  {
    public void StatCalcConsumer(TFltDataPacket data) //it works!
    {
      //TODO: calc stat here
      
      //TODO: The great logic will be here
    }
  }
}

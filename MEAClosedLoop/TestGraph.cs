using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MEAClosedLoop;
namespace MEAClosedLoop
{
  public partial class TestGraph : Form
  {
    public TestGraph()
    {
      InitializeComponent();
    }
    public void DrawRawData( UInt16[] DataPacket)
    {
      Graphics DataPlot = pictureBox1.CreateGraphics();
      Pen RedPen = new Pen(Color.Red);
      for(int i = 0; i < DataPacket.Length; i++)
      {
        DataPlot.DrawLine(RedPen,
          (int)(DataPacket[i]),
          (int)(i),
          (int)(DataPacket[i]),
          (int)(i)
          );
      }
    }
    private void TestGraph_Load(object sender, EventArgs e)
    {

    }
  }
}

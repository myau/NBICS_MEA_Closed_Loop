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
    UInt16[] DataPacket;
    string Label;
    public TestGraph()
    {
      InitializeComponent();

    }
    public void DrawRawData(UInt16[] _DataPacket, string PacketHeader)
    {
      DataPacket = _DataPacket;
      Label = PacketHeader;
      Pen RedPen = new Pen(Color.Red, 1);
      Graphics Gr = pictureBox1.CreateGraphics();
      Gr.Clear(Color.White);
      pictureBox1.Invalidate();

      //e.Graphics.DrawLine(RedPen, 0, 0, 500, 500);
      /*
      for (int i = 0; i < DataPacket.Length - 1; i++)
      {
        Gr.DrawLine(RedPen,
          (int)(i *  pictureBox1.Width/DataPacket.Length),
          (int)(((DataPacket[i]) - 32768) / 3 + pictureBox1.Height / 2),
          (int)((i + 1) * pictureBox1.Width / DataPacket.Length),
          (int)(((DataPacket[i + 1]) - 32768) / 3 + pictureBox1.Height / 2)
          );
      }
      */
      //pictureBox1_Paint(this, );
      //pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
      //this.Controls.Add(pictureBox1);
    }
    public void DrawSigma(float x1, float x2, float AverValue, float sigma)
    {
      Graphics Gr = pictureBox1.CreateGraphics();
      Pen GreenPen = new Pen(Color.Green, 1);
      Pen BluePen = new Pen(Color.Blue, 1);
      Gr.DrawLine(GreenPen, 
        x1 * pictureBox1.Width / DataPacket.Length,
        (AverValue - 32768) / 3 + pictureBox1.Height / 2, 
        x2 * pictureBox1.Width / DataPacket.Length, 
        (AverValue - 32768) / 3 + pictureBox1.Height / 2);
      //Gr.Dispose();
      //Gr.DrawLine(BluePen, x1, AverValue + sigma, x2, AverValue + sigma);
    }
    private void TestGraph_Load(object sender, EventArgs e)
    {

    }
    private void pictureBox1_Click(object sender, EventArgs e)
    {

    }
    private void pictureBox1_Paint(object sender, PaintEventArgs e)
    {
      //Graphics DataPlot = pictureBox1.CreateGraphics();
      Pen RedPen = new Pen(Color.Red, 1);
      //e.Graphics.DrawLine(RedPen, 0, 0, 500, 500);
      e.Graphics.DrawString(Label, new System.Drawing.Font("Times New Roman", 12, FontStyle.Bold), new SolidBrush(Color.LimeGreen), 20, 20, new StringFormat());
      for (int i = 0; i < DataPacket.Length - 1; i++)
      {
        e.Graphics.DrawLine(RedPen,
          (int)(i * pictureBox1.Width / DataPacket.Length),
          (int)(((DataPacket[i]) - 32768) / 3 + pictureBox1.Height / 2),
          (int)((i + 1) * pictureBox1.Width / DataPacket.Length),
          (int)(((DataPacket[i + 1]) - 32768) / 3 + pictureBox1.Height / 2)
          );
      }

    }

    private void button1_Click(object sender, EventArgs e)
    {
      this.Close();
    }
  }
}

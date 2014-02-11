using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using MEAClosedLoop;
using System.Windows;
namespace MEAClosedLoop
{
  #region Definitions
  using TData = System.Double;
  using TTime = System.UInt64;
  using TStimIndex = System.Int16;
  using TAbsStimIndex = System.UInt64;
  using TRawData = UInt16;
  using TRawDataPacket = Dictionary<int, ushort[]>;
  #endregion
  public class CGraphRender : Microsoft.Xna.Framework.Game
  {
    GraphicsDeviceManager graphics;
    public TRawData[] DataPacket;
    private CStimDetectShift detector;
    // эффект BasicEffect
    BasicEffect basicEffect;
    // массив вершин
    VertexPositionColor[] vertices;

    public void SetData(TRawData[] datapacket)
    {
      DataPacket = datapacket;
    }
    public void SetDataObj(CStimDetectShift obj)
    {
      detector = obj;
    }
    public CGraphRender()
    {
      graphics = new GraphicsDeviceManager(this);
      graphics.PreferredBackBufferWidth = 1600; // ширина приложения
      graphics.PreferredBackBufferHeight = 1000; // высота приложения
      graphics.IsFullScreen = false; // флаг полноэкранного приложения
      graphics.ApplyChanges(); // применяем параметры
      Content.RootDirectory = "Content";
    }
    protected override void Initialize()
    {
      basicEffect = new BasicEffect(graphics.GraphicsDevice);
      basicEffect.VertexColorEnabled = true;
      basicEffect.Projection = Matrix.CreateOrthographicOffCenter
         (0, graphics.GraphicsDevice.Viewport.Width,     // left, right
          graphics.GraphicsDevice.Viewport.Height, 0,    // bottom, top
          0, 1);                                         // near, far plane

      if (detector.inner_data_to_display != null)
      {
        vertices = new VertexPositionColor[detector.inner_data_to_display.Length];
        for (int i = 0; i < detector.inner_data_to_display.Length; i++)
        {
          vertices[i].Position = new Vector3(i /2, (detector.inner_data_to_display[i] - 32768)/6  + 500, 0);
          vertices[i].Color = Color.Black;
        }
      }
      // TODO: Add your initialization logic here
      base.Initialize();
    }

    protected override void LoadContent()
    {
      // TODO: use this.Content to load your game content here
      // создать массив-контейнер для хранения трёх вершин

    }

    protected override void UnloadContent()
    {
      // TODO: Unload any non ContentManager content here
    }

    protected override void Update(GameTime gameTime)
    {
      // Allows the game to exit
      if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
        this.Exit();

      // TODO: Add your update logic here

      base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
      this.Initialize();
      if (detector.inner_data_to_display != null)
      {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        basicEffect.CurrentTechnique.Passes[0].Apply();
        try
        {
          graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, vertices, 0, detector.inner_data_to_display.Length - 1);
        }
        catch(ArgumentNullException ex)
        {
          System.Windows.Forms.MessageBox.Show(ex.Message);
        }
        base.Draw(gameTime);
      }
    }
  }
}



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

    // эффект BasicEffect
    BasicEffect basicEffect;
    // массив вершин
    VertexPositionColor[] vertices;
    // описание формата вершин
    VertexDeclaration vertexDeclaration;



    public void SetData(TRawData[] datapacket)
    {
      DataPacket = datapacket;
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
      /*
      vertices = new VertexPositionColor[4];
      vertices[0].Position = new Vector3(100, 100, 0);
      vertices[0].Color = Color.Black;
      vertices[1].Position = new Vector3(200, 100, 0);
      vertices[1].Color = Color.Red;
      vertices[2].Position = new Vector3(200, 200, 0);
      vertices[2].Color = Color.Black;
      vertices[3].Position = new Vector3(100, 200, 0);
      vertices[3].Color = Color.Red;
       */
      if (DataPacket != null)
      {
        vertices = new VertexPositionColor[DataPacket.Length];
        for (int i = 0; i < DataPacket.Length; i++)
        {
          vertices[i].Position = new Vector3(i /2, (DataPacket[i] - 32768)/100  + 100, 0);
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
      if (DataPacket != null)
      {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        basicEffect.CurrentTechnique.Passes[0].Apply();
        graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, vertices, 0, DataPacket.Length - 1);

        base.Draw(gameTime);
      }
    }
  }
}



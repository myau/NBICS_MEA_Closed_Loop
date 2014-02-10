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

    // массив вершин
    VertexPositionColor[] vertexList;

    // описание формата вершин
    VertexDeclaration vertexDeclaration;

    // эффект BasicEffect
    BasicEffect effect;

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
        for (int i = 0; i < DataPacket.Length - 1; i++)
        {
          VertexPositionColor[] line = new VertexPositionColor[2];
          VertexBuffer vertexBuffer;
          BasicEffect effect;

          line[0] = new VertexPositionColor(new Vector3((float)i / 1250 - 1, (float)DataPacket[i] / 32000 - 1, 0), Color.White);          // юмора не понял, но координаты - относительные
          line[1] = new VertexPositionColor(new Vector3((float)(i + 1) / 1250 - 1,(float) DataPacket[i + 1] / 32000 - 1, 0), Color.White);         // 1 - это край экрана, 0 - центр
          vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), line.Length, BufferUsage.None);
          vertexBuffer.SetData(line);
          effect = new BasicEffect(GraphicsDevice);
          GraphicsDevice.SetVertexBuffer(vertexBuffer);

          foreach (EffectPass pass in effect.CurrentTechnique.Passes)
          {
            pass.Apply();
            GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, line, 0, 1);
          }

          base.Draw(gameTime);
        }
      }
    }
  }
}



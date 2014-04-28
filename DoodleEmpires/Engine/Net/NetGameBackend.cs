using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DoodleEmpires.Engine.Utilities;
using DoodleEmpires.Engine.Terrain;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DoodleEmpires.Engine.Entities;
using Microsoft.Xna.Framework.Input;

namespace DoodleEmpires.Engine.Net
{
    public class NetGameBackend : SPGame
    {
        NetGame _netGame;
        PlayerInfo _playerInfo;

        public NetGameBackend()
            : base()
        {

        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _tileManager = new TileManager();
            _tileManager.RegisterTile("Grass", 0, Color.Green, RenderType.Land, true);
            _tileManager.RegisterTile("Stone", 0, Color.Gray, RenderType.Land, true);
            _tileManager.RegisterTile("Concrete", 0, Color.LightGray, RenderType.Land, true);
            _tileManager.RegisterTile("Wood", 20, Color.Brown, RenderType.Land, false);
            _tileManager.RegisterTile("Leaves", new Leaves(0));
            _tileManager.RegisterTile("Cobble", 60, Color.Gray, RenderType.Land, true);
            _tileManager.RegisterTile("Wooden Spikes", new WoodSpike(0));
            _tileManager.RegisterTile("Ladder", new Ladder(0));
            _tileManager.RegisterTile("Door", new Door(0));

            _tileManager.RegisterConnect("Grass", "Stone");
            _tileManager.RegisterConnect("Grass", "Concrete");
            _tileManager.RegisterConnect("Stone", "Concrete");
            _tileManager.RegisterConnect("Wood", "Grass");
            _tileManager.RegisterConnect("Wood", "Leaves");

            _tileManager.RegisterOneWayConnect("Ladder", "Door");
            _tileManager.RegisterOneWayConnect("Ladder", "Wood");
            _tileManager.RegisterOneWayConnect("Ladder", "Concrete");
            _tileManager.RegisterOneWayConnect("Ladder", "Stone");

            _blockAtlas = new TextureAtlas(Content.Load<Texture2D>("Atlas"), 20, 20);

            _netGame = new NetGame(GraphicsDevice, _tileManager, _blockAtlas, _playerInfo.UserName, null, null);

            _netGame.OnJoinedServer += _netGame_OnJoinedServer;


            _view = new Camera2D(GraphicsDevice);

            _cameraController = new CameraControl(_view);
            _cameraController.Position = new Vector2(0, 200 * SPMap.TILE_HEIGHT);

            _view.Focus = _cameraController;

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            switch (_gameState)
            {
                case GameState.InGame:
                    _netGame.Draw(_view);
                    break;
            }
        }

        protected override void MouseDown(MouseEventArgs args)
        {
            switch (_gameState)
            {
                case GameState.InGame:
                    if (args.LeftButton == ButtonState.Pressed)
                    {
                        Vector2 pos = _view.PointToWorld(args.Location);
                        _netGame.RequestBlockChange((int)pos.X / SPMap.TILE_WIDTH, (int)pos.Y / SPMap.TILE_HEIGHT, _editType);
                    }
                    break;
            }
        }

        void _netGame_OnJoinedServer(ServerInfo info)
        {
            _gameState = GameState.InGame;
            _view.ScreenBounds = new Rectangle(0, 0, _netGame.WorldWidth, _netGame.WorldHeight);
        }
    }
}

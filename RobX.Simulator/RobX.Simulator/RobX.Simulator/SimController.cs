# region Includes

using System;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

# endregion

namespace RobX.Simulator
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class SimController : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch Frame;

        Simulator Simulator;

        Form RedundantForm;
        IntPtr DrawingSurface;
        Form SimulatorForm;
        PictureBox SimulatorPictureBox;

        int draw_total_frames = 0;
        float draw_elapsed_time = 0.0f;
        int update_total_count = 0;
        float update_elapsed_time = 0.0f;
        public static int DrawFPS = 0;
        public static int UpdateRate = 0;

        public SimController(IntPtr DrawingSurface, frmSimulator SimulatorForm, PictureBox SimulatorPictureBox)
        {
            Simulator = SimulatorForm.Simulator;

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.DrawingSurface = DrawingSurface;
            this.SimulatorForm = SimulatorForm;
            this.SimulatorPictureBox = SimulatorPictureBox;

            // prepare graphics event
            graphics.PreparingDeviceSettings += graphics_PreparingDeviceSettings;

            RedundantForm = (Form)Control.FromHandle(Window.Handle);
            RedundantForm.VisibleChanged += RedundantForm_VisiblilityChanged;

            //Tell the mouse it will be getting it's input through the pictureBox
            Mouse.WindowHandle = DrawingSurface;

            TargetElapsedTime = TimeSpan.FromSeconds(1.0f / Properties.Settings.Default.UpdateRate);

            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = SimulatorPictureBox.ClientSize.Width;
            graphics.PreferredBackBufferHeight = SimulatorPictureBox.ClientSize.Height;
        }

        private void RedundantForm_VisiblilityChanged(object sender, EventArgs e)
        {
            if (RedundantForm.Visible)
                RedundantForm.Visible = false;
        }

        private void graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            // Finally attach draw ability to the picture box in winforms.
            e.GraphicsDeviceInformation.PresentationParameters.DeviceWindowHandle = DrawingSurface;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            GraphicsEngine.Initialize(GraphicsDevice, Content.Load<SpriteFont>("Grid"),
                Content.Load<SpriteFont>("Statistics"), Content.Load<SpriteFont>("SimulationSpeed"));
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            Frame = new SpriteBatch(GraphicsDevice);

            Simulator.Robot.Image = new Texture2D[360];

            for (var i = 0; i < Simulator.Robot.Image.Length; ++i)
                Simulator.Robot.Image[i] = Content.Load<Texture2D>("RobotImages\\Robot" + i);
        }

        protected override void UnloadContent() { }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                Exit();

            update_elapsed_time += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            update_total_count++;

            // 1 Second has passed
            if (update_elapsed_time >= 1000.0f)
            {
                UpdateRate = update_total_count;
                update_total_count = 0;
                update_elapsed_time = 0;
            }

            Simulator.StepSimulation();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            draw_elapsed_time += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            draw_total_frames++;
            // 1 Second has passed
            if (draw_elapsed_time >= 1000.0f)
            {
                DrawFPS = draw_total_frames;
                draw_total_frames = 0;
                draw_elapsed_time = 0;
            }

            // Check if screen dimensions are changed (simulation form is resized)
            try
            {
                if (graphics.PreferredBackBufferWidth != SimulatorPictureBox.ClientSize.Width ||
                graphics.PreferredBackBufferHeight != SimulatorPictureBox.ClientSize.Height)
                {
                    graphics.PreferredBackBufferWidth = SimulatorPictureBox.ClientSize.Width;
                    graphics.PreferredBackBufferHeight = SimulatorPictureBox.ClientSize.Height;
                    graphics.ApplyChanges();
                }
            }
            catch { }

            GraphicsDevice.Clear(Color.CornflowerBlue);
            Frame.Begin();

            var ElapsedTime = TimeSpan.Zero;
            if (Simulator.IsRunning)
                ElapsedTime = gameTime.ElapsedGameTime;

            Simulator.Drawer.Render(ref Frame, SimulatorPictureBox.ClientSize.Width, SimulatorPictureBox.ClientSize.Height,
                ref Simulator.Environment, Simulator.Robot, ElapsedTime, Simulator.Render.Type);
            
            Frame.End();
            
            base.Draw(gameTime);
        }
    }
}

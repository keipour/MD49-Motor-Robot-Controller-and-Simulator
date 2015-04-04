# region Includes

using System;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RobX.Simulator.Properties;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;

# endregion

namespace RobX.Simulator
{
    /// <summary>
    /// This class manages all the simulation aspects (update, draw, etc.).
    /// </summary>
    public class SimController : Game
    {
        # region Private Fields

        readonly GraphicsDeviceManager _graphics;
        SpriteBatch _frame;

        readonly Simulator _simulator;

        readonly Form _redundantForm;
        readonly IntPtr _drawingSurface;
        readonly PictureBox _simulatorPictureBox;

        int _drawTotalFrames;
        float _drawElapsedTime;
        int _updateTotalCount;
        float _updateElapsedTime;

        # endregion

        # region Public Fields

        /// <summary>
        /// Number of drawn frames per second.
        /// </summary>
        public static int DrawFps;

        /// <summary>
        /// The update rate (times per second). 
        /// </summary>
        public static int UpdateRate;

        # endregion

        # region Constructor

        /// <summary>
        /// Constructor for the SimController class.
        /// </summary>
        /// <param name="drawingSurface">Pointer to the drawing suface.</param>
        /// <param name="simulatorPictureBox">PictureBox control that is used for drawing of the simulation frames.</param>
        /// <param name="simulator">Simulator class instance.</param>
        public SimController(IntPtr drawingSurface, PictureBox simulatorPictureBox, Simulator simulator)
        {
            _simulator = simulator;

            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            _drawingSurface = drawingSurface;
            _simulatorPictureBox = simulatorPictureBox;

            // prepare graphics event
            _graphics.PreparingDeviceSettings += graphics_PreparingDeviceSettings;

            _redundantForm = (Form)Control.FromHandle(Window.Handle);
            _redundantForm.VisibleChanged += RedundantForm_VisiblilityChanged;

            //Tell the mouse it will be getting it's input through the pictureBox
            Mouse.WindowHandle = drawingSurface;

            TargetElapsedTime = TimeSpan.FromSeconds(1.0f / Settings.Default.UpdateRate);

            _graphics.IsFullScreen = false;
            _graphics.PreferredBackBufferWidth = simulatorPictureBox.ClientSize.Width;
            _graphics.PreferredBackBufferHeight = simulatorPictureBox.ClientSize.Height;
        }

        # endregion

        # region Private and Protected XNA-related Methods

        private void RedundantForm_VisiblilityChanged(object sender, EventArgs e)
        {
            if (_redundantForm.Visible)
                _redundantForm.Visible = false;
        }

        private void graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            // Finally attach draw ability to the picture box in winforms.
            e.GraphicsDeviceInformation.PresentationParameters.DeviceWindowHandle = _drawingSurface;
        }

        /// <summary>
        /// Allows the simulation to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content. Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            GraphicsEngine.Initialize(GraphicsDevice, Content.Load<SpriteFont>("Grid"),
                Content.Load<SpriteFont>("Statistics"), Content.Load<SpriteFont>("SimulationSpeed"));
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per simulation and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _frame = new SpriteBatch(GraphicsDevice);

            _simulator.Robot.Image = new Texture2D[360];

            for (var i = 0; i < _simulator.Robot.Image.Length; ++i)
                _simulator.Robot.Image[i] = Content.Load<Texture2D>("RobotImages\\Robot" + i);
        }

        /// <summary>
        /// In this method all the unloading for after the simulation is perormed.
        /// </summary>
        protected override void UnloadContent() { }

        # endregion

        # region Private and Protected Simulation-related Methods

        /// <summary>
        /// Allows the simulation to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="simTime">Provides a snapshot of timing values (update time and simulation time).</param>
        protected override void Update(GameTime simTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();

            _updateElapsedTime += (float)simTime.ElapsedGameTime.TotalMilliseconds;
            _updateTotalCount++;

            // 1 Second has passed
            if (_updateElapsedTime >= 1000.0f)
            {
                UpdateRate = _updateTotalCount;
                _updateTotalCount = 0;
                _updateElapsedTime = 0;
            }

            _simulator.StepSimulation();

            base.Update(simTime);
        }

        /// <summary>
        /// Draws simulation frame on the screen.
        /// </summary>
        /// <param name="simTime">Provides a snapshot of timing values (frame time and simulation time).</param>
        protected override void Draw(GameTime simTime)
        {
            _drawElapsedTime += (float)simTime.ElapsedGameTime.TotalMilliseconds;
            _drawTotalFrames++;
            // 1 Second has passed
            if (_drawElapsedTime >= 1000.0f)
            {
                DrawFps = _drawTotalFrames;
                _drawTotalFrames = 0;
                _drawElapsedTime = 0;
            }

            // Check if screen dimensions are changed (simulation form is resized)
            try
            {
                if (_graphics.PreferredBackBufferWidth != _simulatorPictureBox.ClientSize.Width ||
                _graphics.PreferredBackBufferHeight != _simulatorPictureBox.ClientSize.Height)
                {
                    _graphics.PreferredBackBufferWidth = _simulatorPictureBox.ClientSize.Width;
                    _graphics.PreferredBackBufferHeight = _simulatorPictureBox.ClientSize.Height;
                    _graphics.ApplyChanges();
                }
            }
            catch
            {
                // ignored
            }

            GraphicsDevice.Clear(Color.CornflowerBlue);
            _frame.Begin();

            var elapsedTime = TimeSpan.Zero;
            if (_simulator.IsRunning)
                elapsedTime = simTime.ElapsedGameTime;

            _simulator.Drawer.Render(ref _frame, _simulatorPictureBox.ClientSize.Width, _simulatorPictureBox.ClientSize.Height,
                ref _simulator.Environment, _simulator.Robot, elapsedTime, _simulator.Render.Type);
            
            _frame.End();
            
            base.Draw(simTime);
        }

        # endregion
    }
}

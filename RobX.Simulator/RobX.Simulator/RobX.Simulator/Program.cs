# region Includes

using System;

# endregion

namespace RobX.Simulator
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var form = new frmSimulator();
            form.Show();
            // This line creates a XNA object in the form created earlier.
            form.SimulationController = new SimController(form.picSimulation.Handle, form.picSimulation, form.Simulator);
            form.SimulationController.Run();
        }
    }
#endif
}

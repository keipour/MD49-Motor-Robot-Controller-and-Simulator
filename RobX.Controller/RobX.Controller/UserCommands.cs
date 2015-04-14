using RobX.Library.Robot;

namespace RobX.Controller
{
    /// <summary>
    /// This class contains ordered list of commands given to the robot by the user.
    /// </summary>
    public static class UserCommands
    {
        /// <summary>
        /// Adds commands defined by user in the command body to the commands execution queue.
        /// </summary>
        /// <param name="controller">Controller variable.</param>
        public static void AddCommands(ref Library.Robot.Controller controller)
        {
            // Prepare robot for execution of commands
            controller.PrepareForExecution();
            controller.ResetEncoders();

            controller.SetXyAngle(1500, 7550 - 2500, 0);

            controller.Commands.Enqueue(new Command(Command.Types.MoveForwardForDistance, 2400, 20));
            controller.Commands.Enqueue(new Command(Command.Types.SetSpeedForDegrees, 90, 30, 10));
            controller.Commands.Enqueue(new Command(Command.Types.MoveForwardForDistance, 3020, 20));
            controller.Commands.Enqueue(new Command(Command.Types.SetSpeedForDegrees, -90, 10, 25));
            controller.Commands.Enqueue(new Command(Command.Types.MoveForwardForDistance, 1000, 20));
            controller.Commands.Enqueue(new Command(Command.Types.MoveBackwardForDistance, 1000, 20));
            controller.Commands.Enqueue(new Command(Command.Types.SetSpeedForDegrees, 90, -10, -25));
            controller.Commands.Enqueue(new Command(Command.Types.MoveBackwardForDistance, 3020, 20));
            controller.Commands.Enqueue(new Command(Command.Types.SetSpeedForDegrees, -90, -30, -10));
            controller.Commands.Enqueue(new Command(Command.Types.MoveBackwardForDistance, 2000, 20));
            controller.Commands.Enqueue(new Command(Command.Types.Stop));
        }
    }
}
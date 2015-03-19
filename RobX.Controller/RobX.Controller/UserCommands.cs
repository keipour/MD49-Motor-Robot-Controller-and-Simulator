namespace RobX.Controller
{
    public static class UserCommands
    {
        public static void AddCommands(ref Controller Controller)
        {
            // Prepare robot for execution of commands
            Controller.PrepareForExecution();
            Controller.Robot.ResetEncoders();

            Controller.Robot.SetXYAngle(1500, 7550 - 2500, 0);

            Controller.AddCommandToQueue(new Command(Command.Types.MoveForwardForDistance, 2400, 20));
            Controller.AddCommandToQueue(new Command(Command.Types.SetSpeedForDegrees, 90, 30, 10));
            Controller.AddCommandToQueue(new Command(Command.Types.MoveForwardForDistance, 3020, 20));
            Controller.AddCommandToQueue(new Command(Command.Types.SetSpeedForDegrees, -90, 10, 25));
            Controller.AddCommandToQueue(new Command(Command.Types.MoveForwardForDistance, 1000, 20));
            Controller.AddCommandToQueue(new Command(Command.Types.MoveBackwardForDistance, 1000, 20));
            Controller.AddCommandToQueue(new Command(Command.Types.SetSpeedForDegrees, 90, -10, -25));
            Controller.AddCommandToQueue(new Command(Command.Types.MoveBackwardForDistance, 3020, 20));
            Controller.AddCommandToQueue(new Command(Command.Types.SetSpeedForDegrees, -90, -30, -10));
            Controller.AddCommandToQueue(new Command(Command.Types.MoveBackwardForDistance, 2000, 20));
            Controller.AddCommandToQueue(new Command(Command.Types.Stop));
        }
    }
}

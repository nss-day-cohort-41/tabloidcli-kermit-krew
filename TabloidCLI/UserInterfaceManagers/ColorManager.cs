using System;

namespace TabloidCLI.UserInterfaceManagers
{
    class ColorManager : IUserInterfaceManager
    {
        private readonly IUserInterfaceManager _parentUI;

        public ColorManager(IUserInterfaceManager parentUI)
        {
            _parentUI = parentUI;
        }

        public IUserInterfaceManager Execute()
        {
            Console.Clear();
            Console.WriteLine("Color Menu");
            Console.WriteLine("Choose a new background color from below:");

            // Create an array of all available color options, then render as a menu
            ConsoleColor[] colors = (ConsoleColor[]) ConsoleColor.GetValues(typeof(ConsoleColor));
            for (int i = 0; i < colors.Length; i++ )
            {
                Console.WriteLine($" {i + 1}) {colors[i]}");
            }
            // Add a "0" option to exit this menu
            Console.WriteLine(" 0) Go Back");

            string input = Console.ReadLine();

            try
            {
                int choice = int.Parse(input);
                if (choice == 0)
                    return _parentUI;
                if (colors[choice - 1] == Console.ForegroundColor)
                {
                    // The user is about to make text invisible! Warn the user.
                    Console.WriteLine("This is the same as the text color, and is not recommended!");
                    Console.Write("Are you sure? > ");
                    string confirmation = Console.ReadLine();
                    if (confirmation.ToLower() != "y" && confirmation.ToLower() != "yes")
                        return this;
                }   
                Console.BackgroundColor = colors[choice - 1];
            }
            catch (Exception ex)
            {
                Console.WriteLine("Invalid Selection");
            }

            return this;
        }
    }
}

using TabloidCLI.UserInterfaceManagers;
using System;
namespace TabloidCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            // MainMenuManager implements the IUserInterfaceManager interface
            IUserInterfaceManager ui = new MainMenuManager();
            
            Console.WriteLine("Welcome");
            while (ui != null)
            {
                // Each call to Execute will return the next IUserInterfaceManager we should execute
                // When it returns null, we should exit the program;
                ui = ui.Execute();
            }
        }
    }
}

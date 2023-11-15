using TShockAPI;

namespace Renamer
{
    public class CommandResearch
    {
        public static void RenameCommand(CommandArgs args)
        {
            if (!args.Player.IsLoggedIn) args.Player.SendErrorMessage("Create an account or log in to use this command.");
            else if (args.Parameters.Count != 1) args.Player.SendErrorMessage("Invalid syntax. Proper syntax: {0}changename <new_name>.", TShock.Config.Settings.CommandSpecifier);
            else if (args.Parameters[0] == args.Player.Account.Name) args.Player.SendErrorMessage("You can not use your current account name to change the name.");
            else if (!Tools.VerifyName(args.Parameters[0])) args.Player.SendErrorMessage("Sorry, {0} was already taken by another person.", args.Parameters[0]);

            else Tools.Rename(args.Player.Account.Name, args.Parameters[0], args);
        }
        public static void Manageusers(CommandArgs args)
        {
            if (args.Parameters.Count < 1) args.Player.SendErrorMessage("Invalid user syntax. Try {0}user help.", TShock.Config.Settings.CommandSpecifier);
            else if (args.Parameters[0] != "name") Tools.ManageUsers(args);

            else if (args.Parameters.Count != 3) args.Player.SendErrorMessage("Invalid user syntax. Try {0}user help.", TShock.Config.Settings.CommandSpecifier);
            else if (Tools.VerifyName(args.Parameters[1])) args.Player.SendErrorMessage("User {0} does not exist.", args.Parameters[1]);
            else if (args.Parameters[1] == args.Parameters[2]) args.Player.SendErrorMessage("You can not use account's current name to change the name.");
            else if (!Tools.VerifyName(args.Parameters[2])) args.Player.SendErrorMessage("Sorry, {0} was already taken by another person.", args.Parameters[2]);

            else Tools.Rename(args.Parameters[1], args.Parameters[2], args);
        }
        public static void Username(CommandArgs args)
        {
            if (args.Parameters.Count != 2) args.Player.SendErrorMessage("Invalid user syntax. Proper syntax: {0}username <username> <new_name>.", TShock.Config.Settings.CommandSpecifier);
            else if (Tools.VerifyName(args.Parameters[0])) args.Player.SendErrorMessage("User {0} does not exist.", args.Parameters[0]);
            else if (args.Parameters[0] == args.Parameters[1]) args.Player.SendErrorMessage("You can not use account's current name to change the name.");
            else if (!Tools.VerifyName(args.Parameters[1])) args.Player.SendErrorMessage("Sorry, {0} was already taken by another person.", args.Parameters[1]);

            else Tools.Rename(args.Parameters[0], args.Parameters[1], args);
        }
    }
}

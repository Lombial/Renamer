using System.Data;
using TShockAPI;
using TShockAPI.DB;

namespace Renamer
{
    public class Tools
    {
        public static void Rename(string arg1, string arg2, CommandArgs args)
        {
            if (TShock.DB.Query("UPDATE Users SET Username = @0 WHERE Username = @1;", arg2, arg1) == 0)
            {
                TShock.Log.ConsoleInfo("{0} ({1}) failed to change account name ({2})", args.Player.IP, args.Player.Name, arg1);
                args.Player.SendErrorMessage("Unexpected error.");
            }
            else
            {
                foreach (var player in TShock.Players.Where(p => p != null && p.Account != null && p.Account.Name == arg1))
                {
                    player.Account.Name = Convert.ToString(TShock.UserAccounts.GetUserAccountByName(arg2));
                }
                TShock.Log.ConsoleInfo("{0} ({1}) changed account name {2} => {3}.", args.Player.IP, args.Player.Name, arg1, arg2);
                args.Player.SendSuccessMessage("The name has changed successfully.");
            }
        }
        public static bool VerifyName(string arg)
        {
            try
            {
                return TShock.UserAccounts.GetUserAccountByName(arg) == null;
            }
            catch
            {
                return true;
            }
        }
        internal static void ManageUsers(CommandArgs args)
        {
            string subcmd = args.Parameters[0];
            if (subcmd == "add" && args.Parameters.Count == 4)
            {
                var account = new UserAccount();

                account.Name = args.Parameters[1];
                try
                {
                    account.CreateBCryptHash(args.Parameters[2]);
                }
                catch (ArgumentOutOfRangeException)
                {
                    args.Player.SendErrorMessage("Password must be greater than or equal to {0} characters.", TShock.Config.Settings.MinimumPasswordLength);
                    return;
                }
                account.Group = args.Parameters[3];
                try
                {
                    TShock.UserAccounts.AddUserAccount(account);
                    args.Player.SendSuccessMessage("Account {0} has been added to group {1}.", account.Name, account.Group);
                    TShock.Log.ConsoleInfo("{0} added account {1} to group {2}.", args.Player.Name, account.Name, account.Group);
                }
                catch (GroupNotExistsException)
                {
                    args.Player.SendErrorMessage("Group {0} does not exist.", account.Group);
                }
                catch (UserAccountExistsException)
                {
                    args.Player.SendErrorMessage("User {0} already exists.", account.Name);
                }
                catch (UserAccountManagerException e)
                {
                    args.Player.SendErrorMessage("User {0} could not be added, check console for details.", account.Name);
                    TShock.Log.ConsoleError(e.ToString());
                }
            }
            else if (subcmd == "del" && args.Parameters.Count == 2)
            {
                var account = new UserAccount();
                account.Name = args.Parameters[1];

                try
                {
                    TShock.UserAccounts.RemoveUserAccount(account);
                    args.Player.SendSuccessMessage("Account removed successfully.");
                    TShock.Log.ConsoleInfo("{0} successfully deleted account: {1}.", args.Player.Name, args.Parameters[1]);
                }
                catch (UserAccountNotExistException)
                {
                    args.Player.SendErrorMessage("The user {0} does not exist! Therefore, the account was not deleted.", account.Name);
                }
                catch (UserAccountManagerException ex)
                {
                    args.Player.SendErrorMessage(ex.Message);
                    TShock.Log.ConsoleError(ex.ToString());
                }
            }
            else if (subcmd == "password" && args.Parameters.Count == 3)
            {
                var account = new UserAccount();
                account.Name = args.Parameters[1];

                try
                {
                    TShock.UserAccounts.SetUserAccountPassword(account, args.Parameters[2]);
                    TShock.Log.ConsoleInfo("{0} changed the password for account {1}", args.Player.Name, account.Name);
                    args.Player.SendSuccessMessage("Password change succeeded for {0}.", account.Name);
                }
                catch (UserAccountNotExistException)
                {
                    args.Player.SendErrorMessage("Account {0} does not exist! Therefore, the password cannot be changed.", account.Name);
                }
                catch (UserAccountManagerException e)
                {
                    args.Player.SendErrorMessage("Password change attempt for {0} failed for an unknown reason. Check the server console for more details.", account.Name);
                    TShock.Log.ConsoleError(e.ToString());
                }
                catch (ArgumentOutOfRangeException)
                {
                    args.Player.SendErrorMessage("Password must be greater than or equal to {0} characters.", TShock.Config.Settings.MinimumPasswordLength);
                }
            }
            else if (subcmd == "group" && args.Parameters.Count == 3)
            {
                var account = new UserAccount();
                account.Name = args.Parameters[1];

                try
                {
                    TShock.UserAccounts.SetUserGroup(account, args.Parameters[2]);
                    TShock.Log.ConsoleInfo("{0} changed account {1} to group {2}.", args.Player.Name, account.Name, args.Parameters[2]);
                    args.Player.SendSuccessMessage("Account {0} has been changed to group {1}.", account.Name, args.Parameters[2]);
                    var player = TShock.Players.FirstOrDefault(p => p != null && p.Account?.Name == account.Name);
                    if (player != null && !args.Silent)
                        player.SendSuccessMessage($"{args.Player.Name} has changed your group to {args.Parameters[2]}.");
                }
                catch (GroupNotExistsException)
                {
                    args.Player.SendErrorMessage("That group does not exist.");
                }
                catch (UserAccountNotExistException)
                {
                    args.Player.SendErrorMessage($"User {account.Name} does not exist.");
                }
                catch (UserAccountManagerException e)
                {
                    args.Player.SendErrorMessage($"User {account.Name} could not be added. Check console for details.");
                    TShock.Log.ConsoleError(e.ToString());
                }
            }
            else if (subcmd == "help")
            {
                args.Player.SendInfoMessage("User management command help:");
                args.Player.SendInfoMessage("{0}user add username password group   -- Adds a specified user", TShock.Config.Settings.CommandSpecifier);
                args.Player.SendInfoMessage("{0}user del username                  -- Removes a specified user", TShock.Config.Settings.CommandSpecifier);
                args.Player.SendInfoMessage("{0}user password username newpassword -- Changes a user's password", TShock.Config.Settings.CommandSpecifier);
                args.Player.SendInfoMessage("{0}user group username newgroup       -- Changes a user's group", TShock.Config.Settings.CommandSpecifier);
                args.Player.SendInfoMessage("{0}user name username newusername     -- Changes a user's name", TShock.Config.Settings.CommandSpecifier);
            }
            else
            {
                args.Player.SendErrorMessage("Invalid user syntax. Try {0}user help.", TShock.Config.Settings.CommandSpecifier);
            }
        }
    }
}

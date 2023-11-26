using TShockAPI;
using TShockAPI.Configuration;
using Terraria;
using TerrariaApi.Server;

namespace Renamer
{
    [ApiVersion(2, 1)]
    public class ThePlugin : TerrariaPlugin
    {
        public override string Name => "Renamer";
        public override string Description => "Allows you to change user's account name.";
        public override Version Version => new Version(0, 0, 1);
        public override string Author => "Lombial";
        public static ConfigFile<Settings> Config { get; private set; }
        public ThePlugin(Main game) : base(game) { }
        public override void Initialize()
        {
            string path = Path.Combine(TShock.SavePath, "Renamer.json");
            Config = new();
            Config.Read(path, out bool write);
            if (write)
                Config.Write(path);
            Action<Command> add = arg =>
            {
                Commands.ChatCommands.RemoveAll(arg1 => arg1.Names.Exists(arg2 => arg.Names.Contains(arg2)));
                Commands.ChatCommands.Add(arg);
            };

            if (Config.Settings.TurnOnUserCommandExtension)
                add(new Command(Permissions.user, CommandResearch.Manageusers, "user")
                {
                    DoLog = false,
                    HelpText = "Manages user accounts."
                });
            else if (Config.Settings.TurnOnAdminCommand)
                add(new Command("renamer.admin", CommandResearch.Username, "username")
                {
                    HelpText = "Manages user account name."
                });
            add(new Command("renamer.self", CommandResearch.RenameCommand, "changename", "cname")
            {
                AllowServer = false,
                HelpText = "Allows you to change your account name."
            });
        }
    }
    public class Settings
    {
        public bool TurnOnUserCommandExtension = true;
        public bool TurnOnAdminCommand = true;
    }
}
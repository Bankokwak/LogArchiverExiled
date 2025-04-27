using System;
using Exiled.API.Features;

namespace LogArchiver;

public class Plugin : Plugin<Config>
{
    public override string Name => "LogArchiver";
    public override string Author => "Bankokwak";
    public override Version Version => new Version(1, 0, 0);
    public override Version RequiredExiledVersion { get; } = new Version(9,6,0);

    public static Plugin Instance;

    private static EventHandlers Handlers;

    public override void OnEnabled()
    {
        Instance = this;
        Handlers = new EventHandlers();

        Exiled.Events.Handlers.Server.WaitingForPlayers += Handlers.OnWaitingForPlayers;
        base.OnEnabled();
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Server.WaitingForPlayers -= Handlers.OnWaitingForPlayers;
        
        Instance = null;
        Handlers = null;
        base.OnDisabled();
    }
}
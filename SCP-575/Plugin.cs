using Exiled.API.Features;
using Player = Exiled.Events.Handlers.Player;
using Server = Exiled.Events.Handlers.Server;
using Map = Exiled.Events.Handlers.Map;

namespace SCP_575
{
    public class Plugin : Plugin<Config>
    {
        public EventHandlers Handler;

        public override void OnEnabled()
        {
            Handler = new EventHandlers(this);
            Server.RoundStarted += Handler.OnRoundStarted;
            Server.RoundEnded += Handler.OnRoundEnded;
            Player.TriggeringTesla += Handler.OnTriggeringTesla;
            Player.ChangingRole += Handler.OnChangingRole;
            Player.DamagingWindow += Handler.OnDamagingWindow;
            Map.AnnouncingScpTermination += Handler.OnAnnouncingScpTermination;

            base.OnEnabled();
        }
        public override void OnDisabled()
        {
            Server.RoundStarted -= Handler.OnRoundStarted;
            Server.RoundEnded -= Handler.OnRoundEnded;
            Player.TriggeringTesla -= Handler.OnTriggeringTesla;
            Player.ChangingRole -= Handler.OnChangingRole;
            Player.DamagingWindow -= Handler.OnDamagingWindow;
            Map.AnnouncingScpTermination -= Handler.OnAnnouncingScpTermination;
            Handler = null;
            base.OnDisabled();
        }
    }
}
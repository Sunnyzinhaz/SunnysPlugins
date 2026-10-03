using Exiled.API.Features;
using PlayerRoles;
using ProjectMER.Features.Objects;

namespace Scp035.Models
{
    internal sealed class Scp035Session
    {
        public Player Host { get; set; }
        public Player Controller { get; set; }
        public RoleTypeId HostRole { get; set; }
        public string OldCustomName { get; set; }
        public string OldDisplayNickname { get; set; }
        public SchematicObject Mask { get; set; }
        public bool IsEnding { get; set; }
    }
}

using Exiled.API.Interfaces;
using System.ComponentModel;
using CustomClasses.Roles;

namespace CustomClasses.Configs
{
    public class Config : IConfig
    {
        [Description("Se o plugin está habilitado.")]
        public bool IsEnabled { get; set; } = true;

        public bool Debug { get; set; } = true;

        [Description("Coisas do Classe-E")]
        public ClassE ClassE { get; set; } = new ClassE();

        [Description("Coisas do Cientista Major")]
        public MajorScientist MajorScientist { get; set; } = new MajorScientist();

        /*[Description("Coisas do Serpent's Hand")]
        public SerpentsHand SerpentsHand { get; set; } = new SerpentsHand();*/

        [Description("Lider guarda")]
        public GuardLeader GuardLeader { get; set; } = new GuardLeader();

        [Description("RRT")]
        public RRT RRT { get; set; } = new RRT();

        [Description("Configurações para broadcasts e hints que aparecem no round.")]
        public BroadcastsAndHints BroadcastsAndHints { get; set; } = new BroadcastsAndHints();

        [Description("Coisas do Classe-D gordo")]
        public FatClassD FatClassD { get; set; } = new FatClassD();

        [Description("Configurações do Classe-D Bombado")]
        public BuffedClassD BuffedClassD { get; set; } = new BuffedClassD();

        [Description("Config classe-d faxineiro")]
        public JanitorClassD JanitorClassD { get; set; } = new JanitorClassD();

        [Description("Config do rato de laboratório")]
        public LabRatClassD LabRatClassD { get; set; } = new LabRatClassD();


    }
}

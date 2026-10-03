namespace CustomItems.Core.Interfaces
{
    /// <summary>
    /// Permite que um CustomItem tenha uma hint persistente e limpa enquanto estiver selecionado.
    /// </summary>
    public interface ICustomItemHeldHint
    {
        bool HasCustomHeldHint { get; set; }
        string HeldHint { get; set; }
        float HeldHintPosition { get; set; }
    }
}

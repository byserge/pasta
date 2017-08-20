namespace Pasta.Core
{
    public interface IEditableEffect : IEffect
    {
        void StartEdit(IEffectEditContext context);
        void CommitEdit();
        void CancelEdit();
    }
}

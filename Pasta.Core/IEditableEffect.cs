namespace Pasta.Core
{
    public interface IEditableEffect : IEffect
    {
        void StartEdit(EffectContext context);
        void CommitEdit();
        void CancelEdit();
    }
}

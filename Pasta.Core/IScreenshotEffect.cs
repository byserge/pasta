namespace Pasta.Core
{
    /// <summary>
    /// Describes an effect that takes scrrenshots.
    /// </summary>
    public interface IScreenshotEffect
    {
        /// <summary>
        /// Takes the full screenshot of all existing screens.
        /// <param name="context">The context of all effects.</param>
        /// </summary>
        void CaptureScreen(EffectContext context);
    }
}

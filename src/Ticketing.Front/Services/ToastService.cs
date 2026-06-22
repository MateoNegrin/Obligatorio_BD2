namespace Ticketing.Front.Services;

public enum ToastLevel { Success, Error }

public sealed record ToastMessage(string Text, ToastLevel Level);

// Servicio compartido para disparar toasts desde cualquier página.
// El ToastContainer (en el layout) se suscribe a OnShow y los renderiza.
public sealed class ToastService
{
    public event Action<ToastMessage>? OnShow;

    public void ShowSuccess(string text) => OnShow?.Invoke(new ToastMessage(text, ToastLevel.Success));

    public void ShowError(string text) => OnShow?.Invoke(new ToastMessage(text, ToastLevel.Error));
}

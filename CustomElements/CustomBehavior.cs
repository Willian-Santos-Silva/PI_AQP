using CommunityToolkit.Maui.Core.Platform;

namespace PI_AQP.CustomElements
{

    public class CustomBehavior : Behavior<ContentPage>
    {
        protected override void OnAttachedTo(ContentPage bindable)
        {
            base.OnAttachedTo(bindable);
            // Defina a cor da barra de status aqui
            StatusBar.SetColor(Colors.Black);
        }

        protected override void OnDetachingFrom(ContentPage bindable)
        {
            base.OnDetachingFrom(bindable);
            // Reverter a cor da barra de status, se necessário
            StatusBar.SetColor(Colors.Black);
        }
    }
}

using CommunityToolkit.Maui.Views;

namespace PI_AQP.Views;

public partial class ModalView : Popup
{
	public ModalView()
	{
		InitializeComponent();
    }
    async void OnConfirmResult(object sender, TappedEventArgs args)
    {
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        await CloseAsync(true, cts.Token);
    }
}
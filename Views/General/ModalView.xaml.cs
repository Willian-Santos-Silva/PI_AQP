using CommunityToolkit.Maui.Views;

namespace PI_AQP.Views.General;

public partial class ModalView : Popup
{
	public ModalView()
	{
		InitializeComponent();
	}
    async void OnYesButtonClicked(object? sender, EventArgs e)
    {
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        await CloseAsync(true, cts.Token);
    }
}
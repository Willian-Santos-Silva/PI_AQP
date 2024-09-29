using CommunityToolkit.Maui.Views;

namespace PI_AQP.Views.General;

public partial class PopupLoadingSpinner : Popup
{
	public PopupLoadingSpinner()
	{
		CanBeDismissedByTappingOutsideOfPopup = false;

        InitializeComponent();
	}
}
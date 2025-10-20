using CharacomMaui.Presentation.ViewModels;


namespace CharacomMaui.Presentation.Pages;

public partial class BoxLoginPage : ContentPage
{
    private readonly BoxLoginViewModel _loginViewModel;

    public BoxLoginPage(BoxLoginViewModel loginViewModel)
    {
        InitializeComponent();
        _loginViewModel = loginViewModel;
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        try
        {
            var result = await _loginViewModel.LoginAsync();
            await DisplayAlert("ログイン成功", $"AccessToken: {result.AccessToken}", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("エラー", ex.Message, "OK");
        }
    }
}
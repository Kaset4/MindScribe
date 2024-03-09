namespace MindScribe.ViewModels
{
    public class MainViewModel
    {
        public RegisterViewModel RegisterView { get; set; }
        public MainViewModel()
        {
            RegisterView = new RegisterViewModel();
        }
    }
}

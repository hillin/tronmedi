namespace Tronmedi.GUI.ViewModels.BrowserPage
{
    class BrowserPageViewModel : PageViewModel
    {
        public override string PageTitle => "Browse";
        public string FileName { get; }

        public BrowserPageViewModel(string fileName)
        {
            this.FileName = fileName;
        }

        
    }
}

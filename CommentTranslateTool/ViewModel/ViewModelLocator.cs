using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using Workshop.View;

namespace Workshop.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {


        }

        public MainViewModel Main => Ioc.Default.GetRequiredService<MainViewModel>();
        public IndexPageViewModel IndexPage => Ioc.Default.GetRequiredService<IndexPageViewModel>();
        public SettingPageViewModel SettingPage => Ioc.Default.GetRequiredService<SettingPageViewModel>();

        public BatchProcessViewModel BatchProcessPage => Ioc.Default.GetRequiredService<BatchProcessViewModel>();

        public static void Cleanup<T>() where T : ObservableObject
        {

        }
    }
}
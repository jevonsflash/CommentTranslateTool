using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Workshop.View;

namespace Workshop.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<IndexPageViewModel>();
            SimpleIoc.Default.Register<SettingPageViewModel>();
            SimpleIoc.Default.Register<BatchProcessViewModel>();

        }

        public MainViewModel Main => SimpleIoc.Default.GetInstance<MainViewModel>();
        public IndexPageViewModel IndexPage => SimpleIoc.Default.GetInstance<IndexPageViewModel>();
        public SettingPageViewModel SettingPage => SimpleIoc.Default.GetInstance<SettingPageViewModel>();

        public BatchProcessViewModel BatchProcessPage => SimpleIoc.Default.GetInstance<BatchProcessViewModel>();

        public static void Cleanup<T>() where T : ViewModelBase
        {
            SimpleIoc.Default.Unregister<T>();
            SimpleIoc.Default.Register<T>();
        }
    }
}
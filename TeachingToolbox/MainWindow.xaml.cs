using MahApps.Metro.Controls;
using System.Windows.Controls;
using TeachingToolbox.ViewModel;

namespace TeachingToolbox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {            
            Initialize( new HomePage () );
        }

        public MainWindow( Page aPage )
        {
            Initialize( aPage );
        }

        private void Initialize( Page aPage )
        {
            InitializeComponent();

            mMainFrame.Navigate( aPage );

            Closing += ( s, e ) => ViewModelLocator.Cleanup();            
        }

        private void HandleHomeClick( object sender, System.Windows.RoutedEventArgs e )
        {
            NavigateTo( new HomePage () );
        }

        private void HandleNavigating( object sender, System.Windows.Navigation.NavigatingCancelEventArgs aArgs )
        {
            if( aArgs.Content.GetType() == typeof( HomePage ) )
            {
                mHomeButton.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                mHomeButton.Visibility = System.Windows.Visibility.Visible;
            }
        }

        public void NavigateTo( object aPage )
        {
            mMainFrame.Navigate( aPage );
        }
    }
}
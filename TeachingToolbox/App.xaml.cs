using System.Windows;
using GalaSoft.MvvmLight.Threading;
using System.Globalization;
using System.Threading;
using System;
using System.IO;
using Microsoft.Practices.ServiceLocation;
using GalaSoft.MvvmLight.Ioc;
using TeachingToolbox.Model;

namespace TeachingToolbox
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, INavigationService
    {
        static App()
        {
            DispatcherHelper.Initialize();
        }

        protected override void OnStartup( StartupEventArgs e )
        {
            base.OnStartup( e );

            if( TeachingToolbox.Properties.Settings.Default.Language == "en" )
            {
                SetCulture( new CultureInfo ( "en" ) );
            }
            else if( TeachingToolbox.Properties.Settings.Default.Language == "de" )
            {
                SetCulture( new CultureInfo( "de" ) );
            }

            string theDatabasePath = TeachingToolbox.Properties.Settings.Default.DatabasePath;
            if( String.IsNullOrEmpty( theDatabasePath ) )
            {
                var thePathToTry = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "Data" );
                if( Directory.Exists( thePathToTry ) )
                {
                    TeachingToolbox.Properties.Settings.Default.DatabasePath = thePathToTry;
                }
                else
                {
                    TeachingToolbox.Properties.Settings.Default.DatabasePath = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.ApplicationData ), "Teaching Toolbox" );    
                }
                
                TeachingToolbox.Properties.Settings.Default.Save();
            }

            ServiceLocator.SetLocatorProvider( () => SimpleIoc.Default );
            SimpleIoc.Default.Register<INavigationService>( () => { return this; } );

            var theMainWindow = new MainWindow ();

            Application.Current.MainWindow = theMainWindow;            
            Application.Current.MainWindow.Show();
        }

        public static void ChangeCulture( CultureInfo aCultureInfo )
        {
            SetCulture( aCultureInfo );

            var theOldWindow = Application.Current.MainWindow;

            Application.Current.MainWindow = new MainWindow ( new SettingsPage() );
            Application.Current.MainWindow.Show();

            theOldWindow.Close();
        }

        private static void SetCulture( CultureInfo aCultureInfo )
        {
            Thread.CurrentThread.CurrentCulture = aCultureInfo;
            Thread.CurrentThread.CurrentUICulture = aCultureInfo;            
        }

        public void NavigateTo( object aPage )
        {
            var theMainWindow = Application.Current.MainWindow as MainWindow;
            if( theMainWindow != null )
            {
                theMainWindow.NavigateTo( aPage );                
            }
        }
    }
}

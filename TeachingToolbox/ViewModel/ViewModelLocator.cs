using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using System;
using TeachingToolbox.Design;
using TeachingToolbox.Model;

namespace TeachingToolbox.ViewModel
{
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            if (ViewModelBase.IsInDesignModeStatic)
            {
                ServiceLocator.SetLocatorProvider( () => SimpleIoc.Default );

                SimpleIoc.Default.Register<INavigationService, DesignNavigationService>();
                SimpleIoc.Default.Register<IDataService, DesignDataService>();
            }
            else
            {
                SimpleIoc.Default.Register<IDataService, DataService>();
            }

            SimpleIoc.Default.Register<IAppService, AppService>();

            SimpleIoc.Default.Register<SettingsViewModel>();
            SimpleIoc.Default.Register<HomeViewModel>();
            SimpleIoc.Default.Register<ClassViewModel>();
            SimpleIoc.Default.Register<TimerViewModel>();
            SimpleIoc.Default.Register<AttendanceViewModel>();
            SimpleIoc.Default.Register<RandomizerViewModel>();
        }

        public SettingsViewModel Settings
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SettingsViewModel>();
            }
        }

        public HomeViewModel Home
        {
            get
            {
                var theVm = ServiceLocator.Current.GetInstance<HomeViewModel>();
                return theVm;
            }
        }

        public ClassViewModel Class
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ClassViewModel>();
            }
        }

        public TimerViewModel Timer
        {
            get
            {
                return ServiceLocator.Current.GetInstance<TimerViewModel>();
            }
        }

        public AttendanceViewModel Attendance
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AttendanceViewModel>();
            }
        }

        public RandomizerViewModel Randomizer
        {
            get
            {
                return ServiceLocator.Current.GetInstance<RandomizerViewModel>();
            }
        }

        public static void Cleanup()
        {
        }
    }
}
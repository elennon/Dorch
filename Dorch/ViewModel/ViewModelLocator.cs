using Dorch.View;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dorch.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            var navigationService = this.CreateNavigationService();
            SimpleIoc.Default.Register<INavigationService>(() => navigationService);

            SimpleIoc.Default.Register<IDialogService, DialogService>();

            SimpleIoc.Default.Register<MainPageViewModel>();
            SimpleIoc.Default.Register<AddTeamViewModel>();
            SimpleIoc.Default.Register<ViewTeamViewModel>();
            SimpleIoc.Default.Register<SignUpViewModel>();
            SimpleIoc.Default.Register<AddPlayerViewModel>();
            SimpleIoc.Default.Register<AllPlayersViewModel>();
                
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
        }

        private INavigationService CreateNavigationService()
        {
            var navigationService = new NavigationService();
            navigationService.Configure("MainPage", typeof(MainPage));
            navigationService.Configure("AddTeam", typeof(AddTeam));
            navigationService.Configure("ViewTeam", typeof(ViewTeam));
            navigationService.Configure("SignUp", typeof(SignUp));
            navigationService.Configure("AddPlayer", typeof(AddPlayer));
            navigationService.Configure("ShowAllPlayers", typeof(ShowAllPlayers));
            return navigationService;
        }

        public MainPageViewModel MainPageViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainPageViewModel>();
            }
        }
        public AddTeamViewModel AddTeamViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AddTeamViewModel>();
            }
        }
        public ViewTeamViewModel ViewTeamViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ViewTeamViewModel>();
            }
        }
        public SignUpViewModel SignUpViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SignUpViewModel>();
            }
        }
        public AddPlayerViewModel AddPlayerViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AddPlayerViewModel>();
            }
        }
        public AllPlayersViewModel AllPlayersViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AllPlayersViewModel>();
            }
        }   
    }
    
}

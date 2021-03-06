﻿using System;
using MonoTouch.UIKit;
using ReactiveUI;
using RepositoryStumble.Core.ViewModels.Application;
using RepositoryStumble.Core.ViewModels.Interests;
using RepositoryStumble.Core.ViewModels.Profile;
using RepositoryStumble.Core.ViewModels.Trending;
using RepositoryStumble.ViewControllers.Interests;
using RepositoryStumble.ViewControllers.Trending;
using System.Drawing;
using RepositoryStumble.ViewControllers.Profile;
using RepositoryStumble.Core.Services;
using Xamarin.Utilities.Core.Services;
using System.Linq;

namespace RepositoryStumble.ViewControllers.Application
{
    public class MainViewController : UITabBarController, IViewFor<MainViewModel>
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var profileViewController = new ProfileViewController { ViewModel = IoC.Resolve<ProfileViewModel>() };
            profileViewController.ViewModel.GoToInterestsCommand.Subscribe(_ => SelectedIndex = 1);
            profileViewController.TabBarItem = new UITabBarItem("Profile", Images.User, Images.UserFilled);
            profileViewController.ViewModel.View = profileViewController;

            var interestsViewController = new InterestsViewController { ViewModel = IoC.Resolve<InterestsViewModel>() };
            interestsViewController.TabBarItem = new UITabBarItem("Interests", Images.Heart, Images.HeartFilled);
            interestsViewController.ViewModel.View = interestsViewController;

            var trendingViewController = IoC.Resolve<TrendingViewController>();
            trendingViewController.ViewModel = IoC.Resolve<TrendingViewModel>();
            trendingViewController.TabBarItem = new UITabBarItem("Trending", Images.Trending, Images.TrendingFilled);
            trendingViewController.ViewModel.View = trendingViewController;

            var showcasesViewController = new ShowcasesViewController { ViewModel = IoC.Resolve<ShowcasesViewModel>() };
            showcasesViewController.TabBarItem = new UITabBarItem("Showcase", Images.Spotlight, Images.Spotlight);
            showcasesViewController.ViewModel.View = showcasesViewController;

            var stumble = new UIViewController();
            stumble.TabBarItem = new UITabBarItem("Stumble!", Images.Search, Images.Search);

            this.ViewControllers = new[]
            {
                new UINavigationController(profileViewController),
                new UINavigationController(interestsViewController),
                stumble,
                new UINavigationController(trendingViewController),
                new UINavigationController(showcasesViewController)
            };

            float width = 60;
            if (TabBar.Subviews.Length == 5)
                width = TabBar.Subviews[2].Bounds.Width;

            var stumbleView = new StumbleView(new RectangleF(TabBar.Bounds.Width / 2f - width / 2, 0, width, TabBar.Bounds.Height));
            stumbleView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
            stumbleView.BackgroundColor = UITabBar.Appearance.TintColor;
            stumbleView.UserInteractionEnabled = false;
            TabBar.Add(stumbleView);

            var previousSelectedIndex = 0;
            ViewControllerSelected += (sender, e) =>
            {
                if (e.ViewController == stumble)
                {
                    if (IoC.Resolve<IApplicationService>().Account.Interests.Count() == 0)
                    {
                        SelectedIndex = 1;
                        IoC.Resolve<IAlertDialogService>().Alert(
                            "You need some interests!", 
                            "Please add at least one interest before you stumble!");
                    }
                    else
                    {
                        ViewModel.GoToStumbleCommand.ExecuteIfCan();
                        SelectedIndex = previousSelectedIndex;
                    }
                }
                else
                {
                    previousSelectedIndex = SelectedIndex;
                }
            };
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (MainViewModel)value; }
        }

        public MainViewModel ViewModel { get; set; }

        private class StumbleView : UIView
        {
            public StumbleView(RectangleF rect)
                : base(rect)
            {
                var img = new UIImageView(new RectangleF(rect.Width / 2f - 14f, 4f, 28f, 28f));
                img.AutoresizingMask = UIViewAutoresizing.FlexibleLeftMargin | UIViewAutoresizing.FlexibleRightMargin;
                img.Image = Images.Search.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                img.ContentMode = UIViewContentMode.ScaleAspectFit;
                img.TintColor = UIColor.White;
                Add(img);

                var lbl = new UILabel(new RectangleF(0, rect.Height - 12f, rect.Width, 10f));
                lbl.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleTopMargin;
                lbl.Font = UIFont.SystemFontOfSize(10f);
                lbl.TextAlignment = UITextAlignment.Center;
                lbl.TextColor = UIColor.White;
                lbl.Text = "Stumble!";
                Add(lbl);
            }
        }
    }
}


using System;
using MonoTouch.UIKit;
using Xamarin.Utilities.ViewControllers;
using RepositoryStumble.Core.ViewModels.Repositories;
using ReactiveUI;
using Xamarin.Utilities.DialogElements;
using System.Reactive.Linq;
using RepositoryStumble.Views;
using System.Drawing;

namespace RepositoryStumble.ViewControllers.Repositories
{
    public abstract class BaseRepositoryViewController<TViewModel> : ViewModelPrettyDialogViewController<TViewModel> where TViewModel : BaseRepositoryViewModel
	{
        private UIActionSheet _actionSheet;
		protected readonly UIBarButtonItem DislikeButton;
		protected readonly UIBarButtonItem LikeButton;

		protected BaseRepositoryViewController()
		{
            DislikeButton = new UIBarButtonItem(Images.ThumbDown, UIBarButtonItemStyle.Plain, (s, e) => ViewModel.DislikeCommand.ExecuteIfCan());
            DislikeButton.TintColor = UITabBar.Appearance.TintColor;

            LikeButton = new UIBarButtonItem(Images.ThumbUp, UIBarButtonItemStyle.Plain, (s, e) => ViewModel.LikeCommand.ExecuteIfCan());
            LikeButton.TintColor = UITabBar.Appearance.TintColor;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Action, (s, e) => ShowMore());
            NavigationItem.RightBarButtonItem.EnableIfExecutable(this.WhenAnyValue(x => x.ViewModel)
                .Where(x => x != null)
                .Select(x => x.WhenAnyValue(y => y.Repository))
                .Switch().Select(x => x != null));

            HeaderView.Text = string.Empty;
            HeaderView.TextColor = UIColor.White;
            HeaderView.SubTextColor = UIColor.FromWhiteAlpha(0.9f, 1.0f);

            var section = new Section { HeaderView = HeaderView };
            var section2 = new Section();

            var split = new SplitButtonElement();
            var stars = split.AddButton("Stargazers", "-");
            var watchers = split.AddButton("Watchers", "-");
            var collaborators = split.AddButton("Contributors", "-");
            section.Add(split);

            this.WhenAnyValue(x => x.ViewModel)
                .Where(x => x != null)
                .Select(x => x.WhenAnyValue(y => y.RepositoryIdentifier).Where(y => y != null))
                .Switch()
                .Subscribe(x => 
                {
                    Title = HeaderView.Text = x.Name;
                    ReloadData();
                });

            this.WhenAnyValue(x => x.ViewModel)
                .Where(x => x != null)
                .Select(x => x.WhenAnyValue(y => y.CollaboratorCount))
                .Switch()
                .Subscribe(x => collaborators.Text = x.HasValue ? x.Value.ToString() : "-");

            this.WhenAnyValue(x => x.ViewModel)
                .Where(x => x != null)
                .Select(x => x.WhenAnyValue(y => y.Repository))
                .Switch()
                .Subscribe(x =>
                {
                    if (x == null)
                    {
                        HeaderView.ImageUri = null;
                        HeaderView.Text = null;
                        HeaderView.SubText = null;
                        stars.Text = "-";
                        watchers.Text = "-";
                    }
                    else
                    {
                        HeaderView.ImageUri = x.Owner.AvatarUrl;
                        HeaderView.Text = x.Name;
                        HeaderView.SubText = x.Description;
                        stars.Text = x.StargazersCount.ToString();
                        watchers.Text = x.SubscribersCount.ToString();
                    }

                    ReloadData();
                });

            this.WhenAnyValue(x => x.ViewModel)
                .Where(x => x != null)
                .Select(x => x.WhenAnyValue(y => y.Liked))
                .Switch()
                .Subscribe(x =>
                {
                    if (x == null)
                    {
                        DislikeButton.Image = Images.ThumbDown;
                        LikeButton.Image = Images.ThumbUp;
                    }
                    else if (x.Value)
                    {
                        DislikeButton.Image = Images.ThumbDown;
                        LikeButton.Image = Images.ThumbUpFilled;
                    }
                    else
                    {
                        DislikeButton.Image = Images.ThumbDownFilled;
                        LikeButton.Image = Images.ThumbUp;
                    }
                });

            var webElement = new WebElement("readme");
            webElement.UrlRequested += (obj) => ViewModel.GoToUrlCommand.ExecuteIfCan(obj);

            this.WhenAnyValue(x => x.ViewModel)
                .Where(x => x != null)
                .Select(x => x.WhenAnyValue(y => y.Readme))
                .Switch()
                .Subscribe(x =>
                {
                    if (x == null)
                    {
                        webElement.Value = null;
                        section2.HeaderView = new LoadingView();
                        if (webElement.GetRootElement() != null)
                            section2.Remove(webElement);
                    }
                    else
                    {
                        var view = new ReadmeRazorView { Model = x };
                        webElement.Value = view.GenerateString();
                        section2.HeaderView = null;
                        if (webElement.GetRootElement() == null)
                            section2.Add(webElement);
                    }

                    ReloadData();
                });

            Root.Reset(section, section2);

            ToolbarItems = new [] 
            { 
                new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
                DislikeButton,
                new UIBarButtonItem(UIBarButtonSystemItem.FixedSpace) { Width = 80 },
                LikeButton,
                new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
            };
        }

		private void ShowMore()
		{
            _actionSheet = new UIActionSheet(ViewModel.RepositoryIdentifier.ToString());
            var show = _actionSheet.AddButton("Show in GitHub");
            var share = _actionSheet.AddButton("Share");
            _actionSheet.CancelButtonIndex = _actionSheet.AddButton("Cancel");
            _actionSheet.Clicked += (sender, e) =>
            {
                if (e.ButtonIndex == show)
                {
                    ViewModel.GoToGitHubCommand.ExecuteIfCan();
                }
                else if (e.ButtonIndex == share)
                {
                    var item = MonoTouch.Foundation.NSObject.FromObject(ViewModel.Repository.HtmlUrl);
                    var activityItems = new [] { item };
                    UIActivity[] applicationActivities = null;
                    var activityController = new UIActivityViewController(activityItems, applicationActivities);
                    PresentViewController(activityController, true, null);
                }

                _actionSheet = null;
            };

            _actionSheet.ShowFrom(NavigationItem.RightBarButtonItem, true);
		}

        private class LoadingView : UIView
        {
            private readonly UIActivityIndicatorView _activity;

            public LoadingView()
                : base(new RectangleF(0, 0, 320f, 44f))
            {
                BackgroundColor = UIColor.Clear;

                _activity = new UIActivityIndicatorView();
                _activity.Color = UINavigationBar.Appearance.BackgroundColor;
                _activity.Center = new PointF(Bounds.Width / 2, Bounds.Height / 2);
                _activity.AutoresizingMask = UIViewAutoresizing.FlexibleLeftMargin | UIViewAutoresizing.FlexibleRightMargin |
                                             UIViewAutoresizing.FlexibleTopMargin | UIViewAutoresizing.FlexibleBottomMargin;
                _activity.StartAnimating();
                Add(_activity);
            }
        }
    }
}


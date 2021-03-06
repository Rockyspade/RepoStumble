using MonoTouch.UIKit;

namespace RepositoryStumble
{
    public static class Images
    {
		public static UIImage ThumbUp { get { return UIImage.FromBundle("Images/thumb_up"); } }
		public static UIImage ThumbDown { get { return UIImage.FromBundle("Images/thumb_down"); } }
        public static UIImage ThumbUpFilled { get { return UIImage.FromBundle("Images/thumb_up_filled"); } }
        public static UIImage ThumbDownFilled { get { return UIImage.FromBundle("Images/thumb_down_filled"); } }

		public static UIImage Back { get { return UIImage.FromBundle("Images/back"); } }
		public static UIImage Forward { get { return UIImage.FromBundle("Images/forward"); } }
		public static UIImage Reload { get { return UIImage.FromBundle("Images/reload"); } }
		public static UIImage Gear { get { return UIImage.FromBundle("Images/gear"); } }

        public static UIImage CenterSearch { get { return UIImage.FromBundle("Images/center-search"); } }
		public static UIImage CenterSearchDisabled { get { return UIImage.FromBundle("Images/center-search_disabled"); } }

        public static UIImage User { get { return UIImage.FromBundle("Images/user"); } }
        public static UIImage UserFilled { get { return UIImage.FromBundle("Images/user_filled"); } }

        public static UIImage Trending { get { return UIImage.FromBundle("Images/trending"); } }
        public static UIImage TrendingFilled { get { return UIImage.FromBundle("Images/trending_filled"); } }

        public static UIImage Spotlight { get { return UIImage.FromBundle("Images/spotlight"); } }

        public static UIImage Search { get { return UIImage.FromBundle("Images/search"); } }

        public static UIImage Heart { get { return UIImage.FromBundle("Images/heart"); } }
        public static UIImage HeartFilled { get { return UIImage.FromBundle("Images/heart_filled"); } }

        public static UIImage DownChevron { get { return UIImage.FromBundle("Images/down_chevron"); } }

        public static UIImage GreyButton { get { return UIImageHelper.FromFileAuto("Images/grey_button"); } }

        public static UIImage PurchaseIcon { get { return UIImageHelper.FromFileAuto("Images/purchase_icon"); } }
    }
}


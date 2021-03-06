using System;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Cheesebaron.Folding;

namespace Sample
{
    [Activity(Label = "FoldingPaneLayout Sample", MainLauncher = false, Theme = "@style/Theme.AppCompat.Light")]
    public class FoldingPaneLayoutActivity : ActionBarActivity
    {
        private FoldingPaneLayout _slidingLayout;
        private ListView _list;
        private TextView _content;

        private string _title, _drawerTitle;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_pane);

            _slidingLayout = FindViewById<FoldingPaneLayout>(Resource.Id.sliding_pane_layout);
            _list = FindViewById<ListView>(Resource.Id.left_pane);
            _content = FindViewById<TextView>(Resource.Id.content_text);

            _slidingLayout.PanelOpened += (sender, args) =>
            {
                ActionBar.SetHomeButtonEnabled(false);
                ActionBar.SetDisplayHomeAsUpEnabled(false);
                ActionBar.Title = _drawerTitle;
            };

            _slidingLayout.PanelClosed += (sender, args) =>
            {
                ActionBar.SetDisplayHomeAsUpEnabled(true);
                ActionBar.SetHomeButtonEnabled(true);
                ActionBar.Title = _title;
            };

            _slidingLayout.OpenPane();

            _list.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1,
                Shakespeare.Titles);
            _list.ItemClick += (sender, args) =>
            {
                _content.Text = Shakespeare.Dialogue[args.Position];
                _title = Shakespeare.Titles[args.Position];
                _slidingLayout.SmoothSlideClosed();
            };

            ActionBar.SetDisplayHomeAsUpEnabled(true);
            ActionBar.SetHomeButtonEnabled(true);
            _title = Shakespeare.Titles[0];
            _drawerTitle = "Pick a title";

            _slidingLayout.ViewTreeObserver.GlobalLayout += FirstLayoutListener;
        }

        private void FirstLayoutListener(object sender, EventArgs args)
        {
            if (_slidingLayout.CanSlide() && !_slidingLayout.IsOpen)
            {
                ActionBar.SetDisplayHomeAsUpEnabled(true);
                ActionBar.SetHomeButtonEnabled(true);
                ActionBar.Title = _title;
            }
            else
            {
                ActionBar.SetHomeButtonEnabled(false);
                ActionBar.SetDisplayHomeAsUpEnabled(false);
                ActionBar.Title = _drawerTitle;
            }

            _slidingLayout.ViewTreeObserver.GlobalLayout -= FirstLayoutListener;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home && !_slidingLayout.IsOpen)
            {
                _slidingLayout.SmoothSlideOpen();
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }
    }
}
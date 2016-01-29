﻿using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Onliner_for_windows_10.ParsingHtml;
using Onliner_for_windows_10.Model;
using Windows.UI.Xaml.Controls.Primitives;
using Onliner_for_windows_10.Login;
using System;
using Windows.UI.Popups;
using Windows.Phone.UI.Input;

namespace Onliner_for_windows_10.Views
{
    public sealed partial class ViewNewsPage : Page
    {
        private ParsingFullNewsPage fullPagePars;
        private FullItemNews fullItem = new FullItemNews();
        private Request request = new Request();
        private string loaderPage = string.Empty;
        private string NewsID = string.Empty;

        public ViewNewsPage()
        {
            this.InitializeComponent();
            Loaded += ViewNewsPage_Loaded;
            //back button mobile event
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            }
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;
            Frame.GoBack();
        }

        private async void ViewNewsPage_Loaded(object sender, RoutedEventArgs e)
        {
            //set Main informatiom news
            try
            {
                fullPagePars = new ParsingFullNewsPage(loaderPage);
                fullItem = await fullPagePars.NewsMainInfo();
                MainNewsData.DataContext = fullItem;
            }
            catch (FormatException ex)
            {
                MessageDialog message = new MessageDialog(ex.ToString());
                await message.ShowAsync();
            }


            //fullitem content in <p>
            var resUIElement = await fullPagePars.PostItemDate(loaderPage, fullItem);
            foreach (var item in resUIElement)
            {
                NewsListData.Children.Add(item);
            }

            //comments data
            NewsID = fullItem.NewsID;
            CommentsListView.ItemsSource = await fullPagePars.CommentsMainInfo();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //get page
            loaderPage = e.Parameter.ToString();
        }

        // visibility comments grid
        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (GridComments.Visibility != Visibility.Visible)
            {
                GridComments.Visibility = Visibility.Visible;
            }
            else
            {
                GridComments.Visibility = Visibility.Collapsed;
            }
        }

        // add comments button
        private void AddCommentButton_Click(object sender, RoutedEventArgs e)
        {
            request.ResponceResult += Request_ResponceResult;
            request.AddComments(NewsID, ContentCommentTextBox.Text);
        }

        private async void Request_ResponceResult(string ok)
        {
            if(ok=="ok")
            {
                MessageDialog messageExeption = new MessageDialog("Add");
                await messageExeption.ShowAsync();
            }
        }
    }
}

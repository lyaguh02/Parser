using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HtmlAgilityPack;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace NewsAppAdaptive
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static int articleCount = 25;
        string[] links = new string[articleCount];

        public MainWindow()
        {
            InitializeComponent();
            Parse();
        }

        private void Link_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.newsvl.ru" + links[(int)(sender as Button).Tag]);
        }

        public void Parse()
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load("https://www.newsvl.ru/");
            string[] headers = new string[articleCount];
            string[] texts = new string[articleCount];
            string[] dates = new string[articleCount];
            string[] images = new string[articleCount];

            HtmlNodeCollection article_headers = doc.DocumentNode.SelectNodes("//h3[@class='story-list__item-title']//a");
            HtmlNodeCollection article_texts = doc.DocumentNode.SelectNodes("//p[@class='story-list__item-overview']//a");
            HtmlNodeCollection article_dates = doc.DocumentNode.SelectNodes("//div[@class='story-list__item-info']//span");
            HtmlNodeCollection article_links = doc.DocumentNode.SelectNodes("//h3[@class='story-list__item-title']//a");
            HtmlNodeCollection imgvalue = doc.DocumentNode.SelectNodes("//div[@class='story-list__item-content']//img");

            int i = 0;
            foreach (var tag in article_headers)
            {
                if (i >= headers.Length) break;
                headers[i] = tag.InnerText;
                headers[i] = Regex.Replace(headers[i], "(&.+?;)", "");
                i++;
            }
            i = 0;
            foreach (var tag in article_texts)
            {
                if (i >= headers.Length) break;
                texts[i] = tag.InnerText;
                i++;
            }
            i = 0;
            foreach (var tag in article_dates)
            {
                if (i >= headers.Length) break;
                dates[i] = tag.InnerText;
                i++;
            }
            i = 0;
            foreach (var tag in imgvalue)
            {
                if (i >= headers.Length) break;
                images[i] = tag.Attributes["src"].Value;
                i++;
            }
            i = 0;
            foreach (var tag in article_links)
            {
                if (i >= headers.Length) break;
                links[i] = tag.Attributes["href"].Value;
                i++;
            }
            i = 0;


            Grid[] articleGrids = new Grid[articleCount];
            TextBlock[] headerBlocks = new TextBlock[articleCount];
            TextBlock[] textBlocks = new TextBlock[articleCount];
            BitmapImage[] imageSrc = new BitmapImage[articleCount];
            Image[] imageElements = new Image[articleCount];
            Button[] linkButtons = new Button[articleCount];
            Border[] borders = new Border[articleCount];
            Border[] imageBorders = new Border[articleCount];

            while (i < articleCount)
            {
                articleGrids[i] = new Grid();
                articleGrids[i].MinHeight = 125;
                articleGrids[i].Background = Brushes.LightGray;
                NewsPanel.Children.Insert(i, articleGrids[i]);
                RowDefinition rowDef1 = new RowDefinition();
                RowDefinition rowDef2 = new RowDefinition();
                RowDefinition rowDef3 = new RowDefinition();
                ColumnDefinition colDef1 = new ColumnDefinition();
                ColumnDefinition colDef2 = new ColumnDefinition();
                colDef1.Width = new GridLength(180);
                articleGrids[i].RowDefinitions.Add(rowDef1);
                articleGrids[i].RowDefinitions.Add(rowDef2);
                articleGrids[i].RowDefinitions.Add(rowDef3);
                articleGrids[i].ColumnDefinitions.Add(colDef1);
                articleGrids[i].ColumnDefinitions.Add(colDef2);

                rowDef2.MinHeight = 100;
                rowDef3.Height = new GridLength(25);

                headerBlocks[i] = new TextBlock();
                headerBlocks[i].Text = headers[i];
                headerBlocks[i].Background = Brushes.GhostWhite;
                headerBlocks[i].FontSize = 16;
                headerBlocks[i].Padding = new Thickness(0, 0, 0, 5);
                headerBlocks[i].FontWeight = FontWeights.Bold;
                headerBlocks[i].TextWrapping = TextWrapping.Wrap;
                articleGrids[i].Children.Add(headerBlocks[i]);
                Grid.SetRow(headerBlocks[i], 0);
                Grid.SetColumn(headerBlocks[i], 1);

                textBlocks[i] = new TextBlock();
                textBlocks[i].Text = texts[i];
                textBlocks[i].FontSize = 15;
                textBlocks[i].TextWrapping = TextWrapping.Wrap;
                textBlocks[i].Padding = new Thickness(8, 0, 0, 0);
                articleGrids[i].Children.Add(textBlocks[i]);
                Grid.SetRow(textBlocks[i], 1);
                Grid.SetColumn(textBlocks[i], 1);

                linkButtons[i] = new Button() {Tag = i};
                linkButtons[i].Width = 100;
                linkButtons[i].Height = 25;
                linkButtons[i].Content = "Подробнее...";
                linkButtons[i].FontStyle = FontStyles.Italic;
                linkButtons[i].FontSize = 13;
                linkButtons[i].HorizontalAlignment = HorizontalAlignment.Right;
                articleGrids[i].Children.Add(linkButtons[i]);
                Grid.SetRow(linkButtons[i], 2);
                Grid.SetColumn(linkButtons[i], 1);
                linkButtons[i].Click += Link_Click;

                imageSrc[i] = new BitmapImage();
                imageSrc[i].BeginInit();
                imageSrc[i].UriSource = new Uri(images[i]);
                imageSrc[i].EndInit();
                imageElements[i] = new Image();
                imageElements[i].Source = imageSrc[i];
                imageElements[i].Width = 160;
                imageElements[i].Margin = new Thickness(5);
                articleGrids[i].Children.Add(imageElements[i]);
                Grid.SetRowSpan(imageElements[i], 3);
                Grid.SetColumn(imageElements[i], 0);

                imageBorders[i] = new Border();
                imageBorders[i].Background = Brushes.GhostWhite;
                articleGrids[i].Children.Add(imageBorders[i]);
                Panel.SetZIndex(imageBorders[i], -1);
                Grid.SetRowSpan(imageBorders[i], 3);
                Grid.SetColumn(imageBorders[i], 0);

                borders[i] = new Border();
                borders[i].BorderBrush = Brushes.Black;
                borders[i].BorderThickness = new Thickness(1);
                articleGrids[i].Children.Add(borders[i]);
                Grid.SetRowSpan(borders[i], 3);
                Grid.SetColumnSpan(borders[i], 2);              
                i++;
            }
        }
    }
}

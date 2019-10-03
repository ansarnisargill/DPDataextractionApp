using HandyControl.Data;
using HandyControl.Tools;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Linq;

namespace DPSQLDumpApp
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Window
    {
        public List<HouseDTO> ListOfHouses { get; set; }
        public List<HouseDTO> ExtractedList { get; set; }

        public Home()
        {
            ConfigHelper.Instance.SetLang("en-us");
            InitializeComponent();
        }
        public void window()
        {
            this.CBFrom.SelectedDate = DateTime.Now;
            this.CBTo.SelectedDate = DateTime.Now;

        }
        public void RunQuery()
        {
            var from = CBFrom.SelectedDate.Value;
            var to = CBTo.SelectedDate.Value;
            using (var context = new MainContext())
            {
                this.ListOfHouses = new List<HouseDTO>();
                this.ExtractedList = this.ListOfHouses = context.Houses.Where(x => x.PostingDate >= from.Date && x.PostingDate <= to.Date)
                    .Select(x => new HouseDTO
                    {
                        Address = x.Address,
                        PostingDate = x.PostingDate,
                        Bedrooms = x.Bedrooms,
                        City = x.City,
                        LivingSpaceArea = x.LivingSpaceArea,
                        LotDimensions = x.LotDimensions,
                        OriginalURL = x.OriginalURL,
                        Price = x.Price,
                        Washrooms = x.Washrooms
                    }).ToList();
                SetDGList();
            }
        }
        public void SetDGList()
        {
            dg.ItemsSource = this.ExtractedList;
            dg.Items.Refresh();
            CountText.Text = $"{this.ExtractedList.Count} Houses.";
        }

        public void UpdateSkin(SkinType skin)
        {
            Resources.MergedDictionaries.Clear();
            Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri($"pack://application:,,,/HandyControl;component/Themes/Skin{skin.ToString()}.xaml")
            });
            Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/HandyControl;component/Themes/Theme.xaml")
            });
        }

        private void CBFrom_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}

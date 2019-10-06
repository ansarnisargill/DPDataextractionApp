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
using OfficeOpenXml;
using Microsoft.Win32;
using System.IO;
using OfficeOpenXml.Style;

namespace DPSQLDumpApp
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Window
    {
        public List<HouseDTO> ListOfHouses { get; set; }
        public List<HouseDTO> ExtractedList { get; set; }
        public bool IsFormLoaded { get; set; } = false;

        public Home()
        {
            ConfigHelper.Instance.SetLang("en-us");
            InitializeComponent();
            window();
        }
        public void window()
        {
            this.CBFrom.SelectedDate = DateTime.Now;
            this.CBTo.SelectedDate = DateTime.Now;
            this.IsFormLoaded = true;
            RunQuery();
        }
        public void RunQuery()
        {
            try
            {
                if (this.IsFormLoaded)
                {
                    var from = CBFrom.SelectedDate.Value;
                    var to = CBTo.SelectedDate.Value;
                    using (var context = new MainContext())
                    {
                        this.ListOfHouses = new List<HouseDTO>();
                     this.ListOfHouses = context.Houses.Where(x => x.PostingDate >= from.Date && x.PostingDate <= to.Date)
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
                        this.ListOfHouses.ForEach(x =>
                        {
                            var piecesOfLink=x.OriginalURL.Split('/').ToList();
                            var enNode=piecesOfLink.FindIndex(x=>x.Trim()=="en");
                            x.City=piecesOfLink.ElementAt(enNode+2);
                            if (x.City == null)
                            {
                                x.City = "";
                            }
                            if (x.LotDimensions == null)
                            {
                                x.LotDimensions = "";
                            }
                            if (x.LivingSpaceArea == null)
                            {
                                x.LivingSpaceArea = "";
                            }

                        });
                        this.ExtractedList=this.ListOfHouses;
                        SetDGList();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("DB Error");
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
            RunQuery();
        }
        public bool ListToExcel<T>(List<T> query)
        {
            using (ExcelPackage pck = new ExcelPackage())
            {
                //Create the worksheet
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Result");

                //get our column headings
                var t = typeof(T);
                var Headings = t.GetProperties();
                for (int i = 0; i < Headings.Count(); i++)
                {

                    ws.Cells[1, i + 1].Value = Headings[i].Name;
                }

                //populate our Data
                if (query.Count() > 0)
                {
                    ws.Cells["A2"].LoadFromCollection(query);
                }

                //Format the header
                using (ExcelRange rng = ws.Cells["A1:BZ1"])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;                      //Set Pattern for the background to Solid
                    rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));  //Set color to dark blue
                    rng.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                //Write it back to the client
                SaveFileDialog sfd = new SaveFileDialog()
                {
                    FileName = "Data", // Default file name
                    DefaultExt = ".xlsx", // Default file extension
                    Filter = "Excel Sheet (.xlsx)|*.xlsx" // Filter files by extension
                };
                var result = sfd.ShowDialog();
                if (result == true)
                {
                    using (FileStream fs = new FileStream(sfd.FileName, FileMode.Create))
                    {
                        pck.SaveAs(fs);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            Search();
        }
        public void Search()
        {
            var term = TBSearch.Text;
            this.ExtractedList = new List<HouseDTO>();
            this.ListOfHouses.ForEach((x) =>
            {
                if (x.Address.ToUpper().Contains(term) || x.Bedrooms.ToString().ToUpper().Contains(term) || x.City.ToUpper().Contains(term) || x.LivingSpaceArea.ToUpper().Contains(term) || x.LotDimensions.ToUpper().Contains(term) || x.OriginalURL.ToUpper().Contains(term) || x.PostingDate.ToString().ToUpper().Contains(term) || x.Price.ToUpper().Contains(term) || x.Washrooms.ToString().ToUpper().Contains(term))
                {
                    this.ExtractedList.Add(x);
                }
            });
            SetDGList();
        }
        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            if (ListToExcel(this.ExtractedList))
            {
                MessageBox.Show("Data has been extracted!");
            }
        }
    }

}


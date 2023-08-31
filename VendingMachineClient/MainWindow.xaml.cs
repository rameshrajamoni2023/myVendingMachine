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
using System.Net.Http;
using System.Net.Http.Json;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using System.Text.Json.Nodes;
using System.Runtime.ConstrainedExecution;
using VendingMachineClient.Models;
using static System.Net.WebRequestMethods;
using System.Reflection.Metadata;
using Newtonsoft.Json;
using System.Net;
using System.Drawing;
using Color = System.Windows.Media.Color;
using System.IO;

namespace VendingMachineClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string baseUrl = "https://localhost:7243/api/vendingmachine";
        private readonly HttpClient httpClient;
        public MainWindow()
        {
            InitializeComponent();

            httpClient = new HttpClient();

            InitializeVendingMachine();

        }

        private async void InitializeVendingMachine()
        {
            try
            {

                RefreshItems();

                btnAddAmount.Content = "Load Amount";
                btnCollectAmount.Content = "Collect Amount";
                txtAmount.Text = string.Format("{0,10:F2}", 0.00);
                txtResponse.Text = string.Empty;
                lblCurr.Content = "$";
                
            }
            catch (Exception ex)
            {
                txtResponse.Text = "Error: " + ex.Message;
            }

        }

        private async void RefreshItems()
        {
            // Send a POST request to the API 
            string result = await CallAPI(string.Empty, "/items", "GET");

            if (result != null)
            {
                List<Product> response = JsonConvert.DeserializeObject<List<Product>>(result);

                if (response != null)
                {
                    InitializeItemButtons(response);
                }
            }
        }

        private void InitializeItemButtons(List<Product> response)
        {
            try
            {
                //allow to throw exception 
                //restrict to 12 items 
                btnProduct_1.Content = String.Format("{0}\n   ( ${1,10:F2} )\n\n\n    {2} Left", response[0].Name, response[0].Rate, response[0].Quantity);
                btnProduct_1.Tag = response[0];

                btnProduct_2.Content = String.Format("{0}\n   ( ${1,10:F2} )\n\n\n    {2} Left", response[1].Name, response[1].Rate, response[1].Quantity);
                btnProduct_2.Tag = response[1];

                btnProduct_3.Content = String.Format("{0}\n   ( ${1,10:F2} )\n\n\n    {2} Left", response[2].Name, response[2].Rate, response[2].Quantity);
                btnProduct_3.Tag = response[2];

                btnProduct_4.Content = String.Format("{0}\n   ( ${1,10:F2} )\n\n\n    {2} Left", response[3].Name, response[3].Rate, response[3].Quantity);
                btnProduct_4.Tag = response[3];

                btnProduct_5.Content = String.Format("{0}\n   ( ${1,10:F2} )\n\n\n    {2} Left", response[4].Name, response[4].Rate, response[4].Quantity);
                btnProduct_5.Tag = response[4];

                btnProduct_6.Content = String.Format("{0}\n   ( ${1,10:F2} )\n\n\n    {2} Left", response[5].Name, response[5].Rate, response[5].Quantity);
                btnProduct_6.Tag = response[5];

                btnProduct_7.Content = String.Format("{0}\n   ( ${1,10:F2} )\n\n\n    {2} Left", response[6].Name, response[6].Rate, response[6].Quantity);
                btnProduct_7.Tag = response[6];

                btnProduct_8.Content = String.Format("{0}\n   ( ${1,10:F2} )\n\n\n    {2} Left", response[7].Name, response[7].Rate, response[7].Quantity);
                btnProduct_8.Tag = response[7];

                btnProduct_9.Content = String.Format("{0}\n   ( ${1,10:F2} )\n\n\n    {2} Left", response[8].Name, response[8].Rate, response[8].Quantity);
                btnProduct_9.Tag = response[8];

                btnProduct_10.Content = String.Format("{0}\n   ( ${1,10:F2} )\n\n\n    {2} Left", response[9].Name, response[9].Rate, response[9].Quantity);
                btnProduct_10.Tag = response[9];

                btnProduct_11.Content = String.Format("{0}\n   ( ${1,10:F2} )\n\n\n    {2} Left", response[10].Name, response[10].Rate, response[10].Quantity);
                btnProduct_11.Tag = response[10];

                btnProduct_12.Content = String.Format("{0}\n   ( ${1,10:F2} )\n\n\n    {2} Left", response[11].Name, response[11].Rate, response[11].Quantity);
                btnProduct_12.Tag = response[11];




            }
            catch (Exception ex)
            {
                //log error
            }
            finally
            {

                if (btnProduct_1.Content.Equals("Button")) btnProduct_1.Visibility = Visibility.Collapsed;
                if (btnProduct_2.Content.Equals("Button")) btnProduct_2.Visibility = Visibility.Collapsed;
                if (btnProduct_3.Content.Equals("Button")) btnProduct_3.Visibility = Visibility.Collapsed;
                if (btnProduct_4.Content.Equals("Button")) btnProduct_4.Visibility = Visibility.Collapsed;
                if (btnProduct_5.Content.Equals("Button")) btnProduct_5.Visibility = Visibility.Collapsed;
                if (btnProduct_6.Content.Equals("Button")) btnProduct_6.Visibility = Visibility.Collapsed;
                if (btnProduct_7.Content.Equals("Button")) btnProduct_7.Visibility = Visibility.Collapsed;
                if (btnProduct_8.Content.Equals("Button")) btnProduct_8.Visibility = Visibility.Collapsed;
                if (btnProduct_9.Content.Equals("Button")) btnProduct_9.Visibility = Visibility.Collapsed;
                if (btnProduct_10.Content.Equals("Button")) btnProduct_10.Visibility = Visibility.Collapsed;
                if (btnProduct_11.Content.Equals("Button")) btnProduct_11.Visibility = Visibility.Collapsed;
                if (btnProduct_12.Content.Equals("Button")) btnProduct_12.Visibility = Visibility.Collapsed;



            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = (Button)sender;

            Product item = (Product)clickedButton.Tag;

            PurchaseItem(item);

            //MessageBox.Show($"Button '{clickedButton.Content}' was clicked.");
        }

        private async void PurchaseItem(Product item)
        {
            //UI Valiate better
            if (item == null) return;

                // Send a POST request to the API 
                string path = "/purchase/" + item.Id;

                string result = await CallAPI(string.Empty, path, "POST");

                if (result != null)
                {
                    ShowInPanel(result);
                }

                RefreshItems();
        }

        private void ChangeTextColor(Color color, FontWeight fontWeight)
        {
            txtResponse.Foreground = new SolidColorBrush(color);
            txtResponse.FontWeight = fontWeight;
        }

        private void SetBoldRedColor(object sender, RoutedEventArgs e)
        {
            ChangeTextColor(Colors.Red, FontWeights.Bold);
        }

        private void SetBoldGreenColor(object sender, RoutedEventArgs e)
        {
            ChangeTextColor(Colors.Green, FontWeights.Bold);
        }

        private void ShowInPanel(string message, string mode = "ERROR")
        {
            if (mode.Equals("ERROR"))
            { SetBoldRedColor(txtResponse, new RoutedEventArgs()); }
            else
            { SetBoldGreenColor(txtResponse, new RoutedEventArgs()); }

            txtResponse.Text = message;
        }

        private async Task<string> CallAPI(string request, string path, string action)
        {

            // Create an instance of HttpClient
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Define the API endpoint URL
                    string apiUrl = baseUrl + path;
                    string? responseContent = string.Empty;


                    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.NoContent);

                    if (action.Equals("GET"))
                    {
                        // Send an HTTP GET request to the API
                        response = await client.GetAsync(apiUrl);
                    }
                    else if (action.Equals("POST"))
                    {
                        // Create a JSON content to send as the request body
                        string jsonContent = JsonConvert.SerializeObject(request);
                        StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                        // Send an HTTP POST request to the API
                        response = await client.PostAsync(apiUrl, content);
                    }
                    else
                    {
                        //action not supported for now
                        //[TODO]
                    }

                    // Check if the request was successful (status code 200)
                    if (response.IsSuccessStatusCode)
                    {
                        // Read and display the response content as a string
                        responseContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(responseContent);
                    }
                    else
                    {
                        responseContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Error: {response.StatusCode}");
                    }

                    return responseContent;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    ShowInPanel("Service Down!, Try Refresh Button.", "ERROR");
                }

                return string.Empty;
            }








        }

        private void btnAddAmount_Click(object sender, RoutedEventArgs e)
        {
            decimal amt = decimal.Parse(txtAmount.Text);
            LoadAmount(amt);
            txtAmount.Text = "0.00";

        }

        private void btnCollectAmount_Click(object sender, RoutedEventArgs e)
        {
            ReturnMoney();
           
        }

        private async void LoadAmount(decimal amount)
        {
            if (amount <= 0)
            {
                ShowInPanel("Invalid Amount.Retry.");
            }
            else
            {
                // Send a POST request to the API 
                string path = "/loadMoney";
                string request = amount.ToString();

                string result = await CallAPI(request, path, "POST");

                if (result != null)
                {
                    ShowInPanel(result);
                }

            }
        }

        private async void ReturnMoney()
        {
            // Send a POST request to the API 
            string path = "/ReturnMoney";

            string result = await CallAPI(string.Empty, path, "GET");

            if (result != null)
            {
                ShowInPanel(result);
            }
        }

        private async void UploadProducts()
        {
            // Send a POST request to the API 
            string path = "/UploadProducts";
            
            // API endpoint URL
            string apiUrl = baseUrl + path;  // Adjust the URL as needed

            // Create an HttpClient instance
            using (HttpClient client = new HttpClient())
            {
                // Create a multipart form content to send the file
                MultipartFormDataContent form = new MultipartFormDataContent();

                // Add the CSV file to the form data with the "file" field name
                byte[] fileBytes = System.IO.File.ReadAllBytes("c:\\users\\Rames\\Downloads\\file.csv"); // Replace with the actual file path
                ByteArrayContent fileContent = new ByteArrayContent(fileBytes);
                form.Add(fileContent, "file", "file.csv");

                try
                {
                    // Send the POST request
                    HttpResponseMessage response = await client.PostAsync(apiUrl, form);

                    // Check if the request was successful
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the response content as a string
                        string responseBody = await response.Content.ReadAsStringAsync();
                        Console.WriteLine("Response:");
                        Console.WriteLine(responseBody);
                    }
                    else
                    {
                        Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }


        }

        private void Reload_Click(object sender, RoutedEventArgs e)
        {
            InitializeVendingMachine();
        }
        private async void PrintReceipt()
        {

            string header = string.Format("ABC Vending Machine \n\n Purchase Receipt");
            //string top_section = string.Format("Date : {0} \n Receipt Id : {1} \n\n Address Line1 \n Address Line2 ");
            string top_section = string.Empty;
            string row_header = string.Format("\n===========================\n No \t Item \t Qty \t Rate \t Amount \n===========================\n");
            string row_items = string.Empty;
            string row_footer = string.Empty;
            string footer = string.Format("\n===========================\n Thank You \n");
            string page = string.Empty;

            string path = "/receipt";

            var result = await CallAPI(string.Empty, path, "GET");

            if (result != null)
            {
                Receipt? receipt =  JsonConvert.DeserializeObject<Receipt>(result);

                top_section = string.Format("\n\n Date : {0} \n Receipt Id : {1} \n Address Line1, Address Line2 ",receipt.PurchaseDate,receipt.ReceiptId);

                int slNo = 0;
                foreach(Product item in receipt.Items) 
                {
                    slNo++;

                    row_items = row_items + "\n" + string.Format("{0}\t{1}\t{2}\t{3}\t{4}\n ",slNo,item.Name,item.Quantity,item.Rate,item.Rate*item.Quantity);
                }

                row_footer = string.Format("\n\t\t Total Amount : {0}", receipt.TotalAmount);

                page = string.Concat(header, top_section, row_header,row_items,row_footer, footer);
                
                MessageBox.Show(page);
            }



        }

        private void btnPrintReceipt_Click(object sender, RoutedEventArgs e)
        {
            PrintReceipt();
        }

        private void Upload_Click(object sender, RoutedEventArgs e)
        {
            UploadProducts();
        }
    }

}

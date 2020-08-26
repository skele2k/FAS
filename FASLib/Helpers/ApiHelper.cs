using FASLib.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FASLib.Helpers
{
    public static class ApiHelper
    {
        public static HttpClient ApiClient { get; set; }

        public static bool InitializeClient()
        {
            string api = ConfigurationManager.AppSettings["api"];
            bool output = true;
            try
            {
                ApiClient = new HttpClient();
                ApiClient.BaseAddress = new Uri(api);
                ApiClient.DefaultRequestHeaders.Accept.Clear();
                ApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
            catch
            {
                MessageBox.Show("Сүлжээтэй холбогдоход алдаа гарлаа. IP хаягаа шалгана уу?");
                output = false;
            }
            return output;
        }
    }
}

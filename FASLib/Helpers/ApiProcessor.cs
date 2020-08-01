using FASLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace FASLib.Helpers
{
    public static class ApiProcessor
    {
        public static async Task<List<StaffModel>> LoadStaffs()
        {
            string url = "https://localhost:44360/api/staff";

            using (HttpResponseMessage response = await ApiHelper.ApiClient.GetAsync(url))
            {
                if(response.IsSuccessStatusCode)
                {
                    List<StaffModel> staffs = await response.Content.ReadAsAsync<List<StaffModel>>();

                    return staffs;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
        public static async Task<StaffModel> LoadStaffByID(int id)
        {
            string url = $"https://localhost:44360/api/staff/ { id } ";
            using (HttpResponseMessage response = await ApiHelper.ApiClient.GetAsync(url))
            {
                if(response.IsSuccessStatusCode)
                {
                    StaffModel staff = await response.Content.ReadAsAsync<StaffModel>();
                    return staff;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public static async Task<List<BranchModel>> LoadBranches()
        {
            string url = "https://localhost:44360/api/branch";

            using (HttpResponseMessage response = await ApiHelper.ApiClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    List<BranchModel> branches = await response.Content.ReadAsAsync<List<BranchModel>>();

                    return branches;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
        public static async Task<BranchModel> LoadBranchByID(int id)
        {
            string url = $"https://localhost:44360/api/branch { id }";
            using (HttpResponseMessage response = await ApiHelper.ApiClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    BranchModel branch = await response.Content.ReadAsAsync<BranchModel>();
                    return branch;
                }
                else
                {
                    return null;
                }
            }
        }
        public static async Task<string> SaveBranch(BranchModel theBranch)
        {
            string url = "https://localhost:44360/api/branch";

            var json = new JavaScriptSerializer().Serialize(theBranch);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            using (HttpResponseMessage response = await ApiHelper.ApiClient.PostAsync(url, content))
            {
                if (response.IsSuccessStatusCode)
                {
                    return "success";
                }
                else
                {
                    return "fail";
                }
            }
        }
        public static async Task<string> SaveStaff(StaffModel theStaff)
        {
            string url = "https://localhost:44360/api/staff";
            
            var json = new JavaScriptSerializer().Serialize(theStaff);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");



            using (HttpResponseMessage response = await ApiHelper.ApiClient.PostAsync(url,content))
            {
                if (response.IsSuccessStatusCode)
                {
                    return "success";
                }
                else
                {
                    return "fail";
                }
            }
        }
        public static async Task<string> SaveToAttendanceSheet(AttendanceModel theStaff)
        {
            string url = "https://localhost:44360/api/attendance";

            var json = new JavaScriptSerializer().Serialize(theStaff);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            using (HttpResponseMessage response = await ApiHelper.ApiClient.PostAsync(url, content))
            {
                if (response.IsSuccessStatusCode)
                {
                    return "Success";
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
        public static async Task<string> EditStaffByID(StaffModel theStaff)
        {
            string url = $"https://localhost:44360/api/staff/ { theStaff.id }";
            var json = new JavaScriptSerializer().Serialize(theStaff);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            using (HttpResponseMessage response = await ApiHelper.ApiClient.PutAsync(url, content))
            {
                if (response.IsSuccessStatusCode)
                {
                    return "success";
                }
                else
                {
                    return "fail";
                }
            }
        }
        public static async Task<string> EditBranchByID(BranchModel theBranch)
        {
            string url = $"https://localhost:44360/api/branch/ { theBranch.id }";
            var json = new JavaScriptSerializer().Serialize(theBranch);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            using (HttpResponseMessage response = await ApiHelper.ApiClient.PutAsync(url, content))
            {
                if (response.IsSuccessStatusCode)
                {
                    return "success";
                }
                else
                {
                    return "fail";
                }
            }    
        }
        public static async Task<string> DeleteStaffByID(int id)
        {
            string url = $"https://localhost:44360/api/staff/ { id }";
            
            using (HttpResponseMessage response = await ApiHelper.ApiClient.DeleteAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    return "success";
                }
                else
                {
                    return "fail";
                }
            }
        }
        public static async Task<string> DeleteBranchByID(int id)
        {
            string url = $"https://localhost:44360/api/branch/ { id }";

            using (HttpResponseMessage response = await ApiHelper.ApiClient.DeleteAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    return "success";
                }
                else
                {
                    return "fail";
                }
            }
        }
    }
}

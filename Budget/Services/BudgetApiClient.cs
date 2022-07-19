using Budget.Models.BudgetApiModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Budget.Services
{
    public class BudgetApiClient
    {
        private const string BaseUrl = "https://dev.budget.api.jezzarah.com/api";
        private readonly string _token;

        public BudgetApiClient()
        {
        }

        public BudgetApiClient(string token)
        {
            _token = token;
        }


        #region Sync

        /// <summary>
        /// Get information about our own account
        /// </summary>
        /// <returns></returns>
        //public AsanaResponse<AsanaUser> GetMe()
        //{
        //    return GetMeAsync().GetAwaiter().GetResult();
        //}

        #endregion

      


        #region Async


        #region Users

        /// <summary>
        /// Get information about our own account
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> GetAvailableCurrencies()
        {
            var httpRequestMessage = new HttpRequestMessage { Method = HttpMethod.Get };
            var response = await SendRequest("/Profiles/GetAvailableCurrencies", httpRequestMessage).ConfigureAwait(false);
            var me = JsonConvert.DeserializeObject<List<string>>(ContentToString(response.Content));
            return me;
        }

        public async Task<BudgetApiGetCategories> GetAvailableCategories()
        {
            var httpRequestMessage = new HttpRequestMessage { Method = HttpMethod.Get };
            var response = await SendRequest("/Categories/GetCategories", httpRequestMessage).ConfigureAwait(false);
            var me = JsonConvert.DeserializeObject<BudgetApiGetCategories>(ContentToString(response.Content));
            return me;
        }

        public async Task<List<string>> GetAvailableTransactionTypes()
        {
            var httpRequestMessage = new HttpRequestMessage { Method = HttpMethod.Get };
            var response = await SendRequest("/Profiles/GetAvailableTransactionTypes", httpRequestMessage).ConfigureAwait(false);
            var me = JsonConvert.DeserializeObject<List<string>>(ContentToString(response.Content));
            return me;
        }

        public async Task<BudgetApiGetTransactions> GetTransactions()
        {
            var httpRequestMessage = new HttpRequestMessage { Method = HttpMethod.Get };
            var response = await SendRequest("/Transactions/GetTransactions", httpRequestMessage).ConfigureAwait(false);
            var me = JsonConvert.DeserializeObject<BudgetApiGetTransactions>(ContentToString(response.Content));
            return me;
        }

        public async Task<BudgetApiGetTransactions> GetTransactionsForCategory(int categoryId)
        {
            var httpRequestMessage = new HttpRequestMessage { Method = HttpMethod.Get };
            var response = await SendRequest($"/Transactions/GetTransactionsForCategory?categoryId={categoryId}", httpRequestMessage).ConfigureAwait(false);
            var me = JsonConvert.DeserializeObject<BudgetApiGetTransactions>(ContentToString(response.Content));
            return me;
        }

        public async Task<BudgetApiGetCategorySettings> GetCategorySetting()
        {
            var httpRequestMessage = new HttpRequestMessage { Method = HttpMethod.Get };
            var response = await SendRequest("/Profiles/GetCategorySettings", httpRequestMessage).ConfigureAwait(false);
            var me = JsonConvert.DeserializeObject<BudgetApiGetCategorySettings>(ContentToString(response.Content));
            return me;
        }

        //public async Task<string> GetTasksAsync(string assignee = "", string project = "", string section = "", string workspace = "")
        //{
        //    var httpRequestMessage = new HttpRequestMessage { Method = HttpMethod.Get };

        //    var queryString = HttpUtility.ParseQueryString(string.Empty);

        //    if (!string.IsNullOrEmpty(assignee)) queryString.Add(nameof(assignee), assignee);
        //    if (!string.IsNullOrEmpty(project)) queryString.Add(nameof(project), project);
        //    if (!string.IsNullOrEmpty(section)) queryString.Add(nameof(section), section);
        //    if (!string.IsNullOrEmpty(workspace)) queryString.Add(nameof(workspace), workspace);


        //    var response = await SendRequest($"/tasks{ToQueryString(queryString)}", httpRequestMessage)
        //        .ConfigureAwait(false);

        //    var tasks = JsonConvert.DeserializeObject<AsanaResponse<IEnumerable<AsanaBase>>>(ContentToString(response.Content));
        //    return tasks;
        //}



        public async Task<BudgetApiGetToken> RegisterUser(BudgetApiRegistration req)
        {           

            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                Content = new StringContent(JsonConvert.SerializeObject(req, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),Encoding.UTF8, "application/json")
            };

            var response = await SendRequest($"/User/SignUp/signup", httpRequestMessage).ConfigureAwait(false);
            var token = ContentToString(response.Content);
            var me = JsonConvert.DeserializeObject<BudgetApiGetToken>(ContentToString(response.Content));
            return me;
        }

        public async Task<BudgetApiGetToken> LoginUser(BudgetApiLogin req)
        {
            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                Content = new StringContent(JsonConvert.SerializeObject(req, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), Encoding.UTF8, "application/json")
            };

            var response = await SendRequest($"/User/SignIn/signin", httpRequestMessage).ConfigureAwait(false);
            var me = JsonConvert.DeserializeObject<BudgetApiGetToken>(ContentToString(response.Content));
            return me;
        }

        public async Task<BudgetApiGetCategory> AddCategory(BudgetApiAddCategory req)
        {
            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                Content = new StringContent(JsonConvert.SerializeObject(req, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), Encoding.UTF8, "application/json")
            };

            var response = await SendRequest($"/Categories/AddCategory", httpRequestMessage).ConfigureAwait(false);
            var me = JsonConvert.DeserializeObject<BudgetApiGetCategory>(ContentToString(response.Content));
            return me;
        }

        public async Task<BudgetApiBaseResponse> DeleteCategory(int id)
        {
            var httpRequestMessage = new HttpRequestMessage() { Method = HttpMethod.Delete };
            var response = await SendRequest($"/Categories/DeleteCategory?id={id}", httpRequestMessage).ConfigureAwait(false);
            var me = JsonConvert.DeserializeObject<BudgetApiBaseResponse>(ContentToString(response.Content));
            return me;
        }

        public async Task<BudgetApiAddAction> AddAction(BudgetApiAddAction req)
        {
            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                Content = new StringContent(JsonConvert.SerializeObject(req, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, DateTimeZoneHandling = DateTimeZoneHandling.Unspecified }), Encoding.UTF8, "application/json")
            };

            var response = await SendRequest($"/Transactions/InitAction", httpRequestMessage).ConfigureAwait(false);
            var me = JsonConvert.DeserializeObject<BudgetApiAddAction>(ContentToString(response.Content));
            return me;
        }

        public async Task<BudgetApiBaseResponse> DeleteTransaction(int id)
        {
            var httpRequestMessage = new HttpRequestMessage() { Method = HttpMethod.Delete };
            var response = await SendRequest($"/Transactions/DeleteTransaction?id={id}", httpRequestMessage).ConfigureAwait(false);
            var me = JsonConvert.DeserializeObject<BudgetApiBaseResponse>(ContentToString(response.Content));
            return me;
        }

        public async Task<BudgetApiAddCategorySetting> AddCategorySetting(BudgetApiAddCategorySetting req)
        {
            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                Content = new StringContent(JsonConvert.SerializeObject(req, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), Encoding.UTF8, "application/json")
            };

            var response = await SendRequest($"/Profiles/AddCategorySetting", httpRequestMessage).ConfigureAwait(false);
            var me = JsonConvert.DeserializeObject<BudgetApiAddCategorySetting>(ContentToString(response.Content));
            return me;
        }

        public async Task<BudgetApiBaseResponse> DeleteCategorySetting(int id)
        {
            var httpRequestMessage = new HttpRequestMessage()  { Method = HttpMethod.Delete };
            var response = await SendRequest($"/Profiles/DeleteCategorySetting?id={id}", httpRequestMessage).ConfigureAwait(false);
            var me = JsonConvert.DeserializeObject<BudgetApiBaseResponse>(ContentToString(response.Content));
            return me;
        }

        //public async Task<AsanaResponse<AsanaTask>> UpdateTaskAsync(string gid, AsanaRequest<PushAsanaTask> req)
        //{
        //    var httpRequestMessage = new HttpRequestMessage()
        //    {
        //        Method = HttpMethod.Put,
        //        Content = new StringContent(
        //            JsonConvert.SerializeObject(req, Formatting.None,
        //                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
        //            Encoding.UTF8, "application/json"
        //        )
        //    };

        //    var response = await SendRequest($"/tasks/{gid}", httpRequestMessage)
        //        .ConfigureAwait(false);

        //    var task = JsonConvert.DeserializeObject<AsanaResponse<AsanaTask>>(ContentToString(response.Content));
        //    return task;
        //}

        //public async Task<AsanaResponse<AsanaBase>> DeleteWebhookAsync(string gid)
        //{
        //    var httpRequestMessage = new HttpRequestMessage { Method = HttpMethod.Delete };

        //    var response = await SendRequest($"/webhooks/{gid}", httpRequestMessage)
        //        .ConfigureAwait(false);

        //    var webhook = JsonConvert.DeserializeObject<AsanaResponse<AsanaBase>>(ContentToString(response.Content));
        //    return webhook;
        //}

        #endregion

        #endregion

        #region Help Methods

        private async Task<HttpResponseMessage> SendRequest(string url, HttpRequestMessage requestBody)
        {
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(_token))
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_token}");

                requestBody.RequestUri = new Uri(BaseUrl + url);
                var response = await client.SendAsync(requestBody).ConfigureAwait(false);
                return response;
            }
        }


        private static string ContentToString(HttpContent httpContent)
        {
            return httpContent == null ? "" : httpContent.ReadAsStringAsync().Result;
        }

        private static byte[] RequestBodyToBytes(HttpRequestMessage requestBody)
        {
            return requestBody.Content == null ?
                new byte[] { } : requestBody.Content.ReadAsByteArrayAsync().Result;
        }

        private static string ToQueryString(NameValueCollection nvc)
        {
            var array = (
                from key in nvc.AllKeys
                from value in nvc.GetValues(key)
                select string.Format(
            "{0}={1}",
            HttpUtility.UrlEncode(key),
            HttpUtility.UrlEncode(value))
                ).ToArray();
            return "?" + string.Join("&", array);
        }

        #endregion
    }
}

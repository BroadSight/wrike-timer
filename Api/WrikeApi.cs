using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace wrike_timer.Api
{
    public class WrikeApi
    {
        public static bool AppHasRefreshToken
        {
            get
            {
                return !string.IsNullOrEmpty((string)System.Windows.Application.Current.Properties[Constants.WrikeAuth.TokenAppPropKey]);
            }
        }

        private string _host = "www.wrike.com";

        private Uri _baseUrl
        {
            get { return new Uri($"https://{_host}/api/v4"); }
        }

        private string _accessToken;

        private string _refreshToken;

        private readonly IRestClient _client;

        public WrikeApi(string authorizationCode)
        {
            _client = new RestClient();
            redeemAuthorizationCode(authorizationCode);
        }

        public WrikeApi()
        {
            _client = new RestClient();
            _refreshToken = (string)System.Windows.Application.Current.Properties[Constants.WrikeAuth.TokenAppPropKey];
            refreshToken();
        }

        private void initializeRestClient()
        {
            _client.BaseUrl = _baseUrl;
            _client.Authenticator = new RestSharp.Authenticators.OAuth2AuthorizationRequestHeaderAuthenticator(_accessToken, "bearer");
        }

        private void redeemAuthorizationCode(string authorizationCode)
        {
            getToken("authorization_code", "code", authorizationCode, includeRedirectUrl: true);
        }

        private void refreshToken()
        {
            getToken("refresh_token", "refresh_token", _refreshToken, includeScope: true);
        }

        private void getToken(string grantType, string tokenName, string token, bool includeRedirectUrl = false, bool includeScope = false)
        {
            var client = new RestClient(Constants.WrikeAuth.TokenUrl.GetLeftPart(UriPartial.Authority));
            var request = new RestRequest(Constants.WrikeAuth.TokenUrl, Method.POST);
            request.AddParameter("client_id", Constants.WrikeAuth.ApiClientId, ParameterType.GetOrPost);
            request.AddParameter("client_secret", Constants.WrikeAuth.ApiClientSecret, ParameterType.GetOrPost);
            request.AddParameter("grant_type", grantType, ParameterType.GetOrPost);
            request.AddParameter(tokenName, token, ParameterType.GetOrPost);
            if (includeRedirectUrl)
            {
                request.AddParameter("redirect_uri", Constants.WrikeAuth.RedirectUrl.ToString(), ParameterType.GetOrPost);
            }
            if (includeScope)
            {
                request.AddParameter("scope", Constants.WrikeAuth.ApiScope, ParameterType.GetOrPost);
            }

            var response = Execute<Model.Token>(request, client);
            _accessToken = response.AccessToken;
            _refreshToken = response.RefreshToken;
            System.Windows.Application.Current.Properties[Constants.WrikeAuth.TokenAppPropKey] = _refreshToken;
            _host = response.Host;
            initializeRestClient();
        }

        private T Execute<T>(RestRequest request, IRestClient clientOverride = null) where T : new()
        {
            var response = (clientOverride ?? _client).Execute<T>(request);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                refreshToken();
                response = (clientOverride ?? _client).Execute<T>(request);
            }

            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response. Check inner details for more information.";
                var exception = new ApplicationException(message, response.ErrorException);
                throw exception;
            }
            return response.Data;
        }

        public Model.Contact GetUser()
        {
            var request = new RestRequest("/contacts", Method.GET);
            request.AddParameter("me", true, ParameterType.GetOrPost);
            request.AddParameter("fields", new JArray() { "metadata" }, ParameterType.GetOrPost);

            return Execute<Model.Response<Model.Contact>>(request).Data.FirstOrDefault();
        }

        public List<Model.Workflow> GetWorkflows()
        {
            var request = new RestRequest("/workflows", Method.GET);

            return Execute<Model.Response<Model.Workflow>>(request).Data;
        }

        public List<Model.Task> GetTasks(string title = null, IEnumerable<Model.StatusGroup> status = null,
           IEnumerable<string> responsibles = null, string permalink = null, string metadataKey = null,
           string metadataValue = null, IEnumerable<string> customStatuses = null)
        {
            var request = new RestRequest("/tasks", Method.GET);
            request.AddParameter("fields", new JArray() { "metadata" }, ParameterType.GetOrPost);
            if (title != null)
            {
                request.AddParameter("title", title, ParameterType.GetOrPost);
            }
            if (status != null)
            {
                request.AddParameter("status", new JArray(status), ParameterType.GetOrPost);
            }
            if (responsibles != null)
            {
                request.AddParameter("responsibles", new JArray(responsibles), ParameterType.GetOrPost);
            }
            if (permalink != null)
            {
                request.AddParameter("permalink", permalink, ParameterType.GetOrPost);
            }
            if (metadataKey != null)
            {
                if (metadataValue != null)
                {
                    request.AddParameter("metadata", new JObject() { ["key"] = metadataKey, ["value"] = metadataValue }, ParameterType.GetOrPost);
                }
                else
                {
                    request.AddParameter("metadata", new JObject() { ["key"] = metadataKey }, ParameterType.GetOrPost);
                }
            }
            if (customStatuses != null)
            {
                request.AddParameter("customStatuses", new JArray(customStatuses), ParameterType.GetOrPost);
            }

            return Execute<Model.Response<Model.Task>>(request).Data;
        }

        public List<Model.Task> GetTasksById(IEnumerable<string> taskIds)
        {
            int requestLimit = 100;
            List<Model.Task> tasks = new List<Model.Task>();

            for (int count = 0; count < taskIds.Count(); count += requestLimit)
            {
                var request = new RestRequest("/tasks/{taskIds}", Method.GET);
                request.AddParameter("taskIds", taskIds.Skip(count).Take(count).Aggregate((x, y) => $"{x},{y}"), ParameterType.UrlSegment);
                request.AddParameter("fields", new JArray() { "metadata" }, ParameterType.GetOrPost);

                tasks.AddRange(Execute<Model.Response<Model.Task>>(request).Data);
            }
            return tasks;
        }

        public Model.Task ModifyTask(Model.Task task)
        {
            return ModifyTask(task.Id, task.Metadata);
        }

        public Model.Task ModifyTask(string taskId, IEnumerable<KeyValuePair<string, string>> metadata)
        {
            var request = new RestRequest("/tasks/{taskId}", Method.PUT);
            request.AddParameter("taskId", taskId, ParameterType.UrlSegment);
            request.AddParameter("metadata", new JArray(metadata.Select(d => new JObject() { ["key"] = d.Key, ["value"] = d.Value })), ParameterType.GetOrPost);
            request.AddParameter("fields", new JArray() { "metadata" }, ParameterType.GetOrPost);

            return Execute<Model.Response<Model.Task>>(request).Data.FirstOrDefault();
        }

        public List<Model.Timelog> GetTimelogs(DateTime since)
        {
            var request = new RestRequest("/timelogs", Method.GET);
            request.AddParameter("trackedDate", new JObject() { ["start"] = since.ToString("yyyy-MM-ddTHH:mm:ssZ") }, ParameterType.GetOrPost);
            request.AddParameter("me", true, ParameterType.GetOrPost);

            return Execute<Model.Response<Model.Timelog>>(request).Data;
        }

        public Model.Timelog CreateTimelog(Model.Timelog timelog)
        {
            var request = new RestRequest("/tasks/{taskId}/timelogs", Method.POST);
            request.AddParameter("taskId", timelog.TaskId, ParameterType.UrlSegment);
            request.AddParameter("hours", timelog.Hours, ParameterType.GetOrPost);
            request.AddParameter("trackedDate", timelog.TrackedDate.ToString("yyyy-MM-dd"), ParameterType.GetOrPost);
            if (timelog.CategoryId != null)
            {
                request.AddParameter("categoryId", timelog.CategoryId, ParameterType.GetOrPost);
            }
            if (timelog.Comment != null)
            {
                request.AddParameter("comment", timelog.Comment, ParameterType.GetOrPost);
            }

            return Execute<Model.Response<Model.Timelog>>(request).Data.FirstOrDefault();
        }

        public List<Model.TimelogCategory> GetTimelogCategories()
        {
            var request = new RestRequest("/timelog_categories", Method.POST);

            return Execute<Model.Response<Model.TimelogCategory>>(request).Data;
        }

        public List<Model.Color> GetColors()
        {
            var request = new RestRequest("/colors", Method.GET);

            return Execute<Model.Response<Model.Color>>(request).Data;
        }
    }
}

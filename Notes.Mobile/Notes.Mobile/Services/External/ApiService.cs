using Flurl.Http;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Notes.Mobile.Services.External
{
    public class ApiService
    {
        public static string Token { get; set; }
        public string ApiUrl { get; set; } = null;
        private readonly string _route = null;

        public ApiService(string apiUrl, string route)
        {
            _route = route;
            ApiUrl = apiUrl;
        }
        public async Task<T> Get<T>(string action)
        {
            try
            {
                var url = $"{ApiUrl}/{_route}/{action}";
                return await url.WithOAuthBearerToken(Token).GetJsonAsync<T>();
            }
            catch (FlurlHttpException ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }
        public async Task<T> Post<T>(object request, string action)
        {
            try
            {
                return await $"{ApiUrl}/{_route}/{action}".WithOAuthBearerToken(Token).PostJsonAsync(request).ReceiveJson<T>();
            }
            catch (FlurlHttpException ex)
            {
                if (ex.Call.HttpStatus == System.Net.HttpStatusCode.BadRequest)
                {
                    var err = await ex.GetResponseJsonAsync<object>();
                    await Application.Current.MainPage.DisplayAlert("Error", err.ToString(), "OK");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
                }
                throw;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
                throw;
            }
        }
        public async Task<T> PostNoToken<T>(object request, string action)
        {
            try
            {
                return await $"{ApiUrl}/{_route}/{action}".PostJsonAsync(request).ReceiveJson<T>();
            }
            catch (FlurlHttpException ex)
            {
                if (ex.Call.HttpStatus == System.Net.HttpStatusCode.BadRequest)
                {
                    var err = await ex.GetResponseJsonAsync<object>();
                    await Application.Current.MainPage.DisplayAlert("Error", err.ToString(), "OK");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
                }
                throw;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
                throw;
            }
        }
        public async Task<T> Put<T>(object request, string action)
        {
            try
            {
                return await $"{ApiUrl}/{_route}/{action}".WithOAuthBearerToken(Token).PutJsonAsync(request).ReceiveJson<T>();
            }
            catch (FlurlHttpException ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }
        public async Task<T> Patch<T>(object request, string action)
        {
            try
            {
                return await $"{ApiUrl}/{_route}/{action}".WithOAuthBearerToken(Token).PatchJsonAsync(request).ReceiveJson<T>();
            }
            catch (FlurlHttpException ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }
    }
}

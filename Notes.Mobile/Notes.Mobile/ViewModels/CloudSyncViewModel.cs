using Notes.Mobile.Services.External;
using Notes.Models.ApplicationUser;
using System;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Notes.Mobile.ViewModels
{
    public class CloudSyncViewModel : BaseViewModel
    {
        private readonly ApiService _userService = null;

        private bool loggedIn = false;
        private string serverUrl = string.Empty;
        private string username = string.Empty;
        private string email = string.Empty;
        private string password = string.Empty;
        private DateTime? validTill = null;

        public CloudSyncViewModel()
        {
            _userService = new ApiService(ServerUrl, "User");
        }

        public bool LoggedIn {
            get
            {
                return loggedIn;
            }
            set 
            {
                loggedIn = value;
                OnPropertyChanged();
            } 
        }
        public string ServerUrl
        {
            get
            {
                return serverUrl;
            }
            set
            {
                serverUrl = value;
                OnPropertyChanged();
            }
        }
        public string Username
        {
            get
            {
                return username;
            }
            set
            {
                username = value;
                OnPropertyChanged();
            }
        }

        public string Email
        {
            get
            {
                return email;
            }
            set
            {
                email = value;
                OnPropertyChanged();
            }
        }
        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                password = value;
                OnPropertyChanged();
            }
        }
        public DateTime? ValidTill
        {
            get
            {
                return validTill;
            }
            set
            {
                validTill = value;
                OnPropertyChanged();
            }
        }

        public async Task LoadData()
        {
            try {
                var token = await SecureStorage.GetAsync("token");
                if (token != null)
                {
                    LoggedIn = true;
                    var handler = new JwtSecurityTokenHandler();
                    var tokenS = handler.ReadToken(token) as JwtSecurityToken;
                    var claims = tokenS.Claims;
                    ValidTill = tokenS.ValidTo;
                    Username = claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                    Email = claims.First(c => c.Type == ClaimTypes.Email).Value;
                }
                if (Preferences.ContainsKey("serverUrl")){
                    ServerUrl = Preferences.Get("serverUrl", "");
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        public async Task Register()
        {
            if (!string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password))
            {
                _userService.ApiUrl = ServerUrl;
                var upsertUser = new ApplicationUserUpsertModel
                {
                    Email = this.Email,
                    Password = this.Password,
                    Username = this.Username
                };
                try
                {
                    await _userService.PostNoToken<dynamic>(upsertUser, "Register");
                    Preferences.Set("serverUrl", ServerUrl);
                    await Application.Current.MainPage.DisplayAlert("Success", "Registered successfully. You can log in now.", "OK");
                }
                catch { }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Username/Email/Password can not be empty.", "OK");
            }
        }
        public async Task Login()
        {
            if ((!string.IsNullOrEmpty(Email) || !string.IsNullOrEmpty(Username)) && !string.IsNullOrEmpty(Password))
            {
                _userService.ApiUrl = ServerUrl;
                var upsertUser = new ApplicationUserGetRequestModel
                {
                    Email = this.Email,
                    Password = this.Password,
                    Username = this.Username
                };
                try
                {
                    var token = await _userService.PostNoToken<TokenModel>(upsertUser, "GetToken");
                    Preferences.Set("serverUrl", ServerUrl);
                    await Application.Current.MainPage.DisplayAlert("Success", "Login successful. You can sync your notes with external server now.", "OK");
                    await SecureStorage.SetAsync("token", token.Token);
                    await LoadData();
                    ApiService.Token = token.Token;
                    Preferences.Set("loggedIn", "true");
                }
                catch { }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Username/Email/Password can not be empty.", "OK");
            }
        }
        public async Task Logout()
        {
            if (LoggedIn) 
            {
                SecureStorage.Remove("token");
                LoggedIn = false;
                Username = "";
                Password = "";
                ValidTill = null;
                Email = "";
                await LoadData();
                await Application.Current.MainPage.DisplayAlert("Alert", "Token deleted.", "OK");
                Preferences.Remove("loggedIn");
            }
        }
    }
}

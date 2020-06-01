using Notes.Mobile.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Notes.Mobile.ViewModels
{
    public class EncryptionKeyViewModel : BaseViewModel
    {
        private string secretKey = "************";
        public string SecretKey {
            get
            {
                return secretKey;
            }
            set
            {
                secretKey = value;
                OnPropertyChanged();
            }
        }
        public async Task GetKey()
        {
            SecretKey = await SecureStorage.GetAsync("secrets_key");
        }
        public async Task SetNewKey()
        {
            try {
                await NoteService.DeleteAll();
                await SecureStorage.SetAsync("secrets_key", SecretKey);
                await NoteService.SetKey();
            }
            catch(Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }
        public void ResetKey()
        {
            SecretKey = SecureService.GenerateKey();
        }
    }
}

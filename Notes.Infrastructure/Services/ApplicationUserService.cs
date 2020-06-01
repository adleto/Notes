using AutoMapper;
using Notes.Database;
using Notes.Database.Entities;
using Notes.Infrastructure.Interfaces;
using Notes.Models.ApplicationUser;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Notes.Infrastructure.Services
{
    public class ApplicationUserService : IApplicationUser
    {
        private readonly Context _contex;
        private readonly IMapper _mapper;
        public ApplicationUserService(Context contex, IMapper mapper)
        {
            _contex = contex;
            _mapper = mapper;
        }

        public async Task Add(ApplicationUserUpsertModel model)
        {
            if(IsUsernameAndEmailUnique(model.Username, model.Email))
            {
                throw new Exception("User with same username or email aready exists.");
            }
            var user = _mapper.Map<ApplicationUser>(model);
            user.PasswordSalt = GenerateSalt();
            user.PasswordHash = GenerateHash(user.PasswordSalt, model.Password);
            user.Active = true;
            _contex.Add(user);
            await _contex.SaveChangesAsync();
        }

        public async Task<ApplicationUser> Get(int id)
        {
            return await _contex.ApplicationUser.FindAsync(id);
        }

        public ApplicationUser Get(ApplicationUserGetRequestModel model)
        {
            ApplicationUser user = null;
            if (!string.IsNullOrEmpty(model.Username))
            {
                user = _contex.ApplicationUser.FirstOrDefault(au => au.Active == true && au.Username == model.Username);
            }
            else if (!string.IsNullOrEmpty(model.Email))
            {
                user = _contex.ApplicationUser.FirstOrDefault(au => au.Active == true && au.Email == model.Email);
            }
            if(user != null)
            {
                if(user.PasswordHash == GenerateHash(user.PasswordSalt, model.Password))
                {
                    return user;
                }
            }
            return null;
        }
        private bool IsUsernameAndEmailUnique(string username, string email)
        {
            var user = _contex.ApplicationUser
                .Where(au => au.Username == username || au.Email == email)
                .FirstOrDefault();
            return (user!=null);
        }
        private string GenerateHash(string passwordSalt, string password)
        {
            byte[] source = Convert.FromBase64String(passwordSalt);
            byte[] bytes = Encoding.Unicode.GetBytes(password);
            byte[] destination = new byte[source.Length + bytes.Length];

            Buffer.BlockCopy(source, 0, destination, 0, source.Length);
            Buffer.BlockCopy(bytes, 0, destination, source.Length, bytes.Length);

            return Convert.ToBase64String(HashAlgorithm.Create("SHA256").ComputeHash(destination));
        }

        private string GenerateSalt()
        {
            var buffer = new byte[16];
            new RNGCryptoServiceProvider().GetBytes(buffer);
            return Convert.ToBase64String(buffer);
        }
    }
}

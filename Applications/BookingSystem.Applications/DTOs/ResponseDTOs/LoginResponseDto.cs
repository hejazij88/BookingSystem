using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Applications.DTOs.ResponseDTOs
{
    public class LoginResponseDto
    {
        public LoginResponseDto(string accessToken, string refreshToken, DateTime accessTokenExpiration)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            AccessTokenExpiration = accessTokenExpiration;
        }

        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime AccessTokenExpiration { get; set; }

    }
}

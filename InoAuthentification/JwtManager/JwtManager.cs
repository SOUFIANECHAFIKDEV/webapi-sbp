using InoAuthentification.JwtManagers.Models;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace InoAuthentification.JwtManagers
{
    class JwtManager
    {
        const string secret = "GQDstcKsx0NHjPOuXOYg5MbeJ1XT0uFiwDVvVBrkDFJGDFHGLSHKGSDHhjkdfhlsgkguyserSDFHJSDFghghGHjtuiUILhY";
        public string CreatToken(int userId)
        {
            var payload = new Dictionary<string, object>
                {
                    { "Expire", DateTime.UtcNow.AddDays(15).Ticks },
                    { "UserId", userId }
                };



            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            var token = encoder.Encode(payload, secret);
            return token;
        }

        public TokenValidation VerifyingToken(string token)
        {
            try
            {
                IJsonSerializer serializer = new JsonNetSerializer();
                IDateTimeProvider provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder);
                var json = decoder.Decode(token, secret, verify: true);
                return new TokenValidation()
                {
                    IsValid = true,
                    CurrentToken = JsonConvert.DeserializeObject<TokenModel>(json)
                };
            }
            catch (TokenExpiredException)
            {
                return new TokenValidation()
                {
                    IsValid = false,
                    CurrentToken = null

                };
            }
            catch (SignatureVerificationException)
            {
                return new TokenValidation()
                {
                    IsValid = false,
                    CurrentToken = null

                };
            }
        }
    }
}

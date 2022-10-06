﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using Oxyzen8SelectorServer.Models;
using System.Net.Mail;


namespace Oxyzen8SelectorServer.Controllers
{
    public class AuthController : ApiController
    {

        [HttpGet]
        [ActionName("SessionValue")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public string SessionValue()
        {
            var Session = HttpContext.Current.Session;
            return Session["UAL"].ToString();
        }

        [HttpPost]
        [ActionName("Login")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // POST api/auth/login
        public object Login([FromBody]ClsLoginParams info)
        {
            DataTable dt = AuthModel.GetUserByEmail(info.email);

            var Session = HttpContext.Current.Session;

            if (dt.Rows.Count > 0)
            {
                if (Convert.ToInt32(dt.Rows[0]["access"]) == 1)
                {
                    if (CalculateMD5Hash(info.password).ToUpper() == dt.Rows[0]["password"].ToString().ToUpper())
                    {
                        Session["userId"] = Convert.ToInt32(dt.Rows[0]["id"]);
                        Session["UAL"] = Convert.ToInt32(dt.Rows[0]["access_level"]);
                        Session["representativeID"] = Convert.ToInt32(dt.Rows[0]["customer_id"]);

                        long expiredTime = DateTime.Now.Millisecond + 5184000000L;

                        return new { action = "success", data = dt, accessToken = JwtManager.GenerateToken(Newtonsoft.Json.JsonConvert.SerializeObject(new { exp = expiredTime  })) };
                    } else
                    {
                        return new { action = "incorrect_password"};
                    }
                } else
                {
                    return new { action = "no_user_access" };
                }
            } else
            {
                return new { action = "no_user_exist" };
            }
        }

        [HttpPost]
        [ActionName("sendrequest")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public bool SendMailEmailVerification([FromBody]dynamic info)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("teddan817@gmail.com");
            mail.To.Add(info.email);
            mail.Subject = "Sending Email Using Asp.Net & C#";
            mail.Body = "<h1>In this article you will learn how to send a email using Asp.Net & C#</h1>";
            mail.IsBodyHtml = true;

            try
            {
                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new NetworkCredential("teddan817@gmail.com", "Welcome!0817");
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }

        public string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }

    }
}
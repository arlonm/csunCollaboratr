using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web;

namespace CollaboratR
{
   
    public static class Utility
    {
        /// <summary>
        /// Used to get the username for our method of cookie storing user information
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static string GetRequestUsername(IPrincipal user)
        {
            return user.Identity.Name.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[1];
        }

        /// <summary>
        /// Used to get the userid for our method of cookie storing user information
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static string GetRequestUserid(IPrincipal user)
        {
            return user.Identity.Name.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0];
        }


        public static string Sanitize(this string input)
        {
            return HttpUtility.HtmlEncode(input);
        }

        public static string Sanitize(this string input, char[] whitelist)
        {
            string inputTemp = HttpUtility.HtmlEncode(input);
            StringBuilder sbTemp = new StringBuilder();
            foreach(char c in inputTemp)
            {
                foreach(char w in whitelist)
                {
                    if(c == w)
                    {
                        sbTemp.Append(c);
                        break;
                    }
                }
            }
            return sbTemp.ToString();
        }


        //For usernames and basic user information
        public static char[] UserInfoWhiteList =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890-_".ToCharArray();
        public static char[] EmailWhiteList =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890@-_.".ToCharArray();
        public static char[] PasswordWhiteList =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890@!$#%*()[]{}:;.-_+=~`".ToCharArray();
        public static char[] StandardInputWhiteList =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()\\\"{}[]:;~`.-_+=\r\n\t".ToCharArray();
    }

    public static class Constants
    {
        private static int __moduleTypeId_Chat__        = 1;
        private static int __moduleTypeId_WhiteBoard__  = 2;
        private static int __moduleTypeId_ImageShare__  = 4;
        private static int __moduleTypeId_LinkShare__   = 8;
        private static int __moduleTypeId_CodeShare__   = 16;
        private static int __moduleTypeId_VideoShare__  = 32;

        public static int MODULE_TYPE_CHAT 
        { 
            get 
            { 
                return __moduleTypeId_Chat__; 
            } 
        }
        public static int MODULE_TYPE_WHITEBOARD
        {
            get 
            {
                return __moduleTypeId_WhiteBoard__;
            }
        }
        public static int MODULE_TYPE_IMAGESHARE
        {
            get
            {
                return __moduleTypeId_ImageShare__;
            }
        }
        public static int MODULE_TYPE_LINKSHARE
        {
            get
            {
                return __moduleTypeId_LinkShare__;
            }
        }
        public static int MODULE_TYPE_CODESHARE
        {
            get
            {
                return __moduleTypeId_CodeShare__;
            }
        }
        public static int MODULETYPE_VIDEOSHARE
        {
            get
            {
                return __moduleTypeId_VideoShare__;
            }
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebSignOn.Application
{
    public class SecurityQuestions
    {
        protected static SelectListItem[] _securityQuestions = null;

        static SecurityQuestions()
        {
            List<SelectListItem> q = new List<SelectListItem>();

            for (int i = 1; i <= 20; i++)
            {
                q.Add(new SelectListItem()
                {
                    Text = String.Format("This is a security question number {0} of 20?", i),
                    Value = i.ToString()
                });
            }

            _securityQuestions = q.ToArray();
        }

        internal static SelectListItem[] Items
        {
            get { return _securityQuestions; }
        }
    }
}
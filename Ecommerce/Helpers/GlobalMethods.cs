using Ecommerce.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ecommerce.Helpers
{
    public static class GlobalMethods
    {
        private static Random random = new Random();
        public static string GenerateRandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static void DontUpdateProperty<TEntity>(this DbContext context, string propertyName)
        {
            var objectContext = ((IObjectContextAdapter)context).ObjectContext;
            foreach (var entry in objectContext.ObjectStateManager.GetObjectStateEntries(EntityState.Modified).Where(entity => entity.Entity.GetType() == typeof(TEntity)))
            {
                entry.RejectPropertyChanges(propertyName);
            }
        }

        public static dynamic get_user(string guid)
        {
            var context = new ApplicationDbContext();
            var user = context.Users.SingleOrDefault(u => u.Id == guid);

            return user;
        }

        public static string Pagination(int count, int per_page, int cur_page, bool first_btn, bool previous_btn, bool next_btn, bool last_btn)
        {
            string pag_navigation = "";

            /* Bellow is the navigation logic and view */
            decimal nop_ceil = Decimal.Divide(count, per_page);
            int no_of_paginations = Convert.ToInt32(Math.Ceiling(nop_ceil));

            var start_loop = 1;
            var end_loop = no_of_paginations;

            if (cur_page >= 7)
            {
                start_loop = cur_page - 3;
                if (no_of_paginations > cur_page + 3)
                {
                    end_loop = cur_page + 3;
                }
                else if (cur_page <= no_of_paginations && cur_page > no_of_paginations - 6)
                {
                    start_loop = no_of_paginations - 6;
                    end_loop = no_of_paginations;
                }
            }
            else
            {
                if (no_of_paginations > 7)
                {
                    end_loop = 7;
                }
            }

            pag_navigation += "<ul>";

            if (first_btn && cur_page > 1)
            {
                pag_navigation += "<li p='1' class='active'>First</li>";
            }
            else if (first_btn)
            {
                pag_navigation += "<li p='1' class='inactive'>First</li>";
            }

            if (previous_btn && cur_page > 1)
            {
                var pre = cur_page - 1;
                pag_navigation += "<li p='" + pre + "' class='active'>Previous</li>";
            }
            else if (previous_btn)
            {
                pag_navigation += "<li class='inactive'>Previous</li>";
            }

            for (int i = start_loop; i <= end_loop; i++)
            {

                if (cur_page == i)
                    pag_navigation += "<li p='" + i + "' class = 'selected' >" + i + "</li>";
                else
                    pag_navigation += "<li p='" + i + "' class='active'>" + i + "</li>";
            }

            if (next_btn && cur_page < no_of_paginations)
            {
                var nex = cur_page + 1;
                pag_navigation += "<li p='" + nex + "' class='active'>Next</li>";
            }
            else if (next_btn)
            {
                pag_navigation += "<li class='inactive'>Next</li>";
            }

            if (last_btn && cur_page < no_of_paginations)
            {
                pag_navigation += "<li p='" + no_of_paginations + "' class='active'>Last</li>";
            }
            else if (last_btn)
            {
                pag_navigation += "<li p='" + no_of_paginations + "' class='inactive'>Last</li>";
            }

            pag_navigation = pag_navigation + "</ul>";

            return pag_navigation;
        }

        public static void MaybeInitializeSession()
        {
            /* Initialize session if not yet set */
            if (SessionSingleton.Current.Cart == null)
            {
                Dictionary<int, string> session_object = new Dictionary<int, string>();
                SessionSingleton.Current.Cart = session_object;
            }
        }
    }
}
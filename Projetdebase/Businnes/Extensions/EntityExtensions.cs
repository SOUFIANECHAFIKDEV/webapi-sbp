using InoAuthentification.Entities;
using InoAuthentification.UserManagers;
using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Extensions
{
    public static class EntityExtensions
    {
        public static List<ModifyEntryModel> GetModification<T>(T origin, T current)
        {

            List<ModifyEntryModel> res = new List<ModifyEntryModel>();
            var entityType = origin.GetType();

            foreach (var attr in entityType.GetProperties())
            {
                var proprieter = entityType.GetProperty(attr.Name);

                var oldvalue = proprieter.GetValue(origin);
                var newvalue = proprieter.GetValue(current);
                if (!(oldvalue is null && newvalue is null))
                {
                    if (oldvalue is null && !(newvalue is null))
                    {
                        res.Add(new ModifyEntryModel()
                        {
                            Attribute = attr.Name,
                            Before = "",
                            After = newvalue.ToString()
                        });
                    }
                    else if (!(oldvalue is null) && newvalue is null)
                    {
                        res.Add(new ModifyEntryModel()
                        {
                            Attribute = attr.Name,
                            Before = oldvalue.ToString(),
                            After = ""
                        });
                    }
                    else if (!proprieter.GetValue(origin).Equals(proprieter.GetValue(current)))
                    {
                        res.Add(new ModifyEntryModel()
                        {
                            Attribute = attr.Name,
                            Before = oldvalue.ToString(),
                            After = newvalue.ToString()
                        });
                    }

                }

            }
            return res;
        }

        public static User GetCurrentUser(ProjetBaseContext projetBase)
        {
            try
            {
                var userManager = new UserManager(projetBase);
                return  userManager.GetCurrentUser();
            }catch(Exception ex)
            {
                Log.Error(ex.Message, ex);
                throw ex;
            }
        }

        public static long UnixTimestampFromDateTime(DateTime? date)
        {
            var dateNow = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now);
            return (Int64)(dateNow.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;
        }


        public static double RoundingDouble(this double nombre)
        {
            try
            {
                var rouding = string.Format("{0:0.00}", nombre);
                return double.Parse(rouding);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return 0;
            }
        }
        public static DateTime GetDateForMonth(int month)
        {
            try
            {
                var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

                if (DateTime.Now.Month == month) // Actual mois
                {
                    date = date.AddDays(-1);
                    return date.Date;
                }
                else // Pour les mois avant 
                {
                    date = date.AddMonths(month);
                    return date.Date;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }



    }
}

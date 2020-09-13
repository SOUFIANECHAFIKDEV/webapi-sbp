using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Calendar.v3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Outils.SynchroAgenda
{
    interface IAgenda
    {
        string AddeventCalendarGroup(string ApplicationName, string ClientId, string ClientSecret, string CalendarId, string titre_event, DateTime? start, DateTime? end);
        void UpdateeventCalendar(string ApplicationName, string ClientId, string ClientSecret, string CalendarId, string titre_event, DateTime? start, DateTime? end, string eventId);
        void DeleteeventCalendar(string ApplicationName, string ClientId, string ClientSecret, string CalendarId, string titre_event, DateTime? start, DateTime? end, string eventId);
        // Google.Apis.Calendar.v3.Data.Event UpdateCalenderEvents(string refreshToken, string emailAddress, string summary, DateTime? start, DateTime? end, string eventId, out string error)
        //IAuthorizationCodeFlow GoogleAuthorizationCodeFlow(out string error);
        //UserCredential GetGoogleUserCredentialByRefreshToken(string refreshToken, out string error);
        //CalendarService GetCalendarService(string refreshToken, out string error);
        //string AddCalenderEvents(string refreshToken, string emailAddress, string summary, DateTime? start, DateTime? end, out string error);
        //Google.Apis.Calendar.v3.Data.Event UpdateCalenderEvents(string refreshToken, string emailAddress, string summary, DateTime? start, DateTime? end, string eventId, out string error);
        //void DeletCalendarEvents(string refreshToken, string emailAddress, string eventId, out string error);

    }
}

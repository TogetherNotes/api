using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace TogetherNotes.Models.Management
{
    public static class ContractsOrm
    {
        public static List<Utils.Event> GetEventsForToday()
        {
            DateTime today = DateTime.Today;
            DateTime tomorrow = today.AddDays(1);

            // Carrega contractes d'avui
            var contracts = Orm.db.contracts
                .Where(c => c.init_hour >= today && c.init_hour < tomorrow)
                .ToList();

            // Carrega tots els usuaris (artistes i espais)
            var apps = Orm.db.app.ToList();

            var events = contracts.Select(c =>
            {
                var artist = apps.FirstOrDefault(a => a.id == c.artist_id);
                var space = apps.FirstOrDefault(a => a.id == c.space_id);

                string artistName = artist != null ? artist.name : "Unknown Artist";
                string spaceName = space != null ? space.name : "Unknown Space";
                string type = c.meet_type;

                string title = $"Event at {c.init_hour.LocalDateTime.ToShortTimeString()} with {artistName} and {spaceName} - {type}";

                return new Utils.Event(c.init_hour, title);

            }).ToList();

            return events;
        }

        public static List<object> GetEventsByDate(DateTime selectedDate)
        {
            try
            {
                var events = Orm.db.contracts
                    .Select(e => new
                    {
                        e.init_hour,
                        e.meet_type
                    })
                    .ToList(); 

               
                return events
                    .Where(e => e.init_hour.Date == selectedDate.Date)
                    .Select(e => new
                    {
                        Time = e.init_hour.ToString("HH:mm"),
                        Title = e.meet_type
                    })
                    .ToList<object>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetEventsByDate: " + ex.Message);
                return new List<object>();
            }
        }
    }
}
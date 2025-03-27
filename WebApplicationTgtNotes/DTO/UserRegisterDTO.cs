using System.Collections.Generic;

namespace WebApplicationTgtNotes.DTO
{
    public class UserRegisterDTO // DTO = Data Transfer Object
    {
        public string name { get; set; }
        public string mail { get; set; }
        public string password { get; set; }
        public string role { get; set; } = "artist";
        public int? rating { get; set; }
        public decimal? latitude { get; set; }
        public decimal? longitude { get; set; }
        public bool active { get; set; }
        public int? language_id { get; set; }

        // Artists
        public List<int> genre_ids { get; set; }

        // Spaces
        public int? capacity { get; set; }
        public string zip_code { get; set; }
    }
}
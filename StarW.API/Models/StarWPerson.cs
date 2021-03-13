using System.ComponentModel.DataAnnotations;

namespace StarW.API.Models
{
    public class StarWPerson
    {
        /// <summary>
        /// The Id of the person
        /// </summary>
        /// <example>11</example>
        [Required]
        public string Id { get; set; }
        /// <summary>
        /// The name of the person
        /// </summary>
        /// <example>Luke Skywalker</example>
        [Required]
        public string Name { get; set; }
        [Required]
        public string Height { get; set; }
        public string BirthYear { get; set; }
    }
}

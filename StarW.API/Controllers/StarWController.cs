using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StarW.API.Models;

namespace StarW.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StarWController : ControllerBase
    {
        private readonly string mainUrl = "https://swapi.dev/api";
        private readonly HttpClient _httpClient;

        public StarWController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Gets all the people from Swapi
        /// </summary>
        /// <remarks>
        /// Sample respons:
        ///
        ///     Get /StarW
        ///     {
        ///         "id": "1",
        ///         "Name: "Luke Skywalker",
        ///         "Height": "172",
        ///         "BirthYear": "19BBY"
        ///     }
        ///
        /// </remarks>
        /// <returns>List of Starwars persons</returns>
        /// <response code="200">Returns a list of Starwars persons</response>
        /// <response code="400">If the request is in awrong format</response>
        /// <response code="404">If the requested url is not found</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get()
        {
            var peopleUrl = $"{mainUrl}/people/";

            //peopleUrl += (pageNumber != null) ? $"?page={pageNumber}" : "";

            try
            {
                var response = await _httpClient.GetFromJsonAsync<SwapiPeople>(peopleUrl);

                if (response == null)
                {
                    return NotFound();
                }

                var allPeople = new List<StarWPerson>();

                for (var pageCount = 1; response.next != null; pageCount++)
                {
                    var peoplesPageUrl = peopleUrl + $"?page={pageCount}";
                    response = await _httpClient.GetFromJsonAsync<SwapiPeople>(peoplesPageUrl);

                    foreach (var person in response.results)
                    {

                        var personUrl = person.url.TrimEnd('/');
                        var personId = personUrl.Substring(personUrl.LastIndexOf('/') + 1);

                        StarWPerson newPerson = new()
                        {
                            Id = personId,
                            Name = person.name,
                            BirthYear = person.birth_year,
                            Height = person.height
                        };

                        allPeople.Add(newPerson);
                    }
                }

                return Ok(allPeople);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Gets a person from Swapi
        /// </summary>
        /// <remarks>
        /// Sample respons:
        ///
        ///     Get /StarW/{id}
        ///     {
        ///         "id": "1",
        ///         "Name: "Luke Skywalker",
        ///         "Height": "172",
        ///         "BirthYear": "19BBY"
        ///     }
        ///
        /// </remarks>
        /// <param name="id"></param>
        /// <returns>Person</returns>
        /// /// <response code="200">Returns a Starwars person</response>
        /// <response code="400">If the request is in awrong format</response>
        /// <response code="404">If the requested id is not found</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPerson([FromRoute]int id)
        {
            var peopleUrl = $"{mainUrl}/people/";

            peopleUrl += $"{id}/";

            try
            {
                var response = await _httpClient.GetFromJsonAsync<PeopleResult>(peopleUrl);

                if (response == null)
                {
                    return NotFound($"Person with id: {id} was not found!");
                }

                StarWPerson person = new()
                {
                    Id = id.ToString(),
                    Name = response.name,
                    BirthYear = response.birth_year,
                    Height = response.height
                };

                return Ok(person);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

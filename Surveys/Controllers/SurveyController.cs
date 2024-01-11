using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Surveys.Context;
using Surveys.Models;

namespace Surveys.Controllers
{
    [ApiController]
    [Route("api/surveys")]
    public class SurveyController : Controller
    {
        private readonly AppDbContext _contextDb;
        public SurveyController(AppDbContext context)
        {
            _contextDb = context;
        }

        [HttpPost("form")]
        [Authorize]
        public async Task<ActionResult<dynamic>> CreateForm(Survey surveyModel)
        {
            var newSurvey = new Survey
            {
                Name = surveyModel.Name,
                Description = surveyModel.Description
            };

            _contextDb.Survey.Add(newSurvey);
            await _contextDb.SaveChangesAsync();

            long newSurveyId = newSurvey.Id;

            if(surveyModel.Details == null)
            {
                return BadRequest("El detalle del formulario no puede venir vacio");
            }

            var surveyDetails = surveyModel.Details.Select(detail => new SurveyDetail
            {
                SurveyId = newSurveyId,
                Field = detail.Field
            }).ToList();

            _contextDb.SurveyDetail.AddRange(surveyDetails);
            await _contextDb.SaveChangesAsync();

            var host = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

            return Ok(new
            {
                message = "Formulario generado satisfactoriamente",
                result = $"{host}/api/surveys/{newSurveyId}/responses"
            });
        }

        [HttpPut("form")]
        [Authorize]
        public async Task<ActionResult<Survey>> UpdateForm(Survey surveyModel)
        {
            return Ok();
        }

        [HttpDelete("form/{id}")]
        [Authorize]
        public async Task<ActionResult<Survey>> DeleteForm([FromRoute] long id)
        {
            return Ok();
        }

        [HttpGet("{id}/responses")]
        [Authorize]
        public async Task<ActionResult<dynamic>> GetResponses([FromRoute] long id)
        {
            Survey survey = _contextDb.Survey.Find(id);

            if(survey == null)
            {
                return NotFound("Encuesta no encontrada");
            }

            List<SurveyDetailResponse> surveyDetailResponse = _contextDb.SurveyDetailResponse.Where(detail => detail.SurveyId == survey.Id).ToList();

            if(surveyDetailResponse == null || surveyDetailResponse.Count() == 0)
            {
                return NoContent();
            }

            List<dynamic> responsesList = new List<dynamic>();
            foreach (var surveyResponse in surveyDetailResponse)
            {
                SurveyDetail detail = _contextDb.SurveyDetail.Find(surveyResponse.SurveyDetailId);
                Field field = _contextDb.Field.Find(detail.FieldId);
                var value = surveyResponse.DateValue;
                if (field != null)
                {
                    DataType dataType = _contextDb.DataType.Find(field.DataTypeId);
                    var response = new
                    {
                        fieldName = field.Name,
                        fieldValue = getDynamicSurveyValue(surveyResponse, dataType)
                    };
                    responsesList.Add(response);
                }
            }

            return Ok(new
            {
                survey = survey.Id,
                surveyName = survey.Name,
                responses = responsesList
            });
        }

        private dynamic getDynamicSurveyValue(SurveyDetailResponse surveyResponse, DataType dataType)
        {
            if (dataType != null)
            {
                if (dataType.Description == "Texto") return surveyResponse.TextValue;
                else if (dataType.Description == "Numero") return surveyResponse.NumValue;
                else if (dataType.Description == "Fecha") return surveyResponse.DateValue;
            }
            return "";
        }

        [HttpPost("{id}/responses")]
        public async Task<ActionResult<dynamic>> CreateResponseForm([FromRoute] long id, List<SurveyDetailResponse> responsesModel)
        {
            return Ok();
        }
    }
}

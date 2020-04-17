using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CMS.Personas;
using DeleteBoilerplate.Domain.Repositories;
using DeleteBoilerplate.Infrastructure.Models.Personalization.ConditionTypes.IsInPersona;
using DeleteBoilerplate.Infrastructure.Personalization.ConditionTypes;
using Kentico.PageBuilder.Web.Mvc.Personalization;
using LightInject;

namespace DeleteBoilerplate.Infrastructure.Controllers.Personalization.ConditionTypes
{
    public class IsInPersonaController : ConditionTypeController<IsInPersonaConditionType>
    {
        [Inject]
        protected IPersonaRepository PersonaRepository { get; set; }

        [Inject]
        protected IPersonaPictureUrlCreator PictureCreator { get; set; }

        public IsInPersonaController(IPersonaRepository personaRepository, IPersonaPictureUrlCreator pictureCreator)
        {
            PersonaRepository = personaRepository;
            PictureCreator = pictureCreator;
        }

        [HttpPost]
        public ActionResult Index()
        {
            var conditionTypeParameters = GetParameters();
   
            var viewModel = new IsInPersonaViewModel
            {
                PersonaCodeName = conditionTypeParameters.PersonaName,
                AllPersonas = GetAllPersonas(conditionTypeParameters.PersonaName)
            };
            return PartialView("Personalization/ConditionTypes/_IsInPersonaConfiguration", viewModel);
        }

        [HttpPost]
        public ActionResult Validate(IsInPersonaViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.AllPersonas = GetAllPersonas();
                return PartialView("Personalization/ConditionTypes/_IsInPersonaConfiguration", viewModel);
            }

            var parameters = new IsInPersonaConditionType
            {
                PersonaName = viewModel.PersonaCodeName
            };

            return Json(parameters);
        }

        private List<IsInPersonaListItemViewModel> GetAllPersonas(string selectedPersonaName = "")
        {
            var allPersonas =  PersonaRepository.GetAll().Select(persona => new IsInPersonaListItemViewModel
            {
                CodeName = persona.PersonaName,
                DisplayName = persona.PersonaDisplayName,
                ImagePath = PictureCreator.CreatePersonaPictureUrl(persona, 50),
                Selected = persona.PersonaName == selectedPersonaName
            }).ToList();

            if (allPersonas.Count > 0 && !allPersonas.Exists(x => x.Selected))
            {
                allPersonas.First().Selected = true;
            }

            return allPersonas;
        }
    }
}
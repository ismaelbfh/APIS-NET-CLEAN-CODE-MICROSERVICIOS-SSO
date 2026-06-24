using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Goikoa.Domain.Navision.Producccion.DTOs.Requests;
using FluentValidation;

namespace Goikoa.Domain.Navision.Producccion.DTOs.Validators
{
    public class OrdenProdRequestValidator : AbstractValidator<GKOrdenProdRequest>
    {
        public OrdenProdRequestValidator()
        {
            RuleFor(x => x.pLine)
                .NotEmpty().WithMessage("El campo pLine es obligatorio.");

            RuleFor(x => x.pTipoDestino)
                .NotEqual(0).WithMessage("El campo pTipoDestino es obligatorio y debe ser distinto de 0.");

            RuleFor(x => x.pStartingDate)
                .NotEqual(default(DateTime)).WithMessage("El campo pStartingDate es obligatorio.");
        }
    }
}

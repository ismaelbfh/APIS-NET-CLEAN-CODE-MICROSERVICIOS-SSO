using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Goikoa.Domain.Navision.Producccion.DTOs.Requests;
using FluentValidation;

namespace Goikoa.Domain.Navision.Producccion.DTOs.Validators
{
    public class ProductoRequestValidator : AbstractValidator<GKProductoRequest>
    {
        public ProductoRequestValidator() 
        {
            RuleFor(x => x.pNumProduct)
                .NotEmpty().WithMessage("El campo pNumProduct es obligatorio.");

            RuleFor(x => x.pTipoCodigoBarras)
                .NotEmpty().WithMessage("El campo pTipoCodigoBarras es obligatorio.");

        }
    }
}

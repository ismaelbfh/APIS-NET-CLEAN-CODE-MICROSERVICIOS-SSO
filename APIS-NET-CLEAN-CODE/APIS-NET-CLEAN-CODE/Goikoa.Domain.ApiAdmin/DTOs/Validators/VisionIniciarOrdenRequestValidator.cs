using FluentValidation;
using Goikoa.Domain.ApiAdmin.DTOs.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goikoa.Domain.ApiAdmin.DTOs.Validators
{
    public class VisionIniciarOrdenRequestValidator : AbstractValidator<VisionIniciarOrdenRequest>
    {
        public VisionIniciarOrdenRequestValidator()
        {
            RuleFor(x => x.OrdenFabricacion)
                .NotEmpty().WithMessage("La orden de fabricación es obligatoria.")
                .MaximumLength(50);

            RuleFor(x => x.CodigoProducto)
                .NotEmpty().WithMessage("El código de producto es obligatorio.")
                .MaximumLength(50);

            RuleFor(x => x.DescripcionProducto)
                .NotEmpty().WithMessage("La descripción de producto es obligatoria.")
                .MaximumLength(200);

            RuleFor(x => x.TipoEtiqueta)
                .NotEmpty().WithMessage("El tipo de etiqueta es obligatorio.")
                .Must(x => x == "EAN" || x == "QR")
                .WithMessage("El tipo de etiqueta debe ser 'EAN' o 'QR'.");

            RuleFor(x => x.PosicionQr)
                .Must(x => string.IsNullOrWhiteSpace(x) || x == "Superior" || x == "Inferior")
                .WithMessage("La posición QR debe ser 'Superior', 'Inferior' o nula.");

            RuleFor(x => x.CodigoEsperado)
                .NotEmpty().WithMessage("El código esperado es obligatorio.")
                .MaximumLength(300);
        }
    }
}

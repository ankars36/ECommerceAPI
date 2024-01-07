using ECommerceAPI.Application.ViewModels.Products;
using FluentValidation;

namespace ECommerceAPI.Application.Validators.Products
{
    public class CreateProductValidator : AbstractValidator<VM_Create_Product>
    {
        public CreateProductValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty()
                .NotNull()
                    .WithMessage("Please fill in the name field!")
                .MinimumLength(5)
                .MaximumLength(150)
                    .WithMessage("Name length should be within 5 and 150!");

            RuleFor(p => p.Stock)
                .NotEmpty()
                .NotNull()
                    .WithMessage("Please fill in the stock field!")
                .Must(s => s >= 0)
                    .WithMessage("Stock should be positive!");

            RuleFor(p => p.Price)
                .NotEmpty()
                .NotNull()
                    .WithMessage("Please fill in the price field!")
                .Must(s => s >= 0)
                    .WithMessage("Price should be positive!");
        }
    }
}

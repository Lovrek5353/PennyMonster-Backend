using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PennyMonster.Data;
using PennyMonster.DTOs;

namespace PennyMonster.Validators;

public class TransactionCreateDtoValidator : AbstractValidator<TransactionCreateDto>
{
    private readonly PennyMonsterContext _context;
    public TransactionCreateDtoValidator(PennyMonsterContext context)
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than zero. (Even PennyMonster can't process a $0 transaction!)");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Please provide a short description for this transaction.")
            .MaximumLength(100).WithMessage("Description is too long. Keep it under 100 characters.");

        RuleFor(x => x.Date)
            .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1)) 
            .WithMessage("Transaction date cannot be in the future.");

        RuleFor(x => x.TransactionType)
            .IsInEnum()
            .WithMessage("Invalid transaction type selected.");

        RuleFor(x => x.CategoryId)
            .MustAsync(async (dto, categoryId, cancellationToken) =>
            {
                return await _context.Categories
                    .AnyAsync(c => c.Id == categoryId.Value && c.UserId == dto.UserId, cancellationToken);
            })
            .When(dto => dto.CategoryId.HasValue)
            .WithMessage("The selected category does not exist or does not belong to you.");
    }
}
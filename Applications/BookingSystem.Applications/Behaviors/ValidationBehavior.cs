using FluentValidation;
using MediatR;

namespace BookingSystem.Applications.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // اگر Validator برای این درخواست وجود نداشت، ادامه بده
        if (!_validators.Any())
        {
            return await next();
        }

        // اجرای تمام Validatorهای مرتبط به صورت ناهمزمان
        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        // جمع آوری تمام خطاهای اعتبارسنجی
        var failures = validationResults
            .Where(r => r.Errors.Any())
            .SelectMany(r => r.Errors)
            .ToList();

        // اگر خطا داشتیم، یک استثنا با جزئیات خطا پرتاب کن
        if (failures.Any())
        {
            // این خطا را کنترلر در API دریافت و تبدیل به 400 Bad Request می‌کند
            throw new ValidationException(failures);
        }

        // اگر خطایی نبود، درخواست را به Handler اصلی پاس بده
        return await next();
    }
}
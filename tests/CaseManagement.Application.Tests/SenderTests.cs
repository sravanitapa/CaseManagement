using CaseManagement.Application.Common.Behaviors;
using CaseManagement.Application.Common.Messaging;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CaseManagement.Application.Tests;

public class SenderTests
{
    public sealed record Ping(string Text) : IRequest<string>;

    public sealed record Orphan : IRequest<int>;

    public sealed class PingHandler : IRequestHandler<Ping, string>
    {
        public Task<string> Handle(Ping request, CancellationToken cancellationToken)
            => Task.FromResult($"pong:{request.Text}");
    }

    public sealed class Recorder
    {
        public List<string> Calls { get; } = [];
    }

    public sealed class FirstBehavior(Recorder recorder) : IPipelineBehavior<Ping, string>
    {
        public async Task<string> Handle(Ping request, RequestHandlerDelegate<string> next, CancellationToken cancellationToken)
        {
            recorder.Calls.Add("first-before");
            var result = await next();
            recorder.Calls.Add("first-after");
            return result;
        }
    }

    public sealed class SecondBehavior(Recorder recorder) : IPipelineBehavior<Ping, string>
    {
        public async Task<string> Handle(Ping request, RequestHandlerDelegate<string> next, CancellationToken cancellationToken)
        {
            recorder.Calls.Add("second-before");
            var result = await next();
            recorder.Calls.Add("second-after");
            return result;
        }
    }

    public sealed class PingValidator : AbstractValidator<Ping>
    {
        public PingValidator() => RuleFor(x => x.Text).NotEmpty();
    }

    private static ServiceProvider BuildProvider(Recorder? recorder = null, bool withBehaviors = false)
    {
        var services = new ServiceCollection();
        services.AddMessaging(typeof(SenderTests).Assembly);

        if (withBehaviors)
        {
            services.AddSingleton(recorder ?? new Recorder());
            services.AddScoped<IPipelineBehavior<Ping, string>, FirstBehavior>();
            services.AddScoped<IPipelineBehavior<Ping, string>, SecondBehavior>();
        }

        return services.BuildServiceProvider();
    }

    [Fact]
    public async Task Send_ResolvesTheHandler_AndReturnsItsResponse()
    {
        await using var provider = BuildProvider();
        var sender = provider.GetRequiredService<ISender>();

        var result = await sender.Send(new Ping("hello"));

        Assert.Equal("pong:hello", result);
    }

    [Fact]
    public async Task Send_RunsBehaviorsInRegistrationOrder_FirstRegisteredIsOutermost()
    {
        var recorder = new Recorder();
        await using var provider = BuildProvider(recorder, withBehaviors: true);
        var sender = provider.GetRequiredService<ISender>();

        await sender.Send(new Ping("x"));

        Assert.Equal(
            new[] { "first-before", "second-before", "second-after", "first-after" },
            recorder.Calls.ToArray());
    }

    [Fact]
    public async Task Send_WithNoRegisteredHandler_Throws()
    {
        await using var provider = BuildProvider();
        var sender = provider.GetRequiredService<ISender>();

        await Assert.ThrowsAsync<InvalidOperationException>(() => sender.Send(new Orphan()));
    }

    [Fact]
    public async Task ValidationBehavior_WithInvalidRequest_ThrowsAndSkipsTheHandler()
    {
        var behavior = new ValidationBehavior<Ping, string>(new IValidator<Ping>[] { new PingValidator() });
        var handlerWasCalled = false;

        await Assert.ThrowsAsync<ValidationException>(() =>
            behavior.Handle(new Ping(""), () =>
            {
                handlerWasCalled = true;
                return Task.FromResult("never");
            }, CancellationToken.None));

        Assert.False(handlerWasCalled);
    }

    [Fact]
    public async Task ValidationBehavior_WithValidRequest_CallsTheHandler()
    {
        var behavior = new ValidationBehavior<Ping, string>(new IValidator<Ping>[] { new PingValidator() });

        var result = await behavior.Handle(new Ping("ok"), () => Task.FromResult("done"), CancellationToken.None);

        Assert.Equal("done", result);
    }
}

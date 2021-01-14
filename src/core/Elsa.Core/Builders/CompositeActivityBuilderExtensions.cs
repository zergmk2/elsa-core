using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Elsa.Activities.Primitives;
using Elsa.ActivityResults;
using Elsa.Services.Models;

// ReSharper disable ExplicitCallerInfoArgument

namespace Elsa.Builders
{
    public static class CompositeActivityBuilderExtensions
    {
        public static IActivityBuilder StartWith(this ICompositeActivityBuilder builder, Action action, [CallerLineNumber] int lineNumber = default, [CallerFilePath] string? sourceFile = default) =>
            builder.StartWith<Inline>(inline => inline.Set(x => x.Function, RunInline(action)), null, lineNumber, sourceFile);

        public static IActivityBuilder
            StartWith(this ICompositeActivityBuilder builder, Action<ActivityExecutionContext> action, [CallerLineNumber] int lineNumber = default, [CallerFilePath] string? sourceFile = default) =>
            builder.StartWith<Inline>(inline => inline.Set(x => x.Function, RunInline(action)), null, lineNumber, sourceFile);

        public static IActivityBuilder StartWith(
            this ICompositeActivityBuilder builder,
            Func<ActivityExecutionContext, ValueTask> action,
            [CallerLineNumber] int lineNumber = default,
            [CallerFilePath] string? sourceFile = default) =>
            builder.StartWith<Inline>(inline => inline.Set(x => x.Function, RunInline(action)), null, lineNumber, sourceFile);

        public static IActivityBuilder StartWith(
            this ICompositeActivityBuilder builder,
            Func<ActivityExecutionContext, ValueTask<IActivityExecutionResult>> action,
            [CallerLineNumber] int lineNumber = default,
            [CallerFilePath] string? sourceFile = default) =>
            builder.StartWith<Inline>(inline => inline.Set(x => x.Function, RunInline(action)), null, lineNumber, sourceFile);

        public static IActivityBuilder SetVariable(this ICompositeActivityBuilder builder, string variableName, object? value, [CallerLineNumber] int lineNumber = default, [CallerFilePath] string? sourceFile = default) =>
            builder.StartWith<SetVariable>(
                activity =>
                {
                    activity.Set(x => x.VariableName, _ => variableName);
                    activity.Set(x => x.Value, _ => value);
                }, null, lineNumber, sourceFile);

        public static IActivityBuilder SetVariable(
            this ICompositeActivityBuilder builder,
            string variableName,
            Func<object?> value,
            [CallerLineNumber] int lineNumber = default,
            [CallerFilePath] string? sourceFile = default) =>
            builder.SetVariable(variableName, value(), lineNumber, sourceFile);

        private static Func<ActivityExecutionContext, ValueTask<IActivityExecutionResult>> RunInline(Action action) =>
            _ =>
            {
                action();
                return new ValueTask<IActivityExecutionResult>(new DoneResult());
            };

        private static Func<ActivityExecutionContext, ValueTask<IActivityExecutionResult>> RunInline(Action<ActivityExecutionContext> action) =>
            context =>
            {
                action(context);
                return new ValueTask<IActivityExecutionResult>(new DoneResult());
            };

        private static Func<ActivityExecutionContext, ValueTask<IActivityExecutionResult>> RunInline(Func<ActivityExecutionContext, ValueTask> action) =>
            async context =>
            {
                await action(context);
                return new DoneResult();
            };

        private static Func<ActivityExecutionContext, ValueTask<IActivityExecutionResult>> RunInline(Func<ActivityExecutionContext, ValueTask<IActivityExecutionResult>> action) => async context => await action(context);
    }
}
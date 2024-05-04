// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using Microsoft.Extensions.Configuration;
using Microsoft.FeatureManagement;

namespace ConsoleApp.FeatureFilters;

/// <summary>
/// A filter that uses the feature management context to ensure that the current task has the notion of an username, and that username is allowed.
/// This filter will only be executed if an object implementing <see cref="IUsernameContext"/> is passed in during feature evaluation.
/// </summary>
[FilterAlias(Name)]
internal class UsernameFilter : IContextualFeatureFilter<IUsernameContext>
{
    internal const string Name = nameof(UsernameFilter);

    public Task<bool> EvaluateAsync(FeatureFilterEvaluationContext featureFilterContext, IUsernameContext usernameContext)
    {
        if (string.IsNullOrEmpty(usernameContext?.Username))
            throw new ArgumentNullException(nameof(usernameContext));

        List<string> allowedUsernames = [];

        featureFilterContext.Parameters.Bind("AllowedUsernames", allowedUsernames);

        return Task.FromResult(allowedUsernames.Contains(usernameContext.Username));
    }
}
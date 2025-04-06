// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using Docker.DotNet;
using Docker.DotNet.Models;

namespace FeatureManagement.Database.Common.Utilities;

public static class DockerContainerHelper
{
    // Used just to avoid conflicts with other containers in GitHub Actions environment
    public static async Task RemoveExistingContainerAsync(string containerName)
    {
#pragma warning disable S1075 // URIs should not be hardcoded: tests purposes
        using var client = new DockerClientConfiguration(new Uri("unix:///var/run/docker.sock")).CreateClient();
#pragma warning restore S1075 // URIs should not be hardcoded
        var containers = await client.Containers.ListContainersAsync(new ContainersListParameters { All = true });

        var existingContainer = containers.FirstOrDefault(c => c.Names.Contains($"/{containerName}"));
        if (existingContainer is not null)
        {
            await client.Containers.RemoveContainerAsync(existingContainer.ID, new ContainerRemoveParameters { Force = true });
        }
    }
}
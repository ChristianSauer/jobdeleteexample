using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.ResponseCaching.Internal;
using Microsoft.Azure.Batch;
using Microsoft.Azure.Batch.Auth;
using Microsoft.Azure.Batch.Common;
using Microsoft.Extensions.Hosting;

namespace JobDeleteExample
{
    public class DeleterService: BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var accountUrl = "***";
            var accountName = "***";
            var accountSecret = "**";
            var jobtoDelete = "as_1bf795a45fda4ad8ab0f79bc24a942eb";
            var sharedKeyCredentials = new BatchSharedKeyCredentials(accountUrl, accountName, accountSecret);

            var imageReference = new ImageReference(
                publisher: "microsoft-azure-batch",
                offer: "ubuntu-server-container",
                sku: "16-04-lts",
                version: "latest");

            var virtualMachineConfiguration = new VirtualMachineConfiguration(
                imageReference,
                "batch.node.ubuntu 16.04");

            using (var bc = await BatchClient.OpenAsync(sharedKeyCredentials))
            {
                try
                {
                    var newJob = bc.JobOperations.CreateJob("test", new PoolInformation()
                    {
                        AutoPoolSpecification = new AutoPoolSpecification()
                        {
                            AutoPoolIdPrefix = "test",
                            PoolLifetimeOption = PoolLifetimeOption.Job,
                            KeepAlive = false,
                            PoolSpecification = new PoolSpecification()
                            {
                                TargetDedicatedComputeNodes = 1,
                                VirtualMachineSize = "Standard_A1_v2",
                                VirtualMachineConfiguration = virtualMachineConfiguration
                            },
                        }
                    });

                    newJob.Commit();

                    // never works fails with forbidden
                    await bc.JobOperations.DeleteJobAsync(newJob.Id, cancellationToken: stoppingToken);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Could not delete the job because {e}");
                    throw;
                }

                Console.WriteLine("Could delete job!");
            }
        }
    }
}

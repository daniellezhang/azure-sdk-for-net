// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Core.Pipeline;
using Azure.ResourceManager;
using Azure.ResourceManager.Core;

namespace Azure.ResourceManager.Storage
{
    /// <summary> A class representing collection of FileService and their operations over a StorageAccount. </summary>
    public partial class FileServiceCollection : ArmCollection, IEnumerable<FileService>
    {
        private readonly ClientDiagnostics _clientDiagnostics;
        private readonly FileServicesRestOperations _restClient;

        /// <summary> Initializes a new instance of the <see cref="FileServiceCollection"/> class for mocking. </summary>
        protected FileServiceCollection()
        {
        }

        /// <summary> Initializes a new instance of FileServiceCollection class. </summary>
        /// <param name="parent"> The resource representing the parent resource. </param>
        internal FileServiceCollection(ArmResource parent) : base(parent)
        {
            _clientDiagnostics = new ClientDiagnostics(ClientOptions);
            _restClient = new FileServicesRestOperations(_clientDiagnostics, Pipeline, ClientOptions, Id.SubscriptionId, BaseUri);
        }

        IEnumerator<FileService> IEnumerable<FileService>.GetEnumerator()
        {
            return GetAll().Value.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetAll().Value.GetEnumerator();
        }

        /// <summary> Gets the valid resource type for this object. </summary>
        protected override ResourceType ValidResourceType => StorageAccount.ResourceType;

        // Collection level operations.

        /// <summary> Gets details for this resource from the service. </summary>
        /// <param name="fileServicesName"> The name of the file Service within the specified storage account. File Service Name must be &quot;default&quot;. </param>
        /// <param name="cancellationToken"> A token to allow the caller to cancel the call to the service. The default value is <see cref="CancellationToken.None" />. </param>
        public virtual Response<FileService> Get(string fileServicesName, CancellationToken cancellationToken = default)
        {
            using var scope = _clientDiagnostics.CreateScope("FileServiceCollection.Get");
            scope.Start();
            try
            {
                if (fileServicesName == null)
                {
                    throw new ArgumentNullException(nameof(fileServicesName));
                }

                var response = _restClient.GetServiceProperties(Id.ResourceGroupName, Id.Name, fileServicesName, cancellationToken: cancellationToken);
                if (response.Value == null)
                    throw _clientDiagnostics.CreateRequestFailedException(response.GetRawResponse());
                return Response.FromValue(new FileService(Parent, response.Value), response.GetRawResponse());
            }
            catch (Exception e)
            {
                scope.Failed(e);
                throw;
            }
        }

        /// <summary> Gets details for this resource from the service. </summary>
        /// <param name="fileServicesName"> The name of the file Service within the specified storage account. File Service Name must be &quot;default&quot;. </param>
        /// <param name="cancellationToken"> A token to allow the caller to cancel the call to the service. The default value is <see cref="CancellationToken.None" />. </param>
        public async virtual Task<Response<FileService>> GetAsync(string fileServicesName, CancellationToken cancellationToken = default)
        {
            using var scope = _clientDiagnostics.CreateScope("FileServiceCollection.Get");
            scope.Start();
            try
            {
                if (fileServicesName == null)
                {
                    throw new ArgumentNullException(nameof(fileServicesName));
                }

                var response = await _restClient.GetServicePropertiesAsync(Id.ResourceGroupName, Id.Name, fileServicesName, cancellationToken: cancellationToken).ConfigureAwait(false);
                if (response.Value == null)
                    throw await _clientDiagnostics.CreateRequestFailedExceptionAsync(response.GetRawResponse()).ConfigureAwait(false);
                return Response.FromValue(new FileService(Parent, response.Value), response.GetRawResponse());
            }
            catch (Exception e)
            {
                scope.Failed(e);
                throw;
            }
        }

        /// <summary> Tries to get details for this resource from the service. </summary>
        /// <param name="fileServicesName"> The name of the file Service within the specified storage account. File Service Name must be &quot;default&quot;. </param>
        /// <param name="cancellationToken"> A token to allow the caller to cancel the call to the service. The default value is <see cref="CancellationToken.None" />. </param>
        public virtual Response<FileService> GetIfExists(string fileServicesName, CancellationToken cancellationToken = default)
        {
            using var scope = _clientDiagnostics.CreateScope("FileServiceCollection.GetIfExists");
            scope.Start();
            try
            {
                if (fileServicesName == null)
                {
                    throw new ArgumentNullException(nameof(fileServicesName));
                }

                var response = _restClient.GetServiceProperties(Id.ResourceGroupName, Id.Name, fileServicesName, cancellationToken: cancellationToken);
                return response.Value == null
                    ? Response.FromValue<FileService>(null, response.GetRawResponse())
                    : Response.FromValue(new FileService(this, response.Value), response.GetRawResponse());
            }
            catch (Exception e)
            {
                scope.Failed(e);
                throw;
            }
        }

        /// <summary> Tries to get details for this resource from the service. </summary>
        /// <param name="fileServicesName"> The name of the file Service within the specified storage account. File Service Name must be &quot;default&quot;. </param>
        /// <param name="cancellationToken"> A token to allow the caller to cancel the call to the service. The default value is <see cref="CancellationToken.None" />. </param>
        public async virtual Task<Response<FileService>> GetIfExistsAsync(string fileServicesName, CancellationToken cancellationToken = default)
        {
            using var scope = _clientDiagnostics.CreateScope("FileServiceCollection.GetIfExists");
            scope.Start();
            try
            {
                if (fileServicesName == null)
                {
                    throw new ArgumentNullException(nameof(fileServicesName));
                }

                var response = await _restClient.GetServicePropertiesAsync(Id.ResourceGroupName, Id.Name, fileServicesName, cancellationToken: cancellationToken).ConfigureAwait(false);
                return response.Value == null
                    ? Response.FromValue<FileService>(null, response.GetRawResponse())
                    : Response.FromValue(new FileService(this, response.Value), response.GetRawResponse());
            }
            catch (Exception e)
            {
                scope.Failed(e);
                throw;
            }
        }

        /// <summary> Tries to get details for this resource from the service. </summary>
        /// <param name="fileServicesName"> The name of the file Service within the specified storage account. File Service Name must be &quot;default&quot;. </param>
        /// <param name="cancellationToken"> A token to allow the caller to cancel the call to the service. The default value is <see cref="CancellationToken.None" />. </param>
        public virtual Response<bool> CheckIfExists(string fileServicesName, CancellationToken cancellationToken = default)
        {
            using var scope = _clientDiagnostics.CreateScope("FileServiceCollection.CheckIfExists");
            scope.Start();
            try
            {
                if (fileServicesName == null)
                {
                    throw new ArgumentNullException(nameof(fileServicesName));
                }

                var response = GetIfExists(fileServicesName, cancellationToken: cancellationToken);
                return Response.FromValue(response.Value != null, response.GetRawResponse());
            }
            catch (Exception e)
            {
                scope.Failed(e);
                throw;
            }
        }

        /// <summary> Tries to get details for this resource from the service. </summary>
        /// <param name="fileServicesName"> The name of the file Service within the specified storage account. File Service Name must be &quot;default&quot;. </param>
        /// <param name="cancellationToken"> A token to allow the caller to cancel the call to the service. The default value is <see cref="CancellationToken.None" />. </param>
        public async virtual Task<Response<bool>> CheckIfExistsAsync(string fileServicesName, CancellationToken cancellationToken = default)
        {
            using var scope = _clientDiagnostics.CreateScope("FileServiceCollection.CheckIfExists");
            scope.Start();
            try
            {
                if (fileServicesName == null)
                {
                    throw new ArgumentNullException(nameof(fileServicesName));
                }

                var response = await GetIfExistsAsync(fileServicesName, cancellationToken: cancellationToken).ConfigureAwait(false);
                return Response.FromValue(response.Value != null, response.GetRawResponse());
            }
            catch (Exception e)
            {
                scope.Failed(e);
                throw;
            }
        }

        /// <summary> List all file services in storage accounts. </summary>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual async Task<Response<IReadOnlyList<FileService>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            using var scope = _clientDiagnostics.CreateScope("FileServiceCollection.GetAll");
            scope.Start();
            try
            {
                var response = await _restClient.GetAllAsync(Id.ResourceGroupName, Id.Name, cancellationToken).ConfigureAwait(false);
                return Response.FromValue(response.Value.Value.Select(data => new FileService(Parent, data)).ToArray() as IReadOnlyList<FileService>, response.GetRawResponse());
            }
            catch (Exception e)
            {
                scope.Failed(e);
                throw;
            }
        }

        /// <summary> List all file services in storage accounts. </summary>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Response<IReadOnlyList<FileService>> GetAll(CancellationToken cancellationToken = default)
        {
            using var scope = _clientDiagnostics.CreateScope("FileServiceCollection.GetAll");
            scope.Start();
            try
            {
                var response = _restClient.GetAll(Id.ResourceGroupName, Id.Name, cancellationToken);
                return Response.FromValue(response.Value.Value.Select(data => new FileService(Parent, data)).ToArray() as IReadOnlyList<FileService>, response.GetRawResponse());
            }
            catch (Exception e)
            {
                scope.Failed(e);
                throw;
            }
        }

        // Builders.
        // public ArmBuilder<ResourceIdentifier, FileService, FileServiceData> Construct() { }
    }
}

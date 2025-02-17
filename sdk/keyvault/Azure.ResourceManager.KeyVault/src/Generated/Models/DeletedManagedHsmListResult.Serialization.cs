// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Collections.Generic;
using System.Text.Json;
using Azure.Core;

namespace Azure.ResourceManager.KeyVault.Models
{
    internal partial class DeletedManagedHsmListResult
    {
        internal static DeletedManagedHsmListResult DeserializeDeletedManagedHsmListResult(JsonElement element)
        {
            Optional<IReadOnlyList<DeletedManagedHsm>> value = default;
            Optional<string> nextLink = default;
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("value"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    List<DeletedManagedHsm> array = new List<DeletedManagedHsm>();
                    foreach (var item in property.Value.EnumerateArray())
                    {
                        array.Add(DeletedManagedHsm.DeserializeDeletedManagedHsm(item));
                    }
                    value = array;
                    continue;
                }
                if (property.NameEquals("nextLink"))
                {
                    nextLink = property.Value.GetString();
                    continue;
                }
            }
            return new DeletedManagedHsmListResult(Optional.ToList(value), nextLink.Value);
        }
    }
}

﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

namespace Microsoft.WindowsAzure.Management.Storage.Table.Cmdlet
{
    using Microsoft.WindowsAzure.Management.Storage.Common;
    using Microsoft.WindowsAzure.Management.Storage.Model.Contract;
    using Microsoft.WindowsAzure.Management.Storage.Table;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Microsoft.WindowsAzure.Storage.Table;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using System.Security.Permissions;
    using System.Text;

    [Cmdlet(VerbsCommon.New, StorageNouns.TableSas), OutputType(typeof(String))]
    public class NewAzureStorageTableSasCommand : StorageCloudTableCmdletBase
    {
        [Alias("N", "Table")]
        [Parameter(Position = 0, Mandatory = true,
            HelpMessage = "Table Name",
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        [Parameter(HelpMessage = "Policy Identifier")]
        public string Policy
        {
            get { return accessPolicyIdentifier; }
            set { accessPolicyIdentifier = value; }
        }
        private string accessPolicyIdentifier;

        [Parameter(HelpMessage = "Permissions for a container. Permissions can be any not-empty subset of \"audq\".")]
        public string Permission { get; set; }

        [Parameter(HelpMessage = "Start Time")]
        public DateTime? StartTime { get; set; }

        [Parameter(HelpMessage = "Expiry Time")]
        public DateTime? ExpiryTime { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "Display full uri with sas token")]
        public SwitchParameter FullUri { get; set; }

        [Alias("startpk")]
        [Parameter(HelpMessage = "Start Partition Key")]
        public string StartPartitionKey { get; set; }
        [Alias("startrk")]
        [Parameter(HelpMessage = "Start Row Key")]
        public string StartRowKey { get; set; }
        [Alias("endpk")]
        [Parameter(HelpMessage = "End Partition Key")]
        public string EndPartitionKey { get; set; }
        [Alias("endrk")]
        [Parameter(HelpMessage = "End Row Key")]
        public string EndRowKey { get; set; }

        /// <summary>
        /// Initializes a new instance of the NewAzureStorageTableSasCommand class.
        /// </summary>
        public NewAzureStorageTableSasCommand()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the NewAzureStorageTableSasCommand class.
        /// </summary>
        /// <param name="channel">IStorageBlobManagement channel</param>
        public NewAzureStorageTableSasCommand(IStorageTableManagement channel)
        {
            Channel = channel;
        }

        /// <summary>
        /// Execute command
        /// </summary>
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public override void ExecuteCmdlet()
        {
            if (String.IsNullOrEmpty(Name)) return;
            CloudTable table = Channel.GetTableReference(Name);
            SharedAccessTablePolicy policy = new SharedAccessTablePolicy();
            SetupAccessPolicy(policy);
            SasTokenHelper.ValidateTableAccessPolicy(Channel, table.Name, policy, accessPolicyIdentifier);
            ValidatePkAndRk(StartPartitionKey, StartRowKey, EndPartitionKey, EndRowKey);
            string sasToken = table.GetSharedAccessSignature(policy, accessPolicyIdentifier, StartPartitionKey,
                                StartRowKey, EndPartitionKey, EndRowKey);

            if (FullUri)
            {
                string fullUri = table.Uri.ToString() + sasToken;
                WriteObject(fullUri);
            }
            else
            {
                WriteObject(sasToken);
            }
        }

        /// <summary>
        /// Validate the combination of PartitionKey and RowKey
        /// </summary>
        /// <param name="startPartitionKey"></param>
        /// <param name="startRowKey"></param>
        /// <param name="endPartitionKey"></param>
        /// <param name="endRowKey"></param>
        private void ValidatePkAndRk(string startPartitionKey, string startRowKey, string endPartitionKey, string endRowKey)
        {
            if (!string.IsNullOrEmpty(startRowKey) && string.IsNullOrEmpty(startPartitionKey))
            {
                throw new ArgumentException(Resources.StartpkMustAccomanyStartrk);
            }

            if (!string.IsNullOrEmpty(endRowKey) && string.IsNullOrEmpty(endPartitionKey))
            {
                throw new ArgumentException(Resources.EndpkMustAccomanyEndrk);
            }
        }

        /// <summary>
        /// Update the access policy
        /// </summary>
        /// <param name="policy">Access policy object</param>
        private void SetupAccessPolicy(SharedAccessTablePolicy policy)
        {
            DateTimeOffset? accessStartTime;
            DateTimeOffset? accessEndTime;
            SasTokenHelper.SetupAccessPolicyLifeTime(StartTime, ExpiryTime, out accessStartTime, out accessEndTime);
            policy.SharedAccessStartTime = accessStartTime;
            policy.SharedAccessExpiryTime = accessEndTime;
            SetupAccessPolicyPermission(policy, Permission);
        }

        /// <summary>
        /// Set up access policy permission
        /// </summary>
        /// <param name="policy">SharedAccessBlobPolicy object</param>
        /// <param name="permission">Permisson</param>
        internal void SetupAccessPolicyPermission(SharedAccessTablePolicy policy, string permission)
        {
            if (string.IsNullOrEmpty(permission)) return;
            policy.Permissions = SharedAccessTablePermissions.None;
            permission = permission.ToLower();
            foreach (char op in permission)
            {
                switch (op)
                {
                    case 'a':
                        policy.Permissions |= SharedAccessTablePermissions.Add;
                        break;
                    case 'u':
                        policy.Permissions |= SharedAccessTablePermissions.Update;
                        break;
                    case 'd':
                        policy.Permissions |= SharedAccessTablePermissions.Delete;
                        break;
                    case 'q':
                        policy.Permissions |= SharedAccessTablePermissions.Query;
                        break;
                    default:
                        throw new ArgumentException(string.Format(Resources.InvalidAccessPermission, op));
                }
            }
        }
    }
}

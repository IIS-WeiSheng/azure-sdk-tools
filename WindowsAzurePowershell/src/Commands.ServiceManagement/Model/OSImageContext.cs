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

namespace Microsoft.WindowsAzure.Management.ServiceManagement.Model
{
    using System;
    using Commands.Utilities.Common;
    using Microsoft.WindowsAzure.Management.Utilities.Common;

    public class OSImageContext : ManagementOperationContext
    {
        public string AffinityGroup { get; set; }

        public string Category { get; set; }

        public string Location { get; set; }

        public int LogicalSizeInGB { get; set; }

        public string Label { get; set; }

        public Uri MediaLink { get; set; }

        public string ImageName { get; set; }

        public string OS { get; set; }
        
        public string Eula { get; set; }

        public string Description { get; set; }

        public string ImageFamily { get; set; }

        public DateTime? PublishedDate { get; set; }
     
        public bool? IsPremium { get; set; }
        
        public Uri PrivacyUri { get; set; }

        public string RecommendedVMSize { get; set; }

        public string PublisherName { get; set; }
    }
}
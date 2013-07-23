function Test-NewGetSetAzureStorageAccountAndKeys
{
    $storageName1 = Get-StorageName
    $storageName2 = Get-StorageName
    $storageName3 = Get-StorageName
    $locName = $env:DEFAULT_STORAGE_LOCATION     #contains a valid storage account location

    New-AzureStorageAccount $storageName1 -Location $locName
    $storage = Get-AzureStorageAccount $storageName1
    Assert-True { $storage.GeoReplicationEnabled }

    $storageKey = Get-AzureStorageKey $storageName1

    # verify that the primary and secondary keys are not null or empty
    Assert-NotNull $storageKey.Primary
    Assert-NotNull $storageKey.Secondary
    Assert-False { $storageKey.Primary.Length -eq 0 }
    Assert-False { $storageKey.Secondary.Length -eq 0 }

    # verify that primary has changed and secondary has not 
    New-AzureStorageKey $storageName1 -KeyType Primary
    Assert-True { Retry-Function { $storageKey.Primary -ne (Get-AzureStorageKey $storageName1).Primary } $null 8 1 }
    Assert-True { Retry-Function { $storageKey.Secondary -eq (Get-AzureStorageKey $storageName1).Secondary } $null 8 1 }
    $storageKey2 = Get-AzureStorageKey $storageName1

    # verify that secondary key has changed, but primary key has not
    New-AzureStorageKey $storageName1 -KeyType Secondary
    Assert-True { Retry-Function { $storageKey2.Secondary -ne (Get-AzureStorageKey $storageName1).Secondary } $null 8 1 }
    Assert-True { Retry-Function { $storageKey2.Primary -eq (Get-AzureStorageKey $storageName1).Primary } $null 8 1 }

    New-AzureStorageAccount $storageName2 -Location $locName
    New-AzureStorageAccount $storageName3 -Location $locName
    
    # verify that all 3 accounts are listed
    $accNames = (Get-AzureStorageAccount).Label
    Assert-True { $accNames.Contains($storageName1) }
    Assert-True { $accNames.Contains($storageName2) }
    Assert-True { $accNames.Contains($storageName3) }

    # Cleanup
    ($storageName1, $storageName2, $storageName3) | ForEach-Object { Remove-AzureStorageAccount -StorageAccountName $_ } 
} 

<#
    .SYNOPSIS
    Gets valid storage name.
#> 
function Get-StorageName
{
    $storageName = $null
    $result = Assert-True {
        Retry-Function { 
            param([ref]$storageName)
            $storageName.Value = ("onesdkstorage" + (Get-Random).ToString()).ToLower()
            return -not (Test-AzureName -Storage -Name $storageName.Value)
        } ([ref]$storageName) 100 0
    }

    return $storageName
} 

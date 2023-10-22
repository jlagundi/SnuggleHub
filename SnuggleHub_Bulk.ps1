#powershell -ExecutionPolicy Bypass -File "C:\SnuggleHub_Repo\SnuggleHub_Bulk.ps1"

$SiteURL = "https://smg0h.sharepoint.com/sites/SnuggleHub"
$ListName = "Pets"
$csvContent = Get-Content -Path 'C:\SnuggleHub_Repo\pets_data.csv' -TotalCount 1
$delimiter = ($csvContent | Select-String -Pattern '\s').Matches.Count

Connect-PnPOnline -Url $SiteURL -Interactive

$CSVData = Import-Csv -Path $CSVFilePath

foreach ($Item in $CSVData) {
    $Query = "<View><Query><Where><And><Eq><FieldRef Name='Title'/><Value Type='Text'>$($Item.Name)</Value></Eq><Eq><FieldRef Name='ImageURL'/><Value Type='Text'>$($Item.ImageURL)</Value></Eq></And></Where></Query></View>"
    try {
        $Items = Get-PnPListItem -List $ListName -Query $Query
    }
    catch {
        Write-Host "Error occurred while retrieving list items: $_.Exception.Message"
        continue
    }

    if ($Items.Count -eq 0) {
        $ItemProperties = @{
            "Title" = $Item.Name
            "ImageURL" = $Item.ImageURL
        }

        Add-PnPListItem -List $ListName -Values $ItemProperties

        Write-Host "Item created: $($Item.Name)"
    } else {
        Write-Host "Item with Name $($Item.Name) and ImageURL $($Item.ImageURL) already exists. Skipping..."
    }
}

Disconnect-PnPOnline

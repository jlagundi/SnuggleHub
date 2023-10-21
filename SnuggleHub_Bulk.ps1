#powershell -ExecutionPolicy Bypass -File "C:\SnuggleHub_Repo\SnuggleHub_Bulk.ps1"

$SiteURL = "https://smg0h.sharepoint.com/sites/SnuggleHub"
$ListName = "Pets"
$csvContent = Get-Content -Path 'C:\SnuggleHub_Repo\pets_data.csv' -TotalCount 1
$delimiter = ($csvContent | Select-String -Pattern '\s').Matches.Count


# Connect to SharePoint Online using MFA
Connect-PnPOnline -Url $SiteURL -Interactive

# Get the CSV file data
$CSVData = Import-Csv -Path $CSVFilePath

# Iterate through each row in the CSV
foreach ($Item in $CSVData) {
    # The query seems to be incorrectly formatted. Ensure the CAML query is accurate for your list structure.
    $Query = "<View><Query><Where><And><Eq><FieldRef Name='Title'/><Value Type='Text'>$($Item.Name)</Value></Eq><Eq><FieldRef Name='ImageURL'/><Value Type='Text'>$($Item.ImageURL)</Value></Eq></And></Where></Query></View>"
    # Consider revising the query if it's not providing the expected results.

    # Use try-catch block to handle any errors that may occur during the process.
    try {
        $Items = Get-PnPListItem -List $ListName -Query $Query
    }
    catch {
        Write-Host "Error occurred while retrieving list items: $_.Exception.Message"
        continue  # Skip to the next iteration if an error occurs
    }

    if ($Items.Count -eq 0) {
        $ItemProperties = @{
            "Title" = $Item.Name
            "ImageURL" = $Item.ImageURL  # Adjusted property name
            # Add more properties as needed
        }

        # Add item to SharePoint Online list
        Add-PnPListItem -List $ListName -Values $ItemProperties

        Write-Host "Item created: $($Item.Name)"
    } else {
        Write-Host "Item with Name $($Item.Name) and ImageURL $($Item.ImageURL) already exists. Skipping..."
    }
}

# Disconnect from the SharePoint site
Disconnect-PnPOnline

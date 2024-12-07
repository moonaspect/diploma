# reformat_json.ps1

# Read and parse the input JSON file with UTF8 encoding
$data = Get-Content -Path "katakana_mappings.json" -Raw -Encoding UTF8 | ConvertFrom-Json

# Prepare the output array
$output = @()

# Process each word
foreach ($word in $data.katakana_mappings) {
    $output += [PSCustomObject]@{
        WordId    = [Guid]::NewGuid().ToString()
        Japanese  = $word.katakana
        Ukrainian = $word.ukrainian
    }
}

Write-Host "Start reformatting"

# Convert the output array to JSON
$jsonOutput = $output | ConvertTo-Json -Depth 3

# Save the JSON output to a file with UTF8 encoding
$jsonOutput | Set-Content -Path "katakana.json" -Encoding UTF8

Write-Host $jsonOutput

Write-Host "Reformat completed"
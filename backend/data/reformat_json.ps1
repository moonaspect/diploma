# reformat_json.ps1

# Read and parse the input JSON file with UTF8 encoding
$data = Get-Content -Path "jap_ukr_dict.json" -Raw -Encoding UTF8 | ConvertFrom-Json

# Prepare the output array
$output = @()

# Process each word
foreach ($word in $data.words) {
    $output += [PSCustomObject]@{
        WordId    = [Guid]::NewGuid().ToString()
        Japanese  = $word.japanese
        Ukrainian = $word.ukrainian
    }
}

# Convert the output array to JSON
$jsonOutput = $output | ConvertTo-Json -Depth 3

# Save the JSON output to a file with UTF8 encoding
$jsonOutput | Set-Content -Path "japan_words.json" -Encoding UTF8

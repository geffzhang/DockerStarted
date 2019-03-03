#Requires -Version 3.0

Param(
    [string] [Parameter(Mandatory=$true)] $ResourceGroupLocation,
    [string] $ResourceGroupName = 'AksDemoResources',
    [string] $TemplateFile = 'azuredeploy.json'
)

try {
    [Microsoft.Azure.Common.Authentication.AzureSession]::ClientFactory.AddUserAgent("VSAzureTools-$UI$($host.name)".replace(' ','_'), '3.0.0')
} catch { }

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 3

$OptionalParameters = New-Object -TypeName Hashtable

$TemplateFile = [System.IO.Path]::GetFullPath([System.IO.Path]::Combine($PSScriptRoot, $TemplateFile))

# Create or update the resource group using the specified template file and template parameters file
New-AzureRmResourceGroup `
  -Name $ResourceGroupName `
  -Location $ResourceGroupLocation `
  -Verbose `
  -Force

$deployment = New-AzureRmResourceGroupDeployment -Name ((Get-ChildItem $TemplateFile).BaseName + '-' + ((Get-Date).ToUniversalTime()).ToString('MMdd-HHmm')) `
                                   -ResourceGroupName $ResourceGroupName `
                                   -TemplateFile $TemplateFile `
                                   @OptionalParameters `
                                   -Force -Verbose `
                                   -ErrorVariable ErrorMessages

if ($ErrorMessages) {
    Write-Output '', 'Template deployment returned the following errors:', @(@($ErrorMessages) | ForEach-Object { $_.Exception.Message.TrimEnd("`r`n") })
}

function ToBase64($Text)
{
	$Bytes = [System.Text.Encoding]::Ascii.GetBytes($Text)
	$EncodedText =[Convert]::ToBase64String($Bytes)
	return $EncodedText
}

$CosmosDbConnectionString = ToBase64($deployment.Outputs.cosmosDbConnectionString.Value)
$RedisConnectionString = ToBase64($deployment.Outputs.redisConnectionString.Value)

Write-Host "##vso[task.setvariable variable=CosmosDbConnectionString;]$CosmosDbConnectionString"
Write-Host "##vso[task.setvariable variable=RedisConnectionString;]$RedisConnectionString"
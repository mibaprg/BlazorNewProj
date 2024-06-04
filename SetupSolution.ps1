param (
    [string]$NewRootNamespace = "MB",
	[string]$NewOrganizationName = "MiBaPrg",
    [string]$NewSolutionName = "HBlazorApp",
	[string]$NewHttpPort = "1081",
	[string]$NewHttpsPort = "44313",
	[string]$NewErrorsRecipient = "miba@hotmail.cz", # MiBaPrg developers: use errors@havit.cz
	[string]$NewErrorsSmptServer = "errorssmtp.server.com" # MiBaPrg developers: use mx.havit.cz
)

[string]$SolutionFolder = [System.IO.Path]::GetDirectoryName($MyInvocation.MyCommand.Path);

Get-ChildItem -recurse $SolutionFolder -include *.cs,*.csproj,*.config,*.ps1,*.json,*.tsx,*.cshtml,*.props,*.razor,*.json,*.html,*.js | where { $_ -is [System.IO.FileInfo] } | where { !$_.FullName.Contains("\packages\") } | where { !$_.FullName.Contains("\obj\") } | where { !$_.FullName.Contains("package.json") } | where { !$_.Name.Equals("_SetApplicationName.ps1") } |
Foreach-Object {
    Set-ItemProperty $_.FullName -name IsReadOnly -value $false
    [string]$Content = [IO.File]::ReadAllText($_.FullName)
    $Content = $Content.Replace('MB.HBlazorApp', $NewRootNamespace + '.' + $NewSolutionName)
    $Content = $Content.Replace('HBlazorApp', $NewSolutionName)
    $Content = $Content.Replace("MiBaPrg", $NewOrganizationName)
    $Content = $Content.Replace('1081', $NewHttpPort)
    $Content = $Content.Replace("44313", $NewHttpsPort)
    $Content = $Content.Replace("miba@hotmail.cz", $NewErrorsRecipient)
    $Content = $Content.Replace("errorssmtp.server.com", $NewErrorsSmptServer)
    [IO.File]::WriteAllText($_.FullName, $Content, [System.Text.Encoding]::UTF8)
}

Rename-Item -path ([System.IO.Path]::Combine($SolutionFolder, 'HBlazorApp.sln')) -newName ($NewSolutionName + '.sln')
Rename-Item -path ([System.IO.Path]::Combine($SolutionFolder, 'Entity\HBlazorAppDbContext.cs')) -newName ($NewSolutionName + 'DbContext.cs')
Rename-Item -path ([System.IO.Path]::Combine($SolutionFolder, 'Entity\Migrations\HBlazorAppDbContextModelSnapshot.cs')) -newName ($NewSolutionName + 'DbContextModelSnapshot.cs')
Rename-Item -path ([System.IO.Path]::Combine($SolutionFolder, 'Entity\HBlazorAppDesignTimeDbContextFactory.cs')) -newName ($NewSolutionName + 'DesignTimeDbContextFactory.cs')
Rename-Item -path ([System.IO.Path]::Combine($SolutionFolder, 'Entity.Tests\HBlazorAppDbContextTests.cs')) -newName ($NewSolutionName + 'DbContextTests.cs')
Rename-Item -path ([System.IO.Path]::Combine($SolutionFolder, 'Services\HealthChecks\HBlazorAppDbContextHealthCheck.cs')) -newName ($NewSolutionName + 'DbContextHealthCheck.cs')	
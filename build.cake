#tool nuget:?package=NuGet.CommandLine&version=5.9.1

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("Configuration", "Release");
var outputDirectory = Argument<DirectoryPath>("OutputDirectory", "output");
var packageDirectory = Argument<DirectoryPath>("CodeCoverageDirectory", "output/packages");
var solutionFile = Argument("SolutionFile", "magentoapi.soap-svcutil.sln");
var versionSuffix = Argument("VersionSuffix", "");
var nugetDeployFeed = Argument("NugetDeployFeed", "https://api.nuget.org/v3/index.json");
var nugetDeployApiKey = Argument("NugetDeployApiKey", "");

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

// Target : Clean
// 
// Description
// - Cleans binary directories.
// - Cleans output directory.
// - Cleans the test coverage directory.
Task("Clean")
    .Does(() =>
{
    CleanDirectory(packageDirectory);
    CleanDirectory(outputDirectory);


    // remove all binaries in source files
    var srcBinDirectories = GetDirectories("./src/**/bin");
    foreach(var directory in srcBinDirectories)
    {
        CleanDirectory(directory);
    }

    // remove all intermediates in source files
    var srcObjDirectories = GetDirectories("./src/**/obj");
    foreach(var directory in srcObjDirectories)
    {
        CleanDirectory(directory);
    }
    var srcNuPkgDirectories = GetDirectories("./src/**/nupkg");
    foreach(var directory in srcNuPkgDirectories)
    {
        CleanDirectory(directory);
    }    
});

// Target : Restore-NuGet-Packages
// 
// Description
// - Restores all needed NuGet packages for the projects.
Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    // https://docs.microsoft.com/en-us/nuget/consume-packages/package-restore
    //
    // Reload all nuget packages used by the solution
    NuGetRestore(solutionFile);
});

// Target : Build
// 
// Description
// - Builds the artifacts.
Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    if(IsRunningOnWindows())
    {
    
      // Use MSBuild
      MSBuild(solutionFile, settings => {
        settings.ArgumentCustomization = 
            args => args
                .Append("/p:IncludeSymbols=true")
                .Append("/p:IncludeSource=true")
                .Append($"/p:VersionSuffix={versionSuffix}");
        settings.SetConfiguration(configuration);
      });
    
    } else {
    
      // Use XBuild
      XBuild(solutionFile, settings => {
        settings.ArgumentCustomization = 
            args => args
                .Append("/p:IncludeSymbols=true")
                .Append("/p:IncludeSource=true")
                .Append($"/p:VersionSuffix={versionSuffix}");
        settings.SetConfiguration(configuration);
      });

    }
});

// Target : Deploy
// 
// Description
// - Deploys package to nuget repository.
Task("Deploy")
    .IsDependentOn("Build")
    .Does(() =>
{
    CreateDirectory(packageDirectory);

    DotNetCorePack("src/MagentoApi.SoapSvcUtil/MagentoApi.SoapSvcUtil.csproj");

    var packageFiles = GetFiles("src/MagentoApi.SoapSvcUtil/nupkg/*.nupkg");

    foreach(var packageFile in packageFiles)
    {
        var packageFilename = packageFile.GetFilename();
        var destionation = MakeAbsolute(packageDirectory).CombineWithFilePath(packageFilename);
        CopyFile(packageFile.FullPath, destionation);
    }

    packageFiles = GetFiles(MakeAbsolute(packageDirectory).FullPath + "/*.nupkg");


    if(string.IsNullOrWhiteSpace(nugetDeployApiKey)) 
    {
        Error("No nuget api key provided.");
        return;
    }

    // Push the package.
    NuGetPush(packageFiles, new NuGetPushSettings {
        Source = nugetDeployFeed,
        ApiKey = nugetDeployApiKey,
        SkipDuplicate = true
    });    

});

// Target : Build
// 
// Description
// - Setup the default task.
Task("Default")
    .IsDependentOn("Build");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);

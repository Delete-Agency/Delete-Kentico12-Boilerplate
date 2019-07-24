#addin "Cake.Watch"
#addin Cake.Curl
#addin "Cake.XdtTransform"
using System.Text.RegularExpressions;

const string solution = "./DeleteBoilerplate.sln";
const string webProject = "DeleteBoilerplate.WebApp";
const string projectPrefix = "DeleteBoilerplate";

var target = Argument("target", "Build");
var role = Argument("Role", "");

var configuration = Argument("configuration", "Debug");
var isDevelopment = configuration.Equals("debug", StringComparison.OrdinalIgnoreCase);
var publishDir = MakeAbsolute(Directory("./Published"));
var packageDir = DirectoryPath.FromString("./obj");
var baseDir = DirectoryPath.FromString(".");
var deployArchive = publishDir.Combine("Publish.zip").ToString();

void CopyFast(string source, string destination, bool recursive = true, string fileNames = "", string excludes = "")
{
    Information(source);
    var returnCode = StartProcess("robocopy", new ProcessSettings {
        Arguments = new ProcessArgumentBuilder()
            .Append(@"/MT:128 /NFL /NDL /ETA /NJH /NJS /NP" + (recursive ? " /S" : ""))
            .Append('"'+ source + '"')
            .Append('"'+ destination + '"')
            .Append(fileNames)
            .Append(excludes)
        }
    );
    if (returnCode > 7) {
        throw new Exception(string.Format("Robocopy failed. Return code: {0}", returnCode));
    }
}


Action<DirectoryPath, bool> CopyProjectFiles = (projectDir, skipWebConfig) => 
{
    Information(projectDir);
    var folders = new [] {"Views", "bin", "App_Data", "CMSResources", "Content", "Kentico"};
    Parallel.ForEach(folders, (folder) => 
    {
        Information(projectDir.Combine(folder));
        var sourceDir = projectDir.Combine(folder);
        var destDir = packageDir.Combine(folder);
        if (DirectoryExists(sourceDir))
        {
            EnsureDirectoryExists(destDir);
            CopyFast(sourceDir.ToString(), destDir.ToString(), true, "", skipWebConfig ? @"/xf web.config" : "");
        }
    });
};

Task("MakePackage")	
	.IsDependentOn("ClearPackage")
	.Does(() =>
	{
		var parsedSolution = ParseSolution(solution);
		foreach(var project in parsedSolution.Projects
			.Where(project => project.Path.ToString().EndsWith(".csproj"))
            .Where(project => project.Path.ToString().Contains(projectPrefix)))
		{
			CopyProjectFiles(project.Path.GetDirectory(), !project.Path.ToString().Contains($"/{webProject}"));
		}
        var web = baseDir.Combine(webProject);
        var folders = new [] {"dist"};
        foreach(var f in folders)
        {
            if (DirectoryExists(web.Combine(f).ToString())) {
                CopyFast(web.Combine(f).ToString(), packageDir.Combine(f).ToString());
            }
	    };

        CopyFast(MakeAbsolute(Directory($"./{webProject}")).ToString(), packageDir.ToString(), false, $"Web.config Web.{configuration}.config Global.asax connectionStrings.config");
    });

Task("ConfigPackage")
	.DoesForEach(GetFiles(packageDir.ToString() + "/**/*.config"), file => {
        var dir = file.GetDirectory();
        var name = file.GetFilenameWithoutExtension();
        var ext = file.GetExtension();

		var transforms = new List<string>(); 
        if (configuration != "")                { transforms.Add(configuration); }
        if (role != "")                         { transforms.Add(role); }
        if (configuration != "" && role != "")  { transforms.Add($"{configuration}.{role}"); }

		var list = transforms.Select(transform => dir.CombineWithFilePath(name + "." + transform + ext))
		.Where(FileExists);

		foreach (var transformFile in list) {
		    Information(file);
			Information("Transformation: " + transformFile);
			XdtTransformConfig(file, transformFile, file);
		}
	})
    .Does(() => {
        DeleteFiles($"{packageDir.ToString()}/**/*.{configuration}.config");
	});

Task("ClearPackage")
    .Does(() =>
{
    CleanDirectory(packageDir);
});



Task("ClearLocalSite")
    .Does(() =>
{
    StartProcess("iisreset");
    CleanDirectory(publishDir);
});


Task("BuildAssets")
    .Does(() => 
    {
		var batName = "build-" + (isDevelopment ? "dev" : "prod") + ".bat";
		var batPath = MakeAbsolute(Directory($"./{webProject}")).Combine(batName).ToString();

		Information(batPath);
		Information(batName);

		var returnCode = StartProcess(batPath);
		if (returnCode > 0) {
            throw new Exception(string.Format("Yarn failed. Return code: {0}", returnCode));
        }
    });


Task("Build")
    .Does(() =>
{
    NuGetRestore(solution);
  
    MSBuild(solution, 
          new MSBuildSettings {
              Verbosity = Verbosity.Quiet,
              Configuration = configuration,
              PlatformTarget = PlatformTarget.MSIL,
              MaxCpuCount = 8,
              NodeReuse = true
          }
          .WithRestore()
          .WithTarget("Build"));
});

Task("CopyPackageToLocalSite")
	.Does(() => {
        EnsureDirectoryExists(publishDir.ToString());
        Information($"Copy from [{packageDir}] to [{publishDir}]");
		CopyFast(packageDir.ToString(), publishDir.ToString());
		Information(DateTime.Now);
		});



Task("BuildAssetsAndPublishLocalSite")
    .IsDependentOn("BuildAssets")
    .IsDependentOn("PublishLocalSite")
    .Does(() => {});

Task("PublishLocalSite")
	.IsDependentOn("Build")
	.IsDependentOn("MakePackage")
    .IsDependentOn("ConfigPackage")
	.IsDependentOn("CopyPackageToLocalSite");

Task("PublishLocalSiteFull")
    .IsDependentOn("ClearLocalSite")
    .IsDependentOn("BuildAssetsAndPublishLocalSite")
    .Does(() =>
{
});


Task("Watch")
    .Does(() => 
{
    var destRoot = publishDir.Combine("views").ToString();

    Watch(
        new WatchSettings 
        {
            Recursive = true,
            Path = "./",
            Pattern = "*.cshtml"
        }, 
        (changes) => 
        {
            changes.ToList().ForEach(change => 
            {
                if (change.FullPath.EndsWith(".TMP")) return;
                if (!FileExists(change.FullPath)) return;
                var dest = Regex.Replace(change.FullPath.ToString(), @".*\\views", destRoot, RegexOptions.IgnoreCase).Replace("/", "\\");
                CopyFile(change.FullPath, dest);
                Information(DateTime.Now + " " + dest);
            });
        });
});

Task("Zip").Does(()=> {
    if (DirectoryExists(publishDir))
    {
        Zip(publishDir,deployArchive);
    }
});

Task("Deploy")
    .IsDependentOn("PublishLocalSiteFull")
    .IsDependentOn("Zip")
    .Does(()=>{
        var deployUrl = EnvironmentVariable("DeployUrl");
        if (!string.IsNullOrEmpty(deployUrl)) {
            CurlUploadFile(
                deployArchive,
                new Uri(deployUrl),
                new CurlSettings() {
                    ToolPath = "deploytools/curl/bin/curl.exe",
                    RequestCommand = "POST",
                    Username = EnvironmentVariable("DeployUserName"),
                    Password = EnvironmentVariable("DeployPassword"),
                    ArgumentCustomization = args=> args.Append("--fail")
                }
            );
        }
    });


RunTarget(target);
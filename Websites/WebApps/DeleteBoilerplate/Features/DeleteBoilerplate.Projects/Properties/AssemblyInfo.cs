using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DeleteBoilerplate.DynamicRouting;
using DeleteBoilerplate.Projects.DI;
using LightInject;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("DeleteBoilerplate.Projects")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("DeleteBoilerplate.Projects")]
[assembly: AssemblyCopyright("Copyright ©  2019")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("bfbbaf91-2904-4d5c-9ac5-365aeaee98ef")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers 
// by using the '*' as shown below:
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

// assembly level attribute that simply helps LightInject to locate the compostion root
[assembly: CompositionRootType(typeof(CompositionRoot))]

[assembly: CustomRoutes]
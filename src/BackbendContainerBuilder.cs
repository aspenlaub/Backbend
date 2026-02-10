using Aspenlaub.Net.GitHub.CSharp.Backbend.Core;
using Aspenlaub.Net.GitHub.CSharp.Dvin.Components;
using Autofac;

namespace Aspenlaub.Net.GitHub.CSharp.Backbend;

public static class BackbendContainerBuilder {
    public static ContainerBuilder UseBackbend(this ContainerBuilder builder) {
        builder.UseDvinAndPegh("Backbend");

        builder.RegisterType<BackbendFoldersAnalyser>().As<IBackbendFoldersAnalyser>();
        return builder;
    }
}
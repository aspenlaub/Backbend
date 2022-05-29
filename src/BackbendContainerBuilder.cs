using Aspenlaub.Net.GitHub.CSharp.Backbend.Core;
using Aspenlaub.Net.GitHub.CSharp.Dvin.Components;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Autofac;

namespace Aspenlaub.Net.GitHub.CSharp.Backbend;

public static class BackbendContainerBuilder {
    public static ContainerBuilder UseBackbend(this ContainerBuilder builder) {
        builder.UseDvinAndPegh("Backbend", new DummyCsArgumentPrompter());

        builder.RegisterType<BackbendFoldersAnalyser>().As<IBackbendFoldersAnalyser>();
        return builder;
    }
}
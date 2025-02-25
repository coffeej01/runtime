﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.Extensions.Configuration.Binder.SourceGeneration
{
    public sealed partial class ConfigurationBindingGenerator
    {
        private sealed partial class Emitter
        {
            private bool ShouldEmitMethods(MethodsToGen_Extensions_OptionsBuilder methods) => (_sourceGenSpec.MethodsToGen_OptionsBuilderExt & methods) != 0;

            private void EmitBindingExtensions_OptionsBuilder()
            {
                if (!ShouldEmitMethods(MethodsToGen_Extensions_OptionsBuilder.Any))
                {
                    return;
                }

                EmitBindingExtStartRegion(TypeDisplayString.OptionsBuilderOfTOptions);
                EmitBindMethods_Extensions_OptionsBuilder();
                EmitBindConfigurationMethod();
                EmitBindingExtEndRegion();
            }

            private void EmitBindMethods_Extensions_OptionsBuilder()
            {
                if (!ShouldEmitMethods(MethodsToGen_Extensions_OptionsBuilder.Bind))
                {
                    return;
                }

                const string documentation = @"/// <summary>Registers a configuration instance which <typeparamref name=""TOptions""/> will bind against.</summary>";
                const string paramList = $"{Identifier.IConfiguration} {Identifier.configuration}";

                if (ShouldEmitMethods(MethodsToGen_Extensions_OptionsBuilder.Bind_T))
                {
                    EmitMethodStartBlock(MethodsToGen_Extensions_OptionsBuilder.Bind_T, "Bind", paramList, documentation);
                    _writer.WriteLine($"return Bind({Identifier.optionsBuilder}, {Identifier.configuration}, {Identifier.configureOptions}: null);");
                    EmitEndBlock();
                }

                EmitMethodStartBlock(
                    MethodsToGen_Extensions_OptionsBuilder.Bind_T_BinderOptions,
                    "Bind",
                    paramList + $", {TypeDisplayString.NullableActionOfBinderOptions} {Identifier.configureOptions}",
                    documentation);

                EmitCheckForNullArgument_WithBlankLine(Identifier.optionsBuilder);

                _writer.WriteLine($$"""
                    {{Identifier.Configure}}<{{Identifier.TOptions}}>({{Identifier.optionsBuilder}}.{{Identifier.Services}}, {{Identifier.optionsBuilder}}.Name, {{Identifier.configuration}}, {{Identifier.configureOptions}});
                    return {{Identifier.optionsBuilder}};
                    """);

                EmitEndBlock();
            }

            private void EmitBindConfigurationMethod()
            {
                if (!ShouldEmitMethods(MethodsToGen_Extensions_OptionsBuilder.BindConfiguration_T_path_BinderOptions))
                {
                    return;
                }

                const string documentation = $@"/// <summary>Registers the dependency injection container to bind <typeparamref name=""TOptions""/> against the <see cref=""{Identifier.IConfiguration}""/> obtained from the DI service provider.</summary>";
                string paramList = $"string {Identifier.configSectionPath}, {TypeDisplayString.NullableActionOfBinderOptions} {Identifier.configureOptions} = null";

                EmitMethodStartBlock(MethodsToGen_Extensions_OptionsBuilder.BindConfiguration, "BindConfiguration", paramList, documentation);

                EmitCheckForNullArgument_WithBlankLine(Identifier.optionsBuilder);
                EmitCheckForNullArgument_WithBlankLine(Identifier.configSectionPath);

                EmitStartBlock($"{Identifier.optionsBuilder}.{Identifier.Configure}<{Identifier.IConfiguration}>(({Identifier.obj}, {Identifier.configuration}) =>");
                EmitCheckForNullArgument_WithBlankLine(Identifier.obj);
                EmitCheckForNullArgument_WithBlankLine(Identifier.configuration);
                _writer.WriteLine($$"""
                    {{Identifier.IConfiguration}} {{Identifier.section}} = string.Equals(string.Empty, {{Identifier.configSectionPath}}, StringComparison.OrdinalIgnoreCase) ? {{Identifier.configuration}} : {{Identifier.configuration}}.{{Identifier.GetSection}}({{Identifier.configSectionPath}});
                    {{nameof(MethodsToGen_CoreBindingHelper.BindCoreMain)}}({{Identifier.section}}, {{Identifier.obj}}, typeof({{Identifier.TOptions}}), {{Identifier.configureOptions}});
                    """);

                EmitEndBlock(endBraceTrailingSource: ");");

                _writer.WriteLine();

                _writer.WriteLine($$"""
                    {{Identifier.optionsBuilder}}.{{Identifier.Services}}.{{Identifier.AddSingleton}}<{{Identifier.IOptionsChangeTokenSource}}<{{Identifier.TOptions}}>, {{Identifier.ConfigurationChangeTokenSource}}<{{Identifier.TOptions}}>>();
                    return {{Identifier.optionsBuilder}};
                    """);

                EmitEndBlock();
            }

            private void EmitMethodStartBlock(MethodsToGen_Extensions_OptionsBuilder method, string methodName, string paramList, string documentation)
            {
                paramList = $"this {TypeDisplayString.OptionsBuilderOfTOptions} {Identifier.optionsBuilder}, {paramList}";
                EmitBlankLineIfRequired();
                _writer.WriteLine(documentation);
                EmitInterceptsLocationAnnotations(method);
                EmitStartBlock($"public static {TypeDisplayString.OptionsBuilderOfTOptions} {methodName}<{Identifier.TOptions}>({paramList}) where {Identifier.TOptions} : class");
            }
        }
    }
}

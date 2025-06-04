using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Thindal.Blazor.BindableParameter.SourceGenerator
{
    [Generator]
    public class BindableParameterSourceGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
#if DEBUG
            if(!Debugger.IsAttached)
            {
                Debugger.Launch();
            }
#endif

            var observableParameterData = context.SyntaxProvider.CreateSyntaxProvider(
                  predicate: (node, _) => SelectPropetiesWithBindableParameterAttribute(node)
                , transform: (node, _) => TransformObservableParameter(node))
                .Where(opd => opd is not null);

            // Create the source code to generate
            context.RegisterSourceOutput(observableParameterData, (sourceProductionContext, data) =>
            {
                SourceGeneratorHelper.CreateParameter(sourceProductionContext, data);
            });
        }



        private bool SelectPropetiesWithBindableParameterAttribute(SyntaxNode node)
        {
            return
                node.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.PropertyDeclaration);
        }

        private ObservableParameterData TransformObservableParameter(GeneratorSyntaxContext node)
        {
            var propDeclaration = (PropertyDeclarationSyntax)node.Node;
            
            var propSymbolInstance = (IPropertySymbol)ModelExtensions.GetDeclaredSymbol(node.SemanticModel, propDeclaration);

            var attributes = propSymbolInstance.GetAttributes();

            /*var args = propDeclaration.AttributeLists.SelectMany(attrList => attrList.Attributes)
                .Where(attr => attr.Name.ToString() == "BindableParameter")
                .Select(attr => attr.ArgumentList?.Arguments.Select(a => a.ToString()).ToList())
                .FirstOrDefault();*/

            // Find instance of BindableParameterAttribute on this attribute
            if(!attributes.Any())
            {
                return null;
            }
            var bParamAttr = attributes.FirstOrDefault(a => !a.AttributeClass.Name.Contains("Error") && a.AttributeClass.Name.Contains("BindableParameter"));

            var numCtorArgs = bParamAttr.ConstructorArguments.Length;

            // defaults
            bool includeSelf = true;
            List<string>? changedMethods = null;
            if (numCtorArgs > 0)
            {
                // If we have a value for includeSelf, set our local variable to that value
                if (bParamAttr.ConstructorArguments[0].Value is bool includeSelfValue)
                {
                    includeSelf = includeSelfValue;
                }

                // If we have a value for changedMethods, set our local variable to that value
                if (bParamAttr.ConstructorArguments[1].Value is string[] changedMethodsValue)
                {
                    changedMethods = changedMethodsValue.ToList();
                }
            }

            foreach (var attrList in propDeclaration.AttributeLists)
            {
                foreach (var attr in attrList.Attributes)
                {

                    if (attr.Name.ToString() != "BindableParameter")
                    {
                        return null;
                    }

                    var args = attr.ArgumentList?.Arguments.Select(a=> a.ToString()).ToList();

                    var classDeclaration = (ClassDeclarationSyntax)propDeclaration.Parent;
                    var namespaceName = ((classDeclaration.Parent as NamespaceDeclarationSyntax)?.Name ??
                        (classDeclaration.Parent as FileScopedNamespaceDeclarationSyntax)?.Name)
                        ?? throw new NullReferenceException($"Couldn't find namespace for {classDeclaration.Identifier.Text}.");

                    return new ObservableParameterData(
                        PropertyName: propDeclaration.Identifier.Text,
                        PropertyType: propDeclaration.Type.ToString(),
                        ClassName: classDeclaration.Identifier.Text,
                        NamespaceName: namespaceName.ToString(),
                        ChangedMethods: changedMethods,
                        IncludeSelf: includeSelf);
                }
            }

            return null;
        }
    }
}

internal class ObservableParameterData
{
    public ObservableParameterData(string PropertyType, string PropertyName, string ClassName, string NamespaceName, List<string>? ChangedMethods, bool IncludeSelf)
    {
        this.PropertyType = PropertyType;
        this.PropertyName = PropertyName;
        this.ClassName = ClassName;
        this.NamespaceName = NamespaceName;
        this.ChangedMethods = new List<string>();

        if (IncludeSelf)
        {
            this.ChangedMethods.Add(PropertyName);
        }

        if(ChangedMethods is not null)
        {
            this.ChangedMethods.AddRange(ChangedMethods);
        }
    }

    public string PropertyType { get; }
    public string PropertyName { get; }
    public string ClassName { get; }
    public string NamespaceName { get; }
    public List<string> ChangedMethods { get; set; }
}
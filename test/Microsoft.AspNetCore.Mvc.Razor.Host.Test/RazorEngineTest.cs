// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
#if GENERATE_BASELINES
using System.IO;
using System.Linq;
#endif
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
#if GENERATE_BASELINES
using System.Text;
#endif
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Razor.Chunks.Generators;
using Microsoft.AspNetCore.Razor.CodeGenerators;
using Microsoft.AspNetCore.Razor.Evolution;
using Microsoft.AspNetCore.Razor.Evolution.Intermediate;
using Microsoft.AspNetCore.Razor.Parser;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Testing;
using Xunit;
using Microsoft.AspNetCore.Mvc.Razor.Host;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.Extensions.Options;
using Moq;
using Microsoft.CodeAnalysis;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;

namespace Microsoft.AspNetCore.Mvc.Razor
{
    public class RazorEngineTest
    {
        private static Assembly _assembly = typeof(RazorEngineTest).GetTypeInfo().Assembly;

        [Fact]
        public void RazorEngine_GeneratesTagHelperModelExpressionCode_DesignTime()
        {
            // Arrange
            var fileProvider = new TestFileProvider();

            var expectedLineMappings = new LineMapping[]
            {
                //BuildLineMapping(
                //    documentAbsoluteIndex: 33,
                //    documentLineIndex: 2,
                //    documentCharacterIndex: 14,
                //    generatedAbsoluteIndex: 654,
                //    generatedLineIndex: 17,
                //    generatedCharacterIndex: 48,
                //    contentLength: 91),
                //BuildLineMapping(
                //    documentAbsoluteIndex: 140,
                //    documentLineIndex: 3,
                //    documentCharacterIndex: 14,
                //    generatedAbsoluteIndex: 797,
                //    generatedLineIndex: 18,
                //    generatedCharacterIndex: 48,
                //    contentLength: 102),
                //BuildLineMapping(
                //    documentAbsoluteIndex: 7,
                //    documentLineIndex: 0,
                //    documentCharacterIndex: 7,
                //    generatedAbsoluteIndex: 990,
                //    generatedLineIndex: 20,
                //    generatedCharacterIndex: 28,
                //    contentLength: 8),
                //BuildLineMapping(
                //    documentAbsoluteIndex: 263,
                //    documentLineIndex: 5,
                //    documentCharacterIndex: 17,
                //    generatedAbsoluteIndex: 2841,
                //    generatedLineIndex: 52,
                //    generatedCharacterIndex: 133,
                //    contentLength: 3),
                //BuildLineMapping(
                //    documentAbsoluteIndex: 290,
                //    documentLineIndex: 6,
                //    documentCharacterIndex: 18,
                //    generatedAbsoluteIndex: 3208,
                //    generatedLineIndex: 58,
                //    generatedCharacterIndex: 125,
                //    contentLength: 5),
                //BuildLineMapping(
                //    documentAbsoluteIndex: 322,
                //    documentLineIndex: 8,
                //    documentCharacterIndex: 19,
                //    generatedAbsoluteIndex: 3627,
                //    generatedLineIndex: 64,
                //    generatedCharacterIndex: 153,
                //    contentLength: 5),
                //BuildLineMapping(
                //    documentAbsoluteIndex: 357,
                //    documentLineIndex: 9,
                //    documentCharacterIndex: 19,
                //    generatedAbsoluteIndex: 4055,
                //    generatedLineIndex: 70,
                //    generatedCharacterIndex: 161,
                //    contentLength: 4),
                //BuildLineMapping(
                //    documentAbsoluteIndex: 378,
                //    documentLineIndex: 9,
                //    documentCharacterIndex: 40,
                //    generatedAbsoluteIndex: 4317,
                //    generatedLineIndex: 75,
                //    generatedCharacterIndex: 163,
                //    contentLength: 6),
            };

            // Act and Assert
            RunDesignTimeTest(
                testName: "ModelExpressionTagHelper",
                expectedLineMappings: expectedLineMappings);
        }

        [Theory]
        [InlineData("Basic")]
        [InlineData("_ViewImports")]
        [InlineData("Inject")]
        [InlineData("InjectWithModel")]
        [InlineData("InjectWithSemicolon")]
        [InlineData("Model")]
        [InlineData("ModelExpressionTagHelper")]
        public void RazorEngine_ParsesAndGeneratesCodeForBasicScenarios(string scenarioName)
        {
            // Arrange
            var fileProvider = new TestFileProvider();

            // Act and Assert
            RunRuntimeTest(scenarioName);
        }

        [Fact]
        public void BasicVisitor_GeneratesCorrectLineMappings()
        {
            // Arrange
            var expectedLineMappings = new LineMapping[]
            {
                //BuildLineMapping(
                //    documentAbsoluteIndex: 13,
                //    documentLineIndex: 0,
                //    documentCharacterIndex: 13,
                //    generatedAbsoluteIndex: 1499,
                //    generatedLineIndex: 34,
                //    generatedCharacterIndex: 13,
                //    contentLength: 4),
                //BuildLineMapping(
                //    documentAbsoluteIndex: 43,
                //    documentLineIndex: 2,
                //    documentCharacterIndex: 5,
                //    generatedAbsoluteIndex: 1583,
                //    generatedLineIndex: 39,
                //    generatedCharacterIndex: 6,
                //    contentLength: 21),
            };

            // Act and Assert
            RunDesignTimeTest("Basic", expectedLineMappings);
        }

        [Fact]
        public void RazorEngine_GeneratesCorrectLineMappingsAndUsingStatementsForViewImports()
        {
            // Arrange
            var expectedLineMappings = new LineMapping[]
            {
                //BuildLineMapping(
                //    documentAbsoluteIndex: 8,
                //    documentLineIndex: 0,
                //    documentCharacterIndex: 8,
                //    generatedAbsoluteIndex: 666,
                //    generatedLineIndex: 21,
                //    generatedCharacterIndex: 8,
                //    contentLength: 26),
            };

            // Act and Assert
            RunDesignTimeTest("_ViewImports", expectedLineMappings);
        }

        [Fact]
        public void InjectVisitor_GeneratesCorrectLineMappings()
        {
            // Arrange
            var expectedLineMappings = new LineMapping[]
            {
                //BuildLineMapping(
                //    documentAbsoluteIndex: 1,
                //    documentLineIndex: 0,
                //    documentCharacterIndex: 1,
                //    generatedAbsoluteIndex: 66,
                //    generatedLineIndex: 3,
                //    generatedCharacterIndex: 0,
                //    contentLength: 17),
                //BuildLineMapping(
                //    documentAbsoluteIndex: 28,
                //    documentLineIndex: 1,
                //    documentCharacterIndex: 8,
                //    generatedAbsoluteIndex: 711,
                //    generatedLineIndex: 26,
                //    generatedCharacterIndex: 8,
                //    contentLength: 20),
            };

            // Act and Assert
            RunDesignTimeTest("Inject", expectedLineMappings);
        }

        [Fact]
        public void InjectVisitorWithModel_GeneratesCorrectLineMappings()
        {
            // Arrange
            var expectedLineMappings = new LineMapping[]
            {
                //BuildLineMapping(
                //    documentAbsoluteIndex: 7,
                //    documentLineIndex: 0,
                //    documentCharacterIndex: 7,
                //    generatedAbsoluteIndex: 397,
                //    generatedLineIndex: 11,
                //    generatedCharacterIndex: 28,
                //    contentLength: 7),
                //BuildLineMapping(
                //    documentAbsoluteIndex: 24,
                //    documentLineIndex: 1,
                //    documentCharacterIndex: 8,
                //    generatedAbsoluteIndex: 760,
                //    generatedLineIndex: 25,
                //    generatedCharacterIndex: 8,
                //    contentLength: 20),
                //BuildLineMapping(
                //    documentAbsoluteIndex: 54,
                //    documentLineIndex: 2,
                //    documentCharacterIndex: 8,
                //    generatedAbsoluteIndex: 990,
                //    generatedLineIndex: 33,
                //    generatedCharacterIndex: 8,
                //    contentLength: 23),
            };

            // Act and Assert
            RunDesignTimeTest("InjectWithModel", expectedLineMappings);
        }

        [Fact]
        public void InjectVisitorWithSemicolon_GeneratesCorrectLineMappings()
        {
            // Arrange
            var expectedLineMappings = new LineMapping[]
            {
                //BuildLineMapping(
                //    documentAbsoluteIndex: 7,
                //    documentLineIndex: 0,
                //    documentCharacterIndex: 7,
                //    generatedAbsoluteIndex: 405,
                //    generatedLineIndex: 11,
                //    generatedCharacterIndex: 28,
                //    contentLength: 7),
                //BuildLineMapping(
                //    documentAbsoluteIndex: 24,
                //    documentLineIndex: 1,
                //    documentCharacterIndex: 8,
                //    generatedAbsoluteIndex: 776,
                //    generatedLineIndex: 25,
                //    generatedCharacterIndex: 8,
                //    contentLength: 20),
                //BuildLineMapping(
                //    documentAbsoluteIndex: 58,
                //    documentLineIndex: 2,
                //    documentCharacterIndex: 8,
                //    generatedAbsoluteIndex: 1010,
                //    generatedLineIndex: 33,
                //    generatedCharacterIndex: 8,
                //    contentLength: 23),
                //BuildLineMapping(
                //    documentAbsoluteIndex: 93,
                //    documentLineIndex: 3,
                //    documentCharacterIndex: 8,
                //    generatedAbsoluteIndex: 1247,
                //    generatedLineIndex: 41,
                //    generatedCharacterIndex: 8,
                //    contentLength: 21),
                //BuildLineMapping(
                //    documentAbsoluteIndex: 129,
                //    documentLineIndex: 4,
                //    documentCharacterIndex: 8,
                //    generatedAbsoluteIndex: 1482,
                //    generatedLineIndex: 49,
                //    generatedCharacterIndex: 8,
                //    contentLength: 24),
            };

            // Act and Assert
            RunDesignTimeTest("InjectWithSemicolon", expectedLineMappings);
        }

        [Fact]
        public void ModelVisitor_GeneratesCorrectLineMappings()
        {
            // Arrange
            var expectedLineMappings = new LineMapping[]
            {
                //BuildLineMapping(
                //    documentAbsoluteIndex: 7,
                //    documentLineIndex: 0,
                //    documentCharacterIndex: 7,
                //    generatedAbsoluteIndex: 400,
                //    generatedLineIndex: 11,
                //    generatedCharacterIndex: 28,
                //    contentLength: 30),
            };

            // Act and Assert
            RunDesignTimeTest("Model", expectedLineMappings);
        }

        [Fact]
        public void ModelVisitor_GeneratesLineMappingsForLastModel_WhenMultipleModelsArePresent()
        {
            // Arrange
            var inputFile = "TestFiles/Input/MultipleModels.cshtml";
            var outputFile = "TestFiles/Output/DesignTime/MultipleModels.cs";
            var expectedCode = ResourceFile.ReadResource(_assembly, outputFile, sourceFile: false);

            var engine = RazorEngine.CreateDesignTime(b =>
            {
                InjectDirective.Register(b);
                ModelDirective.Register(b);

                b.Features.Add(new ModelExpressionPass());
                b.Features.Add(new MvcViewDocumentClassifierPass());

                b.Features.Add(new Microsoft.CodeAnalysis.Razor.DefaultTagHelperFeature());

                var references = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .Select(assembly => MetadataReference.CreateFromFile(assembly.Location))
                    .ToList<MetadataReference>();

                b.Features.Add(new Microsoft.CodeAnalysis.Razor.DefaultMetadataReferenceFeature()
                {
                    References = references,
                });
            });

            // Act
            RazorCSharpDocument csharpDocument = null;
            using (var stream = ResourceFile.GetResourceStream(_assembly, inputFile, sourceFile: true))
            {
                // VS tooling passes in paths in all lower case. We'll mimic this behavior in our tests.
                var codeDocument = RazorCodeDocument.Create(RazorSourceDocument.ReadFrom(stream, inputFile.ToLowerInvariant()));
                engine.Process(codeDocument);
                csharpDocument = codeDocument.GetCSharpDocument();
            }

            // Assert
            Assert.Empty(csharpDocument.Diagnostics);
#if GENERATE_BASELINES
            ResourceFile.UpdateFile(_assembly, outputFile, expectedCode, csharpDocument.GeneratedCode);
#else
            Assert.Equal(expectedCode, csharpDocument.GeneratedCode, ignoreLineEndingDifferences: true);
#endif
        }

        private static void RunRuntimeTest(string testName)
        {
            var inputFile = "TestFiles/Input/" + testName + ".cshtml";
            var outputFile = "TestFiles/Output/Runtime/" + testName + ".cs";
            var expectedCode = ResourceFile.ReadResource(_assembly, outputFile, sourceFile: false);

            var engine = RazorEngine.Create(b =>
            {
                InjectDirective.Register(b);
                ModelDirective.Register(b);

                b.Features.Add(new ModelExpressionPass());
                b.Features.Add(new MvcViewDocumentClassifierPass());
                b.Features.Add(new DefaultInstrumentationPass());

                b.Features.Add(new Microsoft.CodeAnalysis.Razor.DefaultTagHelperFeature());

                var references = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .Select(assembly => MetadataReference.CreateFromFile(assembly.Location))
                    .ToList<MetadataReference>();

                b.Features.Add(new Microsoft.CodeAnalysis.Razor.DefaultMetadataReferenceFeature()
                {
                    References = references,
                });
            });

            // Act
            RazorCSharpDocument csharpDocument = null;
            using (var stream = ResourceFile.GetResourceStream(_assembly, inputFile, sourceFile: true))
            {
                var codeDocument = RazorCodeDocument.Create(RazorSourceDocument.ReadFrom(stream, inputFile));
                engine.Process(codeDocument);
                csharpDocument = codeDocument.GetCSharpDocument();
            }

            // Assert
            Assert.Empty(csharpDocument.Diagnostics);

#if GENERATE_BASELINES
            ResourceFile.UpdateFile(_assembly, outputFile, expectedCode, csharpDocument.GeneratedCode);
#else
            Assert.Equal(expectedCode, csharpDocument.GeneratedCode, ignoreLineEndingDifferences: true);
#endif
        }

        private static void RunDesignTimeTest(
            string testName,
            IEnumerable<LineMapping> expectedLineMappings)
        {
            var inputFile = "TestFiles/Input/" + testName + ".cshtml";
            var outputFile = "TestFiles/Output/DesignTime/" + testName + ".cs";
            var expectedCode = ResourceFile.ReadResource(_assembly, outputFile, sourceFile: false);

            var engine = RazorEngine.CreateDesignTime(b =>
            {
                InjectDirective.Register(b);
                ModelDirective.Register(b);

                b.Features.Add(new ModelExpressionPass());
                b.Features.Add(new MvcViewDocumentClassifierPass());

                b.Features.Add(new Microsoft.CodeAnalysis.Razor.DefaultTagHelperFeature());

                var references = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .Select(assembly => MetadataReference.CreateFromFile(assembly.Location))
                    .ToList<MetadataReference>();

                b.Features.Add(new Microsoft.CodeAnalysis.Razor.DefaultMetadataReferenceFeature()
                {
                    References = references,
                });
            });

            // Act
            RazorCSharpDocument csharpDocument = null;
            using (var stream = ResourceFile.GetResourceStream(_assembly, inputFile, sourceFile: true))
            {
                // VS tooling passes in paths in all lower case. We'll mimic this behavior in our tests.
                var codeDocument = RazorCodeDocument.Create(RazorSourceDocument.ReadFrom(stream, inputFile.ToLowerInvariant()));
                engine.Process(codeDocument);
                csharpDocument = codeDocument.GetCSharpDocument();
            }

            // Assert
            Assert.Empty(csharpDocument.Diagnostics);

#if GENERATE_BASELINES
            ResourceFile.UpdateFile(_assembly, outputFile, expectedCode, csharpDocument.GeneratedCode);

            //Assert.NotNull(csharpDocument.LineMappings); // Guard
            //if (expectedLineMappings == null ||
            //    !Enumerable.SequenceEqual(expectedLineMappings, csharpDocument.LineMappings))
            //{
            //    var lineMappings = new StringBuilder();
            //    lineMappings.AppendLine($"// !!! Do not check in. Instead paste content into test method. !!!");
            //    lineMappings.AppendLine();

            //    var indent = "            ";
            //    lineMappings.AppendLine($"{ indent }var expectedLineMappings = new[]");
            //    lineMappings.AppendLine($"{ indent }{{");
            //    foreach (var lineMapping in csharpDocument.LineMappings)
            //    {
            //        var innerIndent = indent + "    ";
            //        var documentLocation = lineMapping.DocumentLocation;
            //        var generatedLocation = lineMapping.GeneratedLocation;
            //        lineMappings.AppendLine($"{ innerIndent }{ nameof(BuildLineMapping) }(");

            //        innerIndent += "    ";
            //        lineMappings.AppendLine($"{ innerIndent }documentAbsoluteIndex: { documentLocation.AbsoluteIndex },");
            //        lineMappings.AppendLine($"{ innerIndent }documentLineIndex: { documentLocation.LineIndex },");
            //        lineMappings.AppendLine($"{ innerIndent }documentCharacterIndex: { documentLocation.CharacterIndex },");
            //        lineMappings.AppendLine($"{ innerIndent }generatedAbsoluteIndex: { generatedLocation.AbsoluteIndex },");
            //        lineMappings.AppendLine($"{ innerIndent }generatedLineIndex: { generatedLocation.LineIndex },");
            //        lineMappings.AppendLine($"{ innerIndent }generatedCharacterIndex: { generatedLocation.CharacterIndex },");
            //        lineMappings.AppendLine($"{ innerIndent }contentLength: { generatedLocation.ContentLength }),");
            //    }

            //    lineMappings.AppendLine($"{ indent }}};");

            //    var lineMappingFile = Path.ChangeExtension(outputFile, "lineMappings.cs");
            //    ResourceFile.UpdateFile(_assembly, lineMappingFile, previousContent: null, content: lineMappings.ToString());
            //}
#else
            Assert.Equal(expectedCode, csharpDocument.GeneratedCode, ignoreLineEndingDifferences: true);
            //Assert.Equal(expectedLineMappings, csharpDocument.LineMappings);
#endif
        }

        //private static LineMapping BuildLineMapping(
        //    int documentAbsoluteIndex,
        //    int documentLineIndex,
        //    int documentCharacterIndex,
        //    int generatedAbsoluteIndex,
        //    int generatedLineIndex,
        //    int generatedCharacterIndex,
        //    int contentLength)
        //{
        //    var documentLocation = new SourceLocation(
        //        documentAbsoluteIndex,
        //        documentLineIndex,
        //        documentCharacterIndex);
        //    var generatedLocation = new SourceLocation(
        //        generatedAbsoluteIndex,
        //        generatedLineIndex,
        //        generatedCharacterIndex);

        //    return new LineMapping(
        //        documentLocation: new MappingLocation(documentLocation, contentLength),
        //        generatedLocation: new MappingLocation(generatedLocation, contentLength));
        //}
    }
}
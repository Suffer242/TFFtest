// <copyright file="DvdExtractorTest.cs" company="Hewlett-Packard Company">Copyright © Hewlett-Packard Company 2015</copyright>
using System;
using JarrettVance.ChapterTools.Extractors;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JarrettVance.ChapterTools.Extractors.Tests
{
    /// <summary>Этот класс содержит параметризованные модульные тесты для DvdExtractor</summary>
    [PexClass(typeof(DvdExtractor))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class DvdExtractorTest
    {
    }
}

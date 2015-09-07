﻿using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Nest.Resolvers.Converters.Aggregations;
using Nest.Searchify.Extensions;
using Nest.Searchify.Queries;
using Nest.Searchify.SearchResults;
using Newtonsoft.Json;
using Xunit;

namespace Nest.Searchify.Tests.Integration
{
    [CollectionDefinition(nameof(SearchDataCollection))]
    public class SearchDataCollection : ICollectionFixture<SearchDataContext>
    {
        [Trait("Category", "Integration")]
        [Collection(nameof(SearchDataCollection))]
        public class WhenAccessingTermsAggregation
        {
            private readonly ISearchResponse<Person> _response;
            private readonly SearchResult<Person, CommonParameters> _result;

            public WhenAccessingTermsAggregation(SearchDataContext context)
            {
                var parameters = new CommonParameters();
                _response = context.Repository.Query(new PersonSearchQueryWithTermsAggregation(parameters));
                _result = new SearchResult<Person, CommonParameters>(parameters, _response);
            }

            [Fact]
            public void ShouldHaveExpectedAggregationCount()
            {
                _result.AggregationHelper.Terms("country").Should().NotBeNull();
                _result.AggregationHelper.Terms("age").Should().NotBeNull();
            }

            [Fact]
            public void ShouldHaveExpectedAggregation()
            {
                var json = JsonConvert.SerializeObject(_result, Formatting.Indented);
            }
        }

        [Trait("Category", "Integration")]
        [Collection(nameof(SearchDataCollection))]
        public class CommonParametersSearchWithDefaultArgs
        {
            private readonly ISearchResponse<Person> _response;

            public CommonParametersSearchWithDefaultArgs(SearchDataContext context)
            {
                _response = context.Repository.Search<Person>(new CommonParameters());
            }

            [Fact]
            public void ShouldQueryAllAvailableDocuments()
            {
                _response.Total.Should().Be(100);
            }

            [Fact]
            public void ShouldReturnFirstPageOfDocuments()
            {
                _response.Documents.Count().Should().Be(10);
            }
        }

        [Trait("Category", "Integration")]
        [Collection(nameof(SearchDataCollection))]
        public class WhenSearchingSpecificPageAndSizeWithCommonParametersSearch
        {
            private readonly ISearchResponse<Person> _response;

            public WhenSearchingSpecificPageAndSizeWithCommonParametersSearch(SearchDataContext context)
            {
                _response = context.Repository.Search<Person>(new CommonParameters(1, 2));
            }

            [Fact]
            public void ShouldQueryAllAvailableDocuments()
            {
                _response.Total.Should().Be(100);
            }

            [Fact]
            public void ShouldReturnFirstPageOfDocuments()
            {
                _response.Documents.Count().Should().Be(1);
            }
        }
    }
}

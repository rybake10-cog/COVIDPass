// © 2020 Partners HealthCare Technology & Innovation Department
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CovidPass.Core.Infrastructure.Services
{
    public class FilterService<TEntity>
    {
        private class FilterPreset
        {
            public Func<object> ShouldApply { get; set; }
            public Expression<Func<TEntity, bool>> Expression { get; set; }
        }

        private readonly IQueryable<TEntity> _query;
        private readonly List<FilterPreset> _presets = new List<FilterPreset>();

        public FilterService(IQueryable<TEntity> query)
        {
            _query = query;
        }

        public FilterService<TEntity> Filter(Func<object> shouldApply, Expression<Func<TEntity, bool>> expression)
        {
            _presets.Add(new FilterPreset
            {
                ShouldApply = shouldApply,
                Expression = expression
            });
            return this;
        }

        public IQueryable<TEntity> Apply()
        {
            var query = _query;
            foreach (var preset in _presets)
            {
                var value = preset.ShouldApply();
                if (value is string stringValue && !string.IsNullOrWhiteSpace(stringValue)
                    || value is bool boolValue && boolValue
                    || value != null)
                    query = query.Where(preset.Expression);
            }
            return query;
        }
    }
}

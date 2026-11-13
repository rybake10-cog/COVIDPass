// © 2020 Partners HealthCare Technology & Innovation Department
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CovidPass.Core.Infrastructure.Services
{
    public class OrderService<TEntity>
    {
        private interface IOrderPreset
        {
            string Field { get; }
            IQueryable<TEntity> Apply(IQueryable<TEntity> query, bool ascSort);
        }

        private class OrderPreset<T1> : IOrderPreset
        {
            public string Field { get; }

            private readonly Expression<Func<TEntity, T1>> _expression;

            private readonly IOrderPreset _parent;

            public OrderPreset(string field, Expression<Func<TEntity, T1>> expression, IOrderPreset parent = null)
            {
                Field = field;
                _expression = expression;
                _parent = parent;
            }

            public IQueryable<TEntity> Apply(IQueryable<TEntity> query, bool ascSort)
            {
                if (_parent == null)
                    return Apply(query, ascSort, _expression, true);
                query = _parent.Apply(query, ascSort);
                return Apply(query, ascSort, _expression, false);
            }

            private IQueryable<TEntity> Apply(IQueryable<TEntity> query, bool ascSort, Expression<Func<TEntity, T1>> expression, bool first)
            {
                return first
                    ? (ascSort ? query.OrderBy(expression) : query.OrderByDescending(expression))
                    : (ascSort ? ((IOrderedQueryable<TEntity>)query).ThenBy(expression) : ((IOrderedQueryable<TEntity>)query).ThenByDescending(expression));
            }
        }

        private readonly IQueryable<TEntity> _query;
        private readonly string _orderBy;
        private readonly string _sortBy;

        private IOrderPreset _default;
        private readonly List<IOrderPreset> _presets = new List<IOrderPreset>();

        public OrderService(IQueryable<TEntity> query, string orderBy, string sortBy)
        {
            _query = query;
            _orderBy = orderBy;
            _sortBy = sortBy;
        }

        public OrderService<TEntity> Field<T1>(string field, Expression<Func<TEntity, T1>> expression)
        {
            _presets.Add(new OrderPreset<T1>(field, expression));
            return this;
        }

        public OrderService<TEntity> Field<T1, T2>(string field, Expression<Func<TEntity, T1>> expression1, Expression<Func<TEntity, T2>> expression2)
        {
            var preset1 = new OrderPreset<T1>(field, expression1);
            var preset2 = new OrderPreset<T2>(field, expression2, preset1);
            _presets.Add(preset2);
            return this;
        }

        public OrderService<TEntity> Field<T1, T2, T3>(string field, Expression<Func<TEntity, T1>> expression1, Expression<Func<TEntity, T2>> expression2, Expression<Func<TEntity, T3>> expression3)
        {
            var preset1 = new OrderPreset<T1>(field, expression1);
            var preset2 = new OrderPreset<T2>(field, expression2, preset1);
            var preset3 = new OrderPreset<T3>(field, expression3, preset2);
            _presets.Add(preset3);
            return this;
        }

        public OrderService<TEntity> Field<T1, T2, T3, T4>(string field, Expression<Func<TEntity, T1>> expression1, Expression<Func<TEntity, T2>> expression2, Expression<Func<TEntity, T3>> expression3, Expression<Func<TEntity, T4>> expression4)
        {
            var preset1 = new OrderPreset<T1>(field, expression1);
            var preset2 = new OrderPreset<T2>(field, expression2, preset1);
            var preset3 = new OrderPreset<T3>(field, expression3, preset2);
            var preset4 = new OrderPreset<T4>(field, expression4, preset3);
            _presets.Add(preset4);
            return this;
        }

        public OrderService<TEntity> Default<T1>(Expression<Func<TEntity, T1>> expression)
        {
            _default = new OrderPreset<T1>(null, expression);
            return this;
        }

        public OrderService<TEntity> Default<T1, T2>(Expression<Func<TEntity, T1>> expression1, Expression<Func<TEntity, T2>> expression2)
        {
            var preset1 = new OrderPreset<T1>(null, expression1);
            var preset2 = new OrderPreset<T2>(null, expression2, preset1);
            _default = preset2;
            return this;
        }

        public OrderService<TEntity> Default<T1, T2, T3>(Expression<Func<TEntity, T1>> expression1, Expression<Func<TEntity, T2>> expression2, Expression<Func<TEntity, T3>> expression3)
        {
            var preset1 = new OrderPreset<T1>(null, expression1);
            var preset2 = new OrderPreset<T2>(null, expression2, preset1);
            var preset3 = new OrderPreset<T3>(null, expression3, preset2);
            _default = preset3;
            return this;
        }

        public IQueryable<TEntity> Apply()
        {
            var preset = _presets.FirstOrDefault(x => x.Field == _orderBy) ?? _default;
            var ascSort = !string.Equals(_sortBy, "desc", StringComparison.InvariantCultureIgnoreCase);
            return preset != null ? preset.Apply(_query, ascSort) : _query;
        }
    }

}

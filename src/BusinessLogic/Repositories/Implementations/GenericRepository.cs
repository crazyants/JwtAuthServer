﻿using LegnicaIT.DataAccess.Context;
using LegnicaIT.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LegnicaIT.BusinessLogic.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        protected DbSet<T> dbSet;
        protected IJwtDbContext context;

        protected GenericRepository(IJwtDbContext _context)
        {
            context = _context;
            dbSet = _context.Set<T>();
        }

        public virtual void Update(T entity)
        {
            dbSet.Attach(entity);
            // context.SetModified(entity);
        }

        public virtual void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public virtual void Delete(T entity)
        {
            dbSet.Remove(entity);
        }

        public IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            IQueryable<T> query = context.Set<T>().Where(predicate);
            return query;
        }

        public virtual IEnumerable<T> GetAll()
        {
            return dbSet.AsEnumerable();
        }

        public virtual T GetById(int id)
        {
            return dbSet.Find(id);
        }

        public virtual void Save()
        {
            context.SaveChanges();
        }
    }
}
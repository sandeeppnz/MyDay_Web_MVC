using SANSurveyWebAPI.DTOs;
using SANSurveyWebAPI.Models;
using SANSurveyWebAPI.Models.Api;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SANSurveyWebAPI.DAL
{
    public class GenericRepository<TEntity> where TEntity : class
    {
        //internal ApplicationDbContext context;
        internal DbContext context;
        internal DbSet<TEntity> dbSet;

       
        public GenericRepository(DbContext context)
        {
            if (context == null) throw new ArgumentNullException("db context is null");
            this.context = context;
            this.dbSet = context.Set<TEntity>();
        }

        public virtual IEnumerable<TEntity> GetUsingNoTracking(
             Expression<Func<TEntity, bool>> filter = null,
             Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
             string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.AsNoTracking().ToList();
            }
        }


        public virtual IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }


        public virtual TEntity GetByID(object id)
        {
            return dbSet.Find(id);
        }

        public async virtual Task<TEntity> GetByIDAsync(object id)
        {
            return await dbSet.FindAsync(id);
        }

        public virtual void Insert(TEntity e)
        {
            if (e == null) throw new ArgumentNullException("insert entity is null");
            dbSet.Add(e);
        }

        public virtual void Delete(object id)
        {
            TEntity entityToDelete = dbSet.Find(id);
            Delete(entityToDelete);
        }

        public virtual void AddRange(IEnumerable<TEntity> entities)
        {
            context.Set<TEntity>().AddRange(entities);
        }

        public virtual void RemoveRange(IEnumerable<TEntity> entities)
        {
            context.Set<TEntity>().RemoveRange(entities);
        }


        public virtual void Delete(TEntity e)
        {
            if (e == null) throw new ArgumentNullException("delete entity is null");
            if (context.Entry(e).State == EntityState.Detached)
            {
                dbSet.Attach(e);
            }
            dbSet.Remove(e);
        }

        public virtual void Update(TEntity e)
        {
            if (e == null) throw new ArgumentNullException("update entity is null");
            dbSet.Attach(e);
            context.Entry(e).State = EntityState.Modified;
        }




        public async virtual Task UpdateProfileAsync(TEntity e)
        {
            if (e == null) throw new ArgumentNullException("update entity is null");
            dbSet.Attach(e);
            context.Entry(e).State = EntityState.Modified;
        }

    }
}
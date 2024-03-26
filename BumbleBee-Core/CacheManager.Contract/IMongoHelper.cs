using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CacheManager.Contract
{
    public interface IMongoHelper<T>: ICollectionDataHelper<T>
    {
        // Generic
        //IQueryable<T> AsQueryable();

        //IEnumerable<T> FilterBy(
        //    Expression<Func<T, bool>> filterExpression);

        //IEnumerable<TProjected> FilterBy<TProjected>(
        //    Expression<Func<T, bool>> filterExpression,
        //    Expression<Func<T, TProjected>> projectionExpression);

        //T FindOne(Expression<Func<T, bool>> filterExpression);

        //Task<T> FindOneAsync(Expression<Func<T, bool>> filterExpression);

        //T FindById(string id);

        //Task<T> FindByIdAsync(string id);

        //void InsertOne(T document, string collectionName);

        //Task InsertOneAsync(T document, string collectionName);

        //void InsertMany(ICollection<T> documents, string collectionName);

        //Task InsertManyAsync(ICollection<T> documents, string collectionName);

        //void ReplaceOne(T document);

        //Task ReplaceOneAsync(T document);

        //void DeleteOne(Expression<Func<T, bool>> filterExpression);

        //Task DeleteOneAsync(Expression<Func<T, bool>> filterExpression);

        //void DeleteById(string id);

        //Task DeleteByIdAsync(string id);

        //void DeleteMany(Expression<Func<T, bool>> filterExpression);

        //Task DeleteManyAsync(Expression<Func<T, bool>> filterExpression);
    }
}

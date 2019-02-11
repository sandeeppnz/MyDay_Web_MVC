using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SANSurveyWebAPI.BLL
{
    public static class RandomTaskGenerationService
    {       
        private static Random randomselectionTask = new Random();
        internal class TModel { };

        public static T GetRandomElement<T>(this IEnumerable<T> list)
        {
            // If there are no elements in the collection, return the default value of T
            if (list.Count() == 0)
                return default(T);

            return list.ElementAt(randomselectionTask.Next(list.Count()));

            //To call in controller write below code
            //RandomTaskGenerationService.GetRandomElement(result);        
        }
        public static IEnumerable<T> GetRandom<T>(this IEnumerable<T> list, int count)
        {
            //This code gives any number of random selection, just pass the count
            if (count <= 0)
                yield break;
            var r = new Random();
            int limit = (count * 10);
            foreach (var item in list.OrderBy(x => r.Next(0, limit)).Take(count))
                yield return item;

            //To call in controller write below code;
            //RandomTaskGenerationService.GetRandom(result, 3);
        }
        public static TList GetSelectedRandom<TList>(this TList list, int count) where TList : IList, new()
        {
            var r = new Random();
            var rList = new TList();
            while (count > 0 && list.Count > 0)
            {
                var n = r.Next(0, list.Count);
                var e = list[n];
                rList.Add(e);
                list.RemoveAt(n);
                count--;
            }

            return rList;

            //To call in controller write below code
            //var _allItems = new List<TModel>(){};
            //var randomItemList = _allItems.GetSelectedRandom(10);
            //List<TModel> r1;
            //r1 = (List<TModel>)result;
            //var randomItemList = r1.GetSelectedRandom(10);
        }
    }
}
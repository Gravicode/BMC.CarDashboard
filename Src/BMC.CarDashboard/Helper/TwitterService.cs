﻿
//using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;

namespace BMC.CarDashboard
{

    public class TwitterService
    {
        // private TwitterContext twitterCtx = null;
        public TwitterService()
        {
            // Set up your credentials
            Auth.SetUserCredentials(APPCONTANTS.Twitter_ConsumerKey, APPCONTANTS.Twitter_ConsumerSecret, APPCONTANTS.Twitter_AccessToken, APPCONTANTS.Twitter_AccessTokenSecret);
            /*
            var auth = new SingleUserAuthorizer
            {
                CredentialStore = new SingleUserInMemoryCredentialStore
                {
                    ConsumerKey = APPCONTANTS.Twitter_ConsumerKey,
                    ConsumerSecret = APPCONTANTS.Twitter_ConsumerSecret,
                    AccessToken = APPCONTANTS.Twitter_AccessToken,
                    AccessTokenSecret = APPCONTANTS.Twitter_AccessTokenSecret
                }
            };
            twitterCtx = new TwitterContext(auth);*/
        }
        public IEnumerable<ITweet> GetTwittByHashtag(string Hashtag, int MaxSearchEntriesToReturn = 10)
        {
            // If you want to handle RateLimits
            RateLimit.RateLimitTrackerMode = RateLimitTrackerMode.TrackAndAwait;
            var searchParameter = new SearchTweetsParameters(Hashtag)
            {
                GeoCode = new GeoCode(-6.121435, 106.774124,10000, DistanceMeasure.Miles),
                SearchType = SearchResultType.Recent,
                MaximumNumberOfResults = 30,
                Filters = TweetSearchFilters.Hashtags 
            };
            var tweets = Search.SearchTweets(searchParameter);
            return tweets;
        }
        /*
           public async Task<List<Status>> GetTwittByHashtag(string Hashtag, int MaxSearchEntriesToReturn = 10)
           {

               string searchTerm = Hashtag;

               // oldest id you already have for this search term
               ulong sinceID = 1;

               // used after the first query to track current session
               ulong maxID;

               var combinedSearchResults = new List<Status>();

               List<Status> searchResponse =
                   await
                   (from search in twitterCtx.Search
                    where search.Type == SearchType.Search &&
                          search.Query == searchTerm &&
                          search.Count == MaxSearchEntriesToReturn &&
                          search.SinceID == sinceID
                    select search.Statuses)
                   .SingleOrDefaultAsync();

               combinedSearchResults.AddRange(searchResponse);
               ulong previousMaxID = ulong.MaxValue;
               do
               {
                   // one less than the newest id you've just queried
                   maxID = searchResponse.Min(status => status.StatusID) - 1;

                   //Debug.Assert(maxID < previousMaxID);
                   previousMaxID = maxID;

                   searchResponse =
                       await
                       (from search in twitterCtx.Search
                        where search.Type == SearchType.Search &&
                              search.Query == searchTerm &&
                              search.Count == MaxSearchEntriesToReturn &&
                              search.MaxID == maxID &&
                              search.SinceID == sinceID
                        select search.Statuses)
                       .SingleOrDefaultAsync();

                   combinedSearchResults.AddRange(searchResponse);
               } while (searchResponse.Any());

               return combinedSearchResults;
               /*
               combinedSearchResults.ForEach(tweet =>
                   Debug.WriteLine(
                       "\n  User: {0} ({1})\n  Tweet: {2}",
                       tweet.User.ScreenNameResponse,
                       tweet.User.UserIDResponse,
                       tweet.Text));*/
        //}*/

    }

}
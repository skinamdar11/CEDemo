using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OAuthClientWebApp.DataAccess
{
    /// <summary>
    /// Class to simulate a data access layer.
    /// </summary>
    public static class DataAccessLayer
    {
        private static Dictionary<string, AuthorisationAttempt> _authAttemptRepo = new Dictionary<string, AuthorisationAttempt>();

        /// <summary>
        /// Persists an authorisation code identifier along with the userId making
        /// the authorisation attempt.
        /// </summary>
        public static void PersistAuthorisationAttempt(AuthorisationAttempt authAttempt)
        {
            _authAttemptRepo.Add(authAttempt.AttemptIdentifier, authAttempt);
        }

        /// <summary>
        /// Gets the authorisation attempt from the persistant storage.
        /// </summary>
        /// <returns>
        /// Null if the attempt could not be found, otherwise the authorisation attempt.
        /// </returns>
        public static AuthorisationAttempt GetAuthorisationAttempt(string attemptIdentifier)
        {
            if (_authAttemptRepo.ContainsKey(attemptIdentifier))
            {
                return _authAttemptRepo[attemptIdentifier];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Removes the authorisation attempt from the peristent storage.
        /// </summary>
        /// <param name="attemptIdentifier"></param>
        public static void RemoveAuthorisationAttempt(string attemptIdentifier)
        {
            if (_authAttemptRepo.ContainsKey(attemptIdentifier))
            {
                _authAttemptRepo.Remove(attemptIdentifier);
            }
        }
    }

    /// <summary>
    /// Tuple to store an authorisation attempt identifier 
    /// along with the unique identifier for the user making
    /// the authorisation attempt.
    /// </summary>
    public class AuthorisationAttempt
    {
        public string AttemptIdentifier { get; set; }
        public Guid UserId { get; set; }
        public bool ForWebAppA { get; set; }
    }

    /// <summary>
    /// Tuple to store a refresh token.  When storing a refresh
    /// token the client also needs to know which user the token is 
    /// for and also what scope was granted for that token.
    /// </summary>
    public class StoredRefreshToken
    {
        public Guid UserId { get; set; }
        public string RefreshToken { get; set; }
    }
}
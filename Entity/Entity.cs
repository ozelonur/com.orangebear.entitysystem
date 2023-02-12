using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace OrangeBear.Entity
{
    public abstract class Entity<TEntity> where TEntity : Entity<TEntity>
    {
        #region Public Variables

        public static Regex IdRegex;

        #endregion

        #region Properties

        public static string[] Ids { get; private set; }
        public string Id { get; private set; }

        #endregion

        #region Private Variables

        private static readonly Dictionary<string, TEntity> Entities;

        private readonly TEntity _derivedTEntity;

        #endregion

        #region Constructors

        static Entity()
        {
            IdRegex = new Regex("^[A-Z0-9]{4,32}$",
                RegexOptions.IgnoreCase | RegexOptions.Compiled
                                        | RegexOptions.Singleline);

            Entities = new Dictionary<string, TEntity>();
            Ids = Array.Empty<string>();
        }

        protected Entity()
        {
            _derivedTEntity = (TEntity)this;

            if (_derivedTEntity == null)
            {
                Debug.LogError("Derived class must be of type TEntity!");
            }

            Id = string.Empty;

            if (!Init())
            {
                Debug.LogError("Failed to initialize entity!");
            }
        }

        #endregion

        #region Protected Methods

        protected abstract bool Init();

        protected void Save()
        {
            if (string.IsNullOrEmpty(Id)) return;

            string text = $"__ENTITY({typeof(TEntity).Name})#{Id}";
            string otherText = JsonUtility.ToJson(_derivedTEntity);
            if (!string.IsNullOrEmpty(otherText))
            {
                PlayerPrefs.SetString(text, otherText);
            }
        }

        public void Load()
        {
            if (string.IsNullOrEmpty(Id)) return;

            string text = $"__ENTITY({typeof(TEntity).Name})#{Id}";
            string @string = PlayerPrefs.GetString(text);
            if (!string.IsNullOrEmpty(@string))
            {
                JsonUtility.FromJsonOverwrite(@string, _derivedTEntity);
            }
        }

        #endregion

        #region Public Methods

        public static TEntity Get(string id = "default")
        {
            return !Entities.ContainsKey(id) ? null : Entities[id];
        }

        public bool Register(string id = "default")
        {
            if (!string.IsNullOrEmpty(Id))
            {
                return false;
            }

            if (!IdRegex.IsMatch(id))
            {
                return false;
            }

            if (Entities.ContainsKey(id))
            {
                return false;
            }

            Entities.Add(id, _derivedTEntity);
            Id = id;
            Ids = new string[Entities.Count];
            Entities.Keys.CopyTo(Ids, 0);
            return true;
        }

        #endregion
    }
}
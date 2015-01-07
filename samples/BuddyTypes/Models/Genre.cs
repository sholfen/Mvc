using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace BuddyTypes.Models
{
    public class Album
    {
        public int AlbumId { get; set; }

        public int GenreId { get; set; }

        public string Title { get; set; }

        public decimal Price { get; set; }

        public string Url { get; set; }

        public virtual Genre Genre { get; set; }
    }

    [PropertiesMatch("Name", "GenreName", ErrorMessage = "Name and GenreName are not the same.")]
    public class Genre
    {
        public int GenreId { get; set; }

        [StringLength(5)]
        [Display(Name="xx")]
        public string Name { get; set; }

        public string Description { get; set; }

        public string GenreName { get; set; }

        public List<Album> Albums { get; set; }
    }

    [ModelMetadataType(typeof(Genre))]
    public class GenreDTO
    {
        [StringLength(3)]
        public string Name { get; set; }

        public string Description { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class PropertiesMatchAttribute : ValidationAttribute
    {
        public String FirstPropertyName { get; set; }
        public String SecondPropertyName { get; set; }

        //Constructor to take in the property names that are supposed to be checked
        public PropertiesMatchAttribute(String firstPropertyName, String secondPropertyName)
        {
            FirstPropertyName = firstPropertyName;
            SecondPropertyName = secondPropertyName;
        }

        public override Boolean IsValid(Object value)
        {
            Type objectType = value.GetType();

            PropertyInfo[] neededProperties =
              objectType.GetProperties()
              .Where(propertyInfo => propertyInfo.Name == FirstPropertyName || propertyInfo.Name == SecondPropertyName)
              .ToArray();

            if (neededProperties.Count() != 2)
            {
                //  throw new ValidationException("PropertiesMatchAttribute error on " + objectType.Name);
                //ValidationResult result = new ValidationResult
                return false;
            }

            Boolean isValid = true;

            if (!Convert.ToString(neededProperties[0].GetValue(value, null)).Equals(Convert.ToString(neededProperties[1].GetValue(value, null))))
            {
                isValid = false;
            }

            return isValid;
        }
    }
}
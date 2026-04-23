using JyoshinmonKarate.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JyoshinmonKarate.Areas.Identity.Data;

// Add profile data for application users by adding properties to the User class
public class User : IdentityUser
{
    //This class was created when I added the Identity scaffolding to the project and I'm only editing it
    //UserId will be inhereted from IdentityUser as Id
    //Username will be inhereted from IdentityUser as UserName
    //Password will be inhereted from IdentityUser as PasswordHash

    [Required(ErrorMessage = "First name is required.")]
    [StringLength(30, ErrorMessage = "First name cannot be more than 30 characters.")]
    [Display(Name = "First Name")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(30, ErrorMessage = "Last name cannot be more than 30 characters.")]
    [Display(Name = "Last Name")]
    public string LastName { get; set; }

    //Email will be inhereted from IdentityUser as Email
    //Phone will be inhereted from IdentityUser as PhoneNumber

    [Display(Name = "Admin User")]
    public bool IsAdmin { get; set; }

    //one user can manage many members
    public ICollection<Member> Members { get; set; }
    //one user can manage one instructor
    public Instructor Instructor { get; set; }

    
}


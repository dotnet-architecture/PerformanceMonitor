using DataTransfer;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication.Services;
using WebApplication.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebApplication.Pages
{
    public class IndexModel : PageModel
    {
        public List<Session> sessions = new List<Session>();
        public static Session selected { get; set; } = new Session();

        [BindProperty]
        public List<Session> sessionsReturned { get; set; } = new List<Session>();

        [Required]
        [BindProperty]
        [DataType(DataType.Text)]
        [Display(Name = "Application")]
        public String application { get; set; } = "";
        
        [Required]
        [BindProperty]
        [DataType(DataType.Text)]
        [Display(Name = "Process")]
        public String process { get; set; } = "";

        public async Task OnGet()
        {
            sessions = await FetchDataService.getSessionData();

            int i = 1 + 1; 
        }

        // Returns true if an appropriate application and process are inputed
        // Returns false if no application and process string found OR they are not in the database
        public Boolean getSession()
        {
            if (process.Equals("") || application.Equals(""))
            {
                return false;
            }

            for (int i = 0; i < sessions.Count; i++)
            {
                String sessApp = sessions[i].application;
                String sessProcess = sessions[i].process; 

                if (sessApp.Equals(application) && sessProcess.Equals(process))
                {
                    selected = sessions[i];
                    return true;
                }
            }

            return false;
        }
    }
}

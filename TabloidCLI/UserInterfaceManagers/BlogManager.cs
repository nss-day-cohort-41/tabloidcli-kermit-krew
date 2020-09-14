using System;
using System.Collections.Generic;
using System.Text;
using TabloidCLI.Models;
using TabloidCLI.Repositories;

namespace TabloidCLI.UserInterfaceManagers
{
    class BlogManager : IUserInterfaceManager
    {
        private readonly IUserInterfaceManager _parentUI;
    private BlogRepository _blogRepository;
    private string _connectionString;

    public BlogManager(IUserInterfaceManager parentUI, string connectionString)
    {
        _parentUI = parentUI;
        _blogRepository = new BlogRepository(connectionString);
        _connectionString = connectionString;
    }

    public IUserInterfaceManager Execute()
    {
           

        Console.WriteLine("Blog Menu");
        Console.WriteLine(" 1) List Blogs");
        Console.WriteLine(" 2) Blog Details");
        Console.WriteLine(" 3) Add Blog");
        Console.WriteLine(" 4) Edit Blog");
        Console.WriteLine(" 5) Remove Blog");
        Console.WriteLine(" 0) Go Back");

        Console.Write("> ");
        string choice = Console.ReadLine();
        Console.Clear();
        switch (choice)
        {
            case "1":
                List();
                return this;
            case "2":
                Blog blog = Choose();
                if (blog == null)
                {
                    return this;
                }
                else
                {
                    return new BlogDetailManager(this, _connectionString, blog.Id);
                }
            case "3":
                Add();
                return this;
            case "4":
                Edit();
                return this;
            case "5":
                Remove();
                return this;
            case "0":
                Console.Clear();
                return _parentUI;
            default:
   
                return this;
        }
    }

    private void List()
    {
        List<Blog> blogs = _blogRepository.GetAll();
        foreach (Blog blog in blogs)
        {
            Console.WriteLine(blog);
        }
    }

    private Blog Choose(string prompt = null)
    {
        if (prompt == null)
        {
            prompt = "Please choose a Blog:";
        }

        Console.WriteLine(prompt);

        List<Blog> blogs = _blogRepository.GetAll();

        for (int i = 0; i < blogs.Count; i++)
        {
            Blog blog = blogs[i];
            Console.WriteLine($" {i + 1}) {blog.Title}");
        }
        Console.Write("> ");

        string input = Console.ReadLine();
        try
        {
            int choice = int.Parse(input);
            return blogs[choice - 1];
        }
        catch (Exception ex)
        {
            Console.WriteLine("Invalid Selection");
            return null;
        }
    }

    private void Add()
    {
        Console.WriteLine("New Blog");
        Blog blog = new Blog();
            blog.Title = "";
        
        do
        {
            if (blog.Title.Length > 55) { Console.WriteLine("The maximum title length is 55 characters. Please shorten the title."); }
            Console.Write("New Blog Title: ");
            blog.Title = Console.ReadLine();
            Console.Clear();
        } while (blog.Title == "" || blog.Title.Length > 55 );

         do
         {
            Console.Write("New Blog Url: ");
            blog.Url = Console.ReadLine();
            Console.Clear();
         } while (blog.Url == "");

        blog.Tags = new List<Tag>();

        _blogRepository.Insert(blog);
    }

    private void Edit()
    {
        Blog blogToEdit = Choose("Which Blog would you like to edit?");
        if (blogToEdit == null)
        {
            return;
        }

        Console.WriteLine();
            string title = "";
        do
        {
            if (title.Length > 55) { Console.WriteLine("The maximum title length is 55 characters. Please shorten the title or hit ENTER to keep the same title."); }
            Console.Write("New Blog Title: ");
            title = Console.ReadLine();
            Console.Clear();
        } while (title.Length > 55);


            if (!string.IsNullOrWhiteSpace(title))
        {
            blogToEdit.Title = title;
        }
        Console.Write("New Url (blank to leave unchanged): ");
        string url = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(url))
        {
            blogToEdit.Url = url;
        }
        

        _blogRepository.Update(blogToEdit);
    }

    private void Remove()
    {
        Blog blogToDelete = Choose("Which blog would you like to remove?");
        if (blogToDelete != null)
        {
            Console.WriteLine("If any Posts contain this blog, they will be deleted as well.");
            Console.Write("Type 'y' or 'yes' to confirm: ");
            string confirmDelete = Console.ReadLine().ToLower();
            if (confirmDelete == "y" || confirmDelete == "yes")
            {
                _blogRepository.Delete(blogToDelete.Id);
            }
        }
    }
}
}

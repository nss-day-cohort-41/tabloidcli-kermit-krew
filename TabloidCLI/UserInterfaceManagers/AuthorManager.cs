using System;
using System.Collections.Generic;
using TabloidCLI.Models;
using TabloidCLI.Repositories;

namespace TabloidCLI.UserInterfaceManagers
{
    public class AuthorManager : IUserInterfaceManager
    {
        private readonly IUserInterfaceManager _parentUI;
        private AuthorRepository _authorRepository;
        private PostRepository _postRepository;
        private BlogRepository _blogRepostory;
        private string _connectionString;

        public AuthorManager(IUserInterfaceManager parentUI, string connectionString)
        {
            _parentUI = parentUI;
            _authorRepository = new AuthorRepository(connectionString);
            _postRepository = new PostRepository(connectionString);
            _blogRepostory = new BlogRepository(connectionString);
            _connectionString = connectionString;
            
            
        }

        public IUserInterfaceManager Execute()
        {
            Console.WriteLine("Author Menu");
            Console.WriteLine(" 1) List Authors");
            Console.WriteLine(" 2) Author Details");
            Console.WriteLine(" 3) Add Author");
            Console.WriteLine(" 4) Edit Author");
            Console.WriteLine(" 5) Remove Author");
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
                    Author author = Choose();
                    if (author == null)
                    {
                        return this;
                    }
                    else
                    {
                        return new AuthorDetailManager(this, _connectionString, author.Id);
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
                    Console.WriteLine("Invalid Selection");
                    return this;
            }
        }

        private void List()
        {
            List<Author> authors = _authorRepository.GetAll();
            foreach (Author author in authors)
            {
                Console.WriteLine(author);
            }
        }

        private Author Choose(string prompt = null)
        {
            if (prompt == null)
            {
                prompt = "Please choose an Author:";
            }

            Console.WriteLine(prompt);

            List<Author> authors = _authorRepository.GetAll();

            for (int i = 0; i < authors.Count; i++)
            {
                Author author = authors[i];
                Console.WriteLine($" {i + 1}) {author.FullName}");
            }
            Console.Write("> ");

            string input = Console.ReadLine();
            try
            {
                int choice = int.Parse(input);
                return authors[choice - 1];
            }
            catch (Exception ex)
            {
                Console.WriteLine("Invalid Selection");
                return null;
            }
        }

        private void Add()
        {
                Console.WriteLine("New Author");
                // FirstName and LastName must be insantiated for character length checks during input
                Author author = new Author()
                {
                    FirstName = "",
                    LastName = ""
                };

            // FirstName and LastName have a 55 character limit in the database
            do
            {
                if (author.FirstName.Length > 55) Console.WriteLine("Name cannot exceed 55 characters.");
                Console.Write("New Author First Name: ");
                author.FirstName = Console.ReadLine();
                Console.Clear();
            } while (author.FirstName == "" | author.FirstName.Length > 55);

            do
            {
                if (author.LastName.Length > 55) Console.WriteLine("Name cannot exceed 55 characters.");
                Console.Write("New Author Last Name: ");
                author.LastName = Console.ReadLine();
                Console.Clear();
            } while (author.LastName == "" | author.LastName.Length > 55);

            do
            {
                Console.Write("New Author Bio: ");
                author.Bio = Console.ReadLine();
                Console.Clear();
            } while (author.Bio == "");

            _authorRepository.Insert(author);
        }

        private void Edit()
        {
            Author authorToEdit = Choose("Which author would you like to edit?");
            if (authorToEdit == null)
            {
                return;
            }

            // FirstName and LastName have a 55 character limit in the database
            Console.WriteLine();
            string firstName = "";
            do
            {
                if (firstName.Length > 55) Console.WriteLine("Name cannot exceed 55 characters.");
                Console.Write("New first name (blank to leave unchanged): ");
                firstName = Console.ReadLine();
                Console.Clear();
            } while (firstName.Length > 55);
            // If a blank is entered at any time, the value in the class remains unchanged
            if (!string.IsNullOrWhiteSpace(firstName))
            {
                authorToEdit.FirstName = firstName;
            }

            string lastName = "";
            do
            {
                if (lastName.Length > 55) Console.WriteLine("Name cannot exceed 55 characters.");
                Console.Write("New last name (blank to leave unchanged): ");
                lastName = Console.ReadLine();
                Console.Clear();
            } while (lastName.Length > 55);
            // If a blank is entered at any time, the value in the class remains unchanged
            if (!string.IsNullOrWhiteSpace(lastName))
            {
                authorToEdit.LastName = lastName;
            }

            Console.Write("New bio (blank to leave unchanged): ");
            string bio = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(bio))
            {
                authorToEdit.Bio = bio;
            }

            _authorRepository.Update(authorToEdit);
        }

        private void Remove()
        {
            Author authorToDelete = Choose("Which author would you like to remove?");
            if (authorToDelete != null)
            {
               try
                {
                    _authorRepository.Delete(authorToDelete.Id);
                }
                catch (Microsoft.Data.SqlClient.SqlException exception)
                {
                    Console.WriteLine("It appears the author has Posts in TabloidCLI.");
                    Console.WriteLine("Would you like to remove the Posts and Author? (Y to delete all, ENTER to return to the menu): ");
                    string response = Console.ReadLine().ToLower();
                    if (response == "y")
                    {
                        List<Post> posts = _postRepository.GetByAuthor(authorToDelete.Id);
                        foreach (Post post in posts)
                        {
                            _blogRepostory.Delete(post.Blog.Id);
                        };
                       _authorRepository.Delete(authorToDelete.Id);
                        Console.Clear();

                    }
                    
                }
                
            }
        }
    }
}

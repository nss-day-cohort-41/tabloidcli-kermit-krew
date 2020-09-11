using System;
using System.Collections.Generic;
using System.Text;
using TabloidCLI.Models;
using TabloidCLI.Repositories;

namespace TabloidCLI.UserInterfaceManagers
{
    class PostManager : IUserInterfaceManager
    {
        private readonly IUserInterfaceManager _parentUI;
        private PostRepository _postRepository;
        private string _connectionString;

        public PostManager(IUserInterfaceManager parentUI, string connectionString)
        {
            _parentUI = parentUI;
            _postRepository = new PostRepository(connectionString);
            _connectionString = connectionString;
        }
        

        public IUserInterfaceManager Execute()
        {


            Console.WriteLine("Post Menu");
            Console.WriteLine(" 1) List Posts");
            Console.WriteLine(" 2) Post Details");
            Console.WriteLine(" 3) Add Post");
            Console.WriteLine(" 4) Edit Post");
            Console.WriteLine(" 5) Remove Post");
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
                    Post post = Choose();
                    if (post == null)
                    {
                        return this;
                    }
                    else
                    {
                        return new AuthorDetailManager(this, _connectionString, post.Id);
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
                    return _parentUI;
                default:
                    Console.WriteLine("Invalid Selection");
                    return this;
            }
        }

        private void List()
        {
            List<Post> posts = _postRepository.GetAll();
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            foreach (Post post in posts)
            {
                Console.WriteLine(post.Title);
            }
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Console.ReadLine();
            Console.Clear();
        }
        private void Add()
        {

            Console.WriteLine("New Post");
            Post post = new Post();
            do
            {
                Console.Clear();
                Console.Write("Title: ");
                post.Title = Console.ReadLine();
            } while (post.Title == "");
            
            do
            {
                Console.Clear();
                Console.Write("Url: ");
                post.Url = Console.ReadLine();
            } while (post.Url == "");
            
            bool authorCheck = false;
            do
            {   
                Console.Clear();
                Console.Write("Author: ");
                post.Author = ChooseAuthor();
                if (post.Author is Author)
                {
                    authorCheck = true;
                }
            } while (authorCheck == false);
           
            bool blogChecker = false;
            do
            {
                Console.Clear();
                Console.Write("Blog: ");
                post.Blog = ChooseBlog();
                if(post.Blog is Blog)
                {
                    blogChecker = true;
                }
            }
            while (blogChecker == false);
            Console.Clear();
            post.PublishDateTime = DateTime.Now;
            Console.WriteLine("Is the following information correct? ");
            Console.WriteLine($"Title: {post.Title}");
            Console.WriteLine($"Url: {post.Url}");
            Console.WriteLine($"Author: {post.Author.LastName}, {post.Author.FirstName}");
            Console.WriteLine($"Blog: {post.Blog.Title}");
            Console.WriteLine($"Yes/No");
            string answer = Console.ReadLine().ToLower();
            if(answer == "no")
            {
                Console.WriteLine("Alright. Lets do this again.");
                Console.Clear();
                Add();
                return;
            }
            Console.Clear();
            _postRepository.Insert(post);
        }

        private void Edit()
        {
            Post postToEdit = Choose("Which Post would you like to edit?");
            if (postToEdit == null)
            {
                return;
            }
            //title,url,pubdate,author,blog
            Console.WriteLine();
            Console.Write("New Title (blank to leave unchanged) ");
            string title = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(title))
            {
                postToEdit.Title = title;
            }
            Console.Clear();
            Console.Write("New Url (blank to leave unchanged) ");
            string url = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(url))
            {
                postToEdit.Url = url;
            }

            Console.Clear();
            bool checkDate = false;
            do
            {   
                Console.Write("New Date (blank to leave unchanged) YYYY/MM/DD ");
                string pubDate = Console.ReadLine();
                DateTime dateTime;
                if(string.IsNullOrWhiteSpace(pubDate))
                {
                    checkDate = true;
                }
                if(DateTime.TryParse(pubDate, out dateTime))
                {
                    postToEdit.PublishDateTime = dateTime;
                    checkDate = true;
                }
                
            }
            while (checkDate == false);
            Console.Clear();
            bool authorChecker = false;
            do
            {
                Console.Write("New Author (blank to leave unchanged) ");
                Author author = ChooseAuthor();
                if(author is Author)
                {
                    postToEdit.Author = author;
                    break;
                }
                
                if (author == null)
                {
                    break;
                }
            } while (authorChecker == false);
              Console.Clear();
           



            bool blogChecker = false;
            do
            {
                Console.Write("New Blog (blank to leave unchanged) ");
                Blog blog = ChooseBlog();
                if (blog is Blog)
                {
                    postToEdit.Blog = blog;
                    blogChecker = true;
                }
                if (blog == null)
                {
                    blogChecker = true;
                }

            } while (blogChecker == false);

            Console.WriteLine("Is the following information correct? ");
            Console.WriteLine($"Title: {postToEdit.Title}");
            Console.WriteLine($"Url: {postToEdit.Url}");
            Console.WriteLine($"Author: {postToEdit.Author.LastName}, {postToEdit.Author.FirstName}");
            Console.WriteLine($"Blog: {postToEdit.Blog.Title}");
            Console.WriteLine($"Yes/No");
            string answer = Console.ReadLine().ToLower();
            if (answer == "no")
            {
                Console.Clear();
                Console.WriteLine("Alright. Lets do this again.");
                Edit();
                return;
            }
            Console.Clear();
            _postRepository.Update(postToEdit);
        }

        private void Remove()
        {
            Post postToDelete = Choose("Which Post would you like to remove?");
            if (postToDelete != null)
            {
                _postRepository.Delete(postToDelete.Id);
            }
            Console.Clear();
        }
        private Post Choose(string prompt = null)
        {
            if (prompt == null)
            {
                prompt = "Please choose a Post:";
            }

            Console.WriteLine(prompt);

            List<Post> posts = _postRepository.GetAll();

            for (int i = 0; i < posts.Count; i++)
            {
                Post post = posts[i];
                Console.WriteLine($" {i + 1}) {post.Title}");
            }
            Console.Write("> ");

            string input = Console.ReadLine();
            try
            {
                int choice = int.Parse(input);
                return posts[choice - 1];
            }
            catch (Exception ex)
            {
                Console.WriteLine("Invalid Selection");
                return null;
            }
        }
        private Author ChooseAuthor(string prompt = null)
        {
            if (prompt == null)
            {
                prompt = "Please choose an Author:";
            }

            Console.WriteLine(prompt);

            List<Author> authors = new AuthorRepository(_connectionString).GetAll();

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
                Console.Clear();
                return authors[choice - 1];
            }
            catch (Exception ex)
            {
                Console.Clear();
                return null;
            }
        }
        private Blog ChooseBlog(string prompt = null)
        {
            if (prompt == null)
            {
                prompt = "Please choose a Blog:";
            }

            Console.WriteLine(prompt);

            List<Blog> blogs = new BlogRepository(_connectionString).GetAll();

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
                Console.Clear();
                return blogs[choice - 1];
            }
            catch (Exception ex)
            {
                Console.Clear();
                return null;
            }
        }
    }
}
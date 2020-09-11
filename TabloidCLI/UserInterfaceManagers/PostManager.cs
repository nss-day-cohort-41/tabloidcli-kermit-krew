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
            foreach (Post post in posts)
            {
                Console.WriteLine(post.Title);
            }
        }
        private void Add()
        {

            Console.WriteLine("New Post");
            Post post = new Post();
            do
            {
                Console.Write("Title: ");
                post.Title = Console.ReadLine();
            } while (post.Title == "");

            do
            {
                Console.Write("Url: ");
                post.Url = Console.ReadLine();
            } while (post.Url == "");
            
            bool authorCheck = false;
            do
            {
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

                Console.Write("Blog: ");
                post.Blog = ChooseBlog();
                if(post.Blog is Blog)
                {
                    blogChecker = true;
                }
            }
            while (blogChecker == false);
            
            post.PublishDateTime = DateTime.Now;


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
            Console.Write("New Url (blank to leave unchanged) ");
            string url = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(url))
            {
                postToEdit.Url = url;
            }
            
            
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
            
            Console.Write("New Author (blank to leave unchanged) ");
            Author author = ChooseAuthor();
            if (author != null)
            {
                postToEdit.Author = author;
            }
            Console.Write("New Blog (blank to leave unchanged) ");
            Blog blog = ChooseBlog();
            if (blog != null)
            {
                postToEdit.Blog = blog;
            }


            _postRepository.Update(postToEdit);
        }

        private void Remove()
        {
            Post postToDelete = Choose("Which Post would you like to remove?");
            if (postToDelete != null)
            {
                _postRepository.Delete(postToDelete.Id);
            }
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
                return authors[choice - 1];
            }
            catch (Exception ex)
            {
                
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
                return blogs[choice - 1];
            }
            catch (Exception ex)
            {
                
                return null;
            }
        }
    }
}
//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace Nb.Common.Themes
//{
//    /// <summary>
//    /// Represents a scope that is tracked by the dependency injection container. The scope is
//    /// used to keep track of resources that have been provided, so that they can then be
//    /// subsequently released when <see cref="IDisposable.Dispose"/> is called.
//    /// </summary>
//    public interface IDependencyScope : IDisposable
//    {
//        /// <summary>
//        /// Gets an instance of the given <paramref name="serviceType"/>. Must return <c>null</c>
//        /// if the service is not available (must not throw).
//        /// </summary>
//        /// <param name="serviceType">The object type.</param>
//        /// <returns>The requested object, if found; <c>null</c> otherwise.</returns>
//        object GetService(Type serviceType);

//        /// <summary>
//        /// Gets all instances of the given <paramref name="serviceType"/>. Must return an empty
//        /// collection if the service is not available (must not return <c>null</c> or throw).
//        /// </summary>
//        /// <param name="serviceType">The object type.</param>
//        /// <returns>A sequence of instances of the requested <paramref name="serviceType"/>. The sequence
//        /// should be empty (not <c>null</c>) if no objects of the given type are available.</returns>
//        IEnumerable<object> GetServices(Type serviceType);
//    }

//    /// <summary>
//    /// Represents a dependency injection container.
//    /// </summary>
//    public interface IDependencyResolver : IDependencyScope
//    {
//        /// <summary>
//        /// Starts a resolution scope. Objects which are resolved in the given scope will belong to
//        /// that scope, and when the scope is disposed, those objects are returned to the container.
//        /// Implementers should return a new instance of <see cref="IDependencyScope"/> every time this
//        /// method is called, unless the container does not have any concept of scope or resource
//        /// release (in which case, it would be okay to return 'this', so long as the calls to
//        /// <see cref="IDisposable.Dispose"/> are effectively NOOPs).
//        /// </summary>
//        /// <returns>The dependency scope.</returns>
//        IDependencyScope BeginScope();
//    }
    
//    internal class EmptyResolver : IDependencyResolver
//    {
//        private static readonly IDependencyResolver _instance = new EmptyResolver();

//        private EmptyResolver()
//        {
//        }

//        public static IDependencyResolver Instance
//        {
//            get { return _instance; }
//        }

//        public IDependencyScope BeginScope()
//        {
//            return this;
//        }

//        public void Dispose()
//        {
//        }

//        public object GetService(Type serviceType)
//        {
//            return null;
//        }

//        public IEnumerable<object> GetServices(Type serviceType)
//        {
//            return Enumerable.Empty<object>();
//        }
//    }

//    public class DependencyConfiguraion
//    {
//        public DependencyConfiguraion() : this(EmptyResolver.Instance)
//        {
//        }

//        public DependencyConfiguraion(IDependencyResolver resolver)
//        {
//            DependencyResolver = resolver;
//        }

//        public IDependencyResolver DependencyResolver { get; private set; } 
//    }
//}

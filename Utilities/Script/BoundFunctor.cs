using System;

namespace SZUtilities
{
    public class StructContainer<TStruct>
        where TStruct : struct
    {
        public TStruct Value;

        public static StructContainer<TStruct> Create(DisposablesGroup allocator)
        {
            var handle = RentingPool<StructContainer<TStruct>>.Rent(out var container);
            allocator.Add(handle);
            return container;
        }

        public static StructContainer<TStruct> Create(TStruct value, DisposablesGroup allocator)
        {
            var container = Create(allocator);
            container.Value = value;
            return container;
        }
    }

    public struct BoundFunctor<TResult>
        : IDisposable
        where TResult : class
    {
        private readonly object m_func;
        private readonly bool m_hasAllocator;
        private DisposablesGroup m_disposablesGroup;
        private object m_context;

        public static (BoundFunctor<TResult> functor, TContext context) Create<TContext>(Func<TContext, TResult> func)
            where TContext : class, new()
        {
            var functor = new BoundFunctor<TResult>(func, hasAllocator: false);
            var context = functor.Bind<TContext>();
            return (functor, context);
        }

        public static (BoundFunctor<TResult> functor, TContext context) Create<TContext>(Func<TContext, DisposablesGroup, TResult> func)
            where TContext : class, new()
        {
            var functor = new BoundFunctor<TResult>(func, hasAllocator: true);
            var context = functor.Bind<TContext>();
            return (functor, context);
        }

        private BoundFunctor(object func, bool hasAllocator)
        {
            m_func = func;
            m_hasAllocator = hasAllocator;
            m_context = null;
            m_disposablesGroup = DisposablesGroup.Rent();
        }

        private TContext Bind<TContext>()
            where TContext : class, new()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (null != m_context)
                throw new Exception("Context already bound");
#endif
            
            m_disposablesGroup.Add(RentingPool<TContext>.Rent(out TContext context));
            m_context = context;
            return context;
        }

        private static readonly object[] s_argument = new object[1];
        private static readonly object[] s_argumentWithAllocator = new object[2];
        public readonly TResult Invoke()
        {
            var methodInfo = m_func.GetType().GetMethod("Invoke");
            
            object result;
            if (m_hasAllocator)
            {
                s_argumentWithAllocator[0] = m_context;
                s_argumentWithAllocator[1] = m_disposablesGroup;
                result = methodInfo.Invoke(m_func, s_argumentWithAllocator);
            }
            else
            {
                s_argument[0] = m_context;
                result = methodInfo.Invoke(m_func, s_argument);
            }

            return (TResult)result;
        }

        public void Dispose()
        {
            m_disposablesGroup?.Dispose();
            m_disposablesGroup = null;
            m_context = null;
        }
    }
}

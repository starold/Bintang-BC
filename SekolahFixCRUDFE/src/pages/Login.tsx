import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { toast } from 'react-hot-toast';
import { Mail, Lock, ArrowRight, Loader2 } from 'lucide-react';
import { authService } from '../services/auth.service';
import { useAuthStore } from '../store/authStore';

export const Login = () => {
  const [isLoading, setIsLoading] = useState(false);
  const { register, handleSubmit, formState: { errors } } = useForm();
  const navigate = useNavigate();
  const setAuth = useAuthStore(state => state.setAuth);

  const onSubmit = async (data: any) => {
    setIsLoading(true);
    try {
      const result = await authService.login(data.email, data.password);
      if (result.success) {
        localStorage.setItem('token', result.data.token);
        setAuth(result.data.token, { email: result.data.email, role: result.data.role });
        toast.success(result.message || 'Login successful');
        navigate(result.data.role === 'Admin' ? '/admin' : '/student');
      } else {
        toast.error(result.message || 'Login failed');
      }
    } catch (error: any) {
      toast.error(error.response?.data?.message || 'Invalid credentials');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="space-y-6">
      <div className="text-center space-y-1">
        <h2 className="text-2xl font-bold text-slate-900 dark:text-white">Welcome Back</h2>
        <p className="text-slate-500 dark:text-slate-400 text-sm">Please enter your details to sign in</p>
      </div>

      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
        <div className="space-y-2">
          <label className="text-sm font-medium text-slate-700 dark:text-slate-300 ml-1">Email Address</label>
          <div className="relative group">
            <div className="absolute inset-y-0 left-0 pl-3.5 flex items-center pointer-events-none text-slate-400 group-focus-within:text-primary-500 transition-colors">
              <Mail size={18} />
            </div>
            <input
              {...register('email', { required: 'Email is required' })}
              type="email"
              className="w-full bg-slate-50 dark:bg-slate-800 border border-slate-200 dark:border-slate-700 text-slate-900 dark:text-white text-sm rounded-xl focus:ring-2 focus:ring-primary-500/20 focus:border-primary-500 block w-full pl-10.5 p-3 transition-all outline-none"
              placeholder="name@sekolah.com"
            />
          </div>
          {errors.email && <p className="text-xs text-rose-500 ml-1">{errors.email.message as string}</p>}
        </div>

        <div className="space-y-2">
          <div className="flex justify-between items-center px-1">
            <label className="text-sm font-medium text-slate-700 dark:text-slate-300">Password</label>
            <a href="#" className="text-xs font-semibold text-primary-600 hover:text-primary-500">Forgot?</a>
          </div>
          <div className="relative group">
            <div className="absolute inset-y-0 left-0 pl-3.5 flex items-center pointer-events-none text-slate-400 group-focus-within:text-primary-500 transition-colors">
              <Lock size={18} />
            </div>
            <input
              {...register('password', { required: 'Password is required' })}
              type="password"
              className="w-full bg-slate-50 dark:bg-slate-800 border border-slate-200 dark:border-slate-700 text-slate-900 dark:text-white text-sm rounded-xl focus:ring-2 focus:ring-primary-500/20 focus:border-primary-500 block w-full pl-10.5 p-3 transition-all outline-none"
              placeholder="••••••••"
            />
          </div>
          {errors.password && <p className="text-xs text-rose-500 ml-1">{errors.password.message as string}</p>}
        </div>

        <button
          disabled={isLoading}
          type="submit"
          className="w-full bg-primary-600 hover:bg-primary-700 text-white font-semibold py-3 px-4 rounded-xl shadow-lg shadow-primary-500/25 transition-all active:scale-[0.98] disabled:opacity-70 disabled:active:scale-100 flex items-center justify-center gap-2 group mt-6"
        >
          {isLoading ? (
            <Loader2 className="animate-spin" size={20} />
          ) : (
            <>
              Sign In
              <ArrowRight size={18} className="group-hover:translate-x-1 transition-transform" />
            </>
          )}
        </button>
      </form>

      <div className="text-center pt-2">
        <p className="text-sm text-slate-500 dark:text-slate-400">
          Don't have an account?{' '}
          <Link to="/register" className="font-semibold text-primary-600 hover:text-primary-500">
            Create Profile
          </Link>
        </p>
      </div>
    </div>
  );
};

import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { toast } from 'react-hot-toast';
import { User, Mail, Lock, MapPin, Calendar, Phone, ArrowRight, Loader2 } from 'lucide-react';
import { authService } from '../services/auth.service';

export const Register = () => {
  const [isLoading, setIsLoading] = useState(false);
  const { register, handleSubmit, formState: { errors } } = useForm();
  const navigate = useNavigate();

  const onSubmit = async (data: any) => {
    setIsLoading(true);
    try {
      const result = await authService.register(data);
      if (result.success) {
        toast.success(result.message || 'Registration successful!');
        navigate('/login');
      } else {
        toast.error(result.message || 'Registration failed');
      }
    } catch (error: any) {
      const errs = error.response?.data?.errors;
      if (errs && Array.isArray(errs)) {
        errs.forEach((e: string) => toast.error(e));
      } else {
        toast.error(error.response?.data?.message || 'Registration failed. Please check your details.');
      }
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="space-y-6">
      <div className="text-center space-y-1">
        <h2 className="text-2xl font-bold text-slate-900 dark:text-white">Create Profile</h2>
        <p className="text-slate-500 dark:text-slate-400 text-sm">Join our academic community today</p>
      </div>

      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
        {/* Full Name */}
        <div className="space-y-1.5">
          <label className="text-xs font-semibold text-slate-600 dark:text-slate-400 uppercase tracking-wider ml-1">Full Name</label>
          <div className="relative group">
            <div className="absolute inset-y-0 left-0 pl-3.5 flex items-center text-slate-400 group-focus-within:text-primary-500 transition-colors">
              <User size={16} />
            </div>
            <input
              {...register('fullName', { required: 'Full name is required' })}
              className="w-full bg-slate-50 dark:bg-slate-800 border border-slate-200 dark:border-slate-700 text-slate-900 dark:text-white text-sm rounded-xl focus:ring-2 focus:ring-primary-500/20 focus:border-primary-500 block w-full pl-10 p-2.5 transition-all outline-none"
              placeholder="Budi Santoso"
            />
          </div>
          {errors.fullName && <p className="text-[10px] text-rose-500 ml-1">{errors.fullName.message as string}</p>}
        </div>

        {/* Email */}
        <div className="space-y-1.5">
          <label className="text-xs font-semibold text-slate-600 dark:text-slate-400 uppercase tracking-wider ml-1">Email</label>
          <div className="relative group">
            <div className="absolute inset-y-0 left-0 pl-3.5 flex items-center text-slate-400 group-focus-within:text-primary-500 transition-colors">
              <Mail size={16} />
            </div>
            <input
              {...register('email', { required: 'Email is required' })}
              type="email"
              className="w-full bg-slate-50 dark:bg-slate-800 border border-slate-200 dark:border-slate-700 text-slate-900 dark:text-white text-sm rounded-xl focus:ring-2 focus:ring-primary-500/20 focus:border-primary-500 block w-full pl-10 p-2.5 transition-all outline-none"
              placeholder="budi@example.com"
            />
          </div>
          {errors.email && <p className="text-[10px] text-rose-500 ml-1">{errors.email.message as string}</p>}
        </div>

        <div className="grid grid-cols-2 gap-4">
          {/* Password */}
          <div className="space-y-1.5">
            <label className="text-xs font-semibold text-slate-600 dark:text-slate-400 uppercase tracking-wider ml-1">Password</label>
            <div className="relative group">
              <div className="absolute inset-y-0 left-0 pl-3.5 flex items-center text-slate-400 group-focus-within:text-primary-500 transition-colors">
                <Lock size={16} />
              </div>
              <input
                {...register('password', { required: 'Required', minLength: { value: 6, message: 'Too short' } })}
                type="password"
                className="w-full bg-slate-50 dark:bg-slate-800 border border-slate-200 dark:border-slate-700 text-slate-900 dark:text-white text-sm rounded-xl focus:ring-2 focus:ring-primary-500/20 focus:border-primary-500 block w-full pl-10 p-2.5 transition-all outline-none"
                placeholder="••••••"
              />
            </div>
            {errors.password && <p className="text-[10px] text-rose-500 ml-1">{errors.password.message as string}</p>}
          </div>

          {/* Date of Birth */}
          <div className="space-y-1.5">
            <label className="text-xs font-semibold text-slate-600 dark:text-slate-400 uppercase tracking-wider ml-1">Birth Date</label>
            <div className="relative group">
              <div className="absolute inset-y-0 left-0 pl-3.5 flex items-center text-slate-400 group-focus-within:text-primary-500 transition-colors">
                <Calendar size={16} />
              </div>
              <input
                {...register('dateOfBirth', { required: 'Required' })}
                type="date"
                className="w-full bg-slate-50 dark:bg-slate-800 border border-slate-200 dark:border-slate-700 text-slate-900 dark:text-white text-sm rounded-xl focus:ring-2 focus:ring-primary-500/20 focus:border-primary-500 block w-full pl-10 p-2.5 transition-all outline-none"
              />
            </div>
            {errors.dateOfBirth && <p className="text-[10px] text-rose-500 ml-1">{errors.dateOfBirth.message as string}</p>}
          </div>
        </div>

        {/* Phone */}
        <div className="space-y-1.5">
          <label className="text-xs font-semibold text-slate-600 dark:text-slate-400 uppercase tracking-wider ml-1">Phone Number</label>
          <div className="relative group">
            <div className="absolute inset-y-0 left-0 pl-3.5 flex items-center text-slate-400 group-focus-within:text-primary-500 transition-colors">
              <Phone size={16} />
            </div>
            <input
              {...register('phoneNumber')}
              className="w-full bg-slate-50 dark:bg-slate-800 border border-slate-200 dark:border-slate-700 text-slate-900 dark:text-white text-sm rounded-xl focus:ring-2 focus:ring-primary-500/20 focus:border-primary-500 block w-full pl-10 p-2.5 transition-all outline-none"
              placeholder="081234..."
            />
          </div>
        </div>

        {/* Address */}
        <div className="space-y-1.5">
          <label className="text-xs font-semibold text-slate-600 dark:text-slate-400 uppercase tracking-wider ml-1">Address</label>
          <div className="relative group">
            <div className="absolute top-3 left-3.5 text-slate-400 group-focus-within:text-primary-500 transition-colors">
              <MapPin size={16} />
            </div>
            <textarea
              {...register('address', { required: 'Address is required' })}
              rows={2}
              className="w-full bg-slate-50 dark:bg-slate-800 border border-slate-200 dark:border-slate-700 text-slate-900 dark:text-white text-sm rounded-xl focus:ring-2 focus:ring-primary-500/20 focus:border-primary-500 block w-full pl-10 p-2.5 transition-all outline-none resize-none"
              placeholder="Jl. Gadjah Mada No. 1..."
            />
          </div>
          {errors.address && <p className="text-[10px] text-rose-500 ml-1">{errors.address.message as string}</p>}
        </div>

        <button
          disabled={isLoading}
          type="submit"
          className="w-full bg-primary-600 hover:bg-primary-700 text-white font-semibold py-3 px-4 rounded-xl shadow-lg shadow-primary-500/25 transition-all active:scale-[0.98] disabled:opacity-70 flex items-center justify-center gap-2 group mt-4"
        >
          {isLoading ? (
            <Loader2 className="animate-spin" size={20} />
          ) : (
            <>
              Create Profile
              <ArrowRight size={18} className="group-hover:translate-x-1 transition-transform" />
            </>
          )}
        </button>
      </form>

      <div className="text-center pt-2">
        <p className="text-xs text-slate-500 dark:text-slate-400">
          Already have a profile?{' '}
          <Link to="/login" className="font-semibold text-primary-600 hover:text-primary-500 uppercase tracking-tight">
            Sign In
          </Link>
        </p>
      </div>
    </div>
  );
};

import { useEffect, useState } from 'react';
import { useAuthStore } from '../store/authStore';
import { studentService } from '../services/student.service';
import type { StudentProfileDto } from '../types';
import { 
  Sparkles, 
  Clock, 
  CheckCircle2, 
  XCircle, 
  BookOpen, 
  UserCircle,
  ArrowRight
} from 'lucide-react';
import { Link } from 'react-router-dom';

export const StudentDashboard = () => {
  const { } = useAuthStore();
  const [profile, setProfile] = useState<StudentProfileDto | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchProfile = async () => {
      try {
        const result = await studentService.getProfile();
        if (result.success) setProfile(result.data);
      } catch (error) {
        console.error('Failed to fetch profile', error);
      } finally {
        setLoading(false);
      }
    };
    fetchProfile();
  }, []);

  if (loading) return (
    <div className="space-y-8 animate-pulse">
      <div className="h-32 bg-slate-200 dark:bg-slate-800 rounded-3xl"></div>
      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        <div className="h-40 bg-slate-200 dark:bg-slate-800 rounded-2xl"></div>
        <div className="h-40 bg-slate-200 dark:bg-slate-800 rounded-2xl"></div>
      </div>
    </div>
  );

  const status = profile?.approvalStatus || 'Pending';

  return (
    <div className="space-y-8 animate-in fade-in slide-in-from-bottom-4 duration-700">
      {/* Welcome Banner */}
      <div className="bg-white dark:bg-slate-900 p-8 rounded-3xl border border-slate-100 dark:border-slate-800 shadow-sm relative overflow-hidden">
        <div className="relative z-10">
          <h1 className="text-3xl font-bold text-slate-900 dark:text-white flex items-center gap-3">
            Welcome back, {profile?.fullName.split(' ')[0]}! <Sparkles className="text-amber-400" size={28} />
          </h1>
          <p className="text-slate-500 dark:text-slate-400 mt-2 max-w-2xl">
            Manage your academic profile and enroll in new courses from your personal dashboard.
          </p>
        </div>
        <div className="absolute top-0 right-0 w-64 h-64 bg-primary-500/5 rounded-full -translate-y-1/2 translate-x-1/2 blur-3xl"></div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
        {/* Status Card */}
        <div className="bg-white dark:bg-slate-900 p-8 rounded-3xl border border-slate-100 dark:border-slate-800 shadow-sm">
          <h3 className="text-lg font-bold text-slate-900 dark:text-white mb-6">Enrollment Status</h3>
          <div className="flex items-center gap-6">
            <div className={`w-20 h-20 rounded-2xl flex items-center justify-center ${
              status === 'Approved' ? 'bg-emerald-50 text-emerald-600 dark:bg-emerald-500/10 dark:text-emerald-400' :
              status === 'Rejected' ? 'bg-rose-50 text-rose-600 dark:bg-rose-500/10 dark:text-rose-400' :
              'bg-amber-50 text-amber-600 dark:bg-amber-500/10 dark:text-amber-400'
            }`}>
              {status === 'Approved' ? <CheckCircle2 size={40} /> :
               status === 'Rejected' ? <XCircle size={40} /> :
               <Clock size={40} />}
            </div>
            <div>
              <p className={`text-xl font-bold ${
                status === 'Approved' ? 'text-emerald-600' :
                status === 'Rejected' ? 'text-rose-600' :
                'text-amber-600'
              }`}>
                {status}
              </p>
              <p className="text-sm text-slate-500 dark:text-slate-400 mt-1">
                {status === 'Approved' ? 'Your profile has been approved. You can now enroll in courses.' :
                 status === 'Rejected' ? 'Your profile was not approved. Please contact the administrator.' :
                 'Your registration is currently under review by our team.'}
              </p>
            </div>
          </div>
        </div>

        {/* Quick Actions */}
        <div className="bg-white dark:bg-slate-900 p-8 rounded-3xl border border-slate-100 dark:border-slate-800 shadow-sm">
          <h3 className="text-lg font-bold text-slate-900 dark:text-white mb-6">Quick Actions</h3>
          <div className="grid grid-cols-1 gap-4">
            <Link 
              to="/student/profile" 
              className="flex items-center justify-between p-4 bg-slate-50 dark:bg-slate-800/50 hover:bg-primary-50 dark:hover:bg-primary-500/10 rounded-2xl transition-all group"
            >
              <div className="flex items-center gap-3">
                <UserCircle className="text-primary-600" size={20} />
                <span className="font-semibold text-slate-900 dark:text-white">Complete Profile</span>
              </div>
              <ArrowRight size={18} className="text-slate-400 group-hover:translate-x-1 transition-transform" />
            </Link>
            <Link 
              to="/student/courses" 
              className="flex items-center justify-between p-4 bg-slate-50 dark:bg-slate-800/50 hover:bg-primary-50 dark:hover:bg-primary-500/10 rounded-2xl transition-all group"
            >
              <div className="flex items-center gap-3">
                <BookOpen className="text-primary-600" size={20} />
                <span className="font-semibold text-slate-900 dark:text-white">Browse Courses</span>
              </div>
              <ArrowRight size={18} className="text-slate-400 group-hover:translate-x-1 transition-transform" />
            </Link>
          </div>
        </div>
      </div>
    </div>
  );
};

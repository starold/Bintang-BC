import { useEffect, useState } from 'react';
import { adminService } from '../services/admin.service';
import { Users, BookOpen, GraduationCap, Clock, ArrowUpRight, TrendingUp } from 'lucide-react';
import { clsx, type ClassValue } from 'clsx';
import { twMerge } from 'tailwind-merge';

function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}

export const AdminDashboard = () => {
  const [summary, setSummary] = useState<any>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchSummary = async () => {
      try {
        const data = await adminService.getSummary();
        setSummary(data);
      } catch (error) {
        console.error('Failed to fetch summary', error);
      } finally {
        setLoading(false);
      }
    };
    fetchSummary();
  }, []);

  const stats = [
    { label: 'Total Students', value: summary?.totalStudents || 0, icon: Users, color: 'primary', trend: '+12%' },
    { label: 'Total Courses', value: summary?.totalCourses || 0, icon: BookOpen, color: 'emerald', trend: '+4%' },
    { label: 'Total Teachers', value: summary?.totalTeachers || 0, icon: GraduationCap, color: 'indigo', trend: '+2%' },
    { label: 'Pending Approvals', value: summary?.pendingApprovals || 0, icon: Clock, color: 'amber', trend: 'Critical' },
  ];

  if (loading) return (
    <div className="space-y-8 animate-pulse">
      <div className="h-10 w-64 bg-slate-200 dark:bg-slate-800 rounded-lg"></div>
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        {[1, 2, 3, 4].map(i => <div key={i} className="h-32 bg-slate-200 dark:bg-slate-800 rounded-2xl"></div>)}
      </div>
    </div>
  );

  return (
    <div className="space-y-8 animate-in fade-in slide-in-from-bottom-4 duration-700">
      <div>
        <h1 className="text-3xl font-bold text-slate-900 dark:text-white">Admin Dashboard</h1>
        <p className="text-slate-500 dark:text-slate-400 mt-1">Overview of SekolahFix academic system</p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        {stats.map((stat) => (
          <div key={stat.label} className="bg-white dark:bg-slate-900 p-6 rounded-2xl border border-slate-100 dark:border-slate-800 shadow-sm hover:shadow-md transition-shadow group">
            <div className="flex justify-between items-start">
              <div className={cn(
                "p-3 rounded-xl",
                stat.color === 'primary' && "bg-primary-50 text-primary-600 dark:bg-primary-500/10 dark:text-primary-400",
                stat.color === 'emerald' && "bg-emerald-50 text-emerald-600 dark:bg-emerald-500/10 dark:text-emerald-400",
                stat.color === 'indigo' && "bg-indigo-50 text-indigo-600 dark:bg-indigo-500/10 dark:text-indigo-400",
                stat.color === 'amber' && "bg-amber-50 text-amber-600 dark:bg-amber-500/10 dark:text-amber-400",
              )}>
                <stat.icon size={24} />
              </div>
              <div className={cn(
                "flex items-center gap-1 text-xs font-medium px-2 py-1 rounded-full",
                stat.trend === 'Critical' ? "bg-rose-50 text-rose-600" : "bg-emerald-50 text-emerald-600"
              )}>
                {stat.trend === 'Critical' ? <Clock size={12} /> : <TrendingUp size={12} />}
                {stat.trend}
              </div>
            </div>
            <div className="mt-4">
              <p className="text-slate-500 dark:text-slate-400 text-sm font-medium">{stat.label}</p>
              <div className="flex items-baseline gap-2 mt-1">
                <p className="text-3xl font-bold text-slate-900 dark:text-white">{stat.value}</p>
              </div>
            </div>
          </div>
        ))}
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
        <div className="lg:col-span-2 bg-white dark:bg-slate-900 p-8 rounded-2xl border border-slate-100 dark:border-slate-800">
          <div className="flex justify-between items-center mb-6">
            <h3 className="text-xl font-bold text-slate-900 dark:text-white">Recent Activities</h3>
            <button className="text-primary-600 text-sm font-semibold flex items-center gap-1 hover:underline">
              View All <ArrowUpRight size={16} />
            </button>
          </div>
          <div className="space-y-6">
            {[1, 2, 3].map(i => (
              <div key={i} className="flex gap-4 items-center">
                <div className="w-2 h-2 rounded-full bg-primary-500"></div>
                <div className="flex-1">
                  <p className="text-sm font-medium text-slate-900 dark:text-white">System update successful</p>
                  <p className="text-xs text-slate-500">2 hours ago</p>
                </div>
              </div>
            ))}
          </div>
        </div>

        <div className="bg-primary-600 rounded-2xl p-8 text-white relative overflow-hidden">
          <div className="relative z-10 h-full flex flex-col justify-between">
            <div>
              <h3 className="text-2xl font-bold mb-2">Academic Year 2026/2027</h3>
              <p className="text-primary-100 text-sm">Registration is currently open for prospective students.</p>
            </div>
            <button className="mt-8 bg-white text-primary-600 px-6 py-3 rounded-xl font-bold shadow-lg transition-transform active:scale-95 w-full">
              Manage Admissions
            </button>
          </div>
          <GraduationCap size={150} className="absolute -bottom-10 -right-10 text-primary-500/20 rotate-12" />
        </div>
      </div>
    </div>
  );
};

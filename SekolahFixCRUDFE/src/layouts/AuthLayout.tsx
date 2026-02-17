import { Outlet } from 'react-router-dom';
import { GraduationCap } from 'lucide-react';

export const AuthLayout = () => {
  return (
    <div className="min-h-screen bg-slate-50 dark:bg-academic-dark flex flex-col justify-center items-center p-6">
      <div className="w-full max-w-md flex flex-col items-center">
        <div className="mb-8 flex flex-col items-center">
          <div className="w-16 h-16 bg-primary-600 rounded-2xl flex items-center justify-center text-white shadow-xl shadow-primary-500/30 mb-4 animate-bounce-subtle">
            <GraduationCap size={36} />
          </div>
          <h1 className="text-3xl font-extrabold text-slate-900 dark:text-white tracking-tight">
            Sekolah<span className="text-primary-600">Fix</span>
          </h1>
          <p className="text-slate-500 dark:text-slate-400 mt-2 text-center">
            Modern School Registration System
          </p>
        </div>
        
        <div className="w-full bg-white dark:bg-slate-900 rounded-3xl p-8 shadow-xl shadow-slate-200/50 dark:shadow-none border border-slate-100 dark:border-slate-800">
          <Outlet />
        </div>
        
        <p className="mt-8 text-sm text-slate-400 dark:text-slate-600">
          &copy; 2026 SekolahFix System. All rights reserved.
        </p>
      </div>
    </div>
  );
};
